# Enterprise Framework — Architecture Document (Living Documentation)

Bu doküman, sistemin temel prensiplerini, katmanlı yapısını, tasarım standartlarını ve iyileştirme yol haritasını tanımlar. Projenin **Anayasası** niteliğinde olup, her yeni özellik geliştirilmesinde referans alınacak tek kaynaktır (Single Source of Truth).

---

## 1. Temel Prensipler (Core Principles)

| Prensip | Açıklama |
|---|---|
| **SOLID** | Dependency Inversion ve Single Responsibility esas alınır. |
| **Clean Architecture** | Altyapı (Infrastructure) veya Arayüz (UI) kararları, Merkezi İş Mantığını (Domain & Application) asla etkilemez. |
| **CQRS** | MediatR tabanlı Command/Query ayrımı; her işlem kendi dosyasında yaşar. |
| **Sıfır Amelelik** | Tekrarlayan kodlar merkezileştirilmiştir. Yeni bir CRUD modülü eklemek maksimum 10 dakika sürmelidir. |
| **Fail-Fast** | Validasyon hataları ve yetkisizlik durumları en dış katmanda (Behavior/Middleware) reddedilir. |
| **Standart Yanıt** | Tüm API yanıtları `ApiResponse<T>` envelope'u ile sarılır. Frontend bu kontrata güvenir. |

---

## 2. Teknoloji Yığını (Technology Stack)

| Katman | Teknoloji |
|---|---|
| **Runtime** | .NET 8 (Minimal API) |
| **ORM** | Entity Framework Core 8 |
| **CQRS** | MediatR |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper (`IMapFrom<T>` convention) |
| **Auth Provider** | Keycloak (JWT + OIDC) |
| **Logging** | Serilog (Console + File/JSON) |
| **Database** | SQL Server / PostgreSQL (switchable via `IDatabaseProvider`) |
| **Frontend** | Angular 17 (standalone components) |
| **UI Kit** | PrimeNG 17 + PrimeFlex 4 |
| **Container** | Docker Compose (Keycloak + MSSQL + Adminer) |

---

## 3. Backend Mimarisi (C# .NET 8)

Backend, sıkı bir **Clean Architecture** kurallarına göre 4 ana katmana bölünmüştür:

### A. Domain Katmanı (`Enterprise.Framework.Domain`)

Sistemdeki tüm varlıklar (Entities), Enumeration'lar, Interface'ler ve Domain Event'ler buradadır.

**Kural:** Asla başka hiçbir katmana referans (dependency) barındırmaz.

| Yapı Taşı | Açıklama |
|---|---|
| `BaseEntity` | `Id (long)` + Domain Events koleksiyonu |
| `AuditableEntity` | BaseEntity + `CreatedAtUtc/By`, `LastModifiedAtUtc/By` + Soft Delete (`IsDeleted`, `DeletedAtUtc/By`) |
| `IOrganizationBoundEntity` | Multi-tenant entity'ler için `OrganizationId` — otomatik query filter uygulanır |
| `ISoftDeletable` | Soft delete desteği — interceptor ile otomatik |
| `IBusinessRule` | Kural motoru için: `Order`, `ErrorCode`, `Message`, `IsBrokenAsync()` |
| `BaseEvent` | Domain event base class — Outbox Pattern ile işlenir |

### B. Application Katmanı (`Enterprise.Framework.Application`)

Uygulamanın iş mantığı (Business Logic), CQRS desenine uygun Command ve Query'ler.

**Kural:** Sadece Domain katmanına referans verir. Dış dünya (DB, HTTP) hakkında hiçbir fikri yoktur. Tüm dış bağımlılıklar interface (`IApplicationDbContext`, `IKeycloakAdminService` vb.) üzerinden tüketilir.

#### MediatR Pipeline Zinciri

Her request aşağıdaki pipeline'dan geçer (sıralı):

