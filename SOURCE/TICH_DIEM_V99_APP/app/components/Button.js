import React, { Component } from "react";
import { Text, View, TouchableOpacity, StyleSheet } from "react-native";
import { ThemeColors } from "react-navigation";
import * as Theme from "../constants/Theme";
import { tsExpressionWithTypeArguments } from "@babel/types";
export default class Button extends Component {
  render() {
    const { onPress, text, top } = this.props;
    return (
      <TouchableOpacity
        onPress={onPress}
        style={{
          height: 50,
          marginHorizontal: 20,
          backgroundColor: Theme.colors.primary,
          justifyContent: "center",
          alignItems: "center",
          borderRadius: 5,
          marginTop: top
        }}
      >
        <Text style={[Theme.fonts.semibold18, { color: Theme.colors.white }]}>{text}</Text>
      </TouchableOpacity>
    );
  }
}
