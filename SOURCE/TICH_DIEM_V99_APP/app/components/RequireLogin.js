import React, { Component } from "react";
import { Text, View, Image, TouchableOpacity } from "react-native";
import FastImage from "./FstImage";
import * as Theme from "../constants/Theme";
import I18n from "../i18n/i18n";
import NavigationUtil from '../navigation/NavigationUtil'
import { SCREEN_ROUTER } from "../constants/Constant";
export default class RequireLogin extends Component {
  render() {
    return (
      <View
        style={{
          flex: 1,
          justifyContent: "center",
          alignItems: "center",
          backgroundColor: Theme.colors.white
        }}
      >
        <FastImage
          style={{
            height: Theme.dimension.width * 0.45,
            width: Theme.dimension.width * 0.45,
            backgroundColor: Theme.colors.white
          }}
          source={require("../assets/images/ic_lock_login.png")}
        />
        <Text style={[Theme.fonts.robotoMedium14, { marginTop: 20, color: Theme.colors.red, marginHorizontal: 20 }]}>
          {I18n.t("not_login")}
        </Text>
        <TouchableOpacity
          style={{
            width: Theme.dimension.width * 0.85,
            height: 50,
            backgroundColor: Theme.colors.primary,
            alignItems: "center",
            justifyContent: "center",
            flexDirection: "row",
            marginTop: 20,
            borderRadius: 5,
            marginBottom: 20
          }}
          onPress={() => {
            NavigationUtil.navigate(SCREEN_ROUTER.LOGIN)
          }}
        >
          <Text style={{ color: Theme.colors.primary_background }}>
            {I18n.t("login_logout")}
          </Text>
        </TouchableOpacity>
      </View>
    );
  }
}
