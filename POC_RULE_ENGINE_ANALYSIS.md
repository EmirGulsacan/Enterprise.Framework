# Kural Motoru (Rule Engine) PoC - Mimari Analiz ve Otopsi Raporu

Bu rapor, Enterprise Framework için geliştirilen Generic Rule Engine PoC'sinin mimari kararlarını, ulaşılan hedefleri ve sistemin potansiyel zafiyetlerini (trade-offs) acımasızca değerlendiren bir otopsi dökümanıdır.

## 1. 🎯 Hedef Gerçekleşme Durumu

**"Domain-Agnostic, Yüksek Performanslı ve Clean Architecture Uyumlu bir kural motoru"** hedefimize ulaştık mı?

- **Domain-Agnostic:** Kısmen Başarılı. Kuralların JSON olarak veritabanında saklanması ve dinamik sözlüklerle (`Dictionary<string, object>`) çalışılması sayesinde, kural motoru spesifik bir Entity'ye bağımlı olmaktan çıkarıldı.
- **Yüksek Performans:** Başarılı. Fast-path API ve memory cache mekanizması ile veritabanı turu engellendi, milisaniyelik yanıt süreleri elde edildi.
- **Clean Architecture Uyumlu:** Başarılı. `Microsoft.RulesEngine` doğrudan API veya Application katmanına sızdırılmadı. Adapter Pattern (`IRuleEvaluator`) kullanılarak dış kütüphane bağımlılığı altyapıya hapsedildi. Admin işlemleri CQRS (MediatR) ile API'ye sunuldu.

## 2. 🟢 Sistemin Artıları (Pros & Strengths)

- **Performans Optimizasyonu (Fast-Path & Cache):** Evaluator, her istekte veritabanına gitmek yerine `IMemoryCache` üzerinden derlenmiş kural ağaçlarını okuyarak yüksek okuma ve değerlendirme (throughput) performansı sağlar.
- **Mimari Esneklik ve Soyutlama:** `IRuleEvaluator` arayüzü sayesinde yarın `Microsoft.RulesEngine` yerine `NRules`, `RulesEngine` (farklı bir fork) veya kendi yazdığımız bir AST motoru ile değiştirmek istediğimizde hiçbir Domain veya Application kodunu değiştirmemize gerek kalmayacak.
- **Yönetilebilirlik (CQRS ve JSON Storage):** Kural tanımlarının Admin endpoint'leri ile (MediatR) CRUD operasyonlarına açılması, kuralların JSON olarak kolayca versionlanıp veritabanında tutulabilmesi sistemin yönetimini çok kolaylaştırıyor.

## 3. 🔴 Sistemin Eksileri ve Riskleri (Cons, Trade-offs & Risks)

Sisteme mimar şapkasıyla baktığımızda, production ortamında bizi bekleyen ciddi riskler mevcuttur:

- **Dağıtık Sistemlerde Cache Tutarsızlığı (Cache Inconsistency):** 
  Sistemde şu an `IMemoryCache` kullanıldı. Uygulama bir Kubernetes veya Docker Swarm kümesinde 5 instance (Pod) olarak çalıştığında, bir admin `PUT /api/rule-definitions/{code}` ile bir kuralı güncellerse, isteği karşılayan Pod'un cache'i temizlenecek ancak diğer 4 Pod eski kuralı bellekten işletmeye devam edecektir. Bu durum "Split-Brain" veya tutarsız iş mantığı işletilmesine yol açacaktır.
- **Debugging (Hata Ayıklama) Zorluğu - Kara Kutu Problemi:**
  Veritabanından okunan JSON'lar `Microsoft.RulesEngine` tarafından çalışma zamanında Lambda Expression'lara (Expression Trees) dönüştürülüp derlenir. Eğer kural içinde karmaşık bir mantık hatası veya NullReference hatası oluşursa, klasik `try-catch` veya breakpoint'lerle hatanın kök nedenini bulmak imkansıza yakındır. Hangi kural düğümünün (node) kırıldığını loglamak mevcut mimaride ciddi bir kör noktadır.
- **Single Point of Failure (Performans Darboğazı) Eğilimi:**
  Kural motoru çok hızlı çalışsa da, `IRuleEvaluator`'a gönderilen bağlam (`RuleContext`) verileri eğer dış sistemlerden (`IContextEnricher` üzerinden) senkronize olarak her seferinde çekilirse (örneğin kuralın işlemesi için SAP'den kullanıcı bakiyesi sorulursa), bu kural motorunun tüm performans kazanımını yok edecek ve I/O kaynaklı bir darboğaz yaratacaktır.
- **UI Kompleksitesi ve Bakım Maliyeti:**
  Angular tarafında Microsoft.RulesEngine'in iç içe geçmiş (nested) JSON formatını %100 kapsayacak, AND/OR operatörlerini, lambda ifadelerini ve nested nesneleri görsel olarak yönetecek bir sürükle-bırak UI yazmak inanılmaz zordur. Genellikle backend'in kabul ettiği esnek yapı ile UI'ın sunabildiği özellikler arasında uçurum oluşur ve "Rule Builder" UI'ı her yeni operatör ihtiyacında patlamaya yatkındır.

## 4. 🚀 Gelecek İyileştirmeler (Next Steps / V2)

Production ortamına çıkmadan (veya V2'de) acilen çözülmesi gereken mimari güncellemeler:

1. **Distributed Cache Invalidation (MassTransit / Redis Pub-Sub):**
   Kural admin tarafından güncellendiğinde, RabbitMQ/Kafka veya Redis Pub/Sub üzerinden bir `RuleUpdatedIntegrationEvent` fırlatılarak tüm Pod'ların kendi local memory cache'lerini eşzamanlı olarak düşürmesi (invalidate) sağlanmalıdır. (Veya Redis Cache'e geçilmelidir, ancak Expression Tree'ler Redis'te saklanamayacağı için hibrid bir "Local Memory Cache + Redis Pub/Sub Invalidation" patterni şarttır).
2. **Rule Execution Telemetry & Tracing:**
   Kuralın hangi yoldan geçtiğini ve hangi kararları aldığını izleyebilmek için OpenTelemetry veya detaylı Audit Logging eklenmelidir. Örneğin: `Rule 'DiscountRate' evaluated to True via Path: A -> B -> C`. Hata ayıklama için Rule Engine'in ürettiği Diagnostic hata mesajları Elasticsearch gibi bir ortama aktarılmalıdır.
3. **Versiyonlama (Rule Versioning):**
   Bir kural güncellendiğinde eskisi silinmemeli (Soft Delete dahi yetersizdir). Aktif kuralın eski versiyonları veritabanında `Version=1`, `Version=2` şeklinde tutulmalıdır. Geçmişe yönelik bir işlemin hangi kuralla işletildiği audit edilebilmelidir.
4. **Context Enrichment Stratejisi:**
   `IContextEnricher` tarafındaki dış veri talepleri, kural işletilmeden önce paralelize edilerek (Task.WhenAll) asenkron olarak toplanmalı ve sonrasında Rule Engine'e enjekte edilmelidir, böylece motor içinde I/O bloğu oluşması engellenir.
5. **DSL (Domain Specific Language) Sınırlandırması:**
   UI tarafının yükünü hafifletmek için, kural motoruna sınırsız C# expression yazma yetkisi vermek yerine; UI tarafından yönetilebilecek kadar kısıtlanmış ve önceden belirlenmiş (Pre-defined) operatörler kümesi ile kuralların oluşturulmasına izin verilmelidir.
