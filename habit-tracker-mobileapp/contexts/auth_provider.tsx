import { getCurrentUser, login, register } from "@/services/auth.service";
import {
  ACCESS_TOKEN_KEY,
  REFRESH_TOKEN_KEY,
  clearToken,
  getToken,
  setToken,
} from "@/utils/helpers/authHelpers";
import React, { useEffect, useState } from "react";

import { User } from "@/types/user";
import { AuthContext } from "./auth-context";

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoadingUser, setIsLoadingUser] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      try {
        const accessToken = await getToken(ACCESS_TOKEN_KEY);
        if (accessToken) {
          const userData = await getCurrentUser(accessToken);
          setUser(userData);
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

  const loginHandler = async (username: string, password: string) => {
    try {
      const { accessToken, refreshToken } = await login(username, password);
      await setToken(ACCESS_TOKEN_KEY, accessToken);
      await setToken(REFRESH_TOKEN_KEY, refreshToken);
      const userData = await getCurrentUser(accessToken);
      setUser(userData);
      return null;
    } catch (error) {
      if (error instanceof Error) {
        return error.message;
      }
      return "An error occurred during Sign In";
    }
  };

  const registerHandler = async (username: string, email: string, password: string) => {
    try {
      await register(username, email, password);
      await loginHandler(username, password); // Tự động đăng nhập sau khi đăng ký
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
      console.error("Error during logout:", error);
    }
  };

  return (
    <AuthContext.Provider
      value={{ user, setUser, isLoadingUser, login: loginHandler, register: registerHandler, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
};