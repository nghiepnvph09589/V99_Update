import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
  ViewStyle,
  Platform,
  StatusBar
} from "react-native";
import { Header, HeaderProps } from "react-native-elements";
import NavigationUtil from "../navigation/NavigationUtil";
import * as theme from "../constants/Theme";
import R from "@app/assets/R";
import FastImage from "react-native-fast-image";

interface BackProps {
  style?: ViewStyle;
}

export type RNHeaderProps = {
  color?: string;
  back?: boolean;
  titleHeader: string;
} & HeaderProps;

export class BackButton extends Component<BackProps> {
  render() {
    const { style } = this.props;
    return (
      <TouchableOpacity
        style={[style || styles.leftComp]}
        onPress={NavigationUtil.goBack}
      >
        <FastImage
          source={R.images.ic_back}
          style={{ width: 30, height: 30 }}
          tintColor={theme.colors.white}
          resizeMode="contain"
        />
      </TouchableOpacity>
    );
  }
}

export default class RNHeader extends Component<RNHeaderProps> {
  render() {
    const {
      color,
      back,
      titleHeader,
      leftComponent,
      ...otherProps
    } = this.props;
    return (
      <Header
        {...otherProps}
        placement="center"
        containerStyle={{
          backgroundColor: theme.colors.headerColor,
          borderBottomColor: theme.colors.headerColor
          // marginTop: Platform.OS == "android" && -StatusBar.currentHeight
        }}
        leftComponent={back ? <BackButton /> : leftComponent}
        centerComponent={
          <Text
            style={[
              {
                fontSize: 18,
                fontFamily: R.fonts.roboto_black
              },
              { color: color || "white" }
            ]}
          >
            {titleHeader}
          </Text>
        }
        statusBarProps={{
          barStyle: "light-content",
          translucent: true,
          backgroundColor: "transparent"
        }}
      />
    );
  }
}

const styles = StyleSheet.create({
  leftComp: {
    height: "100%",
    flexDirection: "row",
    alignItems: "center"
  },
  rightComp: {
    height: "100%",
    justifyContent: "center",
    alignItems: "center",
    marginRight: 10
  }
});
