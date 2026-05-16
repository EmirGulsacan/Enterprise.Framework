# Enterprise Framework — Geliştirici El Kitabı (Developer Playbook)

Bu doküman, **"Sisteme yeni bir CRUD modülünü sıfırdan, standartlara uygun olarak nasıl eklerim?"** sorusuna uçtan uca cevap verir.

Aşağıdaki 7 adım, Domain entity'sinden frontend ekranına kadar tüm süreci kapsar. **Tüm geliştirmelerde bu standartlara kesinlikle uyunuz.**

> **Referans modül:** SampleEntities (Varlık Yönetimi) — tüm örnekler bu modülün gerçek kodundan alınmıştır.

---

## Hızlı Kontrol Listesi (Checklist)

Yeni bir `[Modül]` eklerken sırasıyla:

- [ ] **Domain:** `Entities/[Modül].cs` → `AuditableEntity` miras al
- [ ] **Domain:** `Common/Enums/` altına gerekli enum'ları ekle
- [ ] **Application:** `Common/Security/Permissions.cs` → modül izinlerini ekle
- [ ] **Application:** `Features/[Modül]/Queries/[Modül]Dto.cs` → `IMapFrom<T>` implement et
- [ ] **Application:** `Features/[Modül]/Queries/Get[Modüller]Query.cs` → Query + Handler
- [ ] **Application:** `Features/[Modül]/Commands/Create[Modül]Command.cs` → Command + Handler
- [ ] **Application:** `Features/[Modül]/Commands/Create[Modül]CommandValidator.cs` → FluentValidation
- [ ] **Application:** `Features/[Modül]/Commands/Update[Modül]Command.cs` → Command + Handler
- [ ] **Application:** `Features/[Modül]/Commands/Delete[Modül]Command.cs` → Command + Handler
- [ ] **API:** `Endpoints/[Modül]Endpoints.cs` → `IEndpointDefinition` implement et
- [ ] **Infrastructure:** `AppDbContext` → `DbSet` eklemeye gerek yok (`GetDbSet<T>()` generic)
- [ ] **Migration:** `dotnet ef migrations add Add[Modül]Table ...`
- [ ] **Frontend:** `models/[modül].model.ts` → TypeScript interface
- [ ] **Frontend:** `pages/[modüller]/` → Component (GenericGrid + GenericForm kullan)
- [ ] **Frontend:** `app.routes.ts` → Route ekle + `permissionGuard`
- [ ] **Frontend:** `layout/main-layout.component.ts` → Sidebar menü linki

---

## ADIM 1: Domain — Entity ve Enum

### 1.1 Entity Oluşturma

`Enterprise.Framework.Domain/Entities/` altına entity'nizi ekleyin.

**Kurallar:**
- Her entity `AuditableEntity` sınıfından miras alır (audit + soft delete otomatik)
- Property'lerde default değer kullanın
- Navigation property'ler opsiyonel — lazy load yerine explicit include tercih edilir

```csharp
namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Domain.Common.Enums;

public class SampleEntity : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string SampleField { get; set; } = string.Empty;
    public DateTime SampleDate { get; set; }
    public SampleEntityStatus Status { get; set; } = SampleEntityStatus.Active;

    public long? RelatedEntityId { get; set; }
    public RelatedEntity? AssignedRelatedEntity { get; set; }

    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
}
```

### 1.2 Enum Oluşturma (Gerekirse)

`Enterprise.Framework.Domain/Common/Enums/` altına:

```csharp
namespace Enterprise.Framework.Domain.Common.Enums;

public enum SampleEntityStatus
{
    Active,
    UnderMaintenance,
    Inactive,
    Disposed
}
```

> **Not:** Enum'lar `AppDbContext.OnModelCreating` içinde otomatik olarak `string` tipine çevrilir (max 50 karakter).

### 1.3 Migration

```bash
dotnet ef migrations add AddSampleEntityTable --project src/Enterprise.Framework.Infrastructure --startup-project src/Enterprise.Framework.API
dotnet ef database update --project src/Enterprise.Framework.Infrastructure --startup-project src/Enterprise.Framework.API
```

> **Not:** `AppDbContext`'e `DbSet<SampleEntity>` eklemenize gerek yoktur. `GetDbSet<T>()` generic metodu kullanılır.

