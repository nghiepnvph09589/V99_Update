import R from "@app/assets/R";
import React, { Component } from "react";
import { Text, View } from "react-native";
import { TextField } from "react-native-materialui-textfield";
import theme, * as Theme from "../constants/Theme";
import FstImage from "./FstImage";
export default class TextFieldInput extends Component {
  render() {
    const {
      keyboardType,
      label,
      onChangeText,
      maxLength,
      secureTextEntry,
      top,
      obligatory,
      value,
      refs,
      submit,
      autoCapitalize,
      icon, editable
    } = this.props;
    return (
      <View style={{
        marginTop: top,
        flexDirection: 'row',
        alignItems: 'center',
        marginHorizontal: '5%',
      }}>
        {!!icon && <FstImage source={icon || R.images.ic_telephone}
          style={{ width: 20, height: 20 }}
          resizeMode='contain' tintColor={theme.colors.primary} />}
        <View style={{ flex: 1, paddingHorizontal: !!icon && '3%' || 0 }}>
          <TextField
            label={
              <Text>
                {label}
                {obligatory && <Text style={{ color: "red" }}> *</Text>}
              </Text>
            }
            keyboardType={keyboardType}
            value={value}
            secureTextEntry={secureTextEntry}
            labelHeight={16}
            returnKeyType="done"
            ref={refs}
            onSubmitEditing={submit}
            containerStyle={{
            }}
            fontSize={16}
            labelFontSize={12}
            labelTextStyle={{
              color: Theme.colors.black,
              ...theme.fonts.regular18
            }}
            activeLineWidth={1}
            disabledLineWidth={1}
            lineWidth={1}
            tintColor={Theme.colors.black1}
            maxLength={maxLength}
            onChangeText={onChangeText}
            labelPadding={5}
            autoCapitalize={autoCapitalize}
            editable={editable}
          />
        </View>
      </View>
    );
  }
}
