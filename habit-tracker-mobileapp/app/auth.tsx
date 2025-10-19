import { KeyboardAvoidingView, Platform, StyleSheet, View } from "react-native";
import { Button, Text, TextInput, useTheme } from "react-native-paper";

import { useAuth } from "@/hooks/useAuth";
import { useRouter } from "expo-router";
import { useState } from "react";

export default function AuthScreen() {
  const [isRegister, setIsRegister] = useState<boolean>(false);
  const [username, setUsername] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string | null>("");

  const theme = useTheme();

  const router = useRouter();

  const { login, register } = useAuth();

  const handleAuth = async () => {
    if (isRegister) {
      if (!username || !email || !password) {
        setError("Please fill in all fields for registration");
        return;
      }
    } else {
      if (!username || !password) {
        setError("Please fill in username and password for login");
        return;
      }
    }

    if (password.length < 6) {
        setError("Password must be at least 6 characters long");
        return;
    }

    setError(null);

    if (isRegister) {
      const error = await register(username, email, password);
      if (error) {
        setError(error);
        return;
      }
    } else {
      const error = await login(username, password);
      if (error) {
        setError(error);
        return;
      }

      router.replace("/");
    }
  };

  const handleSwitchMode = () => {
    setIsRegister((prev) => !prev);
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={styles.container}
    >
      <View style={styles.content}>
        <Text style={styles.title} variant="headlineMedium">
          {" "}
          {isRegister ? "Create Account" : "Welcome Back"}{" "}
        </Text>

        <TextInput
          label="Username"
          autoCapitalize="none"
          keyboardType="default"
          style={styles.input}
          onChangeText={setUsername}
        />

        <TextInput
          label="Email"
          autoCapitalize="none"
          keyboardType="email-address"
          placeholder="example@gmail.com"
          style={[styles.input, !isRegister && { display: "none" }]}
          disabled={!isRegister}
          onChangeText={setEmail}
        />

        <TextInput
          label="Password"
          autoCapitalize="none"
          keyboardType="default"
          mode="outlined"
          secureTextEntry
          style={styles.input}
          onChangeText={setPassword}
        />

        {error && <Text style={{ color: theme.colors.error }}> {error}</Text>}

        <Button mode="contained" style={styles.button} onPress={handleAuth}>
          {isRegister ? "Sign Up" : "Sign In"}{" "}
        </Button>

        <Button
          mode="text"
          onPress={handleSwitchMode}
          style={styles.switchModeButton}
        >
          {isRegister
            ? "Already have an account? Sign In"
            : "Don't have an account? Sign Up"}
        </Button>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },

  content: {
    flex: 1,
    padding: 16,
    justifyContent: "center",
  },

  title: {
    textAlign: "center",
    marginBottom: 24,
  },

  input: {
    marginBottom: 16,
  },

  button: {
    marginTop: 8,
  },

  switchModeButton: {
    marginTop: 16,
  },
});
