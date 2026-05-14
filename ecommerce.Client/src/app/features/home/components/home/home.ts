import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BannersApiService } from '../../../../core/services/banners-api.service';
import { HomeApiService } from '../../../../core/services/home-api.service';
import { Banner } from '../../../../shared/models/banner.model';
import { Product } from '../../../products/models/product.model';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { DecimalPipe } from '@angular/common';
import { Button } from '../../../../shared/components/button/button';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, Button, DecimalPipe],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  private readonly bannersApi = inject(BannersApiService);
  private readonly homeApi = inject(HomeApiService);

  protected readonly banners = signal<Banner[]>([]);
  protected readonly bestSellers = signal<Product[]>([]);
  protected readonly newArrivals = signal<Product[]>([]);
  protected readonly featuredProducts = signal<Product[]>([]);

  protected readonly bannersLoading = signal(true);
  protected readonly productsLoading = signal(true);
  protected readonly activeIndex = signal(0);
  private autoSlideInterval: any;

  ngOnInit() {
    this.loadBanners();
    this.loadHomeProducts();

    this.autoSlideInterval = setInterval(() => {
      if (this.banners().length > 1) {
        this.nextSlide();
      }
    }, 5000);
  }

  private loadBanners() {
    this.bannersApi.getActive().subscribe({
      next: (res: ApiResponse<Banner[]>) => {
        if (res.isSuccess) {
          this.banners.set(res.data ?? []);
        }
        this.bannersLoading.set(false);
      },
      error: () => this.bannersLoading.set(false),
    });
  }

  private loadHomeProducts() {
    this.productsLoading.set(true);

    // Load Best Sellers
    this.homeApi.getBestSellers(4).subscribe({
      next: (res) => {
        if (res.isSuccess) this.bestSellers.set(res.data ?? []);
      }
    });

    // Load New Arrivals
    this.homeApi.getNewArrivals(4).subscribe({
      next: (res) => {
        if (res.isSuccess) this.newArrivals.set(res.data ?? []);
      }
    });

    // Load Featured
    this.homeApi.getFeaturedProducts(4).subscribe({
      next: (res) => {
        if (res.isSuccess) this.featuredProducts.set(res.data ?? []);
        this.productsLoading.set(false);
      },
      error: () => this.productsLoading.set(false)
    });
  }

  ngOnDestroy() {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }

  nextSlide() {
    this.activeIndex.update((i) => (i + 1) % this.banners().length);
  }

  prevSlide() {
    this.activeIndex.update((i) => (i === 0 ? this.banners().length - 1 : i - 1));
  }
}
