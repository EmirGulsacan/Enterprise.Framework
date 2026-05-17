# 🧠 Generic Enterprise Rule Engine — PoC Planı

> **Durum:** Planlama Aşaması  
> **Oluşturma Tarihi:** 2026-05-17  
> **Hedef:** Enterprise.Framework üzerine, alandan bağımsız (Domain-Agnostic), tak-çalıştır (plug-in) mimaride evrensel bir Dinamik Kural Motoru geliştirmek.

---

## 🎯 Vizyon

Her türlü kurumsal projeye (İK, Finans, Lojistik vb.) entegre edilebilecek, kuralların veritabanında JSON formatında saklandığı, runtime'da güncellenip değerlendirilebildiği, Clean Architecture uyumlu bir Generic Rule Engine.

---

## 📐 Mimari Kararlar (Architecture Decision Records — ADR)

### ADR-01: Çekirdek Motor — `Microsoft.RulesEngine`

| Karar | Detay |
|---|---|
| **Seçim** | MIT lisanslı `Microsoft.RulesEngine` NuGet paketi |
| **Gerekçe** | Sıfırdan AST derleyicisi yazmak yerine, production-ready, battle-tested bir kütüphane kullanılacak |
| **Alternatif (Reddedildi)** | Custom AST parser — aşırı kompleks, bakımı zor |

### ADR-02: Clean Architecture Uyumu — Katman Kısıtlaması

| Karar | Detay |
|---|---|
| **Kural** | `Microsoft.RulesEngine` NuGet paketi **KESİNLİKLE** `Application` veya `Domain` katmanına eklenmeyecek |
| **Uygulama** | Paket yalnızca `Infrastructure` katmanına kurulacak |
| **Desen** | **Adapter Pattern** — `Application` katmanı yalnızca `IRuleEvaluator` arayüzünü tanır; implementasyonu bilmez |

```
Domain         → IRuleEvaluator (interface)  [sadece sözleşme]
Application    → IRuleEvaluator kullanır     [iş mantığı]
Infrastructure → MicrosoftRulesEngineAdapter [gerçek implementasyon]
```

### ADR-03: Depolama Stratejisi — JSON-First

| Karar | Detay |
|---|---|
| **Seçim** | Tek bir `RuleDefinition` Domain Entity; kural şeması JSON olarak saklanır |
| **Reddedilen** | İlişkisel tablo yapısı (kural satırları, koşul satırları vb.) |
| **Gerekçe** | Kural şeması değişken yapıda olduğundan, ilişkisel modelleme aşırı rijit ve bakımı güç olur. JSON sütunu hem esnek hem de sorgulanabilir |

```
RuleDefinitions tablosu:
  Id          GUID
  Name        NVARCHAR
  Domain      NVARCHAR       -- "HR", "Finance", "Logistics" vb.
  IsActive    BIT
  JsonSchema  NVARCHAR(MAX)  -- Microsoft.RulesEngine'in beklediği JSON formatı
  CreatedAt   DATETIME2
  UpdatedAt   DATETIME2
```

### ADR-04: Evrensel Girdi — `RuleContext`

| Karar | Detay |
|---|---|
| **Seçim** | Kurallar spesifik DTO'larla değil, `Dictionary<string, object>` veya `dynamic` tabanlı `RuleContext` ile çalışır |
| **Gerekçe** | Motor alana bağımsız (domain-agnostic) kalır; her projeden kurallar beslenebilir |
| **Veri Zenginleştirme** | İstek motora girmeden önce harici bir `IContextEnricher` servisi ile zenginleştirilebilir |

### ADR-05: Performans — Fast-Path Evaluation

| Karar | Detay |
|---|---|
| **Seçim** | `Evaluate` endpoint'i standart ağır MediatR pipeline'ından muaf tutulacak |
| **Uygulama** | Minimal API veya Controller'da doğrudan `IRuleEvaluator` inject edilecek; MediatR handler yazılmayacak |
| **Gerekçe** | Kural değerlendirme sıkça çağrılan, latency-sensitive bir işlemdir; middleware overhead'i minimize edilmeli |

---

## 🗺️ Master Todo Listesi (Yol Haritası)

