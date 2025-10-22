import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";
import { useEffect, useRef, useState } from "react";
import { Animated, Image, Modal, StyleSheet, Text, View } from "react-native";
import {
  ActivityIndicator,
  Button,
  SegmentedButtons,
  TextInput,
  useTheme,
} from "react-native-paper";

import { useHabits } from "@/contexts/habit-context";
import { useAuth } from "@/hooks/useAuth";
import { createHabit } from "@/services/habit.service";
import { HabitCreate } from "@/types/habit";
import { useRouter } from "expo-router";
import { Dropdown } from "react-native-element-dropdown";

const FREQUENCIES = Object.values(HabitFrequency);
const CATEGORIES = Object.values(HabitCategory).map((cat) => ({
  label: cat,
  value: cat,
}));

// Danh sách Pokémon cấp 1 tĩnh để hiển thị ngẫu nhiên khi loading
const BASE_POKEMON = [
  "bulbasaur", "chikorita", "treecko", "turtwig", "snivy", // Grass
  "mankey", "machop", "tyrogue", "riolu", "timburr", // Fighting
  "abra", "drowzee", "spoink", "munna", "espurr", // Psychic
  "magnemite", "aron", "beldum", "bronzor", "klink", // Steel
  "rattata", "meowth", "snorlax", "eevee", "tauros", // Normal
  "geodude", "onix", "cranidos", "roggenrola", "dwebble", // Rock
  "clefairy", "jigglypuff", "togepi", "snubbull", "flabebe", // Fairy
  "pidgey", "spearow", "farfetchd", "doduo", "hoothoot", // Flying
  "gastly", "misdreavus", "shuppet", "drifloon", "litwick", // Ghost
  "caterpie", "weedle", "paras", "venonat", "scyther" // Bug
];

