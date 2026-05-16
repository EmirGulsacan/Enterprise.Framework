# Enterprise Framework Boilerplate

**Enterprise Framework Boilerplate**, karmaşık iş kuralları gerektiren büyük ölçekli kurumsal projeler için tasarlanmış, yüksek performanslı ve A-Sınıfı (Tier-1) bir başlangıç şablonudur (Scaffolding). 

## 🌟 Mimari Özellikler (Architecture Overview)

- **Backend:** .NET 8, Clean Architecture, CQRS (MediatR), Repository Pattern
- **Frontend:** Angular 18/19 Standalone, PrimeNG 17, PrimeFlex, Özel Tasarım Premium UI
- **Kimlik ve Güvenlik (Identity):** Keycloak 22+ (OIDC SSO, Rol Tabanlı Erişim Kontrolü)
- **Veritabanı:** Entity Framework Core (SQL Server / PostgreSQL Desteği)
- **Loglama ve İzleme:** Serilog (Yapısal Loglama) & Global Exception Handling
- **Tasarım Deseni:** Modüler, Gevşek Bağlı (Loosely Coupled), Test Edilebilir Mimari

## 🚀 Yeni Projeye Başlarken (Scaffolding Guide)

Bu repository'i yeni bir proje için altlık olarak kullanırken izlemeniz gereken adımlar:

### 1. Klonlama ve Temizlik
Projeyi yeni bir repository olarak klonlayın. İçindeki `.git` klasörünü silip kendi `git init` komutunuzu çalıştırın.

### 2. Namespace (İsim Uzayı) Değişikliği
Eğer projenizin adı `Enterprise.Framework` yerine `Sirketim.IKYS` olacaksa, Visual Studio veya uygun bir araç üzerinden tüm proje dosya isimlerini ve Namespace'leri güncelleyin.

### 3. Veritabanı ve Keycloak Konfigürasyonları
`src/Enterprise.Framework.API/appsettings.Development.json` içerisindeki yer tutucuları (`<YOUR_DB_PASSWORD>`, vb.) kendi geliştirme ortamınıza göre ayarlayın.

#### Keycloak Kurulum Adımları
1. Keycloak'u Docker veya lokal sunucunuzda ayağa kaldırın (Örn: `http://localhost:8080`).
2. Yeni bir Realm oluşturun (Örn: `enterprise-realm`).
3. İki adet Client oluşturun:
   - `enterprise-api` (Client authentication açık, Service Accounts enabled). **Client Secret**'ı kopyalayıp `appsettings.Development.json` içerisindeki `Keycloak:credentials:secret` ve `Keycloak:AdminClientSecret` alanlarına yapıştırın.
   - `enterprise-ui` (Standard Flow enabled, Valid Redirect URIs kısmına UI linkinizi `http://localhost:40744/*` ve Web Origins kısmına `+` ekleyin).
4. `enterprise_admin` adında bir kullanıcı oluşturun ve parolasını belirleyin.

### 4. İlk Çalıştırma (Automated Seeding)
Backend projesini çalıştırdığınızda (`dotnet run`):
- Veritabanı tabloları otomatik oluşur (EF Core Migrations).
- **Otomatik Seeder** devreye girer:
  - Keycloak Realm ayarlarında `editUsernameAllowed` otomatik açılır.
  - UI client'ı için `post_logout_redirect_uri` ayarları otomatik yapılandırılır.
  - Lokal veritabanında tüm modüller için (`Identity.Users.View`, `Identity.Roles.Write` vb.) yetki (Permission) listesi otomatik taranır.
  - `admin` ve `framework-admin` Keycloak rollerine bu yetkiler lokal veritabanında fiziksel olarak otomatik tanımlanır.

Yani tek bir `dotnet run` ile tüm sistem yetkileri sıfırdan sizin için dokunulmuş gibi hazır hale gelir!

## 📘 Geliştirici Rehberi (Developer Playbook)

### Yeni Bir Modül (Feature) Eklemek
Uygulama **CQRS** ile çalıştığı için her yeni işlem bağımsız bir Feature olarak ele alınır.
1. `Enterprise.Framework.Domain/Entities` içerisine yeni varlığınızı (Entity) ekleyin.
2. `Enterprise.Framework.Application/Features/[ModülAdı]` klasörü oluşturun (Örn: `Features/Products`).
3. İçerisinde `Queries` ve `Commands` klasörleri açıp `CreateProductCommand`, `GetProductsQuery` sınıflarınızı ve Handle metotlarınızı yazın.
4. UI tarafında `pages/[modul-adi]` klasörü açıp Angular Standalone component'lerinizi oluşturun.

### Yetki (Permission) Mantığı Nasıl Çalışır?
Yeni bir modül eklediğinizde UI tarafında `authService.hasPermission('Product.View')` diyerek yetki kontrolü yapabilirsiniz. `AuthGuard` ve `PermissionGuard` rotaları korur.
Backend'e yeni bir yetki string'i eklediğinizde, uygulamanın ilk açılışında `PermissionSeeder` onu otomatik bulup DB'ye ekler ve Admin rollerine tanımlar. Yöneticiler UI üzerindeki "Rol ve Yetkiler" ekranından diğer kullanıcı rollerine bu yetkileri dağıtabilir.

---
*Bu Boilerplate, Google Deepmind ekibinin asistanlık desteği ile kurum standartlarına en uygun şekilde sıfır hata prensibiyle derlenmiştir.*
