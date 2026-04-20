import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-labors',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card">
      <h2 class="text-2xl font-bold mb-4">İş Gücü Yönetimi</h2>
      <p class="text-600">Bu modül yapım aşamasındadır.</p>
    </div>
  `
})
export class LaborsComponent {}
