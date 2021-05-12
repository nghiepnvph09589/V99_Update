import React, { Component } from "react";
import {
  Text,
  View,
  Dimensions,
  StyleSheet,
  StyleProp,
  ViewStyle,
  ViewProps,
  TouchableOpacity,
  TouchableOpacityProps,
  ActivityIndicator
} from "react-native";
import theme from "@app/constants/Theme";
import { SCREEN_ROUTER } from "@constant";
import NavigationUtil from "@app/navigation/NavigationUtil";
import R from "@app/assets/R";
import { TextStyle } from "react-native";
import FastImage from "./FstImage";
import Block from "./Block";

const { width, height } = theme.dimension;

type Props = TouchableOpacityProps & {
  title?: string;
  titleColor?: string;
  titleStyle?: object;
  tintColour?: string;
  background?: string;
  icon?: JSX.Element;
  image?: any;
  rightIcon?: JSX.Element;
  righImage?: any;
  isRequesting?: boolean;
};
export default class CustomButton extends Component<Props, ViewProps> {
  constructor(props) {
    super(props);
  }

  render() {
    const {
      style,
      titleStyle,
      title,
      background,
      icon,
      image,
      titleColor,
      righImage,
      tintColour,
      rightIcon,
      isRequesting,
      disabled,
      ...otherProps
    } = this.props;
    return (
      <TouchableOpacity
        style={[
          styles.button,
          {
            backgroundColor: background ? background : theme.colors.button,
            opacity: isRequesting ? 0.75 : 1
          },
          style
        ]}
        disabled={disabled ? disabled : isRequesting}
        {...otherProps}
      >
        <Block />
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          {!!icon && icon}
          {!!image && (
            <FastImage
              source={image}
              style={{
                width: 20,
                height: 20,
              }}
              tintColor={tintColour}
            />
          )}
          <Text
            style={[
              {
                color: titleColor ? titleColor : "white",
                marginLeft: image || icon ? 10 : 0,
                marginRight: righImage || rightIcon ? 10 : 0,
                ...theme.fonts.semibold18
              },
              titleStyle
            ]}
          >
            {title}
          </Text>
          {!!rightIcon && rightIcon}
          {!!righImage && (
            <FastImage
              source={righImage}
              style={{
                width: 20,
                height: 20
              }}
            />
          )}
        </View>
        <Block>
          {isRequesting && (
            <ActivityIndicator
              color="white"
              style={
                {
                  // position: 'absolute',
                  // right: 12,
                }
              }
            />
          )}
        </Block>
      </TouchableOpacity>
    );
  }
}

const styles = StyleSheet.create({
  button: {
    flexDirection: "row",
    width: "100%",
    height: 50,
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 5,
    alignSelf: "center"
  }
});
