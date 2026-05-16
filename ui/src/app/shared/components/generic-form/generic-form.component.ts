import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { PasswordModule } from 'primeng/password';
import { InputSwitchModule } from 'primeng/inputswitch';

export interface DropdownOption {
  label: string;
  value: string | number | boolean;
}

export interface FormField {
  key: string;
  label: string;
  type: 'text' | 'email' | 'number' | 'date' | 'dropdown' | 'textarea' | 'checkbox' | 'password' | 'switch';
  required?: boolean;
  options?: DropdownOption[];
  optionLabel?: string;
  optionValue?: string;
  maxLength?: number;
  minLength?: number;
  placeholder?: string;
  disabled?: boolean;
  defaultValue?: unknown;
  colspan?: 1 | 2;
}

@Component({
  selector: 'app-generic-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, InputTextModule, DropdownModule, 
    InputNumberModule, CalendarModule, InputTextareaModule, ButtonModule,
    CheckboxModule, PasswordModule, InputSwitchModule
  ],
  template: `
    <form [formGroup]="formGroup" (ngSubmit)="submitForm()" class="flex flex-column gap-3 mt-3">
      <div class="formgrid grid">
        <div *ngFor="let field of fields" 
             class="field"
             [ngClass]="field.colspan === 2 ? 'col-12' : 'col-12 md:col-6'">
          
          <label [for]="field.key" *ngIf="field.type !== 'checkbox' && field.type !== 'switch'">
            {{ field.label }} <span *ngIf="field.required" class="text-red-500">*</span>
          </label>
          
          <ng-container [ngSwitch]="field.type">
            <!-- TEXT -->
            <input *ngSwitchCase="'text'" pInputText [id]="field.key" [formControlName]="field.key" 
                   [placeholder]="field.placeholder || field.label" 
                   [maxlength]="field.maxLength || 999"
                   class="w-full" />
            
            <!-- EMAIL -->
            <input *ngSwitchCase="'email'" pInputText type="email" [id]="field.key" [formControlName]="field.key" 
                   [placeholder]="field.placeholder || field.label" class="w-full" />
            
            <!-- PASSWORD -->
            <p-password *ngSwitchCase="'password'" [inputId]="field.key" [formControlName]="field.key" 
                        [toggleMask]="true" [feedback]="false" styleClass="w-full" inputStyleClass="w-full"></p-password>
            
            <!-- NUMBER -->
            <p-inputNumber *ngSwitchCase="'number'" [inputId]="field.key" [formControlName]="field.key" 
                           class="w-full" styleClass="w-full"></p-inputNumber>
            
            <!-- DATE -->
            <p-calendar *ngSwitchCase="'date'" [inputId]="field.key" [formControlName]="field.key" 
                        appendTo="body" dateFormat="yy-mm-dd" styleClass="w-full"></p-calendar>
            
            <!-- DROPDOWN -->
            <p-dropdown *ngSwitchCase="'dropdown'" [id]="field.key" [formControlName]="field.key" 
              [options]="field.options || []" [optionLabel]="field.optionLabel || 'label'" [optionValue]="field.optionValue || 'value'" 
              [placeholder]="field.placeholder || 'Seçiniz'" [style]="{'width':'100%'}" appendTo="body"></p-dropdown>

            <!-- TEXTAREA -->
            <textarea *ngSwitchCase="'textarea'" pInputTextarea [id]="field.key" [formControlName]="field.key" 
                      rows="3" class="w-full" [placeholder]="field.placeholder || ''"></textarea>

            <!-- CHECKBOX -->
            <div *ngSwitchCase="'checkbox'" class="flex align-items-center gap-2 mt-2">
              <p-checkbox [formControlName]="field.key" [binary]="true" [inputId]="field.key"></p-checkbox>
              <label [for]="field.key" class="cursor-pointer">{{ field.label }}</label>
            </div>

            <!-- SWITCH -->
            <div *ngSwitchCase="'switch'" class="flex align-items-center gap-2 mt-2">
              <p-inputSwitch [formControlName]="field.key" [inputId]="field.key"></p-inputSwitch>
              <label [for]="field.key" class="cursor-pointer">{{ field.label }}</label>
            </div>
          </ng-container>

          <!-- Client-side validation error -->
          <small class="p-error block mt-1" *ngIf="formGroup.get(field.key)?.invalid && formGroup.get(field.key)?.touched">
            <span *ngIf="formGroup.get(field.key)?.errors?.['required']">{{ field.label }} zorunludur.</span>
            <span *ngIf="formGroup.get(field.key)?.errors?.['email']">Geçerli bir e-posta giriniz.</span>
            <span *ngIf="formGroup.get(field.key)?.errors?.['minlength']">En az {{ field.minLength }} karakter giriniz.</span>
            <span *ngIf="formGroup.get(field.key)?.errors?.['maxlength']">En fazla {{ field.maxLength }} karakter giriniz.</span>
          </small>

          <!-- Server-side validation error -->
          <small class="p-error block mt-1" *ngIf="serverErrors[field.key]">
            <span *ngFor="let err of serverErrors[field.key]">{{ err }}</span>
          </small>
        </div>
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
  @Input() initialData: Record<string, unknown> | null = null;
  @Input() serverErrors: Record<string, string[]> = {};
  @Output() onSubmit = new EventEmitter<Record<string, unknown>>();
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
        
        const patchData = { ...this.initialData };
        this.fields.forEach(f => {
          if (f.type === 'date' && patchData[f.key]) {
            patchData[f.key] = new Date(patchData[f.key] as string);
          }
        });
        
        this.formGroup.patchValue(patchData);
      } else {
        this.isEditMode = false;
        this.formGroup.reset();
        this.applyDefaults();
      }
    }
  }

  private buildForm() {
    const group: Record<string, unknown[]> = {};
    this.fields.forEach(field => {
      const validators = [];
      if (field.required) validators.push(Validators.required);
      if (field.type === 'email') validators.push(Validators.email);
      if (field.maxLength) validators.push(Validators.maxLength(field.maxLength));
      if (field.minLength) validators.push(Validators.minLength(field.minLength));
      
      const defaultValue = field.defaultValue ?? (field.type === 'checkbox' || field.type === 'switch' ? false : null);
      group[field.key] = [defaultValue, validators];
    });
    this.formGroup = this.fb.group(group);
    
    this.fields.forEach(f => {
      if (f.disabled) {
        this.formGroup.get(f.key)?.disable();
      }
    });

    if (this.initialData) {
      this.isEditMode = true;
      const patchData = { ...this.initialData };
      this.fields.forEach(f => {
        if (f.type === 'date' && patchData[f.key]) {
          patchData[f.key] = new Date(patchData[f.key] as string);
        }
      });
      this.formGroup.patchValue(patchData);
    }
  }

  private applyDefaults() {
    const defaults: Record<string, unknown> = {};
    this.fields.forEach(f => {
      if (f.defaultValue !== undefined) {
        defaults[f.key] = f.defaultValue;
      }
    });
    if (Object.keys(defaults).length > 0) {
      this.formGroup.patchValue(defaults);
    }
  }

  submitForm() {
    if (this.formGroup.valid) {
      const result = { ...this.initialData, ...this.formGroup.getRawValue() };
      this.onSubmit.emit(result);
    } else {
      this.formGroup.markAllAsTouched();
    }
  }
}
