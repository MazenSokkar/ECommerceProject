import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BannersApiService } from '../../../../core/services/banners-api.service';
import { Banner } from '../../../../shared/models/banner.model';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { Button } from '../../../../shared/components/button/button';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, Button],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  private readonly bannersApi = inject(BannersApiService);

  protected readonly banners = signal<Banner[]>([]);
  protected readonly loading = signal(true);
  protected readonly activeIndex = signal(0);
  private autoSlideInterval: any;

  ngOnInit() {
    this.bannersApi.getActive().subscribe({
      next: (res: ApiResponse<Banner[]>) => {
        if (res.isSuccess) {
          this.banners.set(res.data ?? []);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });

    this.autoSlideInterval = setInterval(() => {
      if (this.banners().length > 1) {
        this.nextSlide();
      }
    }, 5000);
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