---

## ADIM 2: İzinler (Permissions)

`Enterprise.Framework.Application/Common/Security/Permissions.cs` dosyasına yeni modül izinlerini ekleyin. Uygulama açılışında `PermissionSeeder` bunları otomatik olarak veritabanına seed eder.

```csharp
public static class SampleEntities
{
    public const string Module = "Varlık Yönetimi";

    public const string SampleEntitiesView = "SampleEntities.View";
    public const string SampleEntitiesWrite = "SampleEntities.Write";
}
```

> **Kural:** `Write` yetkisi otomatik olarak `View` yetkisini kapsar — backend ve frontend bu kurala uyar.

---

## ADIM 3: Application — CQRS (Query + Command + Validator)

Her işlem kendi dosyasında yaşar. Klasör yapısı:

```
Features/SampleEntities/
├── Commands/
│   ├── CreateSampleEntityCommand.cs
│   ├── CreateSampleEntityCommandValidator.cs
│   ├── UpdateSampleEntityCommand.cs
│   └── DeleteSampleEntityCommand.cs
└── Queries/
    ├── SampleEntityDto.cs
    └── GetSampleEntitiesQuery.cs
```

### 3.1 DTO — `IMapFrom<T>` ile Otomatik Mapping

```csharp
namespace Enterprise.Framework.Application.Features.SampleEntities.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record SampleEntityDto : IMapFrom<SampleEntity>
{
    public long Id { get; init; }
    public string Name { get; init; } = default!;
    public string SampleField { get; init; } = default!;
    public DateTime SampleDate { get; init; }
    public string Status { get; init; } = default!;
    public long? RelatedEntityId { get; init; }
}
```

> **Kural:** `IMapFrom<SampleEntity>` implement edildiğinde AutoMapper otomatik profil oluşturur — ekstra mapping konfigürasyonu gerekmez.

### 3.2 Query — Listeleme (Pagination + Grid Options)

```csharp
namespace Enterprise.Framework.Application.Features.SampleEntities.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Common.Extensions;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

public sealed record GetSampleEntitiesQuery : IRequest<PagedResult<SampleEntityDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
    public string? FiltersJson { get; init; }
}

public class GetSampleEntitiesQueryHandler : IRequestHandler<GetSampleEntitiesQuery, PagedResult<SampleEntityDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSampleEntitiesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<SampleEntityDto>> Handle(GetSampleEntitiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<SampleEntity>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchTerm) ||
                x.SampleField.ToLower().Contains(searchTerm));
        }

        query = query.ApplyGridOptions(request.SortOrder, request.FiltersJson);

        return await query.PaginatedProjectToAsync<SampleEntityDto>(
            request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
```

**Önemli noktalar:**
- `AsNoTracking()` — listeleme işlemlerinde performans için zorunlu
- `ApplyGridOptions()` — frontend grid'den gelen filtre ve sıralama otomatik uygulanır
- `PaginatedProjectToAsync()` — AutoMapper projection + sayfalama tek satırda

### 3.3 Command — Create

```csharp
namespace Enterprise.Framework.Application.Features.SampleEntities.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common.Enums;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record CreateSampleEntityCommand : IRequest<long>
{
    public string Name { get; init; } = default!;
    public string SampleField { get; init; } = default!;
    public DateTime SampleDate { get; init; }
    public SampleEntityStatus Status { get; init; } = SampleEntityStatus.Active;
    public long? RelatedEntityId { get; init; }
}

public class CreateSampleEntityCommandHandler : IRequestHandler<CreateSampleEntityCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateSampleEntityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = new SampleEntity
        {
            Name = request.Name,
            SampleField = request.SampleField,
            SampleDate = request.SampleDate,
            Status = request.Status,
            RelatedEntityId = request.RelatedEntityId
        };

        _context.GetDbSet<SampleEntity>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
```

### 3.4 Command — Update

```csharp
public sealed record UpdateSampleEntityCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SampleField { get; init; } = string.Empty;
    public DateTime SampleDate { get; init; }
    public SampleEntityStatus Status { get; init; }
    public long? RelatedEntityId { get; init; }
}

public class UpdateSampleEntityCommandHandler : IRequestHandler<UpdateSampleEntityCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateSampleEntityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<SampleEntity>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(SampleEntity), request.Id);

        entity.Name = request.Name;
        entity.SampleField = request.SampleField;
        entity.SampleDate = request.SampleDate;
        entity.Status = request.Status;
        entity.RelatedEntityId = request.RelatedEntityId;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
```

