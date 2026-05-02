import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';

@Component({
  selector: 'app-documents',
  standalone: true,
  imports: [CommonModule, GenericGridComponent],
  template: `
    <div class="fadein animation-duration-500">
      <app-generic-grid 
          title="Doküman Yönetimi (Documents)" 
          apiEndpoint="api/documents" 
          permissionModule="Documents"
          [columns]="columns">
      </app-generic-grid>
    </div>
  `
})
export class DocumentsComponent {
    columns: GridColumn[] = [
      { field: 'fileName', header: 'Doküman Adı' },
      { field: 'contentType', header: 'İçerik Türü' },
      { field: 'relatedEntityType', header: 'İlgili Varlık Türü' },
      { field: 'relatedEntityId', header: 'İlgili Varlık ID' }
    ];

    constructor() {}
}
