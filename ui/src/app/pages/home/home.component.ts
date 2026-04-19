import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, CardModule],
  template: `
    <div class="fadein animation-duration-500">
        <div class="mb-5">
            <h1 class="text-3xl font-bold text-900 m-0">Kurumsal Özet</h1>
            <p class="text-muted text-lg">Sistem genelindeki güncel durum ve önemli gelişmeler.</p>
        </div>
        <div class="grid">
            <div class="col-12 md:col-6 lg:col-3">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-blue-50 text-blue-600">
                        <i class="pi pi-users"></i>
                    </div>
                    <div>
                        <div class="text-muted text-xs font-bold uppercase tracking-wider mb-1">Aktif Kullanıcı</div>
                        <div class="text-2xl font-bold">152</div>
                        <div class="text-green-500 text-xs mt-1 font-medium"><i class="pi pi-arrow-up text-xs"></i> %12 artış</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-3">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-emerald-50 text-emerald-600">
                        <i class="pi pi-building"></i>
                    </div>
                    <div>
                        <div class="text-muted text-xs font-bold uppercase tracking-wider mb-1">Toplam Şube</div>
                        <div class="text-2xl font-bold">48</div>
                        <div class="text-blue-500 text-xs mt-1 font-medium">Tüm bölgeler</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-3">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-orange-50 text-orange-600">
                        <i class="pi pi-check-square"></i>
                    </div>
                    <div>
                        <div class="text-muted text-xs font-bold uppercase tracking-wider mb-1">Bekleyen İşler</div>
                        <div class="text-2xl font-bold">14</div>
                        <div class="text-orange-500 text-xs mt-1 font-medium">8'i yüksek öncelikli</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-3">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-purple-50 text-purple-600">
                        <i class="pi pi-shield"></i>
                    </div>
                    <div>
                        <div class="text-muted text-xs font-bold uppercase tracking-wider mb-1">Sistem Sağlığı</div>
                        <div class="text-2xl font-bold">%99.9</div>
                        <div class="text-green-500 text-xs mt-1 font-medium">Kararlı çalışma</div>
                    </div>
                </div>
            </div>
            <div class="col-12 lg:col-8 mt-4">
                <div class="premium-card p-4">
                    <div class="flex justify-content-between align-items-center mb-4">
                        <h2 class="m-0 text-xl font-bold text-900">Son Aktiviteler</h2>
                        <div class="flex gap-2">
                             <i class="pi pi-ellipsis-h cursor-pointer text-muted"></i>
                        </div>
                    </div>
                    <ul class="list-none p-0 m-0">
                        <li class="flex align-items-center py-3 border-bottom-1 border-gray-100">
                            <div class="w-3rem h-3rem flex align-items-center justify-content-center bg-blue-50 text-blue-600 border-round-xl mr-3">
                                <i class="pi pi-user-plus text-xl"></i>
                            </div>
                            <div class="flex flex-column flex-grow-1">
                                <span class="text-900 font-bold mb-1">Yeni Kullanıcı Senkronizasyonu</span>
                                <span class="text-muted text-sm">ahmet.yilmaz&#64;isgys.com Keycloak üzerinden tanımlandı.</span>
                            </div>
                            <div class="text-right ml-3">
                                <div class="text-900 font-bold text-sm">Şimdi</div>
                                <div class="text-muted text-xs">Başarılı</div>
                            </div>
                        </li>
                        <li class="flex align-items-center py-3">
                            <div class="w-3rem h-3rem flex align-items-center justify-content-center bg-emerald-50 text-emerald-600 border-round-xl mr-3">
                                <i class="pi pi-building text-xl"></i>
                            </div>
                            <div class="flex flex-column flex-grow-1">
                                <span class="text-900 font-bold mb-1">Şube Veri Güncellemesi</span>
                                <span class="text-muted text-sm">Kadıköy Bölge Müdürlüğü kapasite bilgileri güncellendi.</span>
                            </div>
                            <div class="text-right ml-3">
                                <div class="text-900 font-bold text-sm">12 dk önce</div>
                                <div class="text-muted text-xs">Sistem</div>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="col-12 lg:col-4 mt-4">
                <div class="premium-card p-4 h-full flex flex-column" style="background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); border: none">
                    <h2 class="m-0 text-xl font-bold text-white mb-4">Sistem Durumu</h2>
                    <div class="flex flex-column gap-4 text-white-alpha-80">
                        <div class="flex justify-content-between align-items-center border-bottom-1 border-white-alpha-10 pb-3">
                            <span class="flex align-items-center gap-2"><i class="pi pi-server text-blue-400"></i> API Sunucusu</span>
                            <span class="font-bold text-green-400">AKTİF</span>
                        </div>
                        <div class="flex justify-content-between align-items-center border-bottom-1 border-white-alpha-10 pb-3">
                            <span class="flex align-items-center gap-2"><i class="pi pi-database text-orange-400"></i> SQL Server</span>
                            <span class="font-bold text-green-400">AKTİF</span>
                        </div>
                        <div class="flex justify-content-between align-items-center border-bottom-1 border-white-alpha-10 pb-3">
                            <span class="flex align-items-center gap-2"><i class="pi pi-key text-purple-400"></i> Keycloak</span>
                            <span class="font-bold text-green-400">AKTİF</span>
                        </div>
                        <div class="flex justify-content-between align-items-center">
                            <span class="flex align-items-center gap-2"><i class="pi pi-bolt text-yellow-400"></i> Gecikme</span>
                            <span class="font-bold">42 ms</span>
                        </div>
                    </div>
                    <div class="mt-auto pt-5 text-center">
                        <i class="pi pi-shield text-white-alpha-10" style="font-size: 6rem"></i>
                    </div>
                </div>
            </div>
        </div>
    </div>
  `
})
export class HomeComponent {}
