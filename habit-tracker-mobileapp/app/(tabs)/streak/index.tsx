import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";
import { useEffect, useState } from "react";
import {
    Dimensions,
    Image,
    ScrollView,
    StyleSheet,
    View,
} from "react-native";
import {
    ActivityIndicator,
    Card,
    Surface,
    Text,
    useTheme,
} from "react-native-paper";

import { useHabits } from "@/contexts/habit-context";
import { fetchHabitLogs } from "@/services/habitLog.service";
import { HabitLog } from "@/types/habitLog";
import { MaterialCommunityIcons } from "@expo/vector-icons";

const SCREEN_WIDTH = Dimensions.get("window").width;

const CATEGORY_COLORS: Record<HabitCategory, string> = {
  [HabitCategory.General]: "#e0e0e0",
  [HabitCategory.Health]: "#4caf50",
  [HabitCategory.Fitness]: "#ff9800",
  [HabitCategory.Study]: "#7c4dff",
  [HabitCategory.Work]: "#2196f3",
  [HabitCategory.Finance]: "#26a69a",
  [HabitCategory.SelfGrowth]: "#00bcd4",
  [HabitCategory.Social]: "#e91e63",
  [HabitCategory.Creative]: "#ffc107",
  [HabitCategory.Environment]: "#689f38",
};

interface StatCard {
  icon: string;
  label: string;
  value: string | number;
  color: string;
}

