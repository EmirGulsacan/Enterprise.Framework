import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';
import { PagedResult } from '../../../models/api-models';

import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';

export interface GridColumn {
  field: string;
  header: string;
  type?: 'text' | 'date' | 'boolean' | 'numeric' | 'tags';
  format?: string;
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
  
  // Data inputs
  @Input() data: T[] = [];
  @Input() totalRecords: number = 0;
  @Input() loading: boolean = false;

  @Output() onAdd = new EventEmitter<void>();
  @Output() onEdit = new EventEmitter<T>();
  @Output() onDelete = new EventEmitter<T>();
  @Output() onCustomAction = new EventEmitter<GridRowActionEvent<T>>();
  @Output() onLazyLoad = new EventEmitter<TableLazyLoadEvent>();

  rows: number = 10;
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
    // Add search term to event if present, or pass it separately
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

  refresh() {
    if (this.lastEvent) {
      this.loadData(this.lastEvent);
    }
  }
}

