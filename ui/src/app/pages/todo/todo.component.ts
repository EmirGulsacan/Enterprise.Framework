import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { HttpClient } from '@angular/common/http';
import { NotificationService } from '../../services/notification.service';
import { environment } from '../../../environments/environment';
import { ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { TooltipModule } from 'primeng/tooltip';
@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, ToolbarModule, TooltipModule],
  template: `
    <div class="fadein animation-duration-500">
        <!-- Stats Row -->
        <div class="grid mb-4">
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-blue-100 text-blue-600">
                        <i class="pi pi-list"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Toplam Görev</div>
                        <div class="text-2xl font-bold">{{ totalRecords }}</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-green-100 text-green-600">
                        <i class="pi pi-check-circle"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Tamamlanan</div>
                        <div class="text-2xl font-bold">{{ completedCount }}</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-orange-100 text-orange-600">
                        <i class="pi pi-clock"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Bekleyen</div>
                        <div class="text-2xl font-bold">{{ totalRecords - completedCount }}</div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Main Content -->
        <div class="premium-card p-4">
            <div class="flex flex-column md:flex-row md:align-items-center justify-content-between mb-4 gap-3">
                <div>
                    <h1 class="text-2xl font-bold text-900 m-0">Görev Yönetimi</h1>
                    <p class="text-muted m-0">Günlük iş planınızı buradan yönetebilirsiniz.</p>
                </div>
                <div class="flex gap-2">
                    <span class="p-input-icon-left">
                        <i class="pi pi-search"></i>
                        <input type="text" pInputText placeholder="Görevlerde ara..." (input)="onSearch($event)" class="p-inputtext-sm border-round-lg w-15rem" />
                    </span>
                    <button pButton label="Excel" icon="pi pi-file-excel" class="p-button-outlined p-button-secondary p-button-sm" (click)="exportExcel()"></button>
                    <button pButton label="Yeni Görev" icon="pi pi-plus" class="p-button-accent p-button-sm" (click)="showDialog()"></button>
                </div>
            </div>
            <p-table 
                #dt
                [value]="todos" 
                [lazy]="true" 
                (onLazyLoad)="onLazyLoad($event)"
                [paginator]="true" 
                [rows]="rows" 
                [totalRecords]="totalRecords" 
                [loading]="loading"
                [responsiveLayout]="'scroll'"
                styleClass="p-datatable-gridlines p-datatable-sm p-datatable-striped shadow-2"
                [rowsPerPageOptions]="[5,10,20,50]"
                [showCurrentPageReport]="true"
                [alwaysShowPaginator]="true"
                currentPageReportTemplate="{totalRecords} kayıttan {first} - {last} arası gösteriliyor"
                [sortField]="'createdAtUtc'"
                [sortOrder]="-1"
                [globalFilterFields]="['title']"
            >
                <ng-template pTemplate="header">
                    <tr>
                        <th style="width: 80px" pSortableColumn="id">
                            ID <p-sortIcon field="id"></p-sortIcon>
                        </th>
                        <th pSortableColumn="title">
                            GÖREV TANIMI <p-sortIcon field="title"></p-sortIcon>
                            <p-columnFilter type="text" field="title" display="menu" [showMatchModes]="false" [showOperator]="false" [showAddButton]="false"></p-columnFilter>
                        </th>
                        <th style="width: 150px">DURUM</th>
                        <th style="width: 200px" pSortableColumn="createdAtUtc">
                            OLUŞTURMA <p-sortIcon field="createdAtUtc"></p-sortIcon>
                        </th>
                        <th style="width: 140px" class="text-center">EYLEM</th>
                    </tr>
                </ng-template>
                <ng-template pTemplate="body" let-todo>
                    <tr>
                        <td class="font-bold text-slate-500">#{{todo.id}}</td>
                        <td class="font-medium">{{todo.title}}</td>
                        <td>
                            <span [class]="todo.isCompleted ? 'bg-green-50 text-green-600 border-green-200' : 'bg-orange-50 text-orange-600 border-orange-200'" class="px-2 py-1 border-1 border-round-lg text-xs font-bold flex align-items-center gap-1 w-fit">
                                <i [class]="todo.isCompleted ? 'pi pi-check-circle' : 'pi pi-clock'"></i>
                                {{todo.isCompleted ? 'TAMAMLANDI' : 'BEKLİYOR'}}
                            </span>
                        </td>
                        <td class="text-muted text-sm">{{todo.createdAtUtc | date:'dd.MM.yyyy HH:mm'}}</td>
                        <td class="text-center">
                            <div class="flex justify-content-center gap-1">
                                <button pButton icon="pi pi-check" 
                                        class="p-button-rounded p-button-success p-button-text p-button-sm" 
                                        (click)="completeTodo(todo.id)" 
                                        [disabled]="todo.isCompleted" 
                                        pTooltip="Tamamla">
                                </button>
                                <button pButton icon="pi pi-pencil" 
                                        class="p-button-rounded p-button-info p-button-text p-button-sm" 
                                        (click)="editTodo(todo)" 
                                        pTooltip="Düzenle">
                                </button>
                                <button pButton icon="pi pi-trash" 
                                        class="p-button-rounded p-button-danger p-button-text p-button-sm" 
                                        (click)="deleteTodo(todo.id)" 
                                        pTooltip="Sil">
                                </button>
                            </div>
                        </td>
                    </tr>
                </ng-template>
                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="5" class="text-center p-5 text-muted">
                            <i class="pi pi-folder-open text-4xl mb-3 block"></i>
                            Henüz bir görev eklenmemiş veya arama sonucu bulunamadı.
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
        <p-dialog [header]="editMode ? 'Görevi Düzenle' : 'Yeni Görev Oluştur'" [(visible)]="displayDialog" [modal]="true" [style]="{width: '450px'}" styleClass="p-fluid" [draggable]="false" [resizable]="false">
            <div class="field mt-3">
                <label for="title" class="font-bold mb-2 block">Görev Başlığı</label>
                <input pInputText id="title" [(ngModel)]="todoModel.title" placeholder="Yapılacak işi yazınız..." autofocus (keyup.enter)="saveTodo()" />
            </div>
            <div class="field-checkbox mt-3" *ngIf="editMode">
                <p-checkbox [(ngModel)]="todoModel.isCompleted" [binary]="true" inputId="completed"></p-checkbox>
                <label for="completed" class="ml-2 font-medium">Tamamlandı olarak işaretle</label>
            </div>
            <ng-template pTemplate="footer">
                <button pButton label="İptal" icon="pi pi-times" (click)="displayDialog=false" class="p-button-text p-button-secondary"></button>
                <button pButton [label]="editMode ? 'Güncelle' : 'Oluştur'" icon="pi pi-check" (click)="saveTodo()" [disabled]="!todoModel.title" style="background: var(--accent-color); border: none"></button>
            </ng-template>
        </p-dialog>
    </div>
  `
})
export class TodoComponent implements OnInit {
    todos: any[] = [];
    totalRecords: number = 0;
    completedCount: number = 0;
    loading: boolean = true;
    rows: number = 10;
    displayDialog: boolean = false;
    editMode: boolean = false;
    todoModel: any = {
        id: null,
        title: '',
        isCompleted: false
    };
    searchTerm: string = '';
    currentSortOrder: string = 'desc';
    currentSortField: string = 'createdAtUtc';
  constructor(
    private api: ApiService, 
    private http: HttpClient, 
    private confirmationService: ConfirmationService,
    private notification: NotificationService) {}
  ngOnInit() {
  }
  onLazyLoad(event: any) {
    this.loading = true;
    this.rows = event.rows;
    if (event.sortField) {
        this.currentSortField = event.sortField;
        this.currentSortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
    }
    if (event.filters && event.filters.title) {
        this.searchTerm = event.filters.title.value || '';
    }
    const page = (event.first / event.rows) + 1;
    this.loadTodos(page, event.rows);
  }
  onSearch(event: any) {
    this.searchTerm = event.target.value;
    this.loadTodos(1, this.rows);
  }
  loadTodos(page: number = 1, size: number = 10) {
    let sortQuery = '';
    if (this.currentSortField === 'title') {
        sortQuery = `&SortOrder=title_${this.currentSortOrder}`;
    } else if (this.currentSortField === 'createdAtUtc') {
        sortQuery = this.currentSortOrder === 'asc' ? '&SortOrder=oldest' : '';
    }
    const searchStr = this.searchTerm ? `&SearchTerm=${this.searchTerm}` : '';
    this.api.get<any>(`api/todos?PageNumber=${page}&PageSize=${size}${searchStr}${sortQuery}`).subscribe({
      next: (res) => {
        if (res && res.data) {
            this.todos = res.data.items || [];
            this.totalRecords = res.data.totalCount || 0;
            this.completedCount = this.todos.filter(x => x.isCompleted).length;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Veriler yüklenirken bir hata oluştu.');
      }
    });
  }
  showDialog() {
    this.editMode = false;
    this.todoModel = { id: null, title: '', isCompleted: false };
    this.displayDialog = true;
  }
  editTodo(todo: any) {
    this.editMode = true;
    this.todoModel = { ...todo };
    this.displayDialog = true;
  }
  saveTodo() {
    if (!this.todoModel.title) return;
    if (this.editMode) {
        this.api.put(`api/todos/${this.todoModel.id}`, this.todoModel).subscribe(() => {
            this.notification.success('Görev güncellendi.');
            this.displayDialog = false;
            this.loadTodos();
        });
    } else {
        this.api.post('api/todos', { title: this.todoModel.title }).subscribe(() => {
            this.notification.success('Görev eklendi.');
            this.displayDialog = false;
            this.loadTodos();
        });
    }
  }
  deleteTodo(id: number) {
    this.confirmationService.confirm({
        message: 'Bu görevi silmek istediğinize emin misiniz?',
        header: 'Görev Silme Onayı',
        icon: 'pi pi-exclamation-triangle',
        acceptLabel: 'Evet, Sil',
        rejectLabel: 'İptal',
        acceptButtonStyleClass: 'p-button-danger p-button-text',
        rejectButtonStyleClass: 'p-button-text p-button-secondary',
        accept: () => {
            this.api.delete(`api/todos/${id}`).subscribe(() => {
                this.notification.info('Görev başarıyla silindi.');
                this.loadTodos();
            });
        }
    });
  }
  completeTodo(id: number) {
    this.api.post(`api/todos/${id}/complete`, {}).subscribe(() => {
      this.notification.info('Görev tamamlandı.');
      this.loadTodos();
    });
  }
  exportExcel() {
    this.http.get(`${environment.apiUrl}/api/todos/export`, { responseType: 'blob' }).subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'todos.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);
        this.notification.success('Excel dosyası indirildi.');
    });
  }
}
