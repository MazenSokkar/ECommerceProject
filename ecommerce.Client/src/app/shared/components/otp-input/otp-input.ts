import {
  AfterViewInit,
  Component,
  ElementRef,
  QueryList,
  ViewChildren,
  input,
  output,
  signal,
} from '@angular/core';

@Component({
  selector: 'app-otp-input',
  standalone: true,
  templateUrl: './otp-input.html',
  styleUrl: './otp-input.css',
})
export class OtpInput implements AfterViewInit {
  readonly hasError = input<boolean>(false);
  readonly valueChange = output<string>();

  protected readonly digits = signal<string[]>(['', '', '', '', '', '']);
  protected readonly shaking = signal(false);

  @ViewChildren('digit') private readonly digitRefs!: QueryList<ElementRef<HTMLInputElement>>;

  ngAfterViewInit(): void {
    this.digitRefs.first?.nativeElement.focus();
  }

  protected onInput(index: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const val = input.value.replace(/\D/g, '').slice(-1);
    const updated = [...this.digits()];
    updated[index] = val;
    this.digits.set(updated);
    this.valueChange.emit(updated.join(''));
    if (val && index < 5) {
      this.digitRefs.toArray()[index + 1]?.nativeElement.focus();
    }
  }

  protected onKeydown(index: number, event: KeyboardEvent): void {
    if (event.key === 'Backspace') {
      if (!this.digits()[index] && index > 0) {
        const updated = [...this.digits()];
        updated[index - 1] = '';
        this.digits.set(updated);
        this.valueChange.emit(updated.join(''));
        this.digitRefs.toArray()[index - 1]?.nativeElement.focus();
      }
    }
  }

  protected onPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const text = event.clipboardData?.getData('text') ?? '';
    const cleaned = text.replace(/\D/g, '').slice(0, 6);
    const updated = Array.from({ length: 6 }, (_, i) => cleaned[i] ?? '');
    this.digits.set(updated);
    this.valueChange.emit(updated.join(''));
    const focusIndex = Math.min(cleaned.length, 5);
    this.digitRefs.toArray()[focusIndex]?.nativeElement.focus();
  }

  shake(): void {
    this.shaking.set(true);
    setTimeout(() => this.shaking.set(false), 600);
  }

  clear(): void {
    this.digits.set(['', '', '', '', '', '']);
    this.valueChange.emit('');
    setTimeout(() => this.digitRefs.first?.nativeElement.focus());
  }

  getValue(): string {
    return this.digits().join('');
  }

  protected readonly indices = [0, 1, 2, 3, 4, 5];
}
