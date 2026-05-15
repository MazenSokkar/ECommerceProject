import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-overview',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="space-y-6">
      <div
        class="page-header rounded-2xl p-5 sm:p-6 flex flex-col sm:flex-row sm:items-center gap-4 sm:justify-between"
      >
        <div>
          <h1 class="text-2xl font-extrabold text-white tracking-tight">Admin Dashboard</h1>
          <p class="text-teal-200 text-sm mt-0.5">Centralized management for TechShop.</p>
        </div>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <!-- Manage Users Card -->
        <a
          routerLink="../users"
          class="section-card p-6 block hover:-translate-y-1 hover:shadow-lg transition-all group"
        >
          <div
            class="w-12 h-12 rounded-xl bg-teal-50 dark:bg-teal-900/30 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
          >
            <i class="lni lni-users text-2xl text-primary"></i>
          </div>
          <h3 class="text-lg font-bold text-neutral-800 dark:text-slate-100">Manage Users</h3>
          <p class="text-sm text-neutral-500 dark:text-slate-400 mt-1">
            View, edit, restrict, or delete user accounts and assign roles.
          </p>
        </a>

        <!-- Manage Banners Card -->
        <a
          routerLink="../banners"
          class="section-card p-6 block hover:-translate-y-1 hover:shadow-lg transition-all group"
        >
          <div
            class="w-12 h-12 rounded-xl bg-amber-50 dark:bg-amber-900/30 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
          >
            <i class="lni lni-image text-2xl text-secondary"></i>
          </div>
          <h3 class="text-lg font-bold text-neutral-800 dark:text-slate-100">Manage Banners</h3>
          <p class="text-sm text-neutral-500 dark:text-slate-400 mt-1">
            Configure homepage promotional banners and sliders.
          </p>
        </a>

        <!-- All Orders Card -->
        <a
          routerLink="/app/orders"
          class="section-card p-6 block hover:-translate-y-1 hover:shadow-lg transition-all group"
        >
          <div
            class="w-12 h-12 rounded-xl bg-blue-50 dark:bg-blue-900/30 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
          >
            <i class="lni lni-cart text-2xl text-blue-500"></i>
          </div>
          <h3 class="text-lg font-bold text-neutral-800 dark:text-slate-100">All Orders</h3>
          <p class="text-sm text-neutral-500 dark:text-slate-400 mt-1">
            View and manage all customer orders across the platform.
          </p>
        </a>
      </div>
    </div>
  `,
})
export class AdminOverview {}