export default function StreaksScreen() {
  const { habits, loadHabits } = useHabits();
  const [isLoading, setIsLoading] = useState(true);
  const [habitLogs, setHabitLogs] = useState<Record<number, HabitLog[]>>({});
  const theme = useTheme();

  useEffect(() => {
    const loadData = async () => {
      setIsLoading(true);
      try {
        await loadHabits();
        
        // Fetch logs for all habits
        const logsPromises = habits.map(async (habit) => {
          try {
            const logs = await fetchHabitLogs(habit.id);
            return { habitId: habit.id, logs };
          } catch (error) {
            console.error(`Failed to load logs for habit ${habit.id}:`, error);
            return { habitId: habit.id, logs: [] };
          }
        });
        
        const logsResults = await Promise.all(logsPromises);
        const logsMap: Record<number, HabitLog[]> = {};
        logsResults.forEach(({ habitId, logs }) => {
          logsMap[habitId] = logs;
        });
        setHabitLogs(logsMap);
      } catch (error) {
        console.error("Failed to load habits:", error);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, []);

  // Calculate statistics
  const totalHabits = habits.length;
  const activeHabits = habits.filter(h => !h.isArchived).length;
  const totalStreak = habits.reduce((sum, h) => sum + (h.currentStreak || 0), 0);
  const longestStreak = Math.max(...habits.map(h => h.longestStreak || 0), 0);
  const totalCompletions = Object.values(habitLogs).reduce(
    (sum, logs) => sum + logs.filter(log => log.isCompleted).length,
    0
  );
  const canEvolveCount = habits.filter(h => h.canEvolve).length;

  // Category distribution
  const categoryStats = habits.reduce((acc, habit) => {
    acc[habit.category] = (acc[habit.category] || 0) + 1;
    return acc;
  }, {} as Record<HabitCategory, number>);

  // Frequency distribution
  const frequencyStats = habits.reduce((acc, habit) => {
    acc[habit.frequency] = (acc[habit.frequency] || 0) + 1;
    return acc;
  }, {} as Record<HabitFrequency, number>);

  // Top performers
  const topStreakHabits = [...habits]
    .sort((a, b) => (b.currentStreak || 0) - (a.currentStreak || 0))
    .slice(0, 3);

  const mainStats: StatCard[] = [
    {
      icon: "view-dashboard",
      label: "Total Habits",
      value: totalHabits,
      color: "#2196f3",
    },
    {
      icon: "fire",
      label: "Combined Streak",
      value: `${totalStreak} days`,
      color: "#ff9800",
    },
    {
      icon: "trophy",
      label: "Longest Streak",
      value: `${longestStreak} days`,
      color: "#ffc107",
    },
    {
      icon: "check-circle",
      label: "Total Completions",
      value: totalCompletions,
      color: "#4caf50",
    },
    {
      icon: "chart-line",
      label: "Active Habits",
      value: activeHabits,
      color: "#00bcd4",
    },
    {
      icon: "pokemon-go",
      label: "Ready to Evolve",
      value: canEvolveCount,
      color: "#e91e63",
    },
  ];

  if (isLoading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={theme.colors.primary} />
        <Text style={styles.loadingText}>Loading your stats...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={styles.container} showsVerticalScrollIndicator={false}>
      {/* Header */}
      <View style={styles.header}>
        <Text variant="headlineMedium" style={styles.title}>
          Your Progress
        </Text>
        <Text variant="bodyMedium" style={styles.subtitle}>
          Track your habit journey
        </Text>
      </View>

      {/* Main Stats Grid */}
      <View style={styles.statsGrid}>
        {mainStats.map((stat, index) => (
          <Surface key={index} style={styles.statCard} elevation={2}>
            <MaterialCommunityIcons
              name={stat.icon as any}
              size={32}
              color={stat.color}
            />
            <Text variant="headlineSmall" style={styles.statValue}>
              {stat.value}
            </Text>
            <Text variant="bodySmall" style={styles.statLabel}>
              {stat.label}
            </Text>
          </Surface>
        ))}
      </View>

      {/* Top Performers */}
      <Card style={styles.sectionCard}>
        <Card.Content>
          <View style={styles.sectionHeader}>
            <MaterialCommunityIcons name="podium" size={24} color="#ff9800" />
            <Text variant="titleLarge" style={styles.sectionTitle}>
              Top Performers
            </Text>
          </View>
          {topStreakHabits.length > 0 ? (
            topStreakHabits.map((habit, index) => (
              <Surface
                key={habit.id}
                style={[
                  styles.topHabitCard,
                  { backgroundColor: CATEGORY_COLORS[habit.category] },
                ]}
                elevation={1}
              >
                <View style={styles.rankBadge}>
                  <Text style={styles.rankText}>#{index + 1}</Text>
                </View>
                <View style={styles.topHabitContent}>
                  <View style={styles.topHabitInfo}>
                    <Text style={styles.topHabitTitle}>{habit.title}</Text>
                    <View style={styles.streakBadge}>
                      <MaterialCommunityIcons
                        name="fire"
                        size={16}
                        color="#ff9800"
                      />
                      <Text style={styles.streakText}>
                        {habit.currentStreak} day streak
                      </Text>
                    </View>
                  </View>
                  {habit.petName && (
                    <Image
                      source={{
                        uri: `https://img.pokemondb.net/sprites/heartgold-soulsilver/normal/${habit.petName}.png`,
                      }}
                      style={styles.topPokemonImage}
                      resizeMode="contain"
                    />
                  )}
                </View>
              </Surface>
            ))
          ) : (
            <Text style={styles.emptyText}>
              Start building habits to see top performers!
            </Text>
          )}
        </Card.Content>
      </Card>

      {/* Category Distribution */}
      <Card style={styles.sectionCard}>
        <Card.Content>
          <View style={styles.sectionHeader}>
            <MaterialCommunityIcons
              name="shape"
              size={24}
              color="#7c4dff"
            />
            <Text variant="titleLarge" style={styles.sectionTitle}>
              Category Distribution
            </Text>
          </View>
          <View style={styles.categoryList}>
            {Object.entries(categoryStats).map(([category, count]) => {
              const percentage = ((count / totalHabits) * 100).toFixed(0);
              return (
                <View key={category} style={styles.categoryItem}>
                  <View style={styles.categoryInfo}>
                    <View
                      style={[
                        styles.categoryDot,
                        {
                          backgroundColor:
                            CATEGORY_COLORS[category as HabitCategory],
                        },
                      ]}
                    />
                    <Text style={styles.categoryName}>{category}</Text>
                  </View>
                  <View style={styles.categoryStats}>
                    <Text style={styles.categoryCount}>{count}</Text>
                    <Text style={styles.categoryPercentage}>
                      ({percentage}%)
                    </Text>
                  </View>
                </View>
              );
            })}
          </View>
        </Card.Content>
      </Card>

      {/* Frequency Breakdown */}
      <Card style={styles.sectionCard}>
        <Card.Content>
          <View style={styles.sectionHeader}>
            <MaterialCommunityIcons
              name="calendar-clock"
              size={24}
              color="#00bcd4"
            />
            <Text variant="titleLarge" style={styles.sectionTitle}>
              Frequency Breakdown
            </Text>
          </View>
          <View style={styles.frequencyList}>
            {Object.entries(frequencyStats).map(([frequency, count]) => (
              <Surface key={frequency} style={styles.frequencyChip} elevation={1}>
                <Text style={styles.frequencyLabel}>
                  {frequency.charAt(0).toUpperCase() + frequency.slice(1)}
                </Text>
                <Text style={styles.frequencyCount}>{count}</Text>
              </Surface>
            ))}
          </View>
        </Card.Content>
      </Card>

      {/* Achievement Banner */}
      {totalCompletions > 0 && (
        <Card style={[styles.sectionCard, styles.achievementCard]}>
          <Card.Content>
            <View style={styles.achievementContent}>
              <MaterialCommunityIcons
                name="star-circle"
                size={48}
                color="#ffc107"
              />
              <View style={styles.achievementText}>
                <Text variant="titleMedium" style={styles.achievementTitle}>
                  Amazing Progress!
                </Text>
                <Text variant="bodyMedium" style={styles.achievementSubtitle}>
                  You&apos;ve completed {totalCompletions} habits so far. Keep it up!
                </Text>
              </View>
            </View>
          </Card.Content>
        </Card>
      )}

      <View style={styles.bottomPadding} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  loadingContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f5f5f5",
  },
  loadingText: {
    marginTop: 16,
    color: "#666",
  },
  header: {
    padding: 20,
    paddingTop: 24,
  },
  title: {
    fontWeight: "bold",
    color: "#22223b",
  },
  subtitle: {
    color: "#6c6c80",
    marginTop: 4,
  },
  statsGrid: {
    flexDirection: "row",
    flexWrap: "wrap",
    paddingHorizontal: 12,
    marginBottom: 8,
  },
  statCard: {
    width: (SCREEN_WIDTH - 48) / 2,
    margin: 6,
    padding: 16,
    borderRadius: 16,
    alignItems: "center",
    backgroundColor: "#fff",
  },
  statValue: {
    fontWeight: "bold",
    marginTop: 8,
    color: "#22223b",
  },
  statLabel: {
    color: "#6c6c80",
    textAlign: "center",
    marginTop: 4,
  },
  sectionCard: {
    margin: 16,
    marginTop: 8,
    borderRadius: 16,
    backgroundColor: "#fff",
  },
  sectionHeader: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 16,
  },
  sectionTitle: {
    marginLeft: 8,
    fontWeight: "bold",
    color: "#22223b",
  },
  topHabitCard: {
    padding: 16,
    borderRadius: 12,
    marginBottom: 12,
    flexDirection: "row",
    alignItems: "center",
  },
  rankBadge: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: "rgba(255, 255, 255, 0.9)",
    justifyContent: "center",
    alignItems: "center",
    marginRight: 12,
  },
  rankText: {
    fontWeight: "bold",
    color: "#ff9800",
    fontSize: 16,
  },
  topHabitContent: {
    flex: 1,
    flexDirection: "row",
    alignItems: "center",
  },
  topHabitInfo: {
    flex: 1,
  },
  topHabitTitle: {
    fontSize: 16,
    fontWeight: "bold",
    color: "#22223b",
    marginBottom: 4,
  },
  streakBadge: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: "rgba(255, 255, 255, 0.8)",
    borderRadius: 12,
    paddingHorizontal: 8,
    paddingVertical: 4,
    alignSelf: "flex-start",
  },
  streakText: {
    marginLeft: 4,
    color: "#ff9800",
    fontWeight: "bold",
    fontSize: 12,
  },
  topPokemonImage: {
    width: 60,
    height: 60,
  },
  categoryList: {
    gap: 12,
  },
  categoryItem: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    paddingVertical: 8,
  },
  categoryInfo: {
    flexDirection: "row",
    alignItems: "center",
    flex: 1,
  },
  categoryDot: {
    width: 12,
    height: 12,
    borderRadius: 6,
    marginRight: 12,
  },
  categoryName: {
    fontSize: 15,
    color: "#22223b",
  },
  categoryStats: {
    flexDirection: "row",
    alignItems: "center",
  },
  categoryCount: {
    fontSize: 16,
    fontWeight: "bold",
    color: "#22223b",
    marginRight: 4,
  },
  categoryPercentage: {
    fontSize: 14,
    color: "#6c6c80",
  },
  frequencyList: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 8,
  },
  frequencyChip: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: 16,
    paddingVertical: 10,
    borderRadius: 20,
    backgroundColor: "#fff",
  },
  frequencyLabel: {
    fontSize: 14,
    color: "#22223b",
    marginRight: 8,
  },
  frequencyCount: {
    fontSize: 14,
    fontWeight: "bold",
    color: "#7c4dff",
  },
  achievementCard: {
    backgroundColor: "#fff8e1",
  },
  achievementContent: {
    flexDirection: "row",
    alignItems: "center",
  },
  achievementText: {
    flex: 1,
    marginLeft: 16,
  },
  achievementTitle: {
    fontWeight: "bold",
    color: "#22223b",
    marginBottom: 4,
  },
  achievementSubtitle: {
    color: "#6c6c80",
  },
  emptyText: {
    textAlign: "center",
    color: "#6c6c80",
    fontStyle: "italic",
    paddingVertical: 16,
  },
  bottomPadding: {
    height: 24,
  },
});