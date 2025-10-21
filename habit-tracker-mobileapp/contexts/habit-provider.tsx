import {
    deleteHabit,
    fetchHabits,
    markHabitDone,
} from "@/services/habit.service";
import { useEffect, useState } from "react";

import { useAuth } from "@/hooks/useAuth";
import { Habit } from "@/types/habit";
import { HabitContext } from "./habit-context";

export const HabitProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [habits, setHabits] = useState<Habit[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const { user } = useAuth();

  const loadHabits = async () => {
    if (!user) return;
    setIsLoading(true);
    try {
      const response = await fetchHabits();
      setHabits(response);
    } catch (error) {
      console.error("Failed to load habits:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const addHabit = (habit: Habit) => {
    setHabits((prevHabits) => [...prevHabits, habit]);
  };

  const deleteHabitHandler = async (id: string | number) => {
    if (!user) return;
    try {
      await deleteHabit(id);
      setHabits((prevHabits) => prevHabits.filter((habit) => habit.id !== id));
    } catch (error) {
      console.error("Failed to delete habit:", error);
      throw error; // Ném lỗi để component gọi có thể xử lý
    }
  };

  const markHabitDoneHandler = async (id: string | number): Promise<Habit> => {
    if (!user) throw new Error("User not authenticated");
    const updatedHabit = await markHabitDone(id);
    setHabits((prevHabits) =>
      prevHabits.map((habit) => (habit.id === id ? updatedHabit : habit))
    );
    return updatedHabit;
  };

  useEffect(() => {
    loadHabits();
  }, [user]);

  return (
    <HabitContext.Provider
      value={{
        habits,
        setHabits,
        addHabit,
        deleteHabit: deleteHabitHandler,
        markHabitDone: markHabitDoneHandler,
        loadHabits,
      }}
    >
      {children}
    </HabitContext.Provider>
  );
};
