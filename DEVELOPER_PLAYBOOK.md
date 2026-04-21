# ISGYS - Kurumsal Geliştirici Rehberi (Developer Playbook)

Bu doküman, projeye yeni katılan veya yeni bir özellik (modül) geliştirecek olan yazılımcıların **"Sisteme yeni bir modülü sıfırdan ve standartlara uygun olarak nasıl eklerim?"** sorusuna cevap vermek için hazırlanmıştır. 

Aşağıdaki adımlar; veritabanı entity'sinin oluşturulmasından, arayüzdeki profesyonel tablonun (Grid) çizilmesine kadar tüm süreci uçtan uca anlatmaktadır. **Lütfen tüm geliştirmelerde bu standartlara (CQRS, Minimal API, ApiResponse, PBAC ve PrimeNG Grid) kesinlikle uyunuz.**

---

## ADIM 1: Veritabanı (Entity ve Migration)

Yeni bir modül geliştirirken işe daima en alt katmandan, yani çekirdekten (`Domain` katmanı) başlayın.

### 1.1. Entity Sınıfının Oluşturulması
`Enterprise.Framework.Domain/Entities/` klasörü altına yeni Entity'nizi ekleyin. 
**Kural:** Sistemdeki her entity, kimin ne zaman oluşturduğunu/güncellediğini takip etmek için `BaseAuditableEntity` sınıfından miras almalıdır.

```csharp
using Enterprise.Framework.Domain.Common;

namespace Enterprise.Framework.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
```

### 1.2. AppDbContext'e Eklenmesi
`Enterprise.Framework.Infrastructure/Data/AppDbContext.cs` dosyasına giderek yeni sınıfınızı tablo (`DbSet`) olarak tanıtın:
```csharp
public DbSet<Customer> Customers => Set<Customer>();
```

### 1.3. Migration Alınması
Package Manager Console (veya Terminal) üzerinden yeni tablonuzu veritabanına yansıtın:
```bash
# Terminal (Backend klasöründe):
dotnet ef migrations add AddCustomerTable --project src/Enterprise.Framework.Infrastructure --startup-project src/Enterprise.Framework.API
dotnet ef database update --project src/Enterprise.Framework.Infrastructure --startup-project src/Enterprise.Framework.API
```

---

## ADIM 2: Uygulama Katmanı (CQRS ve DTOs)

ISGYS projesinde **MediatR** tabanlı CQRS (Command Query Responsibility Segregation) deseni kullanılmaktadır. Her bir işlem (Create, Update, Delete, GetList) kendi klasöründe olmalıdır.

### 2.1. DTO ve Mapping
`Enterprise.Framework.Application/Customers/Queries/GetCustomers/CustomerDto.cs` adında DTO'nuzu oluşturun ve `IMapFrom` arayüzü ile AutoMapper'a tanıtın:
```csharp
public class CustomerDto : IMapFrom<Customer>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // AutoMapper otomatik olarak Customer -> CustomerDto dönüşümü yapacaktır.
}
```

### 2.2. Query (Okuma) ve Sayfalama (Pagination)
Listeleme işlemleri her zaman `PagedResult` veya `PaginatedList` dönmelidir. Grid performansımız buna bağlıdır.
```csharp
public record GetCustomersQuery : IRequest<ApiResponse<PagedResult<CustomerDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, ApiResponse<PagedResult<CustomerDto>>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(x => x.FirstName.Contains(request.SearchTerm));

        // PagedResult standart sayfalama yardımcısını kullanın
        var paginatedList = await query
            .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // Her zaman ApiResponse dönün!
        return ApiResponse<PagedResult<CustomerDto>>.SuccessResponse(paginatedList);
    }
}
```

---

## ADIM 3: API Katmanı (Carter ve Minimal API)

Projede geleneksel `Controller` mimarisi yerine **Carter** ile Minimal API'ler kullanılmaktadır.

### 3.1. Endpoint Sınıfının Oluşturulması
`Enterprise.Framework.API/Endpoints/Customers.cs` dosyasını oluşturun ve `IEndpoint` implementasyonunu yapın.

