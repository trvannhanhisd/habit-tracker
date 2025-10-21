import { TokenResponse, User } from "@/types/user";
import api, { authApi, endpoints } from "./api";

import { ApiResponse } from "@/types/response";
import { handleApiError } from "@/utils/helpers/handleApiErrorHelper";

// Lấy thông tin user hiện tại
export const getCurrentUser = async (accessToken: string): Promise<User> => {
  const response = await authApi.get<ApiResponse<User>>(endpoints.currentUser, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  return handleApiError(response.data);
};

// Đăng nhập
export const login = async (username: string, password: string): Promise<TokenResponse> => {
  const response = await api.post<ApiResponse<TokenResponse>>(endpoints.login, {
    username,
    password,
  });
  return handleApiError(response.data);
};

// Đăng ký
export const register = async (username: string, email: string, password: string): Promise<User> => {
  const response = await api.post<ApiResponse<User>>(endpoints.register, {
    username,
    email,
    password,
  });
  return handleApiError(response.data);
};

// Làm mới token
export const refreshToken = async (refreshToken: string): Promise<TokenResponse> => {
  const response = await api.post<ApiResponse<TokenResponse>>(endpoints.refreshToken, {
    refreshToken,
  });
  return handleApiError(response.data);
};