import React, { Component } from "react";
import { Text, StyleSheet, View } from "react-native";
import { Provider } from "react-redux";
import AppNavigator from "./navigation/AppNavigator";
import OneSignal from "react-native-onesignal"; // Import package from node modules
import Reactotron from "reactotron-react-native";
import NavigationUtil from "./navigation/NavigationUtil";
import { Navigation } from "react-navigation";
import { connect } from "react-redux";
import { SCREEN_ROUTER, NOTIFY_NAVIGATE, ASYNCSTORAGE_KEY, GET_HISTORY_POINT_TYPE, ORDER } from "./constants/Constant";
import AsyncStorage from "@react-native-community/async-storage";
import {
  getHome, getNotification, getUserInfo,
  getWalletPoints, updateUserLocal,
  navigateTab, getWalletAccumulate
} from './redux/actions'

class AppContainer extends Component {
  constructor(properties) {
    super(properties);
    OneSignal.init("916a6532-ef76-4606-836c-16c1d3a7329f"); // ios
    OneSignal.inFocusDisplaying(2)
    OneSignal.addEventListener("received", this.onReceived.bind(this));
    OneSignal.addEventListener("opened", this.onOpened.bind(this));
    OneSignal.addEventListener("ids", this.onIds.bind(this));
  }

  componentWillUnmount() {
    OneSignal.removeEventListener("received", this.onReceived);
    OneSignal.removeEventListener("opened", this.onOpened);
    OneSignal.removeEventListener("ids", this.onIds);
  }

  async onReceived(notification) {
    Reactotron.log("Notification received: ", notification);

    const token = await AsyncStorage.getItem(ASYNCSTORAGE_KEY.TOKEN)
    if (!token) return

    let type = notification.payload.additionalData
      ? notification.payload.additionalData.type
      : "";
    const data = notification.payload.additionalData

    this.props.getUserInfo()

    const payloadWalletPoint = {
      page: 1,
      type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
      typePoint: ''
    }
    const payloadWalletAccumulatePoint = {
      page: 1,
      type: GET_HISTORY_POINT_TYPE.WALLET_ACCUMULATE_POINTS,
      typePoint: ''
    }

    switch (type) {
      case NOTIFY_NAVIGATE.HISTORY: {
        break;
      }
      case NOTIFY_NAVIGATE.ORDER_DETAIL: {
        this.props.getWalletPoints(payloadWalletPoint)
        this.props.getWalletAccumulate(payloadWalletAccumulatePoint)
        this.props.updateUserLocal({ point: data.Point })
        if (data.StatusOrder == ORDER.PAID) this.props.updateUserLocal({ pointRanking: data.PointRaking })
        break
      }

      case NOTIFY_NAVIGATE.NOTIFY: {
        this.props.getNotification()
        break
      }

      case NOTIFY_NAVIGATE.POINTS_HISTORY: {
        this.props.getWalletPoints(payloadWalletPoint)
        this.props.getWalletAccumulate(payloadWalletAccumulatePoint)
        break
      }

      default:
        SCREEN_ROUTER.ORDER_DETAIL && NavigationUtil.navigate(SCREEN_ROUTER.HOME)
    }
  }

  async onOpened(openResult) {
    Reactotron.log("Message: ", openResult.notification.payload.body);
    Reactotron.log("Data: ", openResult.notification.payload.additionalData);
    Reactotron.log("isActive: ", openResult.notification.isAppInFocus);
    Reactotron.log("openResult: ", openResult);

    let type = openResult.notification.payload.additionalData
      ? openResult.notification.payload.additionalData.type
      : "";
    if (!!!type) return

    if (type == NOTIFY_NAVIGATE.NEWS) {
      const newsID = openResult.notification.payload.additionalData.id
      NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_UTILITY, { newsID: newsID })
      return
    }

    const token = await AsyncStorage.getItem(ASYNCSTORAGE_KEY.TOKEN)
    if (!token) return

    switch (type) {
      case NOTIFY_NAVIGATE.HISTORY: {
        this.props.getUserInfo()
        NavigationUtil.navigate(SCREEN_ROUTER.HISTORY);
        break;
      }
      case NOTIFY_NAVIGATE.ORDER_DETAIL: {
        let orderID = openResult.notification.payload.additionalData.id
        let code = openResult.notification.payload.additionalData.code
        const payloadWalletPoint = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
          typePoint: ''
        }
        const payloadWalletAccumulatePoint = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_ACCUMULATE_POINTS,
          typePoint: ''
        }
        this.props.getWalletPoints(payloadWalletPoint)
        this.props.getWalletAccumulate(payloadWalletAccumulatePoint)
        // NavigationUtil.replace(SCREEN_ROUTER.DETAIL_ORDER, { orderID: orderID, code: code })
        NavigationUtil.reset(
          {
              index: 1,
              actions: [
                  NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_ORDER),
                  NavigationUtil.replace(SCREEN_ROUTER.DETAIL_ORDER, { orderID: orderID, code: code })
              ]
          }
      )
        break
      }
      case NOTIFY_NAVIGATE.NOTIFY: {
        this.props.getNotification()
        this.props.getUserInfo()
        NavigationUtil.navigate(SCREEN_ROUTER.USE_POINT)
        break
      }
      case NOTIFY_NAVIGATE.POINTS_HISTORY: {
        const payload = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
          typePoint: ''
        }
        this.props.getWalletPoints(payload)
        this.props.navigateTab(0)
        NavigationUtil.navigate(SCREEN_ROUTER.USE_POINT, { initialPage: 0 })
        break
      }
      case NOTIFY_NAVIGATE.DRAW_POINTS_DETAIL: {
        const payload = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
          typePoint: ''
        }
        this.props.getWalletPoints(payload)
        NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_DRAW_POINTS, { id: openResult.notification.payload.additionalData.id })
        break
      }
      case NOTIFY_NAVIGATE.REFUNS_WALLET_POINTS: {
        const payload = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
          typePoint: ''
        }
        this.props.getWalletPoints(payload)
        this.props.navigateTab(1)
        NavigationUtil.navigate(SCREEN_ROUTER.USE_POINT, { initialPage: 1 })
        break
      }
      default:
        NavigationUtil.navigate(SCREEN_ROUTER.HOME)
    }
  }

  async onIds(device) {
    if (device) {
      if (!!device.userId)
        await AsyncStorage.setItem('deviceID', device.userId)
    }
    Reactotron.log("Device info: ", device);
  }

  render() {
    return (
      <AppNavigator
        ref={navigatorRef => NavigationUtil.setTopLevelNavigator(navigatorRef)}
      />
    )
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {
  getHome,
  getNotification,
  getUserInfo,
  getWalletPoints,
  updateUserLocal,
  navigateTab,
  getWalletAccumulate
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);

const styles = StyleSheet.create({});