> **Güvenlik Standardı:** Endpointler kesinlikle dışarı açık olamaz! `.RequireAuthorization()` ve Policy-Based Access Control (PBAC) gereği `.RequireClaim("Permission", "Customers.Write")` veya `Customers.View` eklenmelidir.

```csharp
using Carter;
using MediatR;
using Enterprise.Framework.Application.Common.Models;

namespace Enterprise.Framework.API.Endpoints;

public class Customers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customers")
            .WithTags("Customers")
            .RequireAuthorization(); // JWT Zorunlu

        // Okuma (View) yetkisi olanlar listeyi görebilir
        group.MapGet("/", GetCustomers)
            .RequireClaim("Permission", "Customers.View"); 

        // Yazma (Write) yetkisi olanlar ekleme yapabilir
        group.MapPost("/", CreateCustomer)
            .RequireClaim("Permission", "Customers.Write"); 
    }

    public async Task<IResult> GetCustomers(ISender sender, [AsParameters] GetCustomersQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result); // ApiResponse formatında döner
    }
}
```

---

## ADIM 4: Frontend - Servis Katmanı (Angular)

API hazır olduktan sonra, Angular tarafında UI isteklerini yönetecek Servis oluşturulmalıdır.
`ui/src/app/services/customer.service.ts` dosyasını oluşturun. **Tüm servisler mutlaka `api.service.ts` üzerinden geçmeli ve `ApiResponse<T>` formatında tip güvenliği sağlamalıdır.**

```typescript
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse, PagedResult } from '../models/api-models';

export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class CustomerService {
  constructor(private api: ApiService) { }

  getCustomers(pageNumber = 1, pageSize = 10, searchTerm?: string): Observable<ApiResponse<PagedResult<Customer>>> {
    let url = `api/customers?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    return this.api.get<PagedResult<Customer>>(url);
  }

  createCustomer(payload: Partial<Customer>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/customers', payload);
  }
}
```

---

## ADIM 5: Frontend - UI (Grid ve Yetki Koruması)

Arayüzde PrimeNG kullanılmakta olup, tabloların (Grid) profesyonel ve eksiksiz görünmesi için **zorunlu şablonlar** mevcuttur.

### 5.1. Typescript Tarafı (Debounce Search ve Pagination)
Arama yapıldığında ekranın titrememesi için (Debounce) ve doğru sayfalama için aşağıdaki iskeleti kopyalayabilirsiniz:

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
// ...diğer importlar (TableModule, ButtonModule, vs.)

@Component({
  selector: 'app-customers',
  standalone: true,
  // ... imports
  templateUrl: './customers.component.html'
})
export class CustomersComponent implements OnInit {
  customers: Customer[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  
  // Arama ve Sayfalama için zorunlu alanlar
  searchTerm: string = '';
  searchSubject: Subject<string> = new Subject<string>();
  lastEvent: any = null;

  constructor(
    private customerService: CustomerService,
    public authService: AuthService // Yetki kontrolü için
  ) {}

  ngOnInit() {
    // Arama kutusuna basıldığında ekranı titretmemek için 0.5sn bekleme standardı
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      if (this.lastEvent) {
        this.lastEvent.first = 0;
        this.loadCustomers(this.lastEvent);
      }
    });
  }

  loadCustomers(event: any) {
    this.lastEvent = event;
    this.loading = true;
    const pageNumber = event.first !== undefined && event.rows ? (event.first / event.rows) + 1 : 1;
    const pageSize = event.rows || 10;

    this.customerService.getCustomers(pageNumber, pageSize, this.searchTerm).subscribe({
      next: (res) => {
        if (res.success) {
          this.customers = res.data.items;
          this.totalRecords = res.data.totalCount;
        }
        this.loading = false;
      }
    });
  }

  onSearch(event: any) {
    this.searchSubject.next(event.target.value);
  }
}
```

