import AsyncStorage from '@react-native-async-storage/async-storage';

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";

// Lấy token từ AsyncStorage
export const getToken = async (key: string): Promise<string | null> => {
  try {
    return await AsyncStorage.getItem(key);
  } catch (error) {
    console.error(`Error getting token ${key}:`, error);
    return null;
  }
};

// Lưu token vào AsyncStorage
export const setToken = async (key: string, value: string, days = 7) => {
  try {
    await AsyncStorage.setItem(key, value);
    // AsyncStorage không hỗ trợ expires/max-age như cookie.
    // Nếu cần expires, bạn phải tự quản lý logic (ví dụ: lưu thêm timestamp).
  } catch (error) {
    console.error(`Error setting token ${key}:`, error);
  }
};

// Xóa token khỏi AsyncStorage
export const clearToken = async () => {
  try {
    await AsyncStorage.removeItem(ACCESS_TOKEN_KEY);
    await AsyncStorage.removeItem(REFRESH_TOKEN_KEY);
  } catch (error) {
    console.error('Error clearing tokens:', error);
  }
};

export { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY };

