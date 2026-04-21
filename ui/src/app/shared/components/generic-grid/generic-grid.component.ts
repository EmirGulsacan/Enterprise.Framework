import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { PagedResult } from '../../../models/api-models';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

export interface GridColumn {
  field: string;
  header: string;
  type?: 'text' | 'date' | 'boolean' | 'numeric';
  format?: string; // Optional custom formatting pipe hint
}

export interface GridAction {
  icon: string;
  label?: string;
  action: string;
  severity?: 'success' | 'info' | 'warning' | 'danger' | 'secondary';
  tooltip?: string;
}

@Component({
  selector: 'app-generic-grid',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, TooltipModule, TagModule, ConfirmDialogModule],
  providers: [ConfirmationService],
  templateUrl: './generic-grid.component.html'
})
export class GenericGridComponent implements OnInit {
  @Input() title: string = 'Kayıtlar';
  @Input() apiEndpoint!: string; // e.g. "api/customers"
  @Input() permissionModule!: string; // e.g. "Customers" -> Used for Customers.Write
  @Input() columns: GridColumn[] = [];
  @Input() customActions: GridAction[] = [];

  @Output() onAdd = new EventEmitter<void>();
  @Output() onEdit = new EventEmitter<any>();
  @Output() onDelete = new EventEmitter<any>();
  @Output() onCustomAction = new EventEmitter<{action: string, row: any}>();

  data: any[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  rows: number = 10;
  
  searchTerm: string = '';
  searchSubject: Subject<string> = new Subject<string>();
  lastEvent: any = null;

  constructor(
    private apiService: ApiService,
    public authService: AuthService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      if (this.lastEvent) {
        this.lastEvent.first = 0;
        this.loadData(this.lastEvent);
      }
    });
  }

  loadData(event: any) {
    this.lastEvent = event;
    this.loading = true;
    const pageNumber = event.first !== undefined && event.rows ? (event.first / event.rows) + 1 : 1;
    const pageSize = event.rows || 10;
    let sortQuery = '';
    if (event.sortField) {
      const sortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
      sortQuery = `&SortOrder=${event.sortField}_${sortOrder}`;
    }

    let url = `${this.apiEndpoint}?pageNumber=${pageNumber}&pageSize=${pageSize}${sortQuery}`;
    if (this.searchTerm) {
      url += `&SearchTerm=${encodeURIComponent(this.searchTerm)}`;
    }
    if (event.filters && Object.keys(event.filters).length > 0) {
      const filtersObj: any = {};
      Object.keys(event.filters).forEach(key => {
        const filterMeta = event.filters[key];
        let filterValue = null;
        if (Array.isArray(filterMeta) && filterMeta.length > 0) {
            filterValue = filterMeta[0].value;
        } else if (filterMeta) {
            filterValue = filterMeta.value;
        }
        
        if (filterValue !== null && filterValue !== undefined && filterValue !== '') {
          filtersObj[key] = filterValue;
        }
      });
      
      if (Object.keys(filtersObj).length > 0) {
          url += `&FiltersJson=${encodeURIComponent(JSON.stringify(filtersObj))}`;
      }
    }

    this.apiService.get<PagedResult<any>>(url, { headers: { 'X-Skip-Loading': 'true' } }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.data = res.data.items || [];
          this.totalRecords = res.data.totalCount || 0;
        } else {
          this.data = [];
          this.totalRecords = 0;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.data = [];
      }
    });
  }

  onSearch(event: any) {
    this.searchSubject.next(event.target.value);
  }

  confirmDelete(rowData: any) {
    this.confirmationService.confirm({
      message: 'Bu kaydı silmek istediğinize emin misiniz?',
      header: 'Silme Onayı',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet, Sil',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.onDelete.emit(rowData);
      }
    });
  }
  refresh() {
    if (this.lastEvent) {
      this.loadData(this.lastEvent);
    }
  }
}
