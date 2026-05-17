import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  FormArray,
  Validators
} from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { SkeletonModule } from 'primeng/skeleton';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmationService, MessageService } from 'primeng/api';
import {
  RuleEngineService,
  RuleDefinition,
  CreateRuleCommand,
  UpdateRuleCommand,
  MicrosoftWorkflow
} from './rule-engine.service';

@Component({
  selector: 'app-rule-builder',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputTextareaModule,
    ToastModule,
    ConfirmDialogModule,
    TagModule,
    TooltipModule,
    SkeletonModule,
    CheckboxModule
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './rule-builder.component.html',
  styleUrl: './rule-builder.component.scss'
})
export class RuleBuilderComponent implements OnInit {
  rules: RuleDefinition[] = [];
  isLoading = false;
  isSaving = false;
  isEditMode = false;
  displayDialog = false;
  selectedCode: string | null = null;

  ruleForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private ruleEngineService: RuleEngineService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
    this.ruleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      code: ['', [Validators.required, Validators.maxLength(100)]],
      domain: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(1000)]],
      isActive: [true],
      rules: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadRules();
  }

  get rulesArray(): FormArray {
    return this.ruleForm.get('rules') as FormArray;
  }

  addRule(): void {
    const group = this.fb.group({
      ruleName: ['', Validators.required],
      expression: ['', Validators.required],
      successEvent: ['', Validators.required]
    });
    this.rulesArray.push(group);
  }

  removeRule(index: number): void {
    this.rulesArray.removeAt(index);
  }

  loadRules(): void {
    this.isLoading = true;
    this.ruleEngineService.getRules().subscribe({
      next: (data) => {
        this.rules = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  showAddDialog(): void {
    this.isEditMode = false;
    this.selectedCode = null;
    this.ruleForm.reset({ isActive: true });
    this.rulesArray.clear();
    this.addRule();
    this.displayDialog = true;
  }

  showEditDialog(rule: RuleDefinition): void {
    this.isEditMode = true;
    this.selectedCode = rule.code;
    this.rulesArray.clear();

    let parsedWorkflow: MicrosoftWorkflow | null = null;
    try {
      const workflows: MicrosoftWorkflow[] = JSON.parse(rule.workflowJson);
      parsedWorkflow = workflows?.[0] ?? null;
    } catch {
      parsedWorkflow = null;
    }

    this.ruleForm.patchValue({
      name: rule.name,
      code: rule.code,
      domain: rule.domain,
      description: rule.description,
      isActive: rule.isActive
    });

    if (parsedWorkflow?.Rules?.length) {
      parsedWorkflow.Rules.forEach(r => {
        this.rulesArray.push(this.fb.group({
          ruleName: [r.RuleName, Validators.required],
          expression: [r.Expression, Validators.required],
          successEvent: [r.SuccessEvent, Validators.required]
        }));
      });
    } else {
      this.addRule();
    }

    this.displayDialog = true;
  }

  save(): void {
    if (this.ruleForm.invalid) {
      this.ruleForm.markAllAsTouched();
      return;
    }

    const formValue = this.ruleForm.value;

    const workflow: MicrosoftWorkflow = {
      WorkflowName: formValue.code,
      Rules: (formValue.rules as any[]).map(r => ({
        RuleName: r.ruleName,
        Expression: r.expression,
        SuccessEvent: r.successEvent
      }))
    };

    const workflowJson = JSON.stringify([workflow]);
    this.isSaving = true;

    if (this.isEditMode && this.selectedCode) {
      const command: UpdateRuleCommand = {
        name: formValue.name,
        domain: formValue.domain,
        description: formValue.description,
        workflowJson,
        isActive: formValue.isActive
      };

      this.ruleEngineService.updateRule(this.selectedCode, command).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Kural güncellendi' });
          this.displayDialog = false;
          this.isSaving = false;
          this.loadRules();
        },
        error: () => { this.isSaving = false; }
      });
    } else {
      const command: CreateRuleCommand = {
        name: formValue.name,
        code: formValue.code,
        domain: formValue.domain,
        description: formValue.description,
        workflowJson
      };

      this.ruleEngineService.createRule(command).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Yeni kural oluşturuldu' });
          this.displayDialog = false;
          this.isSaving = false;
          this.loadRules();
        },
        error: () => { this.isSaving = false; }
      });
    }
  }

  deleteRule(rule: RuleDefinition): void {
    this.confirmationService.confirm({
      message: `"${rule.name}" kuralını pasif yapmak istediğinize emin misiniz?`,
      header: 'Silme Onayı',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet, Pasif Yap',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.isSaving = true;
        this.ruleEngineService.deleteRule(rule.code).subscribe({
          next: () => {
            this.messageService.add({ severity: 'info', summary: 'Başarılı', detail: 'Kural pasif yapıldı' });
            this.isSaving = false;
            this.loadRules();
          },
          error: () => { this.isSaving = false; }
        });
      }
    });
  }
}