```
Request
  → UnhandledExceptionBehavior   (catch-all safety net)
  → ValidationBehavior           (FluentValidation — Fail-Fast)
  → LoggingBehavior              (request/response loglama)
  → PerformanceBehavior          (500ms+ süren istekleri uyarı olarak loglar)
  → CachingBehavior              (ICacheableRequest marker ile opt-in cache)
  → IdempotencyBehavior          (IIdempotentCommand marker ile tekrarlı istek koruması)
  → Handler
```

#### Diğer Mekanizmalar

| Mekanizma | Dosya | Açıklama |
|---|---|---|
| Dinamik Grid | `IQueryableExtensions.ApplyGridOptions()` | Frontend'den gelen `FiltersJson` ve `SortOrder` parametrelerini LINQ'e çevirir |
| Convention Mapping | `IMapFrom<T>` | DTO'ya implement edilir, AutoMapper otomatik profile oluşturur |
| İş Kuralı Motoru | `BusinessRuleEngine.CheckAsync()` | `IBusinessRule` array alır, sıralı çalıştırır, kırılan kuralda `BusinessRuleException` fırlatır |
| Pagination | `PaginatedProjectToAsync()` | AutoMapper projection + sayfalama tek satırda |
| Permission Sabitleri | `Permissions` static class | Reflection ile otomatik seed edilir |

### C. Infrastructure Katmanı (`Enterprise.Framework.Infrastructure`)

Application katmanındaki arayüzleri (Interface) implemente eder.

#### EF Core Interceptor Zinciri

`SaveChangesAsync` çağrısında otomatik tetiklenir (sıralı):

| Interceptor | İşlev |
|---|---|
| `SoftDeleteInterceptor` | `EntityState.Deleted` → `IsDeleted=true` + `DeletedBy` stamp |
| `AuditableEntitySaveChangesInterceptor` | `CreatedAtUtc/By` ve `LastModifiedAtUtc/By` otomatik stamp |
| `DispatchDomainEventsInterceptor` | Entity'lerdeki Domain Event'leri `OutboxMessage` tablosuna yazar |

#### Outbox Pattern (Event-Driven Architecture)

Dış sistem entegrasyonları için transactional event dispatch mekanizması:

1. Handler domain event'i entity'ye ekler → `entity.AddDomainEvent(new XyzEvent(...))`
2. `SaveChangesAsync` sırasında `DispatchDomainEventsInterceptor`, event'leri aynı transaction'da `OutboxMessage` tablosuna yazar
3. `OutboxProcessorBackgroundService` her 10 saniyede işlenmemiş mesajları batch (20) halinde okur
4. Her mesajı `DomainEventNotification<T>` wrapper ile MediatR'a publish eder
5. Hata durumunda `Error` alanına yazılır

#### Güvenlik Alt Sistemi

| Bileşen | Dosya | İşlev |
|---|---|---|
| `LocalClaimsTransformation` | Keycloak rolleri → local permission claim'lere dönüştürür (IMemoryCache ile 20dk cache) |
| `PermissionService` | identityId → `HashSet<string>` permission seti (IDistributedCache ile 10dk cache) |
| `CurrentUserService` | Mevcut kullanıcının Id, Email, Organization, Branch, Permissions bilgilerine erişim |
| `JitProvisioningMiddleware` | İlk login'de otomatik local kullanıcı oluşturma (IMemoryCache ile duplicate korumalı) |
| `PermissionSeeder` | `Permissions` static class'tan reflection ile modül ve izinleri DB'ye seed eder |

#### Diğer

| Bileşen | Açıklama |
|---|---|
| `DistributedAppCache` | `IAppCache` implementasyonu — double-check locking, SemaphoreSlim ile thread-safe |
| `DistributedIdempotencyStore` | `IIdempotencyStore` implementasyonu — distributed cache üzerinde |
| `IDatabaseProvider` | SQL Server / PostgreSQL switch desteği (config tabanlı) |

### D. API Katmanı (`Enterprise.Framework.API`)

Sadece dış dünyaya açılan Minimal API Endpoints.

**Kural:** İçinde kesinlikle iş mantığı (If/Else logic) veya Entity bulunamaz. Sadece Request'i alır, `sender.Send()` ile Application'a iletir ve `ApiResponse<T>` formatında döner.