### 3.5 Command — Delete

```csharp
public sealed record DeleteSampleEntityCommand(long Id) : IRequest<Unit>;

public class DeleteSampleEntityCommandHandler : IRequestHandler<DeleteSampleEntityCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteSampleEntityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteSampleEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<SampleEntity>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(SampleEntity), request.Id);

        _context.GetDbSet<SampleEntity>().Remove(entity);  // SoftDeleteInterceptor otomatik devreye girer
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
```

> **Not:** `Remove()` çağrısı veritabanından fiziksel silme yapmaz — `SoftDeleteInterceptor` bunu `IsDeleted=true` olarak işaretler.

### 3.6 Validator — FluentValidation

```csharp
namespace Enterprise.Framework.Application.Features.SampleEntities.Commands;

using FluentValidation;

public class CreateSampleEntityCommandValidator : AbstractValidator<CreateSampleEntityCommand>
{
    public CreateSampleEntityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Varlık adı boş olamaz.")
            .MaximumLength(200).WithMessage("Varlık adı 200 karakteri geçemez.");

        RuleFor(x => x.SampleField)
            .NotEmpty().WithMessage("Seri numarası boş olamaz.")
            .MaximumLength(100).WithMessage("Seri numarası 100 karakteri geçemez.");

        RuleFor(x => x.SampleDate)
            .NotEmpty().WithMessage("Satın alma tarihi zorunludur.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Satın alma tarihi gelecekte olamaz.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz varlık durumu.");

        RuleFor(x => x.RelatedEntityId)
            .GreaterThan(0).When(x => x.RelatedEntityId.HasValue)
            .WithMessage("Çalışan ID geçerli bir değer olmalıdır.");
    }
}
```

> **Not:** Validator'lar `ValidationBehavior` tarafından otomatik keşfedilir. DI'a ekstra kayıt gerekmez.

---

## ADIM 4: API — Minimal API Endpoint

`Enterprise.Framework.API/Endpoints/SampleEntityEndpoints.cs`:

```csharp
namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Features.SampleEntities.Commands;
using Enterprise.Framework.Application.Features.SampleEntities.Queries;
using MediatR;

public class SampleEntityEndpoints : IEndpointDefinition
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sampleEntities")
            .WithTags("SampleEntities")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetSampleEntitiesQuery query, ISender sender) =>
        {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<SampleEntityDto>>.Ok(result));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "SampleEntities.View"));

        group.MapPost("/", async (CreateSampleEntityCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "SampleEntities.Write"));

        group.MapPut("/{id:long}", async (long id, UpdateSampleEntityCommand command, ISender sender) =>
        {
            if (id != command.Id) return Results.BadRequest(ApiResponse<object>.Fail("ID mismatch"));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "SampleEntities.Write"));

        group.MapDelete("/{id:long}", async (long id, ISender sender) =>
        {
            await sender.Send(new DeleteSampleEntityCommand(id));
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "SampleEntities.Write"));
    }
}
```

**Kurallar:**
- `IEndpointDefinition` implement edin — `EndpointExtensions.MapAllEndpoints()` tarafından otomatik keşfedilir
- `.RequireAuthorization()` — JWT zorunlu
- `.RequireClaim("Permission", "...")` — PBAC zorunlu
- Endpoint'te **asla** iş mantığı bulunmaz — sadece `sender.Send()` ve `ApiResponse<T>` wrap
- GET isteklerinde `[AsParameters]` ile query string binding kullanın
- PUT'ta `id != command.Id` kontrolü yapın

---

## ADIM 5: Frontend — Model ve Service

### 5.1 TypeScript Model

`ui/src/app/models/sampleEntity-management.model.ts`:

```typescript
export interface SampleEntity {
  id: number;
  name: string;
  serialNumber: string;
  purchaseDate: string;
  status: SampleEntityStatus;
  assignedRelatedEntityId?: number;
}

export enum SampleEntityStatus {
  Active = 'Active',
  UnderMaintenance = 'UnderMaintenance',
  Inactive = 'Inactive',
  Disposed = 'Disposed'
}
```

