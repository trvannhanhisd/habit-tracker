export interface User {
  id: number;
  username: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface Login {
  username: string;
  password: string;
}

export interface Register {
  username: string;
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  id: number;
  refreshToken: string;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}
