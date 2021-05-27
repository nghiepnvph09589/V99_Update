import React from "react";
import { Image, View } from "react-native";
import { createAppContainer, createSwitchNavigator } from "react-navigation";
import { createStackNavigator } from "react-navigation-stack";
import { BottomTabBar, createBottomTabNavigator } from "react-navigation-tabs";
import { ASYNCSTORAGE_KEY, SCREEN_ROUTER } from "../constants/Constant";
import Utils from "../utils/Utils";
import * as theme from "../constants/Theme";
import I18n from "../i18n/i18n";
import AuthLoadingScreen from "../screens/auth/AuthLoadingScreen";
import ForgotPasswordScreen from "../screens/auth/ForgotPasswordScreen";
import LoginScreen from "../screens/auth/LoginScreen";
import RegisterScreen from "../screens/auth/RegisterScreen";
import HomeScreen from "../screens/HomeScreen";
import NotificationScreen from "../screens/notify/NotificationScreen";
import UsePointScreen from "../screens/point/UsePointScreen";
import ProductScreen from "../screens/product/ProductScreen";
import UpdateUserInfoScreen from "../screens/account/UpdateUserInfoScreen";
import UserScreen from "../screens/UserScreen";
import ListProductScreen from "../screens/product/ListProductScreen";
import OrderScreen from "../screens/order/OrderScreen";
import SearchReferralCodeScreen from "../screens/order/SearchReferralCodeScreen";
import HistoryScreen from "../screens/account/HistoryScreen";
import DetailProductScreen from "../screens/product/DetailProductScreen";
import CartScreen from "../screens/cart/CartScreen";
import ConfirmOrderScreen from "../screens/cart/ConfirmOrderScreen";
import CustomerInfoScreen from "../screens/cart/CustomerInfoScreen";
import DetailOrderScreen from "../screens/order/DetailOrderScreen";
import UtilityScreen from "../screens/utility/UtilityScreen";
import DetailUtilityScreen from "../screens/utility/DetailUtilityScreen";
import ChangePassScreen from "../screens/account/ChangePassScreen";
import ImageViewerScreen from "../screens/ImageViewerScreen";
import UpdateImage from "../screens/account/UpdateImage";
import IntroduceCustomersScreen from "../screens/account/IntroduceCustomersScreen";

import CheckPhoneScreen from "@screen/auth/CheckPhoneScreen";
import OTPScreen from "@screen/auth/OTPScreen";

import MovingPointsScreen from "@app/screens/point/moving_points/MovingPointsScreen";
import HistoryMovingPointsScreen from "@app/screens/point/moving_points/HistoryMovingPointsScreen";
import MovingPointsSuccessScreen from "@app/screens/point/moving_points/MovingPointsSuccessScreen";
import DrawPointsScreen from "@app/screens/point/draw_points/DrawPointsScreen";
import DetailDrawPointsScreen from "@app/screens/point/draw_points/DetailDrawPointsScreen";
import LoadPointsScreen from "@app/screens/point/load_points/LoadPointsScreen";
import HistoryDrawPointsScreen from "@app/screens/point/draw_points/HistoryDrawPointsScreen";
import AddBankScreen from "@screen/point/bank/AddBankScreen";
import BankScreen from "@screen/point/bank/BankScreen";

import R from "@app/assets/R";
import { CHANGE_PASS } from "@app/redux/actions/type";
import AsyncStorage from "@react-native-community/async-storage";
import NavigationUtil from "./NavigationUtil";
import Toast from "@app/utils/Toast";

import PolicyUserScreen from "../screens/account/PolicyUserScreen";
import HistoryAwardScreen from "@app/screens/account/HistoryAwardScreen";

const TabBarComponent = props => <BottomTabBar {...props} />;

const tabbarIcons = {
  [SCREEN_ROUTER.HOME]: R.images.ic_tab_home,
  [SCREEN_ROUTER.PRODUCT]: R.images.ic_tab_product,
  [SCREEN_ROUTER.USER]: R.images.ic_tab_account,
  [SCREEN_ROUTER.USE_POINT]: R.images.ic_tab_accumulate_points
};

const tabbarIconsActive = {
  [SCREEN_ROUTER.HOME]: R.images.ic_tab_home_active,
  [SCREEN_ROUTER.PRODUCT]: R.images.ic_tab_product_active,
  [SCREEN_ROUTER.USER]: R.images.ic_tab_account_active,
  [SCREEN_ROUTER.USE_POINT]: R.images.ic_tab_accumulate_points_active
};

const getTabBarIcon = (navigation, focused, tintColor) => {
  const { routeName } = navigation.state;
  const iconSource =
    (focused && tabbarIconsActive[routeName]) ||
    tabbarIcons[routeName] ||
    R.images.ic_tab_home;
  const iconSize = 20;
  return (
    <View
      style={{
        backgroundColor: focused && "#F7B73146",
        padding: 7,
        borderRadius: 3,
        justifyContent: "center",
        alignItems: "center"
      }}
    >
      <Image
        source={iconSource}
        fadeDuration={0}
        style={{
          width: iconSize,
          height: iconSize
        }}
      />
    </View>
  );
};

