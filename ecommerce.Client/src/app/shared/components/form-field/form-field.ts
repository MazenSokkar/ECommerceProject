import { Component, computed, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

const DEFAULT_ERRORS: Record<string, string> = {
  required: 'This field is required.',
  email: 'Enter a valid email address.',
  maxlength: 'Value is too long.',
  pattern: 'Invalid format.',
};

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './form-field.html',
  styleUrl: './form-field.css',
})
export class FormField {
  readonly label = input('');
  readonly control = input.required<FormControl>();
  readonly type = input<'text' | 'email' | 'tel' | 'number' | 'url'>('text');
  readonly placeholder = input('');
  readonly required = input(false);
  readonly hint = input('');
  readonly prefix = input('');
  readonly autocomplete = input('off');
  readonly inputmode = input('');
  readonly errorMessages = input<Record<string, string>>({});

  private readonly _uid = `ff-${Math.random().toString(36).slice(2, 9)}`;
  protected readonly inputId = this._uid;

  protected readonly hasError = computed(() => this.control().invalid && this.control().touched);

  protected readonly errorText = computed(() => {
    if (!this.hasError()) return '';
    const errs = this.control().errors ?? {};
    const merged = { ...DEFAULT_ERRORS, ...this.errorMessages() };
    const key = Object.keys(errs)[0];
    if (key === 'minlength') {
      return `Minimum ${errs['minlength'].requiredLength} characters.`;
    }
    return merged[key] ?? 'Invalid value.';
  });
}
