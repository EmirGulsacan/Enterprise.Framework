import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface RuleDefinition {
  id: number;
  name: string;
  code: string;
  domain: string;
  description: string;
  isActive: boolean;
  workflowJson: string;
}

export interface CreateRuleCommand {
  name: string;
  code: string;
  domain: string;
  description: string;
  workflowJson: string;
}

export interface UpdateRuleCommand {
  name: string;
  domain: string;
  description: string;
  workflowJson: string;
  isActive: boolean;
}

export interface MicrosoftRule {
  RuleName: string;
  Expression: string;
  SuccessEvent: string;
}

export interface MicrosoftWorkflow {
  WorkflowName: string;
  Rules: MicrosoftRule[];
}

@Injectable({ providedIn: 'root' })
export class RuleEngineService {
  private apiUrl = `${environment.apiUrl}/api/rule-definitions`;

  constructor(private http: HttpClient) {}

  getRules(): Observable<RuleDefinition[]> {
    return this.http.get<RuleDefinition[]>(this.apiUrl);
  }

  createRule(command: CreateRuleCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  updateRule(code: string, command: UpdateRuleCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${code}`, command);
  }

  deleteRule(code: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${code}`);
  }
}
