import React, { Component } from "react";
import { View } from "react-native";
import { BarIndicator, BarIndicatorProps } from "react-native-indicators";
import { colors } from "@app/constants/Theme";

type Props = {} & BarIndicatorProps;

export default class Loading extends Component<Props> {
  render() {
    const { color, ...otherProps } = this.props;
    return (
      <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
        <BarIndicator color={color || colors.primary} {...otherProps} />
      </View>
    );
  }
}
