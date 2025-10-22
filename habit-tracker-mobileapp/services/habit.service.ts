import { Habit, HabitCreate } from "@/types/habit";
import { authApi, endpoints } from "./api";

import { ApiResponse } from "@/types/response";
import { handleApiError } from "@/utils/helpers/handleApiErrorHelper";

export const createHabit = async (habit: HabitCreate): Promise<Habit> => {
  const response = await authApi.post<ApiResponse<Habit>>(endpoints.habit, habit);
  return handleApiError(response.data); 
};

export const fetchHabits = async (): Promise<Habit[]> => {
  const response = await authApi.get<ApiResponse<Habit[]>>(endpoints.userHabits );
  return handleApiError(response.data); 
};

export const deleteHabit = async (id: string | number): Promise<void> => {
  const response = await authApi.delete<ApiResponse<void>>(`${endpoints.habit}/${id}`);
  handleApiError(response.data);
};

export const markHabitDone = async (id: string | number): Promise<Habit> => {
  const response = await authApi.put<ApiResponse<Habit>>(`${endpoints.habit}/${id}/done`);
  return handleApiError(response.data);
};