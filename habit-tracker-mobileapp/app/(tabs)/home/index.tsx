import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";
import { isSameDay, isSameMonth, isSameWeek } from "date-fns";
import { useEffect, useRef, useState } from "react";
import {
  Alert,
  Image,
  ImageBackground,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  View,
} from "react-native";
import {
  ActivityIndicator,
  Button,
  Surface,
  Text,
  useTheme,
} from "react-native-paper";

import { useHabits } from "@/contexts/habit-context";
import { useAuth } from "@/hooks/useAuth";
import { Habit } from "@/types/habit";
import { MaterialCommunityIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import { Swipeable } from "react-native-gesture-handler";

// Bảng màu đơn theo HabitCategory
const CATEGORY_COLORS: Record<HabitCategory, string> = {
  [HabitCategory.General]: "#e0e0e0", // Xám nhạt
  [HabitCategory.Health]: "#4caf50", // Xanh lá
  [HabitCategory.Fitness]: "#ff9800", // Cam
  [HabitCategory.Study]: "#7c4dff", // Tím
  [HabitCategory.Work]: "#2196f3", // Xanh dương
  [HabitCategory.Finance]: "#26a69a", // Xanh lam
  [HabitCategory.SelfGrowth]: "#00bcd4", // Xanh ngọc
  [HabitCategory.Social]: "#e91e63", // Hồng
  [HabitCategory.Creative]: "#ffc107", // Vàng
  [HabitCategory.Environment]: "#689f38", // Xanh olive
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

export default function Index() {
  const router = useRouter();
  const { logout, user } = useAuth();
  const { habits, loadHabits, deleteHabit, markHabitDone } = useHabits();
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const theme = useTheme();

  const swipeableRefs = useRef<{ [key: string]: Swipeable | null }>({});

  const sortedHabits = [...(habits || [])].sort((a, b) => {
    const aCompleted = isHabitCompleted(a);
    const bCompleted = isHabitCompleted(b);
    if (aCompleted === bCompleted) return 0;
    return aCompleted ? 1 : -1;
  });

  const handleLoadHabits = async () => {
    setIsLoading(true);
    setError(null);
    try {
      await loadHabits();
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("An unexpected error occurred. Please try again.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteHabit = async (id: number) => {
    setIsLoading(true);
    setError(null);
    try {
      await deleteHabit(id);
      swipeableRefs.current[id]?.close();
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to delete habit. Please try again.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleMarkHabitDone = async (id: number) => {
    if (!user) {
      setError("You must be logged in to mark a habit as done.");
      return;
    }
    setIsLoading(true);
    setError(null);
    try {
      const updatedHabit = await markHabitDone(id);
      if (!updatedHabit) {
        setError("Failed to mark habit as done. No user logged in.");
        return;
      }
      swipeableRefs.current[id]?.close();
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to mark habit as done. Please try again.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const renderRightActions = (habit: Habit) => {
    if (isHabitCompleted(habit)) {
      return null;
    }
    return (
      <View style={styles.swipeActionRight}>
        <MaterialCommunityIcons
          name="check-circle-outline"
          size={32}
          color={"#fff"}
          onPress={() => handleMarkHabitDone(habit.id)}
        />
      </View>
    );
  };

  const renderLeftActions = (id: number) => (
    <View style={styles.swipeActionLeft}>
      <MaterialCommunityIcons
        name="trash-can-outline"
        size={32}
        color={"#fff"}
        onPress={() => {
          Alert.alert(
            "Delete Habit",
            "Are you sure you want to delete this habit?",
            [
              { text: "Cancel", style: "cancel" },
              {
                text: "Delete",
                style: "destructive",
                onPress: () => handleDeleteHabit(id),
              },
            ]
          );
        }}
      />
    </View>
  );

  useEffect(() => {
    handleLoadHabits();
  }, [user]);

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text variant="headlineSmall" style={styles.title}>
          Today&apos;s Habits
        </Text>
        <Button mode="text" onPress={logout} icon="logout">
          Log Out
        </Button>
      </View>

      {isLoading && (
        <ActivityIndicator
          size="large"
          color={theme.colors.primary}
          style={styles.loading}
        />
      )}

      {error && (
        <View style={styles.errorContainer}>
          <Text
            style={[
              styles.errorText,
              {
                color: theme.colors.error,
                backgroundColor: theme.colors.errorContainer,
              },
            ]}
          >
            {error}
          </Text>
          <Button
            mode="outlined"
            onPress={handleLoadHabits}
            style={styles.retryButton}
          >
            Retry
          </Button>
        </View>
      )}

      <ScrollView showsVerticalScrollIndicator={false}>
        {sortedHabits.length === 0 && !isLoading && !error ? (
          <View style={styles.emptyState}>
            <Text style={styles.emptyStateText}>
              No Habits yet. Add your first Habit!
            </Text>
          </View>
        ) : (
          sortedHabits.map((habit, key) => (
            <Swipeable
              ref={(ref) => {
                if (habit.id) {
                  swipeableRefs.current[habit.id] = ref;
                }
              }}
              key={habit.id || key}
              overshootLeft={false}
              overshootRight={false}
              renderLeftActions={() => renderLeftActions(habit.id)}
              renderRightActions={() => renderRightActions(habit)}
              onSwipeableOpen={(direction) => {
                if (direction === "left" && habit.id) {
                  Alert.alert(
                    "Delete Habit",
                    "Are you sure you want to delete this habit?",
                    [
                      { text: "Cancel", style: "cancel" },
                      {
                        text: "Delete",
                        style: "destructive",
                        onPress: () => handleDeleteHabit(habit.id),
                      },
                    ]
                  );
                } else if (direction === "right" && habit.id) {
                  if (isHabitCompleted(habit)) {
                    setError(
                      `This habit has already been completed for the current ${
                        habit.frequency === HabitFrequency.Daily
                          ? "day"
                          : habit.frequency === HabitFrequency.Weekly
                          ? "week"
                          : "month"
                      }.`
                    );
                  } else {
                    handleMarkHabitDone(habit.id);
                  }
                }
                if (habit.id) {
                  swipeableRefs.current[habit.id]?.close();
                }
              }}
              enabled={!isHabitCompleted(habit)}
            >
              <TouchableOpacity
                activeOpacity={0.9}
                onPress={() =>
                  router.push({
                    pathname: "/home/[habitId]",
                    params: { id: habit.id.toString() },
                  })
                }
              >
                <Surface
                  style={[
                    styles.card,
                    isHabitCompleted(habit) && styles.completedHabit,
                    {
                      backgroundColor:
                        CATEGORY_COLORS[habit.category] ||
                        CATEGORY_COLORS[HabitCategory.General],
                    },
                  ]}
                  elevation={0}
                >
                  <ImageBackground
                    source={{
                      uri: habit.petName
                        ? `https://img.pokemondb.net/artwork/large/${habit.petName.toLowerCase()}.jpg`
                        : "https://img.pokemondb.net/artwork/large/pikachu.jpg",
                    }}
                    imageStyle={styles.cardBackgroundImage}
                    style={[
                      styles.card,
                      isHabitCompleted(habit) && styles.completedHabit,
                    ]}
                  >
                    <View style={styles.cardContent}>
                      <View style={styles.cardTextContainer}>
                        <View style={styles.cardHeader}>
                          <Text style={styles.cardTitle}>{habit.title}</Text>
                          {isHabitCompleted(habit) && (
                            <MaterialCommunityIcons
                              name="check-circle"
                              size={20}
                              color={theme.colors.primary}
                            />
                          )}
                        </View>
                        <Text style={styles.cardDescription}>
                          {habit.description}
                        </Text>
                        <View style={styles.cardFooter}>
                          <View style={styles.streakBadge}>
                            <MaterialCommunityIcons
                              name="fire"
                              size={18}
                              color={"#ff9800"}
                            />
                            <Text style={styles.streakText}>
                              {habit.currentStreak || 0} day streak
                            </Text>
                          </View>
                          <View style={styles.frequencyBadge}>
                            <Text style={styles.frequencyText}>
                              {habit.frequency}
                            </Text>
                          </View>
                        </View>
                      </View>
                      {habit.petName && (
                        <Image
                          source={{
                            uri: habit.petName
                              ? `https://img.pokemondb.net/sprites/heartgold-soulsilver/back-normal/${habit.petName.toLowerCase()}.png`
                              : "https://img.pokemondb.net/sprites/heartgold-soulsilver/back-normal/pikachu.png",
                          }}
                          style={styles.pokemonImage}
                          resizeMode="contain"
                        />
                      )}
                    </View>
                  </ImageBackground>
                </Surface>
              </TouchableOpacity>
            </Swipeable>
          ))
        )}
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
    backgroundColor: "#f5f5f5",
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 24,
  },
  title: {
    fontWeight: "bold",
  },
  card: {
    marginBottom: 18,
    borderRadius: 18,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.08,
    shadowRadius: 8,
    elevation: 4,
  },
  cardBackgroundImage: {
    borderRadius: 18,
    resizeMode: "cover",
    opacity: 0.4,
  },
  cardContent: {
    flexDirection: "row",
    padding: 20,
  },
  cardTextContainer: {
    flex: 1,
  },
  cardHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 4,
  },
  cardTitle: {
    fontSize: 20,
    fontWeight: "bold",
    color: "#22223b",
  },
  cardDescription: {
    fontSize: 15,
    marginBottom: 16,
    color: "#6c6c80",
  },
  cardFooter: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  streakBadge: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: "rgba(255, 255, 255, 0.8)",
    borderRadius: 12,
    paddingHorizontal: 10,
    paddingVertical: 4,
  },
  streakText: {
    marginLeft: 6,
    color: "#ff9800",
    fontWeight: "bold",
    fontSize: 14,
  },
  frequencyBadge: {
    backgroundColor: "rgba(255, 255, 255, 0.8)",
    borderRadius: 12,
    paddingHorizontal: 12,
    paddingVertical: 4,
  },
  frequencyText: {
    color: "#7c4dff",
    fontWeight: "bold",
    fontSize: 14,
  },
  petName: {
    fontSize: 14,
    color: "#6200ee",
    fontWeight: "500",
    marginTop: 8,
  },
  pokemonImage: {
    width: 80,
    height: 80,
    marginLeft: 16,
    alignSelf: "center",
  },
  emptyState: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  emptyStateText: {
    color: "#666666",
  },
  errorContainer: {
    alignItems: "center",
    marginBottom: 16,
  },
  errorText: {
    fontSize: 14,
    padding: 12,
    borderRadius: 8,
    marginBottom: 8,
    textAlign: "center",
    fontWeight: "500",
  },
  retryButton: {
    marginTop: 8,
  },
  loading: {
    marginVertical: 16,
  },
  swipeActionLeft: {
    justifyContent: "center",
    alignItems: "flex-start",
    flex: 1,
    backgroundColor: "#e53935",
    borderRadius: 18,
    marginBottom: 18,
    marginTop: 2,
    paddingLeft: 16,
  },
  swipeActionRight: {
    justifyContent: "center",
    alignItems: "flex-end",
    flex: 1,
    backgroundColor: "#4caf50",
    borderRadius: 18,
    marginBottom: 18,
    marginTop: 2,
    paddingRight: 16,
  },
  completedHabit: {
    opacity: 0.6,
  },
});