### 5.2. HTML Tarafı (Premium Grid Tasarımı ve PBAC Koruması)
Tüm ekranlarda aşağıdaki yapının kullanılması **zorunludur**. `p-table` sınıfları ve buton yetkilendirmelerine dikkat ediniz.

```html
<div class="card">
  <!-- Üst Araç Çubuğu (Toolbar) -->
  <div class="flex flex-column md:flex-row md:align-items-center justify-content-between mb-4 gap-3">
    <div>
        <h2 class="m-0">Müşteriler</h2>
    </div>
    <div class="flex gap-2">
        <span class="p-input-icon-left">
            <i class="pi pi-search"></i>
            <input type="text" pInputText placeholder="Ara..." (input)="onSearch($event)" class="p-inputtext-sm border-round-lg" />
        </span>
        <!-- Dışa aktarma butonu dt.exportCSV()'ye bağlıdır -->
        <button pButton label="Dışa Aktar" icon="pi pi-file-excel" class="p-button-outlined p-button-success p-button-sm" (click)="dt.exportCSV()"></button>
        
        <!-- YETKİ KONTROLÜ: Sadece Write yetkisi olanlar görebilir -->
        <button *ngIf="authService.hasPermission('Customers.Write')" pButton label="Yeni Ekle" icon="pi pi-plus" class="p-button-sm border-round-lg" (click)="showDialog()"></button>
    </div>
  </div>

  <!-- Tablo Tasarımı (Gridlines, Zebra ve Pagination zorunludur) -->
  <p-table #dt [value]="customers" [paginator]="true" [rows]="10" [lazy]="true" (onLazyLoad)="loadCustomers($event)"
           [totalRecords]="totalRecords" [loading]="loading" [rowsPerPageOptions]="[10, 20, 50]"
           styleClass="p-datatable-gridlines p-datatable-striped p-datatable-sm" responsiveLayout="scroll">
    <ng-template pTemplate="header">
      <tr>
        <!-- Sıralama Okları (SortIcon) -->
        <th pSortableColumn="id">ID <p-sortIcon field="id"></p-sortIcon></th>
        <th pSortableColumn="firstName">İsim <p-sortIcon field="firstName"></p-sortIcon></th>
        <th pSortableColumn="lastName">Soyisim <p-sortIcon field="lastName"></p-sortIcon></th>
        
        <!-- İşlemler Kolonu Yetki Koruması -->
        <th style="width: 120px" class="text-center" *ngIf="authService.hasPermission('Customers.Write')">İşlemler</th>
      </tr>
      <tr>
        <!-- Kolon İçi Filtreleme Kutucukları (Premium Görünüm) -->
        <th><p-columnFilter type="text" field="id" display="menu"></p-columnFilter></th>
        <th><p-columnFilter type="text" field="firstName" display="menu"></p-columnFilter></th>
        <th><p-columnFilter type="text" field="lastName" display="menu"></p-columnFilter></th>
        <th *ngIf="authService.hasPermission('Customers.Write')"></th>
      </tr>
    </ng-template>
    <ng-template pTemplate="body" let-customer>
      <tr>
        <td>{{customer.id}}</td>
        <td>{{customer.firstName}}</td>
        <td>{{customer.lastName}}</td>
        
        <!-- Butonlar Yetki Koruması Altında -->
        <td class="text-center" *ngIf="authService.hasPermission('Customers.Write')">
            <div class="flex justify-content-center gap-1">
                <button pButton icon="pi pi-pencil" class="p-button-rounded p-button-info p-button-text p-button-sm"></button>
                <button pButton icon="pi pi-trash" class="p-button-rounded p-button-danger p-button-text p-button-sm"></button>
            </div>
        </td>
      </tr>
    </ng-template>
  </p-table>
</div>
```

---
**Tebrikler!** Yeni ekleyeceğiniz tüm modüllerde bu dokümandaki adımları birebir uygularsanız, projenin kurumsal mimarisini ve profesyonel arayüzünü bozmadan saniyeler içinde yeni ekranlar üretebilirsiniz.
