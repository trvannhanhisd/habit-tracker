export interface ApiResponse<T> {
  status: number;
  data: T;
  error: string | null;
  timestamp: string;
}