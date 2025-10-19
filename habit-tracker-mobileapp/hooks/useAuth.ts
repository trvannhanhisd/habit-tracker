import { AuthContext } from "@/contexts/auth-context";
import { useContext } from "react";

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) throw new Error("useAuth phải dùng trong <AuthProvider>");
  return context;
};