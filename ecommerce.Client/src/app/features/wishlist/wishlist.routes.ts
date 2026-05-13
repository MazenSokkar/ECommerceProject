import { Routes } from '@angular/router';

export const WISHLIST_ROUTES: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./components/wishlist-page/wishlist-page').then(m => m.WishlistPage),
    },
];