> **Kural:** Her entity için kendi model dosyası olmalı. `any` tipi **yasaktır**.

### 5.2 Service — BaseCrudService Kullanımı

Eğer modülünüz standart CRUD ise, `BaseCrudService<T>` yeterlidir — ayrı service dosyası oluşturmaya gerek yoktur:

```typescript
// Component içinde:
private sampleEntityService: BaseCrudService<SampleEntity>;

constructor(private apiService: ApiService) {
    this.sampleEntityService = new BaseCrudService<SampleEntity>(this.apiService, '/api/sampleEntities');
}
```

`BaseCrudService<T>` otomatik olarak `getAll()`, `getById()`, `create()`, `update()`, `delete()` metodlarını sağlar.

---

## ADIM 6: Frontend — Sayfa (GenericGrid + GenericForm)

### 6.1 Component TypeScript

`ui/src/app/pages/sampleEntities/sampleEntities.component.ts`:

```typescript
import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';
import { GenericFormComponent, FormField } from '../../shared/components/generic-form/generic-form.component';
import { BaseCrudService } from '../../shared/services/base-crud.service';
import { SampleEntity } from '../../models/sampleEntity-management.model';
import { PagedResult } from '../../models/api-models';
import { ApiService } from '../../services/api.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sampleEntities',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    ButtonModule, DialogModule, ConfirmDialogModule,
    GenericGridComponent, GenericFormComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './sampleEntities.component.html'
})
export class SampleEntitiesComponent implements OnInit {
  sampleEntities: SampleEntity[] = [];
  totalRecords = 0;
  loading = true;
  @ViewChild('grid') grid!: GenericGridComponent;

  private sampleEntityService: BaseCrudService<SampleEntity>;
  displayDialog = false;
  editMode = false;
  selectedData: SampleEntity | null = null;

  // Grid kolon tanımları
  columns: GridColumn[] = [
    { field: 'name', header: 'İsim' },
    { field: 'serialNumber', header: 'Seri Numarası' },
    { field: 'purchaseDate', header: 'Satın Alma Tarihi', type: 'date' },
    { field: 'status', header: 'Durum' }
  ];

  // Form alan tanımları
  formFields: FormField[] = [
    { key: 'name', label: 'İsim', type: 'text', required: true },
    { key: 'serialNumber', label: 'Seri Numarası', type: 'text', required: true },
    { key: 'purchaseDate', label: 'Satın Alma Tarihi', type: 'date', required: true },
    { key: 'status', label: 'Durum', type: 'dropdown',
      options: [
        { label: 'Aktif', value: 'Active' },
        { label: 'Bakımda', value: 'UnderMaintenance' },
        { label: 'Pasif', value: 'Inactive' },
        { label: 'Kullanım Dışı', value: 'Disposed' }
      ],
      optionLabel: 'label', optionValue: 'value', required: true }
  ];

  constructor(
    private apiService: ApiService,
    private notification: NotificationService,
    private confirmationService: ConfirmationService,
    public authService: AuthService
  ) {
    this.sampleEntityService = new BaseCrudService<SampleEntity>(this.apiService, '/api/sampleEntities');
  }

  ngOnInit() {}

  loadSampleEntities(event: TableLazyLoadEvent) { /* grid lazy load logic */ }
  showDialog() { this.editMode = false; this.selectedData = null; this.displayDialog = true; }
  editSampleEntity(sampleEntity: SampleEntity) { this.editMode = true; this.selectedData = { ...sampleEntity }; this.displayDialog = true; }
  deleteSampleEntity(sampleEntity: SampleEntity) { /* confirmationService ile silme */ }
  saveSampleEntity(payload: Partial<SampleEntity>) { /* create veya update */ }
}
```

### 6.2 Component HTML

`ui/src/app/pages/sampleEntities/sampleEntities.component.html`:

