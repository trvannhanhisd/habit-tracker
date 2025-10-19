import { TokenResponse, User } from "@/types/user";
import {
    ACCESS_TOKEN_KEY,
    REFRESH_TOKEN_KEY,
    clearToken,
    getToken,
    setToken,
} from "@/utils/helpers/authHelpers";
import React, { useEffect, useState } from "react";
import api, { authApi, endpoints } from "../services/api";

import { ApiResponse } from "@/types/response";
import { AuthContext } from "./auth-context";

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoadingUser, setIsLoadingUser] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      try {
        const accessToken = await getToken(ACCESS_TOKEN_KEY); // await
        if (accessToken) {
          const res = await authApi.get(endpoints.currentUser);
          setUser(res.data.data);
        }
      } catch (err) {
        console.error("Không thể lấy user:", err);
        setUser(null);
      } finally {
        setIsLoadingUser(false);
      }
    };

    initAuth();
  }, []);

  const login = async (username: string, password: string) => {
    
    try {
      const res = await api.post<ApiResponse<TokenResponse>>(endpoints.login, {
        username,
        password,
      });
      const { accessToken, refreshToken } = res.data.data; // Lấy từ res.data.data

      await setToken(ACCESS_TOKEN_KEY, accessToken); // await
      await setToken(REFRESH_TOKEN_KEY, refreshToken); // await

      const me = await authApi.get<ApiResponse<User>>(endpoints.currentUser, {
        headers: { Authorization: `Bearer ${accessToken}` },
      });
      setUser(me.data.data);
      return null;
    } catch (error) {
      if (error instanceof Error) {
        return error.message;
      }
      return "An error occurred during Sign In";
    }
  };

  const register = async (
    username: string,
    email: string,
    password: string
  ) => {
    try {
      await api.post<ApiResponse<User>>(endpoints.register, {
        username,
        email,
        password,
      });
      await login(username, password); // Sau khi đăng ký, tự động đăng nhập
      return null;
    } catch (error) {
      if (error instanceof Error) {
        return error.message;
      }
      return "An error occurred during Sign Up";
    }
  };

  const logout = async () => {
    try {
      await clearToken();
      setUser(null);
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <AuthContext.Provider
      value={{ user, setUser, isLoadingUser, login, register, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
};
