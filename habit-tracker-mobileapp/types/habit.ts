import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";

export interface Habit {
  id: number;
  title: string;
  description: string;
  frequency: HabitFrequency; 
  category: HabitCategory;
  currentStreak: number;
  longestStreak: number;
  createdAt: Date;
  isArchived: boolean;
  lastCompletedAt: Date | null;
  petName: string | null;
  canEvolve: boolean;
}

export interface HabitCreate {
  title: string;
  description?: string;
  frequency: HabitFrequency; 
  category: HabitCategory;
}