| Bileşen | Açıklama |
|---|---|
| `IEndpointDefinition` | Endpoint sınıfları bu interface'i implement eder |
| `EndpointExtensions.MapAllEndpoints()` | Reflection ile tüm endpoint tanımlarını otomatik register eder |
| `ApiResponse<T>` | `Success`, `Message`, `Data`, `Errors`, `ValidationErrors`, `TraceId` |
| `GlobalExceptionMiddleware` | Tüm exception türlerini handle eder, standart `ApiResponse` formatında döner |

#### GlobalExceptionMiddleware Hata Eşleştirmesi

| Exception | HTTP Status | Açıklama |
|---|---|---|
| `ValidationException` | 400 | FluentValidation hataları → `ValidationErrors` dict |
| `NotFoundException` | 404 | Kayıt bulunamadı |
| `DomainException` | 400 | Domain kuralı ihlali |
| `BusinessRuleException` | 422 | İş kuralı ihlali (ErrorCode ile) |
| `UnauthorizedAccessException` | 401/403 | Yetki hatası |
| `BadHttpRequestException` | 400 | Geçersiz istek formatı |
| `Exception` (catch-all) | 500 | Beklenmeyen hata |

---

## 4. Frontend Mimarisi (Angular & PrimeNG)

### A. Katmanlı Yapı

| Katman | Konum | Sorumluluk |
|---|---|---|
| **Models** | `models/` | Backend `ApiResponse<T>`, `PagedResult<T>` ve entity DTO arayüzleri |
| **Services** | `services/` | `ApiService` (HTTP), `AuthService` (Keycloak), `NotificationService` (Toast), `LoadingService` (Overlay) |
| **Shared** | `shared/` | `BaseCrudService<T>` (generic CRUD), `GenericGridComponent`, `GenericFormComponent` |
| **Pages** | `pages/` | Modül ekranları — sadece orchestration, iş mantığı servislerde yaşar |
| **Guards** | `guards/` | `authGuard` (login kontrolü), `permissionGuard` (PBAC route koruması) |
| **Interceptors** | `interceptors/` | `loadingInterceptor`, `errorInterceptor`, `authInterceptor` |
| **Layout** | `layout/` | `MainLayoutComponent` (sidebar + topbar + content) |

### B. İletişim Standartları

- **Global Interceptor Zinciri:** `Loading → Error → Auth` (token inject + 401 otomatik refresh + `X-Retry` loop koruması)
- **Grid İletişimi:** Lazy load'da backend'e `FiltersJson`, `SortOrder`, `PageNumber`, `PageSize` gönderilir. Grid isteklerinde `X-Skip-Loading` header ile global overlay engellenir.
- **ApiResponse Kontratı:** Frontend'deki `ApiResponse<T>` ve `PagedResult<T>` interface'leri, backend'deki C# karşılıkları ile birebir eşleşir.

### C. Keycloak Entegrasyonu

| Özellik | Mekanizma |
|---|---|
| SSO | `check-sso` + silent SSO redirect |
| Login | Direct grant (password flow) — custom login UI |
| Token | In-memory storage (XSS koruması — localStorage kullanılmaz) |
| Refresh | 401'de otomatik refresh → retry (`authInterceptor`) |
| Permission | `/api/identity/me` endpoint'inden yüklenir → `hasPermission()` |
| SuperAdmin | Token'dan `realm_access.roles` parse — `admin` veya `framework-admin` |
| Init Timeout | 10sn — Keycloak erişilemezse UI donmaz |

---

## 5. Güvenlik ve Kimlik Yönetimi (PBAC & Keycloak)

- **PBAC (Policy-Based Access Control):** Yetkilendirme Rol bazlı değil, İzin bazlıdır (Örn: `Sample.Write`, `Sample.View`).
- **Write implies View:** `Sample.Write` yetkisi olan kullanıcı otomatik olarak `Sample.View` yetkisine de sahiptir (backend ve frontend simetrik).
- **Keycloak Integration:** Tüm kimlik doğrulaması Keycloak üzerinden yapılır. Token (JWT) içindeki Roller, `LocalClaimsTransformation` ile izinlere çevrilir.
- **Ghost Admin:** `appsettings.json` → `Security:BootstrapAdminEmail` — bu hesap her zaman tüm yetkilere sahiptir ve silinemez.
- **JIT Provisioning:** Keycloak'ta var ama local DB'de olmayan kullanıcılar, ilk API isteklerinde otomatik oluşturulur.
- **Permission Seeder:** `Permissions` static class'taki tüm izinler uygulama açılışında otomatik seed edilir.

