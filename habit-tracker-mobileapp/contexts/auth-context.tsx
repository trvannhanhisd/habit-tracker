import { User } from "@/types/user";
import { createContext } from "react";

export interface AuthContextType {
  user: User | null;
  setUser: (user: User | null) => void;
  loading: boolean;
  login: (username: string, password: string) => Promise<string | null>;
  register: (username: string, email: string, password: string) => Promise<string | null>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);