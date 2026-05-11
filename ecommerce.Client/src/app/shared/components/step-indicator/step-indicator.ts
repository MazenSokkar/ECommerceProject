import { Component, input } from '@angular/core';

export interface Step {
  label: string;
}

@Component({
  selector: 'app-step-indicator',
  standalone: true,
  templateUrl: './step-indicator.html',
  styleUrl: './step-indicator.css',
})
export class StepIndicator {
  readonly steps = input<Step[]>([]);
  readonly currentStep = input(1);
}
