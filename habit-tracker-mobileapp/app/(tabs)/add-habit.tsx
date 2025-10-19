import { HabitCategory, HabitFrequency } from "@/utils/enums/habitEnum";
import { StyleSheet, Text, View } from "react-native";
import { Button, SegmentedButtons, TextInput } from "react-native-paper";

import { useAuth } from "@/hooks/useAuth";
import { createHabit } from "@/services/habit.service";
import { Habit } from "@/types/habit";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Dropdown } from "react-native-element-dropdown";

const FREQUENCIES = Object.values(HabitFrequency);
const CATEGORIES = Object.values(HabitCategory).map((cat) => ({
  label: cat,
  value: cat,
}));

export default function AddHabitScreen() {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [frequency, setFrequency] = useState<HabitFrequency>(
    

    HabitFrequency.Daily
  );
  const {user} = useAuth();
  const router = useRouter();
  const [category, setCategory] = useState<HabitCategory>(
    HabitCategory.General
  );
  const [isFocus, setIsFocus] = useState(false);

  const handleSubmit = async () => {

    if(!user) return;

    const habitData: Habit = {
      title,
      description,
      frequency,
      category,
    };

    try {
      await createHabit(habitData);
      setTitle("");
      setDescription("");
      setFrequency(HabitFrequency.Daily);
      setCategory(HabitCategory.General);

      router.back();
    } catch (error) {
      if(error instanceof Error) {}
    }
  };

  return (
    <View style={styles.container}>
      <TextInput
        label="Title"
        mode="outlined"
        onChangeText={setTitle}
        value={title}
        style={styles.input}
      />
      <TextInput
        label="Description"
        mode="outlined"
        onChangeText={setDescription}
        value={description}
        style={styles.input}
      />

      <View style={styles.frequencyContainer}>
        <SegmentedButtons
          value={frequency}
          onValueChange={(value) => setFrequency(value as HabitFrequency)}
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
          }}
    
        />
      </View>

      <Button
        mode="contained"
        onPress={handleSubmit}
        disabled={!title || !description || !category}
        style={styles.submitBtn}
      >
        Add Habit
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
});
