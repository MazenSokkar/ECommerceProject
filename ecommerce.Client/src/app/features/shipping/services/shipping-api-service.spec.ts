import { TestBed } from '@angular/core/testing';

import { ShippingApiService } from './shipping-api-service';

describe('ShippingApiService', () => {
  let service: ShippingApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShippingApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
