import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';

export interface FormField {
  key: string;
  label: string;
  type: 'text' | 'email' | 'number' | 'date' | 'dropdown' | 'textarea';
  required?: boolean;
  options?: any[];
  optionLabel?: string;
  optionValue?: string;
}

@Component({
  selector: 'app-generic-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, InputTextModule, DropdownModule, 
    InputNumberModule, CalendarModule, InputTextareaModule, ButtonModule
  ],
  template: `
    <form [formGroup]="formGroup" (ngSubmit)="submitForm()" class="flex flex-column gap-3 mt-3">
      <div *ngFor="let field of fields" class="flex flex-column gap-2">
        <label [for]="field.key">{{ field.label }} <span *ngIf="field.required">*</span></label>
        
        <ng-container [ngSwitch]="field.type">
          <!-- TEXT -->
          <input *ngSwitchCase="'text'" pInputText [id]="field.key" [formControlName]="field.key" [placeholder]="field.label" />
          
          <!-- EMAIL -->
          <input *ngSwitchCase="'email'" pInputText type="email" [id]="field.key" [formControlName]="field.key" [placeholder]="field.label" />
          
          <!-- NUMBER -->
          <p-inputNumber *ngSwitchCase="'number'" [inputId]="field.key" [formControlName]="field.key" class="w-full" styleClass="w-full"></p-inputNumber>
          
          <!-- DATE -->
          <p-calendar *ngSwitchCase="'date'" [inputId]="field.key" [formControlName]="field.key" appendTo="body" dateFormat="yy-mm-dd"></p-calendar>
          
          <!-- DROPDOWN -->
          <p-dropdown *ngSwitchCase="'dropdown'" [id]="field.key" [formControlName]="field.key" 
            [options]="field.options || []" [optionLabel]="field.optionLabel || 'label'" [optionValue]="field.optionValue || 'value'" 
            placeholder="Seçiniz" [style]="{'width':'100%'}" appendTo="body"></p-dropdown>

          <!-- TEXTAREA -->
          <textarea *ngSwitchCase="'textarea'" pInputTextarea [id]="field.key" [formControlName]="field.key" rows="3"></textarea>
        </ng-container>

        <small class="p-error" *ngIf="formGroup.get(field.key)?.invalid && formGroup.get(field.key)?.touched">
          Lütfen {{ field.label.toLowerCase() }} alanını doğru giriniz.
        </small>
      </div>

      <div class="flex justify-content-end gap-2 mt-4">
        <p-button label="İptal" icon="pi pi-times" type="button" (onClick)="onCancel.emit()" styleClass="p-button-text"></p-button>
        <p-button [label]="isEditMode ? 'Güncelle' : 'Kaydet'" icon="pi pi-check" type="submit" [disabled]="formGroup.invalid"></p-button>
      </div>
    </form>
  `
})
export class GenericFormComponent implements OnInit, OnChanges {
  @Input() fields: FormField[] = [];
  @Input() initialData: any = null;
  @Output() onSubmit = new EventEmitter<any>();
  @Output() onCancel = new EventEmitter<void>();

  formGroup!: FormGroup;
  isEditMode = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.buildForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['initialData'] && this.formGroup) {
      if (this.initialData) {
        this.isEditMode = true;
        
        // Handle date fields safely
        const patchData = { ...this.initialData };
        this.fields.forEach(f => {
          if (f.type === 'date' && patchData[f.key]) {
            patchData[f.key] = new Date(patchData[f.key]);
          }
        });
        
        this.formGroup.patchValue(patchData);
      } else {
        this.isEditMode = false;
        this.formGroup.reset();
      }
    }
  }

  private buildForm() {
    const group: any = {};
    this.fields.forEach(field => {
      const validators = [];
      if (field.required) validators.push(Validators.required);
      if (field.type === 'email') validators.push(Validators.email);
      
      group[field.key] = [null, validators];
    });
    this.formGroup = this.fb.group(group);
    
    if (this.initialData) {
      this.isEditMode = true;
      const patchData = { ...this.initialData };
      this.fields.forEach(f => {
        if (f.type === 'date' && patchData[f.key]) {
          patchData[f.key] = new Date(patchData[f.key]);
        }
      });
      this.formGroup.patchValue(patchData);
    }
  }

  submitForm() {
    if (this.formGroup.valid) {
      const result = { ...this.initialData, ...this.formGroup.value };
      this.onSubmit.emit(result);
    } else {
      this.formGroup.markAllAsTouched();
    }
  }
}
