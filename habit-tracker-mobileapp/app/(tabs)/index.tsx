import { StyleSheet, Text, View } from "react-native";

import { useAuth } from "@/hooks/useAuth";
import { Button } from "react-native-paper";

export default function Index() {
  const {logout} = useAuth()
  return (
    <View
      style={styles.view}
    >
      <Text>ABC</Text>
      <Button mode="text" onPress={logout} icon="logout">Log Out</Button>
    </View>
  );
}


const styles = StyleSheet.create({
  view: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center"
  },
});
