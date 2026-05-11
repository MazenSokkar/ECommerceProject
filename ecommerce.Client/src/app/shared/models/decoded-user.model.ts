export interface DecodedUser {
  sub: string;
  email: string;
  roles: string[];
  exp?: number;
}
