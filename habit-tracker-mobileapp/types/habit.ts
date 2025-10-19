import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";

export interface Habit {
  id?: number;
  title: string;
  description?: string;
  frequency: HabitFrequency; // Sử dụng enum thay vì string
  category: HabitCategory;
  currentStreak?: number;
  createdAt?: Date;
  isArchived?: boolean;
}
