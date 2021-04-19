import React, { Component } from "react";
import {
  View,
  Text,
  SafeAreaView,
  FlatList,
  RefreshControl,
  TouchableOpacity
} from "react-native";
import { connect } from "react-redux";
import {
  Loading,
  Empty,
  Error,
  DCHeader,
  Block,
  FastImage,
  RequireLogin
} from "../../components";
import I18n from "../../i18n/i18n";
import { SCREEN_ROUTER, NAVIGATE_IN_NOTIFY, GET_HISTORY_POINT_TYPE } from "../../constants/Constant";
import * as theme from "../../constants/Theme";
import { getNotification, updateNotiItem, getWalletPoints, navigateTab, getWalletAccumulate } from "../../redux/actions";
import NavigationUtil from '../../navigation/NavigationUtil'
import DateUtil from "@app/utils/DateUtil";
import { requestUpdateNoti } from '@api'
export class NotificationScreen extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: true,
      error: null,
      data: [],
      isLogin: this.props.homeState.isLogin
    };
  }

  componentDidMount = async () => {
    if (this.state.isLogin) {
      this.props.getNotification();
    }
  };

  renderBody() {
    const { notificationState } = this.props;
    if (notificationState.isLoading) return <Loading />;
    if (notificationState.error)
      return <Error onPress={() => this.props.getNotification()} />;
    if (notificationState.data.length == 0)
      return <Empty onRefresh={this.props.getNotification} />;

    return (
      <FlatList
        refreshControl={
          <RefreshControl
            refreshing={false}
            onRefresh={() => this.props.getNotification()}
          />
        }
        data={notificationState.data}
        keyExtractor={(item, index) => index.toString()}
        renderItem={({ item, index }) => <NotiItem item={item} index={index} onPress={() => this._onPress(item, index)} />}
      />
    );
  }

  updateViewNoti = async (item, index) => {
    const payload = {
      ID: item.notifyID
    }
    try {
      const res = await requestUpdateNoti(payload)
      this.props.updateNotiItem(index)

    } catch (error) {
      console.log(error);
    }
  }

  _onPress(item, index) {

    this.updateViewNoti(item, index)
    switch (item.type) {
      case NAVIGATE_IN_NOTIFY.ORDER: {
        const payload = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
          typePoint: ''
        }
        this.props.getWalletPoints(payload)
        NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_ORDER, { orderID: item.objectID })
        break;
      }

      case NAVIGATE_IN_NOTIFY.NEWS:
        NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_UTILITY, { newsID: item.objectID })
        break
      case NAVIGATE_IN_NOTIFY.NOTIFY_POINTS_HISTORY:
      case NAVIGATE_IN_NOTIFY.NOTIFY_POINTS_SYSTEM: {
        const payload = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
          typePoint: ''
        }
        this.props.getWalletPoints(payload)
        NavigationUtil.navigate(SCREEN_ROUTER.USE_POINT, { initialPage: 0 })
        this.props.navigateTab(0)
        break
      }
      case NAVIGATE_IN_NOTIFY.ADD_ACCUMULATE_POINTS:
      case NAVIGATE_IN_NOTIFY.NOTIFY_ACCUMULATE_POINTS: {
        const payload = {
          page: 1,
          type: GET_HISTORY_POINT_TYPE.WALLET_ACCUMULATE_POINTS,
          typePoint: ''
        }
        this.props.getWalletAccumulate(payload)
        NavigationUtil.navigate(SCREEN_ROUTER.USE_POINT, { initialPage: 1 })
        this.props.navigateTab(1)
        break
      }

      case NAVIGATE_IN_NOTIFY.DRAW_POINTS:
        if (item.objectID) NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_DRAW_POINTS, { id: item.objectID })
        break
      default:
        break;
    }
  }

  render() {
    return (
      <Block>
        <DCHeader title={I18n.t("notify")} />
        <SafeAreaView style={theme.styles.container}>
          {/* {this.renderBody()} */}
          {this.state.isLogin ? this.renderBody() : <RequireLogin />}
        </SafeAreaView>
      </Block>
    );
  }
}

class NotiItem extends Component {
  render() {
    const { item, index, onPress } = this.props;

    return (
      <TouchableOpacity
        style={{
          flexDirection: 'row',
          marginTop: 5,
          paddingHorizontal: 20,
          paddingVertical: 10,
          backgroundColor: item.viewed != 1 && '#F7B73146' || theme.colors.white
        }}
        onPress={onPress}
      >

        <FastImage
          resizeMode="contain"
          style={{
            width: 45,
            height: 45
          }}
          source={{ uri: item.icon }}
        />
        <Block middle paddingLeft={15}>
          <Text
            style={
              ([theme.fonts.robotoMedium16],
                { marginBottom: 5, fontWeight: "600" })
            }
          >
            {item.title}
          </Text>
          <Text
            style={[
              theme.fonts.robotoRegular12,
              {
                color: theme.colors.text_gray
              }
            ]}
          >
            {DateUtil.formatTime(item.createdDate)} {DateUtil.formatShortDate(item.createdDate)}
          </Text>
        </Block>
      </TouchableOpacity>
    );
  }

}

const mapStateToProps = state => ({
  notificationState: state.notificationReducer,
  homeState: state.homeReducer
});

const mapDispatchToProps = {
  getNotification,
  updateNotiItem,
  getWalletPoints,
  navigateTab,
  getWalletAccumulate
};

export default connect(mapStateToProps, mapDispatchToProps)(NotificationScreen);
