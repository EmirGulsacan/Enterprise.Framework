import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';

@Component({
  selector: 'app-labors',
  standalone: true,
  imports: [CommonModule, GenericGridComponent],
  template: `
    <div class="fadein animation-duration-500">
      <app-generic-grid 
          title="İşçilik Yönetimi (Labors)" 
          apiEndpoint="api/labors" 
          permissionModule="Labors"
          [columns]="columns">
      </app-generic-grid>
    </div>
  `
})
export class LaborsComponent {
    columns: GridColumn[] = [
      { field: 'employeeId', header: 'Çalışan ID' },
      { field: 'maintenanceId', header: 'Bakım ID' },
      { field: 'hoursWorked', header: 'Çalışılan Saat', type: 'numeric' },
      { field: 'hourlyRate', header: 'Saatlik Ücret', type: 'numeric' }
    ];

    constructor() {}
}