---

## 6. Proje Klasör Yapısı

```
Enterprise.Framework.sln
├── src/
│   ├── Enterprise.Framework.Domain/           ← Bağımsız çekirdek
│   │   ├── Common/                            ← IEntity, IAuditableEntity, ISoftDeletable, IBusinessRule, BaseEvent
│   │   ├── Entities/                          ← SampleEntity, Identity/*
│   │   └── Events/                            ← Domain Event'ler
│   │
│   ├── Enterprise.Framework.Application/      ← İş mantığı
│   │   ├── Common/
│   │   │   ├── Behaviors/                     ← MediatR pipeline (6 behavior)
│   │   │   ├── Extensions/                    ← IQueryableExtensions (grid)
│   │   │   ├── Interfaces/                    ← IApplicationDbContext, ICurrentUserService, IKeycloakAdminService...
│   │   │   ├── Mappings/                      ← IMapFrom<T>, MappingProfile
│   │   │   ├── Models/                        ← PagedResult, DomainEventNotification
│   │   │   ├── Rules/                         ← BusinessRuleEngine, EntityMustExistRule
│   │   │   ├── Security/                      ← Permissions static class
│   │   │   └── Exceptions/                    ← NotFoundException, BusinessRuleException
│   │   └── Features/                          ← Modül bazlı CQRS (SampleModule/)
│   │       └── [Modül]/
│   │           ├── Commands/                  ← Create, Update, Delete + Validator
│   │           └── Queries/                   ← Get, List + DTO
│   │
│   ├── Enterprise.Framework.Infrastructure/   ← Dış dünya implementasyonları
│   │   ├── Persistence/                       ← AppDbContext, Interceptors, Configurations, Providers, Seeder
│   │   ├── Security/                          ← LocalClaimsTransformation, PermissionService, CurrentUserService
│   │   ├── Identity/                          ← KeycloakAdminService, RoleInitializer
│   │   ├── Caching/                           ← DistributedAppCache
│   │   ├── Idempotency/                       ← DistributedIdempotencyStore
│   │   ├── BackgroundJobs/                    ← OutboxProcessorBackgroundService
│   │   └── Services/                          ← Email, SMS, File, Excel
│   │
│   ├── Enterprise.Framework.API/              ← İnce kabuk
│   │   ├── Common/                            ← ApiResponse, IEndpointDefinition, EndpointExtensions
│   │   ├── Endpoints/                         ← SampleEndpoints, IdentityEndpoints...
│   │   └── Middleware/                        ← GlobalExceptionMiddleware, JitProvisioningMiddleware
│   │
│   └── Keycloak.Identity.Shared/              ← Keycloak JWT doğrulama paylaşımlı kütüphane
│
├── ui/                                        ← Angular 17
│   └── src/app/
│       ├── models/                            ← api-models.ts, sample.model.ts
│       ├── services/                          ← api, auth, notification, loading, identity, user
│       ├── interceptors/                      ← loading, error, auth
│       ├── guards/                            ← auth, permission
│       ├── layout/                            ← MainLayoutComponent (sidebar + topbar)
│       ├── shared/
│       │   ├── components/                    ← GenericGridComponent, GenericFormComponent
│       │   └── services/                      ← BaseCrudService<T>
│       └── pages/                             ← sample-module, users, management, home, login
│
├── ARCHITECTURE.md                            ← Bu doküman
├── DEVELOPER_PLAYBOOK.md                      ← Adım adım modül ekleme rehberi
├── docker-compose.yml                         ← Keycloak + MSSQL + Adminer
└── SetupGuide.md                              ← Kurulum rehberi
```

---

