import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const MERCHANT_ROUTES: Routes = [
    {
        path: '',
        canActivate: [roleGuard('Merchant')],
        children: [
            {
                path: '',
                loadComponent: () =>
                    import('./components/merchant-dashboard/merchant-dashboard').then(m => m.MerchantDashboard),
            },
            {
                path: 'inventory',
                loadComponent: () =>
                    import('./components/merchant-inventory/merchant-inventory').then(m => m.MerchantInventory),
            },
            {
                path: 'products/new',
                loadComponent: () =>
                    import('../products/components/product-form/product-form').then(m => m.ProductForm),
            },
            {
                path: 'products/:id/edit',
                loadComponent: () =>
                    import('../products/components/product-form/product-form').then(m => m.ProductForm),
            },
        ],
    },
];