```html
<div class="fadein animation-duration-500">

  <app-generic-grid
      #grid
      title="Varlıklar (SampleEntities)"
      permissionModule="SampleEntities"
      [columns]="columns"
      [data]="sampleEntities"
      [totalRecords]="totalRecords"
      [loading]="loading"
      (onLazyLoad)="loadSampleEntities($event)"
      (onAdd)="showDialog()"
      (onEdit)="editSampleEntity($event)"
      (onDelete)="deleteSampleEntity($event)">
  </app-generic-grid>

  <p-dialog header="Varlık Formu" [(visible)]="displayDialog"
            [style]="{width: '50vw'}" [breakpoints]="{'960px': '75vw', '640px': '100vw'}">
    <app-generic-form
        *ngIf="displayDialog"
        [fields]="formFields"
        [initialData]="selectedData"
        (onSubmit)="saveSampleEntity($event)"
        (onCancel)="displayDialog = false">
    </app-generic-form>
  </p-dialog>

  <p-confirmDialog></p-confirmDialog>
</div>
```

**Önemli:** `GenericGridComponent` ve `GenericFormComponent` kullanılarak tekrarlayan grid/form kodu elimine edilir. Yeni modül eklerken sadece `columns` ve `formFields` konfigürasyonu değişir.

---

## ADIM 7: Route ve Menü

### 7.1 Route Ekleme

`ui/src/app/app.routes.ts` — `children` array'ine:

```typescript
{
    path: 'sampleEntities',
    loadComponent: () => import('./pages/sampleEntities/sampleEntities.component').then(c => c.SampleEntitiesComponent),
    canActivate: [permissionGuard],
    data: { permissions: ['SampleEntities.View'] }
},
```

> **Kural:** `loadComponent` ile lazy loading, `permissionGuard` ile PBAC route koruması zorunludur.

### 7.2 Sidebar Menü Ekleme

`ui/src/app/layout/main-layout.component.ts` — sidebar `<ul>` içine:

```html
<li *ngIf="authService.hasPermission('SampleEntities.View')">
  <a routerLink="/sampleEntities" routerLinkActive="active" class="nav-link">
    <i class="pi pi-box"></i>
    <span>Varlık Yönetimi</span>
  </a>
</li>
```

> **Kural:** Menü linki `hasPermission()` ile korunmalıdır — yetkisiz kullanıcılar linki göremez.

---

## Sık Kullanılan Pattern'ler

### İş Kuralı Motoru (Business Rule Engine)

Handler içinde kompleks iş kurallarını çalıştırma:

```csharp
public class CreateSampleEntityCommandHandler : IRequestHandler<CreateSampleEntityCommand, long>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessRuleEngine _ruleEngine;

    public async Task<long> Handle(CreateSampleEntityCommand request, CancellationToken ct)
    {
        await _ruleEngine.CheckAsync(ct,
            new EntityMustExistRule<RelatedEntity>(_context, request.RelatedEntityId, "Atanan çalışan bulunamadı"),
            new SampleFieldMustBeUniqueRule(_context, request.SampleField)
        );

        // ... entity oluşturma
    }
}
```

### Domain Event Ekleme (Outbox Pattern)

```csharp
// Handler içinde:
var entity = new SampleEntity { ... };
entity.AddDomainEvent(new SampleEntityCreatedEvent(entity.Id, entity.Name));
_context.GetDbSet<SampleEntity>().Add(entity);
await _context.SaveChangesAsync(ct);
// Event otomatik olarak OutboxMessage tablosuna yazılır ve background service tarafından işlenir.
```

### Cache'lenebilir Query

```csharp
public sealed record GetDashboardQuery : IRequest<DashboardDto>, ICacheableRequest<DashboardDto>
{
    public string CacheKey => "dashboard:summary";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}
// CachingBehavior otomatik devreye girer — cache varsa handler çağrılmaz.
```

### Idempotent Command (Offline Sync Hazırlığı)

```csharp
public sealed record CreateSampleEntityCommand : IRequest<long>, IIdempotentCommand<long>
{
    public string IdempotencyKey { get; init; } = Guid.NewGuid().ToString();
    // ... diğer property'ler
}
// Aynı IdempotencyKey ile tekrar gönderilirse handler çağrılmaz, önceki sonuç döner.
```

---

> **Tebrikler!** Bu dokümandaki adımları birebir uygularsanız, projenin kurumsal mimarisini bozmadan dakikalar içinde yeni modüller üretebilirsiniz.

