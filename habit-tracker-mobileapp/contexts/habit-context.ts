import { createContext, useContext } from "react";

import { Habit } from "@/types/habit";

export interface HabitContextType {
  habits: Habit[];
  setHabits: (habits: Habit[]) => void;
  addHabit: (habit: Habit) => void;
  deleteHabit: (id: string | number) => Promise<void>;
  markHabitDone: (id: string | number) => Promise<Habit>;
  loadHabits: () => Promise<void>;
}

export const HabitContext = createContext<HabitContextType | undefined>(undefined);

export const useHabits = () => {
  const context = useContext(HabitContext);
  if (!context) {
    throw new Error("useHabits must be used within a HabitProvider");
  }
  return context;
};