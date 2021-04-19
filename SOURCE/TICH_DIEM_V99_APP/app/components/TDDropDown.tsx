import React, { Component } from "react";
import {
  Text,
  View,
  LayoutChangeEvent,
  StyleProp,
  TextStyle,
  TouchableWithoutFeedbackProps,
  ViewStyle,
  StyleSheet
} from "react-native";
import { Dropdown, DropDownProps } from "react-native-material-dropdown";
import theme from "@theme";

type Props = DropDownProps & {
  style?: StyleProp<ViewStyle>;
  isBorder?: boolean;
  isRequire?: boolean;
};

export class TADropDown extends Component<Props> {
  render() {
    const {
      label,
      itemColor,
      style,
      isBorder,
      isRequire,
      containerStyle,
      inputContainerStyle,
      ...otherProps
    } = this.props;

    return (
      <View
        style={[
          styles.item,
          {
            borderWidth: isBorder ? 0.5 : 0,
            paddingHorizontal: isBorder ? 10 : 0,
            borderRadius: isBorder ? 5 : 0,
            borderColor: theme.colors.border
          },
          style
        ]}
      >
        {!isBorder && (
          <Text
            style={{
              ...theme.fonts.regular15,
              color: theme.colors.black_title
            }}
          >
            {label}{" "}
            {isRequire && (
              <Text
                style={{
                  color: theme.colors.red
                }}
              >
                (*)
              </Text>
            )}
          </Text>
        )}

        <Dropdown
          label={isBorder ? label : ""}
          // itemCount={0}
          textColor={theme.colors.black_title}
          containerStyle={[
            {
              flex: 1,
              justifyContent: "center"
              // borderBottomColor: "red"
            },
            containerStyle
          ]}
          pickerStyle={{
            borderWidth: 0.5,
            borderColor: theme.colors.border,
            borderRadius: 5
          }}
          itemTextStyle={{
            ...theme.fonts.regular15
          }}
          inputContainerStyle={inputContainerStyle}
          // renderBase={(props) => {
          //     <View style={{
          //         flex: 1,
          //         justifyContent: "center", alignItems: 'center',
          //         backgroundColor: 'white', borderRadius: 10
          //     }}>
          //         <Text>sdjflksdjfkdslf</Text>x
          //     </View>
          // }}
          {...otherProps}
        />
      </View>
    );
  }
}

const styles = StyleSheet.create({
  item: {
    height: 55,
    width: "100%",
    justifyContent: "center",
    marginBottom: 10,
    alignSelf: "center",
    backgroundColor: theme.colors.white
  }
});

export default TADropDown;
