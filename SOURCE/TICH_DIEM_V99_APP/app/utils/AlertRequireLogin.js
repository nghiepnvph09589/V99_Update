import I18n from "../i18n/i18n";
import React, { Component } from "react";
import { Alert } from "react-native";
import NavigationUtil from '../navigation/NavigationUtil'
import { SCREEN_ROUTER } from '../constants/Constant'

export const requireLogin = () => {
    Alert.alert(
        'Yêu cầu đăng nhập',
        'Vui lòng đăng nhập để thực hiện chức năng này',
        [
            {
                text: I18n.t("cancel"),
                style: "cancel"
            },
            {
                text: 'Đăng nhập',
                onPress: () => NavigationUtil.navigate(SCREEN_ROUTER.LOGIN)
            }
        ],
        { cancelable: false }
    )
}