const bottomBar = createBottomTabNavigator(
  {
    [SCREEN_ROUTER.HOME]: {
      screen: HomeScreen,
      title: I18n.t("home"),
      navigationOptions: {
        tabBarLabel: Utils.toPascalCase(I18n.t("home"))
      }
    },
    [SCREEN_ROUTER.PRODUCT]: {
      screen: ProductScreen,
      title: I18n.t("product"),
      navigationOptions: {
        tabBarLabel: Utils.toPascalCase(I18n.t("product"))
      }
    },
    [SCREEN_ROUTER.USE_POINT]: {
      screen: UsePointScreen,
      title: I18n.t("accumulate_points"),
      navigationOptions: {
        tabBarLabel: Utils.toPascalCase(I18n.t("accumulate_points"))
      }
    },
    [SCREEN_ROUTER.USER]: {
      screen: UserScreen,
      title: I18n.t("account"),
      navigationOptions: {
        tabBarLabel: Utils.toPascalCase(I18n.t("account"))
      }
    }
  },
  {
    defaultNavigationOptions: ({ navigation }) => ({
      tabBarIcon: ({ focused, tintColor }) =>
        getTabBarIcon(navigation, focused, tintColor)
    }),
    tabBarOptions: {
      activeBackgroundColor: theme.colors.bottombarBg,
      inactiveBackgroundColor: theme.colors.bottombarBg,
      inactiveTintColor: theme.colors.inactive,
      activeTintColor: theme.colors.active,
      labelStyle: {
        ...theme.fonts.semibold12
      }
    },
    tabBarComponent: props => {
      return (
        <TabBarComponent
          {...props}
          onTabPress={async route => {
            if (
              route.route.key == SCREEN_ROUTER.HOME ||
              route.route.key == SCREEN_ROUTER.PRODUCT
            ) {
              props.onTabPress(route);
            } else {
              const token = await AsyncStorage.getItem(ASYNCSTORAGE_KEY.TOKEN);

              if (!!token) {
                props.onTabPress(route);
              } else {
                NavigationUtil.navigate(SCREEN_ROUTER.LOGIN);
                Toast.show("Đăng nhập để sử dụng chức năng này");
              }
            }
          }}
          style={{
            backgroundColor: theme.colors.bottombarBg,
            height: 58
          }}
        />
      );
    },
    initialRouteName: SCREEN_ROUTER.HOME
  }
);

const Main = createStackNavigator(
  {
    [SCREEN_ROUTER.BOTTOM_BAR]: bottomBar,
    [SCREEN_ROUTER.LIST_PRODUCT]: ListProductScreen,
    [SCREEN_ROUTER.USER]: UserScreen,
    [SCREEN_ROUTER.UPDATE_USER]: UpdateUserInfoScreen,
    [SCREEN_ROUTER.ORDER]: OrderScreen,
    [SCREEN_ROUTER.HISTORY]: HistoryScreen,
    [SCREEN_ROUTER.DETAIL_PRODUCT]: DetailProductScreen,
    [SCREEN_ROUTER.CART]: CartScreen,
    [SCREEN_ROUTER.CONFIRM_ORDER]: ConfirmOrderScreen,
    [SCREEN_ROUTER.CUS_INFO]: CustomerInfoScreen,
    [SCREEN_ROUTER.DETAIL_ORDER]: DetailOrderScreen,
    [SCREEN_ROUTER.UTILITY]: UtilityScreen,
    [SCREEN_ROUTER.DETAIL_UTILITY]: DetailUtilityScreen,
    [SCREEN_ROUTER.CHANGE_PASS]: ChangePassScreen,
    [SCREEN_ROUTER.IMAGE_VIEWER]: ImageViewerScreen,
    [SCREEN_ROUTER.NOTIFY]: NotificationScreen,
    [SCREEN_ROUTER.CHECK_PHONE]: CheckPhoneScreen,
    [SCREEN_ROUTER.OTP]: OTPScreen,
    Login: LoginScreen,
    Register: RegisterScreen,
    ForgotPassword: ForgotPasswordScreen,
    [SCREEN_ROUTER.UPDATE_IMAGE]: UpdateImage,
    [SCREEN_ROUTER.SEARCH_REFERRAL_CODE]: SearchReferralCodeScreen,
    [SCREEN_ROUTER.MOVING_POINTS]: MovingPointsScreen,
    [SCREEN_ROUTER.MOVING_POINTS_SUCCESS]: MovingPointsSuccessScreen,
    [SCREEN_ROUTER.LOAD_POINTS]: LoadPointsScreen,
    [SCREEN_ROUTER.DRAW_POINTS]: DrawPointsScreen,
    [SCREEN_ROUTER.HISTORY_DRAW_POINTS]: HistoryDrawPointsScreen,
    [SCREEN_ROUTER.BANK]: BankScreen,
    [SCREEN_ROUTER.ADD_BANK]: AddBankScreen,
    [SCREEN_ROUTER.DETAIL_DRAW_POINTS]: DetailDrawPointsScreen,
    [SCREEN_ROUTER.HISTORY_MOVING_POINTS]: HistoryMovingPointsScreen,
    [SCREEN_ROUTER.INTRODUCE_CUSTOMERS]: IntroduceCustomersScreen,
    [SCREEN_ROUTER.POLICY_USER_SCREEN]: PolicyUserScreen,
    [SCREEN_ROUTER.HISTORY_AWARD_SCREEN]: HistoryAwardScreen
  },
  {
    defaultNavigationOptions: {
      header: null
    }
  }
);

export default createAppContainer(
  createSwitchNavigator(
    {
      [SCREEN_ROUTER.AUTH_LOADING]: AuthLoadingScreen,
      [SCREEN_ROUTER.MAIN]: Main
    },
    {
      initialRouteName: SCREEN_ROUTER.AUTH_LOADING
    }
  )
);
