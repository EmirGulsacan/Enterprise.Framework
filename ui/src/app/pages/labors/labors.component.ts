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
      { field: 'employee', header: 'Çalışan' },
      { field: 'task', header: 'Görev' },
      { field: 'hours', header: 'Saat', type: 'numeric' }
    ];

    constructor() {}
}

