import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShippingTracking } from './shipping-tracking';

describe('ShippingTracking', () => {
  let component: ShippingTracking;
  let fixture: ComponentFixture<ShippingTracking>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShippingTracking]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShippingTracking);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
