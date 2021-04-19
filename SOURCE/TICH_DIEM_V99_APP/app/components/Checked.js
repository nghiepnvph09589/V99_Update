import React, { Component } from "react";
import { Text, View, TouchableOpacity } from "react-native";
import * as Theme from "../constants/Theme";
export default class Checked extends Component {
  render() {
    const { status, text, onPress,left } = this.props;
    return (
      <TouchableOpacity
        style={{
          flexDirection: "row",
          alignItems: "center",
          justifyContent: "center",
          marginLeft:left
        }}
        onPress={onPress}
      >
        <View
          style={{
            height: 18,
            width: 18,
            borderRadius: 18,
            borderWidth: 1,
            borderColor: Theme.colors.black1,
            alignItems: "center",
            justifyContent: "center"
          }}
        >
          <View
            style={{
              height: 13,
              width: 13,
              borderRadius: 13,
              backgroundColor:
                status == 0 ? Theme.colors.red : Theme.colors.white
            }}
           
          />
        </View>
        <Text
          style={[
            Theme.fonts.robotoRegular16,
            {
              marginLeft: 10,
              color: Theme.colors.black
            }
          ]}
        >
          {text}
        </Text>
      </TouchableOpacity>
    );
  }
}
