# Enterprise Framework - Architecture Document (Living Documentation)

Bu doküman, sistemin temel prensiplerini, katmanlı yapısını ve tasarım standartlarını (Design Patterns) tanımlar. Projenin "Anayasası" niteliğinde olup, her yeni özellik geliştirilmesinde referans alınacak tek kaynaktır (Single Source of Truth).

## 1. Temel Prensipler (Core Principles)
- **SOLID Prensipleri:** Bağımlılıkların tersine çevrilmesi (Dependency Inversion) ve tek sorumluluk (Single Responsibility) esas alınır.
- **Clean Architecture:** Altyapı (Infrastructure) veya Arayüz (UI) kararları, Merkezi İş Mantığını (Domain & Application) asla etkilemez.
- **Sıfır Amelelik (Zero-Boilerplate):** Tekrarlayan kodlar merkezileştirilmiştir (Generic Grid, IQueryableExtensions vb.). Yeni bir CRUD modülü eklemek maksimum 10 dakika sürmelidir.
- **Fail-Fast (Hızlı Hata Ver):** Validasyon hataları veya yetkisizlik durumları sistemin derinliklerine inmeden en dış katmanda (Interceptor/Behavior) reddedilir.

---

## 2. Backend Mimarisi (C# .NET 8)

Backend, sıkı bir **Clean Architecture** kurallarına göre 4 ana katmana bölünmüştür:

### A. Domain Katmanı (`Enterprise.Framework.Domain`)
- **İçerik:** Sistemdeki tüm varlıklar (Entities), Enumaration'lar ve temel kurallar buradadır.
- **Kural:** Asla ve asla başka hiçbir katmana referans (dependency) barındırmaz. Entity'ler `AuditableEntity` veya `BaseEntity` den miras alır.

### B. Application Katmanı (`Enterprise.Framework.Application`)
- **İçerik:** Uygulamanın iş mantığı (Business Logic), CQRS (MediatR) desenine uygun Command ve Query'ler.
- **Kural:** Sadece Domain katmanına referans verir. Dış dünya (DB, HTTP) hakkında hiçbir fikri yoktur.
- **Mekanizmalar:**
  - `ValidationBehavior`: FluentValidation ile request'leri otomatik doğrular.
  - `IQueryableExtensions`: Dinamik filtreleme ve sıralamayı (System.Linq.Dynamic.Core) otomatik uygular.

### C. Infrastructure Katmanı (`Enterprise.Framework.Infrastructure`)
- **İçerik:** Entity Framework Core (Veritabanı işlemleri), Keycloak (Kimlik ve Yetkilendirme), Dış Servis Entegrasyonları.
- **Kural:** Application katmanındaki arayüzleri (Interface) implemente (uygulama) eder.

### D. API Katmanı (`Enterprise.Framework.API`)
- **İçerik:** Sadece dış dünyaya açılan Minimal API Endpoints (Örn: `EmployeeEndpoints.cs`).
- **Kural:** İçinde kesinlikle iş mantığı (If/Else logic) veya Entity bulunamaz. Sadece Request'i alır, MediatR üzerinden `Sender.Send()` ile Application'a iletir ve standart `ApiResponse` veya `PagedResult` formatında döner.

---

## 3. Frontend Mimarisi (Angular & PrimeNG)

Frontend, **Clean UI Architecture** prensiplerine göre yapılandırılmıştır.

### A. Katmanlı Yapı
- **Services (Veri ve API):** `ApiService` gibi servisler, backend ile iletişimi kurar. Component'ler asla doğrudan `HttpClient` çağırmaz.
- **Components (Sayfalar ve UI):** Sadece veri gösteriminden (Data Binding) ve kullanıcı olaylarından (Event Handling) sorumludur. Karmaşık iş kuralları servislerde yaşar.
- **Shared Components:** `GenericGridComponent` gibi tekrar kullanılabilir bileşenler, listeleme, sayfalama ve filtrelemeyi standart hale getirir.

### B. İletişim Standartları
- **Global Interceptor:** `loading.interceptor.ts` ve `error.interceptor.ts` ile tüm HTTP istekleri merkezi yönetilir.
- **Dinamik Grid İletişimi:** Sayfalamada backend'e özel yapılandırılmış `FiltersJson` gönderilir. Global yükleme ekranını (overlay) grid üzerinde engellemek için `X-Skip-Loading` header'ı kullanılır.

---

## 4. Güvenlik ve Kimlik Yönetimi (PBAC & Keycloak)

- **PBAC (Policy-Based Access Control):** Yetkilendirme Rol bazlı değil, İzin bazlıdır (Örn: `Assets.Write`, `Users.View`).
- **Keycloak Integration:** Tüm kimlik doğrulaması (Authentication) Keycloak üzerinden yapılır. Token (JWT) içindeki Roller, uygulama tarafında izinlere çevrilir.
- **Ghost Admin:** Sistemin çökmesi veya süper yöneticinin silinmesi durumunda, konfigürasyon (`appsettings.json`) dosyasındaki `BootstrapAdminEmail` hesabı her zaman tüm yetkilere sahiptir ve silinemez.

---

## 5. Gelecek Mimari Vizyonu (Phase 2 Roadmap)

Sistemin "İleri Düzey" (Advanced) gereksinimleri için belirlenmiş mimari stratejiler şunlardır:

### A. Dış Entegrasyonlar (Integration Bus)
- **Anti-Corruption Layer (ACL):** Core Domain, dış dünyayı asla bilmeyecektir.
- **Haberleşme:** RabbitMQ/Kafka gibi mesaj kuyrukları (MassTransit aracılığıyla) veya HTTP servisleri (Refit aracılığıyla) kullanılacaktır.

### B. Offline & Responsive PWA
- **Mimari:** Angular Service Worker ve IndexedDB (Dexie.js) ile çevrimdışı çalışma desteği eklenecektir.
- **CQRS Sync:** İnternet bağlantısı geldiğinde, local kuyruktaki komutlar *Idempotency Key* (Tekillik Anahtarı) kullanılarak Backend'e senkronize edilecektir.

### C. Dinamik OLAP Raporlama
- **Mimari:** OData/GraphQL yerine mevcut `IQueryableExtensions` altyapısı genişletilerek dinamik JSON tabanlı `Select` ve `GroupBy` özellikleri eklenecektir. Gerekirse veri tabanı Read/Write olarak ayrılacaktır (CQRS Split).

### D. Doküman Yönetimi
- **Depolama:** Fiziksel dosyalar veritabanında değil S3/MinIO gibi Blob depolama ünitelerinde tutulacaktır. Backend sadece Presigned URL ve Metadata yönetimini üstlenecektir.

### E. Server-Driven UI (Dinamik Dashboard)
- **Mimari:** Yönetici dashboard'u Angular tarafında hard-coded olmayacak, Backend'den gelen JSON konfigürasyonuna göre dinamik olarak (Dumb Component Renderer) çalışma anında (`ViewContainerRef` ile) çizilecektir.

---

> **Not:** Bu doküman Yaşayan Dokümantasyon (Living Documentation) kuralına tabiidir. Altyapıda yapılan her büyük değişiklik veya yeni bir Mimari Karar (ADR) bu dosyaya eklenecektir.
