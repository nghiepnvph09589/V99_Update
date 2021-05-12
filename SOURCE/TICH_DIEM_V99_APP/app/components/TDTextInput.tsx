import React, { Component } from "react";
import {
  Text,
  View,
  Dimensions,
  TextInput,
  StyleSheet,
  StyleProp,
  ViewStyle,
  ViewProps,
  KeyboardTypeOptions,
  TextInputProps,
  NativeSyntheticEvent,
  TextInputFocusEventData,
  Platform,
  Image
} from "react-native";
import { TouchableOpacity } from "react-native-gesture-handler";
import theme, { colors, fonts } from "@app/constants/Theme";
import { SCREEN_ROUTER } from "@constant";
import NavigationUtil from "@app/navigation/NavigationUtil";
import R from "@app/assets/R";
import { TextStyle } from "react-native";
import reactotron from "@app/debug/ReactotronConfig";
import FstImage from "./FstImage";
import { FastImageProps, Source } from "react-native-fast-image";

type Props = TextInputProps & {
  fieldName: string;
  isRequire?: boolean;
  containerStyle?: StyleProp<ViewStyle>;
  requireMessage?: string;
  borderRadiusTop?: boolean;
  borderRadiusBottom?: boolean;
  icon: Source | number;
};

interface TAState {
  value: string;
  error: boolean;
  isFocus: boolean;
  errorMessage: string;
}

export function updateFieldValue(state, fieldName, text) {
  let isError = false;

  if (!text || text == "") isError = true;

  return {
    [fieldName]: text,
    errors: { ...state.errors, [fieldName]: isError }
  };
}

export default class TATextInput extends Component<Props, TAState, ViewProps> {
  state: TAState = {
    value: this.props.value,
    error: false,
    isFocus: false,
    errorMessage: ""
  };

  _onBlur = () => {
    const { value } = this.props;
    if (!value || value == "") {
      this.setState({ error: true });
    } else this.setState({ error: false });
  };

  componentWillReceiveProps(newProps) {
    if (newProps.onBlur && newProps.value)
      this.setState({ ...this.state, error: false, value: newProps.value });
    if (
      newProps.onBlur &&
      (!newProps.value || newProps.value == "") &&
      newProps.onBlur &&
      this.state.isFocus
    )
      this.setState({ ...this.state, error: true, value: newProps.value });
  }

  render() {
    const {
      fieldName,
      value,
      isRequire,
      containerStyle,
      requireMessage,
      borderRadiusBottom,
      borderRadiusTop,
      icon,
      ...otherProps
    } = this.props;
    return (
      <View
        style={[
          {
            flexDirection: "row",
            borderWidth: 0.5,
            borderColor: theme.colors.primary,
            alignItems: "center",
            width: "90%",
            alignSelf: "center",
            paddingVertical: (Platform.OS == "ios" && 15) || 0,
            paddingHorizontal: "2.5%",
            marginBottom: 5
          },
          borderRadiusTop && {
            borderTopEndRadius: 10,
            borderTopStartRadius: 10
          },
          borderRadiusBottom && {
            borderBottomEndRadius: 10,
            borderBottomStartRadius: 10
          },
          containerStyle
        ]}
      >
        {!!icon && (
          <FstImage
            source={icon}
            style={{ width: 20, height: 20 }}
            tintColor={theme.colors.primary}
            resizeMode="contain"
          />
        )}
        <TextInput
          value={value ? value.toString() : ""}
          placeholder={fieldName.toString()}
          style={[
            {
              flex: 1,
              paddingLeft: 10,
              color: "#111111"
            },
            fonts.semibold18
          ]}
          onFocus={() => {
            this.setState({ isFocus: true });
          }}
          onBlur={this._onBlur}
          onChange={e => reactotron.log(e)}
          {...otherProps}
        />
      </View>
    );
  }
}
