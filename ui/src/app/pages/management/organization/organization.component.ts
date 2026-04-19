import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TreeTableModule } from 'primeng/treetable';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { SkeletonModule } from 'primeng/skeleton';
import { TreeNode, MessageService } from 'primeng/api';
import { LocationService, LocationNode, LocationType } from '../../../services/location.service';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
@Component({
  selector: 'app-organization',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    TreeTableModule, 
    ButtonModule, 
    DialogModule,
    InputTextModule,
    DropdownModule,
    SkeletonModule,
    TagModule, 
    TooltipModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './organization.component.html',
  styleUrl: './organization.component.scss'
})
export class OrganizationComponent implements OnInit {
  nodes: TreeNode[] = [];
  locationForm: FormGroup;
  displayDialog = false;
  isLoading = false;
  isSaving = false;
  locationTypes = [
    { label: 'Bölge', value: LocationType.Region },
    { label: 'İl', value: LocationType.City },
    { label: 'Şube', value: LocationType.Branch },
    { label: 'Alt Birim', value: LocationType.SubUnit }
  ];
  constructor(
    private fb: FormBuilder,
    private locationService: LocationService,
    private messageService: MessageService
  ) {
    this.locationForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(50)]],
      type: [null, [Validators.required]],
      parentId: [null],
      registryNumbers: ['']
    });
  }
  ngOnInit() {
    this.loadTree();
  }
  loadTree() {
    this.isLoading = true;
    this.locationService.getOrganizationTree().subscribe({
      next: (data) => {
        this.nodes = this.mapToTreeNodes(data);
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
  showAddDialog(parentId: number | null = null) {
    this.locationForm.reset({
      parentId: parentId,
      type: parentId ? LocationType.Branch : LocationType.Region
    });
    this.displayDialog = true;
  }
  saveLocation() {
    if (this.locationForm.invalid) return;
    this.isSaving = true;
    this.locationService.createLocation(this.locationForm.value).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Lokasyon eklendi' });
        this.displayDialog = false;
        this.isSaving = false;
        this.loadTree(); 
      },
      error: () => this.isSaving = false
    });
  }
  mapToTreeNodes(locations: LocationNode[]): TreeNode[] {
    return locations.map(loc => ({
      data: {
        id: loc.id,
        name: loc.name,
        code: loc.code,
        type: loc.type,
        registryNumbers: loc.registryNumbers
      },
      expanded: true,
      children: loc.children ? this.mapToTreeNodes(loc.children) : []
    }));
  }
  getTypeLabel(type: LocationType): string {
    switch (type) {
      case LocationType.Region: return 'Bölge';
      case LocationType.City: return 'İl';
      case LocationType.Branch: return 'Şube';
      case LocationType.SubUnit: return 'Alt Birim';
      default: return 'Bilinmiyor';
    }
  }
  getTypeSeverity(type: LocationType): "success" | "secondary" | "info" | "warning" | "danger" | "contrast" | undefined {
    switch (type) {
      case LocationType.Region: return 'info';
      case LocationType.City: return 'warning';
      case LocationType.Branch: return 'success';
      case LocationType.SubUnit: return 'secondary';
      default: return 'contrast';
    }
  }
}
