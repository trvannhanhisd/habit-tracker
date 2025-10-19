import { authApi, endpoints } from "./api";

import { Habit } from "@/types/habit";
import { ApiResponse } from "@/types/response";

export const createHabit = async (habit: Habit): Promise<ApiResponse<Habit>> => {
  const response = await authApi.post<ApiResponse<Habit>>(endpoints.habit, habit);
  return response.data; 
};