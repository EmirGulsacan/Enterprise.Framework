import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';

@Component({
  selector: 'app-maintenances',
  standalone: true,
  imports: [CommonModule, GenericGridComponent],
  template: `
    <div class="fadein animation-duration-500">
      <app-generic-grid 
          title="Bakım Yönetimi (Maintenances)" 
          apiEndpoint="api/maintenances" 
          permissionModule="Maintenances"
          [columns]="columns">
      </app-generic-grid>
    </div>
  `
})
export class MaintenancesComponent {
    columns: GridColumn[] = [
      { field: 'asset', header: 'Varlık' },
      { field: 'description', header: 'Açıklama' },
      { field: 'date', header: 'Tarih', type: 'date' }
    ];

    constructor() {}
}

