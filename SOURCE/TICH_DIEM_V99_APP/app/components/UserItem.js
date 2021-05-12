import React, { Component } from "react";
import { Text, View, Image, TouchableOpacity } from "react-native";
import theme, * as Theme from "../constants/Theme";
import FstImage from "./FstImage";
export default class UserItem extends Component {
  render() {
    const { text, img, onPress, tintColor } = this.props;
    return (
      <TouchableOpacity
        style={{
          paddingHorizontal: 20,
          justifyContent: "center"
        }}
        onPress={onPress}
      >
        <View
          style={{
            flexDirection: "row",
            justifyContent: "space-between",
            marginTop: 20,
            alignItems: "center",
            borderBottomColor: theme.colors.border,
            borderBottomWidth: 0.5,
            paddingBottom: 10
          }}
        >
          <View style={{ flexDirection: "row" }}>
            <FstImage
              style={{
                width: 20, height: 20,
              }}
              resizeMode='contain'
              tintColor={tintColor}
              source={img}
            />
            <Text style={[Theme.fonts.regular16, { marginLeft: 15 }]}>
              {text}
            </Text>
          </View>
          <FstImage
            style={{ height: 18, width: 8 }}
            source={require("../assets/images/ic_right.png")}
            resizeMode='contain'
          />
        </View>
        {/* <View style={{ height: 0.5, backgroundColor: Theme.colors.border }} /> */}
      </TouchableOpacity>
    );
  }
}
