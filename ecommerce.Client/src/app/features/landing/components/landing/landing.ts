import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ThemeToggle } from '../../../../shared/components/theme-toggle/theme-toggle';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink, ThemeToggle],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  protected readonly currentYear = new Date().getFullYear();

  protected readonly features: { title: string; description: string; icon: string }[] = [
    {
      icon: '<i class="lni lni-bulb text-3xl"></i>',
      title: 'Smart Recommendations',
      description: 'Our AI learns preferences and suggests the perfect gift for every occasion.',
    },
    {
      icon: '<i class="lni lni-bolt text-3xl"></i>',
      title: 'Instant Delivery',
      description: 'Gift cards delivered in seconds directly to the recipient via WhatsApp or email.',
    },
    {
      icon: '<i class="lni lni-cart text-3xl"></i>',
      title: 'Egyptian Merchants',
      description: 'Hundreds of trusted Egyptian brands — from coffee shops to fashion stores.',
    },
  ];
}
