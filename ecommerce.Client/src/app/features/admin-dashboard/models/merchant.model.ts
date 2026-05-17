export interface Merchant {
  id: number;
  userId: number;
  storeName: string;
  storeLogo: string | null;
  description: string | null;
  status: string;
  createdAt: string;
}

export type MerchantStatus = 'Pending' | 'Approved' | 'Rejected';

export interface UpdateMerchantStatusRequest {
  status: MerchantStatus;
}
