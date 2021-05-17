/* eslint-disable no-undef */
/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React, { Component } from "react";
import AppNavigator from "./app/navigation/AppNavigator";
import NavigationUtil from "./app/navigation/NavigationUtil";
import { Provider } from "react-redux";
import store from "./app/redux/store";
import OneSignal from "react-native-onesignal"; // Import package from node modules
import codePush from "react-native-code-push";
import AsyncStorage from "@react-native-community/async-storage";
import reactotron from "reactotron-react-native";
import AppContainer from "./app/AppContainer";
import "react-native-gesture-handler";

class App extends Component {
  render() {
    return (
      <Provider store={store}>
        <AppContainer />
      </Provider>
    );
  }

  codePushStatusDidChange(status) {
    reactotron.log("Codepush status : ", status);
  }

  codePushDownloadDidProgress(progress) {
    reactotron.log(
      progress.receivedBytes + " of " + progress.totalBytes + " received."
    );
  }
}

let codePushOptions = {
  checkFrequency: codePush.CheckFrequency.MANUAL
};

MyApp = codePush(codePushOptions)(App);

export default MyApp;
