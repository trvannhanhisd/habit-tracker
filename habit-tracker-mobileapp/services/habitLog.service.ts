import { authApi, endpoints } from "./api";

import { HabitLog } from "@/types/habitLog";
import { ApiResponse } from "@/types/response";
import { handleApiError } from "@/utils/helpers/handleApiErrorHelper";

export const fetchHabitLogs = async (id: string | number): Promise<HabitLog[]> => {
  const response = await authApi.get<ApiResponse<HabitLog[]>>(`${endpoints.habit}/${id}/logs`);
  return handleApiError(response.data); 
};