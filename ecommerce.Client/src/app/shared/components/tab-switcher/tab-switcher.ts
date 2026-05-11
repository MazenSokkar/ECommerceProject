import { Component, input, output } from '@angular/core';

export interface Tab {
  value: string;
  label: string;
}

@Component({
  selector: 'app-tab-switcher',
  standalone: true,
  templateUrl: './tab-switcher.html',
  styleUrl: './tab-switcher.css',
})
export class TabSwitcher {
  readonly tabs = input<Tab[]>([]);
  readonly activeTab = input('');
  readonly tabChange = output<string>();
}