export default function AddHabitScreen() {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [frequency, setFrequency] = useState<HabitFrequency>(HabitFrequency.Daily);
  const [category, setCategory] = useState<HabitCategory>(HabitCategory.General);
  const [isFocus, setIsFocus] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [displayPokemon, setDisplayPokemon] = useState<string | null>(null);
  const [finalPokemon, setFinalPokemon] = useState<string | null>(null);
  const [loadingStartTime, setLoadingStartTime] = useState<number | null>(null);

  const { user } = useAuth();
  const { addHabit } = useHabits();
  const router = useRouter();
  const theme = useTheme();

  // Animated values cho hiệu ứng
  const opacity = useRef(new Animated.Value(1)).current;
  const translateX = useRef(new Animated.Value(100)).current; // Bắt đầu từ bên phải

  // Hiệu ứng lóe sáng
  const flashAnimation = () => {
    Animated.sequence([
      Animated.timing(opacity, {
        toValue: 0.4,
        duration: 300,
        useNativeDriver: true,
      }),
      Animated.timing(opacity, {
        toValue: 1,
        duration: 300,
        useNativeDriver: true,
      }),
    ]).start();
  };

  // Hiệu ứng trượt
  const slideAnimation = (isFinal: boolean = false) => {
    translateX.setValue(100); // Bắt đầu từ bên phải
    Animated.timing(translateX, {
      toValue: 0, // Trượt đến giữa
      duration: isFinal ? 800 : 200, // Nhanh hơn: 300ms cho ngẫu nhiên, 500ms cho cuối
      useNativeDriver: true,
    }).start();
  };

  // Quản lý Pokémon và hiệu ứng
  useEffect(() => {
    let interval: number | null = null;
    if (isLoading && !finalPokemon) {
      const randomIndex = Math.floor(Math.random() * BASE_POKEMON.length);
      setDisplayPokemon(BASE_POKEMON[randomIndex]);
      flashAnimation();
      slideAnimation();
      // Thay đổi Pokémon mỗi 1 giây
      interval = setInterval(() => {
        const newIndex = Math.floor(Math.random() * BASE_POKEMON.length);
        setDisplayPokemon(BASE_POKEMON[newIndex]);
        flashAnimation();
        slideAnimation();
      }, 1000);
    } else if (isLoading && finalPokemon) {
      setDisplayPokemon(finalPokemon);
      flashAnimation();
      slideAnimation(true); // Hiệu ứng nổi bật cho Pokémon cuối
    } else {
      setDisplayPokemon(null);
      setFinalPokemon(null);
      setLoadingStartTime(null);
      opacity.setValue(1);
      translateX.setValue(100);
    }
    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isLoading, finalPokemon]);

  // Xóa lỗi khi người dùng nhập lại
  const clearError = () => {
    if (error) {
      setError(null);
    }
  };

  // Kiểm tra dữ liệu đầu vào
  const validateInput = () => {
    if (!user) {
      return "You must be logged in to create a habit";
    }
    if (!title || title.length < 3) {
      return "Title must be at least 3 characters long";
    }
    if (!description || description.length < 5) {
      return "Description must be at least 5 characters long";
    }
    if (!category) {
      return "Please select a category";
    }
    return null;
  };

  const handleSubmit = async () => {
    const validationError = validateInput();
    if (validationError) {
      setError(validationError);
      return;
    }

    const habitData: HabitCreate = {
      title,
      description,
      frequency,
      category,
    };

    setIsLoading(true);
    setLoadingStartTime(Date.now()); // Ghi lại thời gian bắt đầu
    try {
      const response = await createHabit(habitData);
      addHabit(response);
      setFinalPokemon(response.petName); // Lấy petName từ API

      // Đảm bảo loading kéo dài ít nhất 5 giây
      const elapsedTime = Date.now() - (loadingStartTime || Date.now());
      const remainingTime = 3000 - elapsedTime;
      if (remainingTime > 0) {
        await new Promise(resolve => setTimeout(resolve, remainingTime));
      } else {
        await new Promise(resolve => setTimeout(resolve, 1000)); // Hiển thị Pokémon cuối 1 giây
      }

      setTitle("");
      setDescription("");
      setFrequency(HabitFrequency.Daily);
      setCategory(HabitCategory.General);
      router.back();
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

  return (
    <View style={styles.container}>
      <Modal
        animationType="fade"
        transparent={true}
        visible={isLoading}
        onRequestClose={() => {}} // Không cho phép đóng modal khi đang loading
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <ActivityIndicator size="large" color={theme.colors.primary} />
            {displayPokemon && (
              <Animated.View style={{ opacity, transform: [{ translateX }] }}>
                <Image
                  source={{
                    uri: `https://img.pokemondb.net/sprites/heartgold-soulsilver/normal/${displayPokemon}.png`
                  }}
                  style={styles.pokemonImage}
                  resizeMode="contain"
                />
                <Text style={styles.pokemonName}>
                  {displayPokemon.charAt(0).toUpperCase() + displayPokemon.slice(1)}
                </Text>
              </Animated.View>
            )}
          </View>
        </View>
      </Modal>

      <TextInput
        label="Title"
        mode="outlined"
        onChangeText={(text) => {
          setTitle(text);
          clearError();
        }}
        value={title}
        style={styles.input}
        disabled={isLoading}
      />
      <TextInput
        label="Description"
        mode="outlined"
        onChangeText={(text) => {
          setDescription(text);
          clearError();
        }}
        value={description}
        style={styles.input}
        disabled={isLoading}
      />

      <View style={styles.frequencyContainer}>
        <SegmentedButtons
          value={frequency}
          onValueChange={(value) => {
            setFrequency(value as HabitFrequency);
            clearError();
          }}
          buttons={FREQUENCIES.map((freq) => ({
            value: freq,
            label: freq.charAt(0).toUpperCase() + freq.slice(1),
          }))}
        />
      </View>

      <View style={styles.dropdownContainer}>
        <Text style={styles.label}>Category</Text>
        <Dropdown
          style={[styles.dropdown, isFocus && { borderColor: "#6200ee" }]}
          placeholderStyle={styles.placeholderStyle}
          selectedTextStyle={styles.selectedTextStyle}
          data={CATEGORIES}
          labelField="label"
          valueField="value"
          value={category}
          onFocus={() => setIsFocus(true)}
          onBlur={() => setIsFocus(false)}
          onChange={(item) => {
            setCategory(item.value as HabitCategory);
            setIsFocus(false);
            clearError();
          }}
          disable={isLoading}
        />
      </View>

      {error && (
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
      )}

      <Button
        mode="contained"
        onPress={handleSubmit}
        disabled={isLoading || !title || !description || !category}
        style={styles.submitBtn}
      >
        {isLoading ? (
          <ActivityIndicator color={theme.colors.onPrimary} />
        ) : (
          "Add Habit"
        )}
      </Button>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
    backgroundColor: "#f5f5f5",
    justifyContent: "flex-start",
  },
  input: {
    marginBottom: 16,
  },
  frequencyContainer: {
    marginBottom: 24,
  },
  dropdownContainer: {
    marginBottom: 24,
  },
  label: {
    fontSize: 16,
    color: "#333",
    marginBottom: 8,
    fontWeight: "500",
  },
  dropdown: {
    height: 50,
    borderColor: "#ccc",
    borderWidth: 1,
    borderRadius: 8,
    paddingHorizontal: 12,
    backgroundColor: "#fff",
  },
  placeholderStyle: {
    fontSize: 16,
    color: "#999",
  },
  selectedTextStyle: {
    fontSize: 16,
    color: "#000",
  },
  submitBtn: {
    marginTop: 24,
  },
  errorText: {
    fontSize: 14,
    padding: 12,
    borderRadius: 8,
    marginBottom: 16,
    textAlign: "center",
    fontWeight: "500",
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: "rgba(0, 0, 0, 0.7)",
    justifyContent: "center",
    alignItems: "center",
  },
  modalContent: {
    backgroundColor: "#fff",
    borderRadius: 16,
    padding: 24,
    alignItems: "center",
    width: "80%",
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
  },
  modalText: {
    fontSize: 18,
    fontWeight: "bold",
    marginVertical: 16,
    color: "#22223b",
    textAlign: "center",
  },
  pokemonImage: {
    width: 120,
    height: 120,
    marginVertical: 8,
  },
  pokemonName: {
    fontSize: 16,
    fontWeight: "600",
    color: "#6200ee",
    textTransform: "capitalize",
  },
});