## 7. Gelecek Mimari Vizyonu (Phase 2 Roadmap)

Sistemin ileri düzey gereksinimleri için belirlenmiş mimari stratejiler:

### A. Dış Entegrasyonlar (Integration Bus)
- **Anti-Corruption Layer (ACL):** Core Domain, dış dünyayı asla bilmeyecektir.
- **Haberleşme:** RabbitMQ/Kafka (MassTransit aracılığıyla) veya HTTP servisleri (Refit aracılığıyla).

### B. Offline & Responsive PWA
- Angular Service Worker ve IndexedDB (Dexie.js) ile çevrimdışı çalışma desteği.
- İnternet geldiğinde local kuyruktaki komutlar *Idempotency Key* ile Backend'e senkronize edilecektir.

### C. Dinamik OLAP Raporlama
- Mevcut `IQueryableExtensions` genişletilerek dinamik `Select` ve `GroupBy`. Gerekirse CQRS Read/Write split.

### D. Doküman Yönetimi
- Fiziksel dosyalar S3/MinIO Blob depolama ünitelerinde. Backend sadece Presigned URL ve Metadata yönetimi.

### E. Server-Driven UI (Dinamik Dashboard)
- Dashboard Angular'da hard-coded olmayacak, Backend JSON konfigürasyonu ile dinamik render (`ViewContainerRef`).

---

## 8. İyileştirme Yol Haritası (Improvement Backlog)

Mimari analiz sonucunda tespit edilen ve planlanan iyileştirmeler:

### 🔴 Yüksek Öncelik

| # | Alan | Açıklama | Durum |
|---|---|---|---|
| 1 | **Structured Logging** | Correlation ID propagation, Seq/ELK entegrasyonu — her request zinciri boyunca izlenebilirlik | ⬜ Planlandı |
| 2 | **Frontend Type Safety** | `any` tipi kullanımlarının kaldırılması, tüm entity'ler için model interface'leri, strict TS config | ⬜ Planlandı |
| 3 | **Frontend Reusability** | `GenericGridComponent` ve `GenericFormComponent` olgunlaştırma — her modül config bazlı çalışmalı | ⬜ Planlandı |
| 4 | **IHttpClientFactory** | `KeycloakAdminService` socket exhaustion riskinin giderilmesi | ⬜ Planlandı |

### 🟡 Orta Öncelik

| # | Alan | Açıklama | Durum |
|---|---|---|---|
| 5 | **Outbox Pattern** | Retry count, max retry limiti, Dead Letter mekanizması, multi-instance concurrency koruması | ⬜ Planlandı |
| 6 | **IQueryableExtensions** | Property name whitelist validation, hata loglama, SQL injection koruması güçlendirme | ⬜ Planlandı |
| 7 | **CI/CD Pipeline** | GitHub Actions: build → test → deploy otomasyonu | ⬜ Planlandı |
| 8 | **API Versioning** | URL path versioning (`/api/v1/`) | ⬜ Planlandı |
| 9 | **Rate Limiting** | API katmanında rate limiting middleware | ⬜ Planlandı |

### 🟢 Düşük Öncelik (Vizyon)

| # | Alan | Açıklama | Durum |
|---|---|---|---|
| 10 | **Test Projesi** | Unit test (Domain + Application) ve Integration test (API) projeleri | ⬜ En son eklenecek |
| 11 | **Polly Resilience** | Keycloak ve dış servis çağrıları için retry/circuit-breaker | ⬜ Planlandı |
| 12 | **Health Check** | Keycloak connectivity check eklenmesi | ⬜ Planlandı |
| 13 | **Configuration Validation** | `IOptions<T>` + `IValidateOptions<T>` ile startup fail-fast | ⬜ Planlandı |

---

> **Not:** Bu doküman Yaşayan Dokümantasyon (Living Documentation) kuralına tabiidir. Altyapıda yapılan her büyük değişiklik veya yeni bir Mimari Karar (ADR) bu dosyaya eklenecektir. İyileştirme Yol Haritası tamamlandıkça `⬜ Planlandı` → `✅ Tamamlandı` olarak güncellenecektir.
