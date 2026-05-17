import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ToastService } from '../../../../core/services/toast.service';
import { Button } from '../../../../shared/components/button/button';
import { Tab, TabSwitcher } from '../../../../shared/components/tab-switcher/tab-switcher';
import { AdminMerchantsApiService } from '../../services/admin-merchants-api.service';
import { Merchant, MerchantStatus } from '../../models/merchant.model';

type StatusFilter = 'All' | MerchantStatus;

@Component({
  selector: 'app-admin-merchants-page',
  standalone: true,
  imports: [CommonModule, Button, TabSwitcher],
  templateUrl: './admin-merchants-page.html',
})
export class AdminMerchantsPage implements OnInit {
  private readonly api = inject(AdminMerchantsApiService);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal(true);
  protected readonly merchants = signal<Merchant[]>([]);
  protected readonly statusFilter = signal<StatusFilter>('All');
  protected readonly updatingId = signal<number | null>(null);

  protected readonly filtered = computed(() => {
    const filter = this.statusFilter();
    if (filter === 'All') return this.merchants();
    return this.merchants().filter((m) => m.status === filter);
  });

  protected readonly counts = computed(() => {
    const all = this.merchants();
    return {
      all: all.length,
      pending: all.filter((m) => m.status === 'Pending').length,
      approved: all.filter((m) => m.status === 'Approved').length,
      rejected: all.filter((m) => m.status === 'Rejected').length,
    };
  });

  protected readonly tabs = computed<Tab[]>(() => {
    const c = this.counts();
    return [
      { value: 'All', label: `All (${c.all})` },
      { value: 'Pending', label: `Pending (${c.pending})` },
      { value: 'Approved', label: `Approved (${c.approved})` },
      { value: 'Rejected', label: `Rejected (${c.rejected})` },
    ];
  });

  ngOnInit(): void {
    this.load();
  }

  protected setFilter(filter: string): void {
    this.statusFilter.set(filter as StatusFilter);
  }

  protected approve(merchant: Merchant): void {
    this.updateStatus(merchant, 'Approved');
  }

  protected reject(merchant: Merchant): void {
    this.updateStatus(merchant, 'Rejected');
  }

  protected isUpdating(id: number): boolean {
    return this.updatingId() === id;
  }

  private updateStatus(merchant: Merchant, status: MerchantStatus): void {
    if (this.updatingId()) return;
    this.updatingId.set(merchant.id);
    this.api.updateStatus(merchant.id, status).subscribe({
      next: () => {
        this.updatingId.set(null);
        const msg = status === 'Approved' ? 'Merchant approved.' : 'Merchant rejected.';
        this.toast.success('Success', msg);
        this.load();
      },
      error: () => {
        this.updatingId.set(null);
        this.toast.error('Error', 'Failed to update merchant status.');
      },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.api.getAll().subscribe({
      next: (res) => {
        this.merchants.set(res.data ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toast.error('Error', 'Failed to load merchants.');
      },
    });
  }
}
