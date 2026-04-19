# Enterprise.Framework - Clean Architecture

Bu proje temel Clean Architecture katmanlarini su sekilde ayirir:

- `Enterprise.Framework.Domain`: Cekirdek is kurallari, entity ve interface tanimlari.
- `Enterprise.Framework.Application`: Use-case seviyesinde uygulama kurallari.
- `Enterprise.Framework.Infrastructure`: Dis bagimliliklar ve teknik implementasyonlar.
- `Enterprise.Framework.API`: HTTP endpointleri ve uygulamanin giris noktasi.

## Klasor Yapisi

```text
Enterprise.Framework
├─ Enterprise.Framework.sln
└─ src
   ├─ Enterprise.Framework.Domain
   ├─ Enterprise.Framework.Application
   ├─ Enterprise.Framework.Infrastructure
   └─ Enterprise.Framework.API
```

