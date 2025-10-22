import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";
import { format, isSameDay, isSameMonth, isSameWeek } from "date-fns";
import { useLocalSearchParams, useRouter } from "expo-router";
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
    Button,
    Card,
    ProgressBar,
    Surface,
    Text,
    useTheme,
} from "react-native-paper";

import { useHabits } from "@/contexts/habit-context";
import { fetchHabitLogs } from "@/services/habitLog.service";
import { Habit } from "@/types/habit";
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

const isHabitCompleted = (habit: Habit, now: Date = new Date()): boolean => {
  if (!habit.lastCompletedAt) return false;

  switch (habit.frequency) {
    case HabitFrequency.Daily:
      return isSameDay(habit.lastCompletedAt, now);
    case HabitFrequency.Weekly:
      return isSameWeek(habit.lastCompletedAt, now, { weekStartsOn: 1 });
    case HabitFrequency.Monthly:
      return isSameMonth(habit.lastCompletedAt, now);
    default:
      return false;
  }
};

// Tính toán milestone tiến hóa tiếp theo
const getNextEvolutionMilestone = (currentStreak: number): number => {
  if (currentStreak < 10) return 10;
  if (currentStreak < 20) return 20;
  if (currentStreak < 30) return 30;
  if (currentStreak < 50) return 50;
  if (currentStreak < 100) return 100;
  // Sau 100, cứ mỗi 50 ngày là 1 milestone
  return Math.ceil(currentStreak / 50) * 50 + 50;
};

