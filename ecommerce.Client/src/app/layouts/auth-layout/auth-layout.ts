import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeToggle } from '../../shared/components/theme-toggle/theme-toggle';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet, ThemeToggle],
  templateUrl: './auth-layout.html',
  styleUrl: './auth-layout.css',
})
export class AuthLayout {
  protected readonly features: { title: string; subtitle: string; icon: string }[] = [
    {
      title: 'Smart Recommendations',
      subtitle: 'AI picks the perfect gift',
      icon: '<i class="lni lni-bulb text-xl"></i>',
    },
    {
      title: 'Instant Send',
      subtitle: 'Delivered in seconds via WhatsApp',
      icon: '<i class="lni lni-bolt text-xl"></i>',
    },
    {
      title: 'Egyptian Merchants',
      subtitle: 'Hundreds of local brands',
      icon: '<i class="lni lni-cart text-xl"></i>',
    },
  ];
}
