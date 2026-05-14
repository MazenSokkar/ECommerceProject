import { Component, EventEmitter, inject, Input, Output, signal, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BannersApiService } from '../../../../core/services/banners-api.service';
import { Banner } from '../../../../shared/models/banner.model';
import { ToastService } from '../../../../core/services/toast.service';
import { Button } from '../../../../shared/components/button/button';
import { FormField } from '../../../../shared/components/form-field/form-field';

@Component({
  selector: 'app-banner-form-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Button, FormField],
  template: `
    @if (isOpen) {
      <!-- Backdrop -->
      <div class="fixed inset-0 bg-neutral-900/50 backdrop-blur-sm z-40 transition-opacity"></div>
      
      <!-- Modal -->
      <div class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6 overflow-y-auto">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-xl w-full max-w-2xl transform transition-all relative overflow-hidden flex flex-col max-h-[90vh]">
          
          <!-- Header -->
          <div class="px-6 py-5 border-b border-neutral-100 dark:border-slate-700 flex items-center justify-between shrink-0">
            <h3 class="text-xl font-bold text-neutral-800 dark:text-white">
              {{ banner ? 'Edit Banner' : 'Add New Banner' }}
            </h3>
            <button (click)="close.emit()" class="text-neutral-400 hover:text-neutral-600 dark:hover:text-neutral-300 transition-colors w-8 h-8 flex items-center justify-center rounded-lg hover:bg-neutral-100 dark:hover:bg-slate-700">
              <i class="lni lni-close"></i>
            </button>
          </div>
          
          <!-- Body -->
          <div class="p-6 overflow-y-auto">
            <form [formGroup]="form" class="space-y-5">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
                  <app-form-field label="Title" [control]="form.controls.title">
                    <input type="text" formControlName="title" class="form-input w-full" placeholder="E.g., Summer Sale" />
                  </app-form-field>

                  <app-form-field label="Display Order" [control]="form.controls.displayOrder">
                    <input type="number" formControlName="displayOrder" class="form-input w-full" />
                  </app-form-field>
              </div>

              <app-form-field label="Content / Subtitle" [control]="form.controls.content">
                <textarea formControlName="content" class="form-input w-full h-24 resize-none" placeholder="Description or sub-text..."></textarea>
              </app-form-field>

              <app-form-field label="Image URL" [control]="form.controls.imageUrl">
                <input type="url" formControlName="imageUrl" class="form-input w-full" placeholder="https://example.com/image.jpg" />
              </app-form-field>

              <!-- Image Preview -->
              @if (form.controls.imageUrl.value) {
                  <div class="w-full h-32 rounded-xl bg-neutral-100 dark:bg-slate-700 overflow-hidden border border-neutral-200 dark:border-slate-600">
                      <img [src]="form.controls.imageUrl.value" class="w-full h-full object-cover" (error)="onImageError($event)" alt="Preview" />
                  </div>
              }

              <app-form-field label="Link URL (Optional)" [control]="form.controls.linkUrl">
                <input type="url" formControlName="linkUrl" class="form-input w-full" placeholder="https://example.com/promo" />
              </app-form-field>

              <div class="flex items-center gap-2 mt-2">
                <input type="checkbox" id="isActive" formControlName="isActive" class="w-4 h-4 text-primary bg-neutral-100 border-neutral-300 rounded focus:ring-primary dark:bg-slate-700 dark:border-slate-600" />
                <label for="isActive" class="text-sm font-medium text-neutral-700 dark:text-neutral-300">Is Active (Visible)</label>
              </div>
            </form>
          </div>

          <!-- Footer -->
          <div class="px-6 py-4 border-t border-neutral-100 dark:border-slate-700 bg-neutral-50 dark:bg-slate-800/50 flex justify-end gap-3 shrink-0">
            <app-button variant="outline" (click)="close.emit()" [disabled]="submitting()">Cancel</app-button>
            <app-button variant="primary" (click)="save()" [disabled]="form.invalid || submitting()">
              @if (submitting()) {
                <i class="lni lni-spinner-solid animate-spin mr-2"></i> Saving...
              } @else {
                Save Banner
              }
            </app-button>
          </div>
          
        </div>
      </div>
    }
  `
})
export class BannerFormModal implements OnChanges {
  @Input() isOpen = false;
  @Input() banner: Banner | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  private readonly fb = inject(FormBuilder);
  private readonly api = inject(BannersApiService);
  private readonly toast = inject(ToastService);

  protected readonly submitting = signal(false);

  protected form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    content: ['', [Validators.maxLength(1000)]],
    imageUrl: ['', [Validators.required, Validators.maxLength(500)]],
    linkUrl: ['', [Validators.maxLength(500)]],
    isActive: [true],
    displayOrder: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnChanges(changes: SimpleChanges) {
    if (changes['isOpen'] && this.isOpen) {
      if (this.banner) {
        this.form.patchValue({
          title: this.banner.title,
          content: this.banner.content ?? '',
          imageUrl: this.banner.imageUrl,
          linkUrl: this.banner.linkUrl ?? '',
          isActive: this.banner.isActive,
          displayOrder: this.banner.displayOrder
        });
      } else {
        this.form.reset({ isActive: true, displayOrder: 1, title: '', content: '', imageUrl: '', linkUrl: '' });
      }
    }
  }

  onImageError(event: Event) {
      (event.target as HTMLImageElement).src = 'assets/images/placeholder.png'; // Assuming fallback
  }

  save() {
    if (this.form.invalid || this.submitting()) return;
    this.submitting.set(true);

    const data = this.form.getRawValue();
    const req = {
        title: data.title,
        content: data.content || null,
        imageUrl: data.imageUrl,
        linkUrl: data.linkUrl || null,
        isActive: data.isActive,
        displayOrder: data.displayOrder
    };

    const obs$ = this.banner 
      ? this.api.update(this.banner.id, req)
      : this.api.create(req);

    obs$.subscribe({
      next: () => {
        this.toast.success('Success', `Banner ${this.banner ? 'updated' : 'created'} successfully`);
        this.submitting.set(false);
        this.saved.emit();
      },
      error: () => {
        this.toast.error('Error', 'Failed to save banner');
        this.submitting.set(false);
      }
    });
  }
}
