# Enterprise Framework Sistem Kurulum ve Test Rehberi

Bu doküman, projeyi sıfırdan ayağa kaldırıp Keycloak ve veritabanı ayarlarını yaparak test aşamasına geçmeniz için adım adım bir rehberdir.

## 1. Altyapı Servislerini Ayağa Kaldırma (Docker)

Veritabanlarını ve Keycloak sunucusunu başlatmak için ana dizinde terminal açarak şu komutu çalıştırın:
```bash
docker-compose up -d
```
> [!NOTE]
> Bu işlem; **MSSQL** (Uygulama DB), **PostgreSQL** (Keycloak DB) ve **Keycloak** sunucusunu ayağa kaldırır. Konteynerlerin tam olarak hazır olması birkaç dakika sürebilir.

---

## 2. Keycloak Yapılandırması

Servisler başladıktan sonra Keycloak Admin paneline erişin ve uygulamamızın kimlik denetimi altyapısını kurun.

### Adım 2.1: Keycloak Paneline Giriş
*   **URL:** `http://localhost:8080`
*   **Kullanıcı Adı:** `enterprise_admin`
*   **Şifre:** `Enterprise.FrameworkAdmin123!`

### Adım 2.2: Realm Oluşturma
Sol üst köşedeki *Master* menüsüne tıklayıp **Create Realm** deyin.
*   **Realm Name:** `enterprise-realm`
> [!IMPORTANT]
> Harf büyüklüğüne ve tirelere dikkat edin. Birebir `enterprise-realm` olmalıdır.

### Adım 2.3: Backend İçin Client Oluşturma (API)
*Clients* menüsüne gidin ve **Create client** butonuna tıklayın.
*   **Client ID:** `enterprise-api`
*   **Client Authentication (Confidential):** `On` (Açık)
*   **Authorization:** `On` (Açık)
*   *Authentication Flow:* Sadece **Service accounts roles** ve **Standard flow** işaretli olabilir (Backend için genellikle Service Account yeterlidir).
*   Kaydettikten sonra üstteki **Credentials** sekmesine gidin. Oradaki **Client Secret** değerini kopyalayıp, API projenizin `appsettings.json` dosyasındaki `Keycloak:credentials:secret` ve `AdminClientSecret` alanlarına yapıştırın. (Eğer secret'ınız `oh1s0XqihlOBhfzJgmzS6b2Zg33TTmkf` ise değiştirmeye gerek yok, ama yeni oluşursa `appsettings.json` güncellenmeli).

### Adım 2.4: Frontend İçin Client Oluşturma (UI)
Tekrar *Clients* menüsüne gidin ve **Create client** butonuna tıklayın.
*   **Client ID:** `enterprise-ui`
*   **Client Authentication:** `Off` (Kapalı - Public Client olmalı)
*   **Valid Redirect URIs:** `http://localhost:5200/*`
*   **Valid Post Logout Redirect URIs:** `http://localhost:5200/*`
*   **Web Origins:** `http://localhost:5200` veya `*`
Kaydedin.

### Adım 2.5: Kullanıcı ve Rol Tanımlama
*   **Roles:** *Realm Roles* altından sisteme girecek rolleri tanımlayın. (Örn: `Admin`, `User`).
*   **Users:** *Users* menüsünden **Add user** diyerek bir test kullanıcısı oluşturun. 
    *   (Örn: Username: `enterprise_admin`)
    *   Kullanıcıyı kaydettikten sonra **Credentials** sekmesinden şifresini belirleyin (`Temporary` ayarını Off yapın).
    *   **Role Mapping** sekmesinden oluşturduğunuz `Admin` rolünü bu kullanıcıya atayın.

---

## 3. Veritabanı Migrasyonu (Çok Önemli)

Kodda `Location`, `TodoItem`, `Branch` gibi entity'leri sildiğimiz için veritabanı şemasının güncellenmesi gerekiyor. API klasöründeyken şu komutları çalıştırın:

```bash
cd src/Enterprise.Framework.API

# Yeni migrasyon oluştur (Sildiğimiz sınıflar veritabanından düşürülecek)
dotnet ef migrations add RemoveDummyEntities -p ../Enterprise.Framework.Infrastructure -s .

# Veritabanını güncelle
dotnet ef database update
```

---

## 4. Uygulamayı Başlatma

### Backend (API)
Migrasyon sonrası API projesini çalıştırın:
```bash
# src/Enterprise.Framework.API dizinindeyken
dotnet run
```
API `http://localhost:5200` portundan hizmet vermeye başlayacaktır. (Port ayarlarınız farklıysa konsol çıktısını kontrol edin).

### Frontend (UI)
Yeni bir terminal açın ve UI projesini başlatın:
```bash
cd ui
npm install   # Eğer eksik paket uyarısı varsa
npm start
```
Angular uygulaması genellikle `http://localhost:5200` veya `4200` üzerinde ayağa kalkacaktır. (Görselde `5200` kullanıldığı için Keycloak ayarlarını `5200`'e göre verdik).

---

## 5. Test Senaryosu

1. Tarayıcıda `http://localhost:5200` (veya UI nerede çalışıyorsa) adresine gidin.
2. Portal sizi otomatik olarak Keycloak Login sayfasına yönlendirecektir.
3. Keycloak'ta tanımladığınız test kullanıcısı (Adım 2.5) ile giriş yapın.
4. Başarılı giriş sonrası Enterprise Portal'e yönlendirileceksiniz.
5. Varlık (Asset), Personel (Employee) gibi ekranlara girip yeni kayıt ekleyerek tüm standardizasyonun sorunsuz çalıştığını test edebilirsiniz.
