import I18n from "../i18n/i18n";
import React, { Component } from "react";
import { Alert } from "react-native";

export const showConfirm = (title, content, action, confirm) => {
  Alert.alert(
    title,
    content,
    [
      {
        text: I18n.t("cancel"),
        style: "cancel"
      },
      {
        text: confirm || title,
        onPress: action
      }
    ],
    { cancelable: false }
  );
};

export const showMessages = (title, content, action) => {
  setTimeout(() => {
    Alert.alert(
      title,
      content,
      [
        {
          text: "Đóng",
          onPress: action
        }
      ],
      { cancelable: false }
    );
  }, 100);
};
