import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from '../../../services/auth.service';
import * as xlsx from 'xlsx';
import { saveAs } from 'file-saver';

import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';

export interface GridColumn {
  field: string;
  header: string;
  type?: 'text' | 'date' | 'boolean' | 'numeric' | 'tags' | 'enum';
  format?: string;
  enumMap?: Record<string, { label: string; severity?: string }>;
  width?: string;
  sortable?: boolean;
}

export interface GridAction {
  icon: string;
  label?: string;
  action: string;
  severity?: 'success' | 'info' | 'warning' | 'danger' | 'secondary';
  tooltip?: string;
}

export interface GridRowActionEvent<T> {
  action: string;
  row: T;
}

@Component({
  selector: 'app-generic-grid',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, TooltipModule, TagModule],
  templateUrl: './generic-grid.component.html'
})
export class GenericGridComponent<T extends Record<string, unknown> = Record<string, unknown>> implements OnInit {
  @Input() title: string = 'Kayıtlar';
  @Input() permissionModule!: string;
  @Input() columns: GridColumn[] = [];
  @Input() customActions: GridAction[] = [];
  @Input() showExport: boolean = true;
  @Input() showSearch: boolean = true;
  @Input() showAddButton: boolean = true;
  @Input() rowsPerPage: number[] = [10, 20, 50];
  @Input() defaultRows: number = 10;
  @Input() emptyMessage: string = 'Kayıt bulunamadı.';
  @Input() dateFormat: string = 'dd/MM/yyyy';
  @Input() apiEndpoint: string = '';
  
  @Input() data: T[] = [];
  @Input() totalRecords: number = 0;
  @Input() loading: boolean = false;

  @Output() onAdd = new EventEmitter<void>();
  @Output() onEdit = new EventEmitter<T>();
  @Output() onDelete = new EventEmitter<T>();
  @Output() onCustomAction = new EventEmitter<GridRowActionEvent<T>>();
  @Output() onLazyLoad = new EventEmitter<TableLazyLoadEvent>();
  @Output() onExport = new EventEmitter<void>();

  searchTerm: string = '';
  searchSubject: Subject<string> = new Subject<string>();
  lastEvent: TableLazyLoadEvent | null = null;

  constructor(public authService: AuthService) {}

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      if (this.lastEvent) {
        this.lastEvent = { ...this.lastEvent, first: 0 };
        this.loadData(this.lastEvent);
      }
    });
  }

  loadData(event: TableLazyLoadEvent) {
    this.lastEvent = event;
    const customEvent = { ...event };
    if (this.searchTerm) {
        customEvent.globalFilter = this.searchTerm;
    }
    this.onLazyLoad.emit(customEvent);
  }

  onSearch(event: Event) {
    this.searchSubject.next((event.target as HTMLInputElement).value);
  }

  deleteRow(rowData: T) {
    this.onDelete.emit(rowData);
  }

  handleExport(dt: { exportCSV: () => void }) {
    if (this.onExport.observed) {
      this.onExport.emit();
    } else {
      if (this.data && this.data.length > 0) {
        const exportData = this.data.map(row => {
          const exportRow: any = {};
          this.columns.forEach(col => {
            if (col.type === 'enum') exportRow[col.header] = this.getEnumLabel(col, row[col.field]);
            else if (col.type === 'boolean') exportRow[col.header] = row[col.field] ? 'EVET' : 'HAYIR';
            else if (col.type === 'tags') exportRow[col.header] = Array.isArray(row[col.field]) ? (row[col.field] as string[]).join(', ') : row[col.field];
            else exportRow[col.header] = row[col.field];
          });
          return exportRow;
        });
        const worksheet = xlsx.utils.json_to_sheet(exportData);
        const workbook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
        const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
        saveAs(new Blob([excelBuffer], {type: 'application/octet-stream'}), `${this.title.replace(/\s+/g, '_')}_Export.xlsx`);
      }
    }
  }

  getEnumLabel(col: GridColumn, value: unknown): string {
    if (!col.enumMap || value == null) return String(value ?? '');
    const entry = col.enumMap[String(value)];
    return entry?.label ?? String(value);
  }

  getEnumSeverity(col: GridColumn, value: unknown): string {
    if (!col.enumMap || value == null) return 'info';
    const entry = col.enumMap[String(value)];
    return entry?.severity ?? 'info';
  }

  isColumnSortable(col: GridColumn): boolean {
    return col.sortable !== false;
  }

  refresh() {
    if (this.lastEvent) {
      this.loadData(this.lastEvent);
    }
  }
}
