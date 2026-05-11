export interface ApiResponse<T = null> {
  isSuccess: boolean;
  message: string;
  data: T | null;
}
