import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';

export const ADMIN_ROUTES: Routes = [
    {
        path: '',
        canActivate: [roleGuard('Admin')],
        children: [
            {
                path: 'products',
                loadComponent: () =>
                    import('./components/admin-products/admin-products').then(m => m.AdminProducts),
            },
            {
                path: 'categories',
                loadComponent: () =>
                    import('./components/admin-categories/admin-categories').then(m => m.AdminCategories),
            },
        ],
    },
];