### ✅ FAZ 0: Planlama (Tamamlandı)
- [x] Mimari vizyon dokümante edildi
- [x] ADR kararları yazıldı
- [x] POC_RULE_ENGINE_PLAN.md oluşturuldu

---

### ✅ FAZ 1: Domain ve DB Şeması

> **Hedef:** `RuleDefinition` entity'sini ve EF Core konfigürasyonunu oluşturmak.

- [x] `RuleDefinition` Domain Entity sınıfı (`Domain/Rules/RuleDefinition.cs`)
  - `Id` (BaseEntity'den), `Name`, `Code`, `Domain`, `Description`, `IsActive`, `WorkflowJson` + AuditableEntity alanları
- [x] EF Core `IEntityTypeConfiguration<RuleDefinition>` konfigürasyonu (`Infrastructure/Persistence/Configurations/RuleDefinitionConfiguration.cs`)
  - `WorkflowJson` için `HasColumnType("nvarchar(max)")`, `Code` için unique index
- [x] `AppDbContext`'e `DbSet<RuleDefinition> RuleDefinitions` eklendi
- [ ] EF Core Migration oluşturma ve uygulama *(FAZ 2 öncesi yapılacak)*
- [ ] Seed data: Örnek bir kural JSON şeması *(FAZ 4'te eklenecek)*

---

### ✅ FAZ 2: Kural Motoru Adaptasyonu

> **Hedef:** `IRuleEvaluator` arayüzü ve `Microsoft.RulesEngine` Adapter'ını yazmak.

- [x] `Application` katmanında `IRuleEvaluator` arayüzü tanımlandı (`Application/RuleEngine/IRuleEvaluator.cs`)
  - Metod: `Task<RuleEvaluationResult> EvaluateAsync(string workflowName, string workflowJson, RuleContext context)`
- [x] `Application/RuleEngine/RuleContext.cs` — `Dictionary<string, object>`'ten miras alan evrensel girdi nesnesi
- [x] `Application/RuleEngine/RuleEvaluationResult.cs` — `IsSuccess`, `TriggeredRules`, `Errors`
- [x] `Infrastructure` katmanına `RulesEngine 6.0.0` NuGet paketi eklendi
- [x] `Infrastructure/RuleEngine/MicrosoftRulesEngineAdapter.cs` — `IRuleEvaluator` implementasyonu
  - JSON → `Workflow[]` deserializasyonu (`System.Text.Json`)
  - `RuleContext` → `ExpandoObject` dönüşümü
  - `ExecuteAllRulesAsync` → `RuleEvaluationResult` map
- [x] `DependencyInjection.cs`'e `AddTransient<IRuleEvaluator, MicrosoftRulesEngineAdapter>()` eklendi

---

### ✅ FAZ 3: Veri Zenginleştirme ve Fast-Path Evaluation API

> **Hedef:** Yüksek performanslı kural değerlendirme endpoint'i.

- [x] `Application/RuleEngine/IContextEnricher.cs` — opsiyonel veri zenginleştirme arayüzü
- [x] `API/Endpoints/RuleEngine/EvaluateRuleRequest.cs` — `RuleCode` + `ContextData` request DTO'su
- [x] `API/Endpoints/RuleEngine/RuleEvaluationEndpoints.cs` — `POST /api/rules/evaluate`
  - MediatR pipeline tamamen **bypass** — doğrudan `IRuleEvaluator` inject
  - `AsNoTracking()` ile `Code` bazlı DB sorgusu
  - `IContextEnricher` opsiyonel olarak `IServiceProvider` üzerinden resolve
  - `MapAllEndpoints` reflection'ı sayesinde Program.cs değiştirilmedi
- [x] Tüm Faz 1-2 dosyalarındaki yorum/summary satırları temizlendi
- [ ] Performans testi ve benchmark *(FAZ 4 sonrası)*

---

### ✅ FAZ 4: Admin CQRS ve Cache Invalidation

> **Hedef:** Kural yönetimi için CQRS komutları ve önbellek temizleme.

- [x] `IRuleEvaluator` imzası sadeleştirildi — `workflowJson` kaldırıldı, adapter DB+Cache yönetimini üstlendi
- [x] `MicrosoftRulesEngineAdapter` güncellendi — `IApplicationDbContext` + `IMemoryCache` inject, 30 dk TTL ile cache
- [x] `RuleEvaluationEndpoints` refactor edildi — DbContext bağımlılığı tamamen kaldırıldı
- [x] `Application/RuleEngine/Commands/CreateRuleDefinitionCommand.cs` — yeni kural oluşturma
- [x] `Application/RuleEngine/Commands/UpdateRuleDefinitionCommand.cs` — kural güncelleme + `cache.Remove`
- [x] `Application/RuleEngine/Commands/DeleteRuleDefinitionCommand.cs` — soft-delete (`IsActive=false`) + `cache.Remove`
- [x] `IRuleEvaluator` DI kaydı `Transient` → `Scoped` olarak düzeltildi (IApplicationDbContext uyumu)
- [ ] `GetRuleDefinitionsQuery` — Admin listeleme *(Faz 5 Angular UI ile birlikte)*

---

### ✅ FAZ 5: Angular UI — Dinamik Kural Oluşturucu

> **Hedef:** Admin panelinde kural oluşturma ve yönetim arayüzü.

- [x] `features/rule-engine/rule-engine.service.ts` — `RuleDefinition`, `CreateRuleCommand`, `UpdateRuleCommand`, `MicrosoftWorkflow`, `MicrosoftRule` modelleri + HTTP servisi
- [x] `features/rule-engine/rule-builder.component.ts` — FormBuilder + FormArray ile dinamik şart yönetimi, save() → MicrosoftWorkflow JSON oluşturma
- [x] `features/rule-engine/rule-builder.component.html` — PrimeNG Table (liste) + Dialog (oluştur/düzenle) + FormArray döngüsü (Şart Adı, Mantıksal İfade, Başarı Aksiyonu)
- [x] `features/rule-engine/rule-builder.component.scss` — minimal host stili
- [x] `app.routes.ts`'e `/management/rule-engine` lazy-loaded route eklendi
- [x] TypeScript derleme: **0 hata**

---

## 📁 Beklenen Proje Yapısı (Hedef)

```
ISGYS/
├── src/
│   ├── Domain/
│   │   └── RuleEngine/
│   │       └── RuleDefinition.cs          [FAZ 1]
│   │
│   ├── Application/
│   │   └── RuleEngine/
│   │       ├── IRuleEvaluator.cs          [FAZ 2]
│   │       ├── RuleContext.cs             [FAZ 2]
│   │       ├── RuleEvaluationResult.cs    [FAZ 2]
│   │       └── Commands/                  [FAZ 4]
│   │
│   ├── Infrastructure/
│   │   └── RuleEngine/
│   │       ├── MicrosoftRulesEngineAdapter.cs  [FAZ 2]
│   │       └── RuleDefinitionConfiguration.cs  [FAZ 1]
│   │
│   └── API/
│       └── RuleEngine/
│           └── RuleEvaluationEndpoints.cs      [FAZ 3]
│
└── POC_RULE_ENGINE_PLAN.md                     [Bu dosya]
```

---

## ⚠️ Kritik Kısıtlamalar (Değiştirilemez)

1. `Microsoft.RulesEngine` paketi **yalnızca** `Infrastructure` katmanında olacak.
2. Kural koşulları için **ayrı ilişkisel tablo kurulmayacak**; tüm şema JSON'da tutulacak.
3. `Evaluate` endpoint'i MediatR pipeline'ını kullanmayacak (Fast-Path).
4. Motor her zaman `RuleContext` (Dictionary/dynamic) alacak; domain-specific DTO almayacak.

---

## 📚 Referanslar

- [Microsoft.RulesEngine GitHub](https://github.com/microsoft/RulesEngine)
- [Microsoft.RulesEngine NuGet](https://www.nuget.org/packages/RulesEngine)
- [Microsoft.RulesEngine Lisansı: MIT](https://github.com/microsoft/RulesEngine/blob/main/LICENSE)

---

*Bu dosya yaşayan bir dokümandır. Her faz tamamlandığında ilgili checkbox güncellenecektir.*
