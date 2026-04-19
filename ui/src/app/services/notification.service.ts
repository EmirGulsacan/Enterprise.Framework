import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private messageService: MessageService) {}
  success(detail: string, summary: string = 'Başarılı') {
    this.messageService.add({ severity: 'success', summary, detail });
  }
  error(detail: string, summary: string = 'Hata') {
    this.messageService.add({ severity: 'error', summary, detail });
  }
  warn(detail: string, summary: string = 'Uyarı') {
    this.messageService.add({ severity: 'warn', summary, detail });
  }
  info(detail: string, summary: string = 'Bilgi') {
    this.messageService.add({ severity: 'info', summary, detail });
  }
}