export default function HabitDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const { habits, markHabitDone, deleteHabit } = useHabits();
  const [habit, setHabit] = useState<Habit | null>(null);
  const [logs, setLogs] = useState<HabitLog[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();
  const theme = useTheme();

  useEffect(() => {
    const loadHabitDetail = async () => {
      setIsLoading(true);
      try {
        const foundHabit = habits.find((h) => h.id === Number(id));
        if (!foundHabit) {
          setError("Habit not found");
          return;
        }
        setHabit(foundHabit);

        // Load logs
        const habitLogs = await fetchHabitLogs(Number(id));
        setLogs(habitLogs);
      } catch (err) {
        setError("Failed to load habit details");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    if (id) {
      loadHabitDetail();
    }
  }, [id, habits]);

  const handleMarkDone = async () => {
    if (!habit) return;
    try {
      const updatedHabit = await markHabitDone(habit.id);
      setHabit(updatedHabit);
    } catch (err) {
      setError("Failed to mark habit as done");
    }
  };

  const handleDelete = async () => {
    if (!habit) return;
    try {
      await deleteHabit(habit.id);
      router.back();
    } catch (err) {
      setError("Failed to delete habit");
    }
  };

  if (isLoading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={theme.colors.primary} />
      </View>
    );
  }

  if (error || !habit) {
    return (
      <View style={styles.errorContainer}>
        <MaterialCommunityIcons name="alert-circle" size={48} color="#e53935" />
        <Text style={styles.errorText}>{error || "Habit not found"}</Text>
        <Button mode="contained" onPress={() => router.back()}>
          Go Back
        </Button>
      </View>
    );
  }

  const isCompleted = isHabitCompleted(habit);
  const categoryColor = CATEGORY_COLORS[habit.category];
  const completionRate = logs.length > 0 
    ? (logs.filter(log => log.isCompleted).length / logs.length) * 100 
    : 0;

  // Evolution milestone calculation
  const nextMilestone = habit.canEvolve ? getNextEvolutionMilestone(habit.currentStreak) : null;
  const previousMilestone = nextMilestone 
    ? (nextMilestone === 10 ? 0 : nextMilestone === 20 ? 10 : nextMilestone === 30 ? 20 : nextMilestone === 50 ? 30 : nextMilestone === 100 ? 50 : nextMilestone - 50)
    : 0;
  const progressToNextMilestone = nextMilestone 
    ? ((habit.currentStreak - previousMilestone) / (nextMilestone - previousMilestone))
    : 0;

  return (
    <ScrollView style={styles.container} showsVerticalScrollIndicator={false}>
      {/* Hero Section with Pokémon */}
      <Surface
        style={[styles.heroSection, { backgroundColor: categoryColor }]}
        elevation={4}
      >
        <View style={styles.heroContent}>
          <View style={styles.heroText}>
            <Text variant="headlineMedium" style={styles.habitTitle}>
              {habit.title}
            </Text>
            <Text variant="bodyLarge" style={styles.habitDescription}>
              {habit.description}
            </Text>
            <View style={styles.badges}>
              <Surface style={styles.badge} elevation={0}>
                <MaterialCommunityIcons
                  name="calendar-clock"
                  size={16}
                  color="#7c4dff"
                />
                <Text style={styles.badgeText}>{habit.frequency}</Text>
              </Surface>
              <Surface style={styles.badge} elevation={0}>
                <MaterialCommunityIcons
                  name="shape"
                  size={16}
                  color="#00bcd4"
                />
                <Text style={styles.badgeText}>{habit.category}</Text>
              </Surface>
            </View>
          </View>
          {habit.petName && (
            <Image
              source={{
                uri: `https://img.pokemondb.net/artwork/large/${habit.petName.toLowerCase()}.jpg`,
              }}
              style={styles.heroPokemon}
              resizeMode="contain"
            />
          )}
        </View>
      </Surface>

      {/* Streak Stats */}
      <Card style={styles.card}>
        <Card.Content>
          <View style={styles.statsHeader}>
            <MaterialCommunityIcons name="fire" size={28} color="#ff9800" />
            <Text variant="titleLarge" style={styles.sectionTitle}>
              Streak Statistics
            </Text>
          </View>
          <View style={styles.streakStats}>
            <View style={styles.statBox}>
              <Text variant="displaySmall" style={styles.statNumber}>
                {habit.currentStreak}
              </Text>
              <Text variant="bodyMedium" style={styles.statLabel}>
                Current Streak
              </Text>
            </View>
            <View style={styles.statDivider} />
            <View style={styles.statBox}>
              <Text variant="displaySmall" style={styles.statNumber}>
                {habit.longestStreak}
              </Text>
              <Text variant="bodyMedium" style={styles.statLabel}>
                Longest Streak
              </Text>
            </View>
          </View>
        </Card.Content>
      </Card>

      {/* Evolution Progress */}
      {habit.canEvolve && nextMilestone && (
        <Card style={styles.card}>
          <Card.Content>
            <View style={styles.evolutionHeader}>
              <MaterialCommunityIcons
                name="star-circle"
                size={28}
                color="#ffc107"
              />
              <Text variant="titleLarge" style={styles.sectionTitle}>
                Evolution Progress
              </Text>
            </View>
            <View style={styles.milestoneContainer}>
              <View style={styles.milestoneInfo}>
                <Text variant="bodyLarge" style={styles.milestoneText}>
                  {habit.currentStreak} / {nextMilestone} days
                </Text>
                <Text variant="bodySmall" style={styles.milestoneSubtext}>
                  {nextMilestone - habit.currentStreak} days until evolution!
                </Text>
              </View>
              <ProgressBar
                progress={progressToNextMilestone}
                color="#ffc107"
                style={styles.progressBar}
              />
              <View style={styles.milestoneMarkers}>
                <View style={styles.marker}>
                  <View style={[styles.markerDot, styles.markerDotActive]} />
                  <Text style={styles.markerText}>{previousMilestone}</Text>
                </View>
                <View style={styles.marker}>
                  <View style={styles.markerDot} />
                  <Text style={styles.markerText}>{nextMilestone}</Text>
                </View>
              </View>
            </View>
            {habit.petName && (
              <View style={styles.pokemonPreview}>
                <Image
                  source={{
                    uri: `https://img.pokemondb.net/sprites/heartgold-soulsilver/normal/${habit.petName.toLowerCase()}.png`,
                  }}
                  style={styles.smallPokemon}
                  resizeMode="contain"
                />
                <MaterialCommunityIcons
                  name="arrow-right-bold"
                  size={32}
                  color="#ffc107"
                />
                <View style={styles.evolutionMystery}>
                  <MaterialCommunityIcons
                    name="help-circle"
                    size={48}
                    color="#999"
                  />
                  <Text style={styles.mysteryText}>???</Text>
                </View>
              </View>
            )}
          </Card.Content>
        </Card>
      )}

      {/* Activity Overview */}
      <Card style={styles.card}>
        <Card.Content>
          <View style={styles.statsHeader}>
            <MaterialCommunityIcons
              name="chart-line"
              size={28}
              color="#2196f3"
            />
            <Text variant="titleLarge" style={styles.sectionTitle}>
              Activity Overview
            </Text>
          </View>
          <View style={styles.activityStats}>
            <View style={styles.activityRow}>
              <Text style={styles.activityLabel}>Total Completions</Text>
              <Text style={styles.activityValue}>
                {logs.filter(log => log.isCompleted).length}
              </Text>
            </View>
            <View style={styles.activityRow}>
              <Text style={styles.activityLabel}>Completion Rate</Text>
              <Text style={styles.activityValue}>{completionRate.toFixed(1)}%</Text>
            </View>
            <View style={styles.activityRow}>
              <Text style={styles.activityLabel}>Started</Text>
              <Text style={styles.activityValue}>
                {format(new Date(habit.createdAt), "MMM dd, yyyy")}
              </Text>
            </View>
            {habit.lastCompletedAt && (
              <View style={styles.activityRow}>
                <Text style={styles.activityLabel}>Last Completed</Text>
                <Text style={styles.activityValue}>
                  {format(new Date(habit.lastCompletedAt), "MMM dd, yyyy")}
                </Text>
              </View>
            )}
          </View>
        </Card.Content>
      </Card>

      {/* Recent Activity */}
      <Card style={styles.card}>
        <Card.Content>
          <View style={styles.statsHeader}>
            <MaterialCommunityIcons
              name="history"
              size={28}
              color="#00bcd4"
            />
            <Text variant="titleLarge" style={styles.sectionTitle}>
              Recent Activity
            </Text>
          </View>
          {logs.length > 0 ? (
            <View style={styles.logsList}>
              {logs.slice(0, 10).map((log) => (
                <View key={log.id} style={styles.logItem}>
                  <MaterialCommunityIcons
                    name={log.isCompleted ? "check-circle" : "circle-outline"}
                    size={20}
                    color={log.isCompleted ? "#4caf50" : "#999"}
                  />
                  <Text style={styles.logDate}>
                    {format(new Date(log.date), "MMM dd, yyyy")}
                  </Text>
                  {log.isCompleted && (
                    <MaterialCommunityIcons
                      name="fire"
                      size={16}
                      color="#ff9800"
                    />
                  )}
                </View>
              ))}
            </View>
          ) : (
            <Text style={styles.emptyText}>No activity yet. Start today!</Text>
          )}
        </Card.Content>
      </Card>

      {/* Action Buttons */}
      <View style={styles.actions}>
        {!isCompleted ? (
          <Button
            mode="contained"
            onPress={handleMarkDone}
            icon="check-circle"
            style={styles.markDoneButton}
            contentStyle={styles.buttonContent}
          >
            Mark as Done
          </Button>
        ) : (
          <Surface style={styles.completedBanner} elevation={0}>
            <MaterialCommunityIcons
              name="check-circle"
              size={24}
              color="#4caf50"
            />
            <Text style={styles.completedText}>Completed Today!</Text>
          </Surface>
        )}
        <Button
          mode="outlined"
          onPress={handleDelete}
          icon="delete"
          textColor="#e53935"
          style={styles.deleteButton}
        >
          Delete Habit
        </Button>
      </View>

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
  errorContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    padding: 24,
    backgroundColor: "#f5f5f5",
  },
  errorText: {
    fontSize: 16,
    color: "#666",
    marginVertical: 16,
    textAlign: "center",
  },
  heroSection: {
    borderBottomLeftRadius: 32,
    borderBottomRightRadius: 32,
    overflow: "hidden",
  },
  heroContent: {
    flexDirection: "row",
    padding: 24,
    paddingTop: 32,
    minHeight: 200,
  },
  heroText: {
    flex: 1,
    justifyContent: "center",
  },
  habitTitle: {
    fontWeight: "bold",
    color: "#22223b",
    marginBottom: 8,
  },
  habitDescription: {
    color: "#22223b",
    opacity: 0.8,
    marginBottom: 16,
  },
  badges: {
    flexDirection: "row",
    gap: 8,
    flexWrap: "wrap",
  },
  badge: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: "rgba(255, 255, 255, 0.9)",
    borderRadius: 16,
    paddingHorizontal: 12,
    paddingVertical: 6,
    gap: 4,
  },
  badgeText: {
    fontSize: 12,
    fontWeight: "600",
    color: "#22223b",
  },
  heroPokemon: {
    width: 140,
    height: 140,
    marginLeft: 16,
  },
  card: {
    margin: 16,
    marginTop: 16,
    borderRadius: 16,
    backgroundColor: "#fff",
  },
  statsHeader: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 16,
  },
  sectionTitle: {
    marginLeft: 8,
    fontWeight: "bold",
    color: "#22223b",
  },
  streakStats: {
    flexDirection: "row",
    justifyContent: "space-around",
    paddingVertical: 16,
  },
  statBox: {
    alignItems: "center",
    flex: 1,
  },
  statNumber: {
    fontWeight: "bold",
    color: "#ff9800",
  },
  statLabel: {
    color: "#6c6c80",
    marginTop: 4,
    textAlign: "center",
  },
  statDivider: {
    width: 1,
    backgroundColor: "#e0e0e0",
    marginHorizontal: 16,
  },
  evolutionHeader: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 16,
  },
  milestoneContainer: {
    paddingVertical: 8,
  },
  milestoneInfo: {
    marginBottom: 12,
  },
  milestoneText: {
    fontWeight: "bold",
    color: "#22223b",
    marginBottom: 4,
  },
  milestoneSubtext: {
    color: "#6c6c80",
  },
  progressBar: {
    height: 12,
    borderRadius: 6,
    backgroundColor: "#f0f0f0",
  },
  milestoneMarkers: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginTop: 8,
  },
  marker: {
    alignItems: "center",
  },
  markerDot: {
    width: 12,
    height: 12,
    borderRadius: 6,
    backgroundColor: "#e0e0e0",
    marginBottom: 4,
  },
  markerDotActive: {
    backgroundColor: "#ffc107",
  },
  markerText: {
    fontSize: 12,
    color: "#6c6c80",
  },
  pokemonPreview: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    marginTop: 24,
    gap: 16,
  },
  smallPokemon: {
    width: 80,
    height: 80,
  },
  evolutionMystery: {
    alignItems: "center",
    justifyContent: "center",
    width: 80,
    height: 80,
    backgroundColor: "#f5f5f5",
    borderRadius: 12,
  },
  mysteryText: {
    fontSize: 14,
    fontWeight: "bold",
    color: "#999",
    marginTop: 4,
  },
  activityStats: {
    gap: 12,
  },
  activityRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  activityLabel: {
    fontSize: 15,
    color: "#6c6c80",
  },
  activityValue: {
    fontSize: 15,
    fontWeight: "bold",
    color: "#22223b",
  },
  logsList: {
    gap: 8,
  },
  logItem: {
    flexDirection: "row",
    alignItems: "center",
    paddingVertical: 8,
    gap: 12,
  },
  logDate: {
    flex: 1,
    fontSize: 14,
    color: "#22223b",
  },
  emptyText: {
    textAlign: "center",
    color: "#999",
    fontStyle: "italic",
    paddingVertical: 16,
  },
  actions: {
    padding: 16,
    gap: 12,
  },
  markDoneButton: {
    borderRadius: 12,
  },
  buttonContent: {
    paddingVertical: 8,
  },
  deleteButton: {
    borderRadius: 12,
    borderColor: "#e53935",
  },
  completedBanner: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: "#e8f5e9",
    borderRadius: 12,
    padding: 16,
    gap: 8,
  },
  completedText: {
    fontSize: 16,
    fontWeight: "bold",
    color: "#4caf50",
  },
  bottomPadding: {
    height: 24,
  },
});