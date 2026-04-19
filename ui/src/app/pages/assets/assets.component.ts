import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { AssetService } from '../../services/asset.service';
import { Asset } from '../../models/asset-management.model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, DialogModule, InputTextModule, FormsModule],
  templateUrl: './assets.component.html'
})
export class AssetsComponent implements OnInit {
  assets: Asset[] = [];
  displayDialog: boolean = false;
  newAsset: Partial<Asset> = {};

  constructor(private assetService: AssetService, private notification: NotificationService) {}

  ngOnInit() {
    this.loadAssets();
  }

  loadAssets() {
    this.assetService.getAssets().subscribe({
      next: (res) => {
        if (res.success) {
          this.assets = res.data.items;
        }
      },
      error: (err) => {
        this.notification.error('Varlıklar yüklenemedi');
      }
    });
  }

  showDialog() {
    this.newAsset = {};
    this.displayDialog = true;
  }

  saveAsset() {
    this.assetService.createAsset(this.newAsset).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Varlık başarıyla eklendi');
          this.displayDialog = false;
          this.loadAssets();
        }
      },
      error: (err) => {
        this.notification.error('Varlık eklenirken hata oluştu');
      }
    });
  }
}
