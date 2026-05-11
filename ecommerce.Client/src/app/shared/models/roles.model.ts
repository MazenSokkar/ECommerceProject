export const ROLES = {
  Admin: 'Admin',
  Merchant: 'Merchant',
  Corporate: 'Corporate',
  Receiver: 'Receiver',
  Customer: 'Customer',
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];
