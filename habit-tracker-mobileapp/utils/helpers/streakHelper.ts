import { Habit } from "@/types/habit";

export function analyzeStreaks(habits: Habit[]) {
  const total = habits.length;
  const totalStreaks = habits.reduce((a, h) => a + h.currentStreak, 0);
  const longest = Math.max(...habits.map(h => h.longestStreak), 0);
  const consistency =
    (habits.filter(h => h.currentStreak > 0).length / total) * 100;

  return {
    avgStreak: totalStreaks / total,
    longest,
    consistency: Math.round(consistency),
  };
}
