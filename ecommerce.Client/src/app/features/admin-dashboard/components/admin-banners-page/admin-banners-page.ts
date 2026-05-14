import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BannersApiService } from '../../../../core/services/banners-api.service';
import { Banner } from '../../../../shared/models/banner.model';
import { ToastService } from '../../../../core/services/toast.service';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { BannerFormModal } from '../banner-form-modal/banner-form-modal';
import { Button } from '../../../../shared/components/button/button';

@Component({
  selector: 'app-admin-banners-page',
  standalone: true,
  imports: [CommonModule, BannerFormModal, Button],
  templateUrl: './admin-banners-page.html',
  styleUrl: './admin-banners-page.css',
})
export class AdminBannersPage implements OnInit {
  private readonly api = inject(BannersApiService);
  private readonly toast = inject(ToastService);

  protected readonly banners = signal<Banner[]>([]);
  protected readonly loading = signal(true);
  
  protected readonly isModalOpen = signal(false);
  protected readonly selectedBanner = signal<Banner | null>(null);

  ngOnInit() {
    this.loadBanners();
  }

  loadBanners() {
    this.loading.set(true);
    this.api.getAllAdmin().subscribe({
      next: (res: ApiResponse<Banner[]>) => {
        if (res.isSuccess) {
          this.banners.set(res.data ?? []);
        }
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Error', 'Failed to load banners');
        this.loading.set(false);
      }
    });
  }

  openAddModal() {
    this.selectedBanner.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(banner: Banner) {
    this.selectedBanner.set(banner);
    this.isModalOpen.set(true);
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.selectedBanner.set(null);
  }

  onSaved() {
    this.closeModal();
    this.loadBanners();
  }

  deleteBanner(id: number) {
    if (!confirm('Are you sure you want to delete this banner?')) return;
    
    this.api.delete(id).subscribe({
      next: () => {
        this.toast.success('Success', 'Banner deleted successfully');
        this.loadBanners();
      },
      error: () => this.toast.error('Error', 'Failed to delete banner')
    });
  }

  toggleActive(banner: Banner) {
    this.api.update(banner.id, {
        title: banner.title,
        content: banner.content,
        imageUrl: banner.imageUrl,
        linkUrl: banner.linkUrl,
        displayOrder: banner.displayOrder,
        isActive: !banner.isActive
    }).subscribe({
        next: () => {
            this.toast.success('Success', `Banner ${!banner.isActive ? 'activated' : 'deactivated'}`);
            this.loadBanners();
        },
        error: () => {
            this.toast.error('Error', 'Failed to update status');
            // Revert state in UI just to be safe
            this.loadBanners();
        }
    });
  }
}
