import { ApiResponse } from "@/types/response";

export const handleApiError = <T>(response: ApiResponse<T>): T => {
  if (response.status >= 400 && response.error) {
    throw new Error(response.error); // Throw lỗi với thông báo từ trường error
  }
  if (response.status >= 400) {
    throw new Error("An unexpected error occurred");
  }
  return response.data; // Trả về dữ liệu nếu không có lỗi
};