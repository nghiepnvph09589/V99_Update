import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  ScrollView,
  FlatList,
  RefreshControl, Image
} from "react-native";
import { connect } from "react-redux";
import {
  Loading,
  Empty,
  Error,
  Block,
  NumberFormat, FastImage,
} from "../../components";
import { getListOrder } from "../../redux/actions";
import { ORDER, SCREEN_ROUTER } from "../../constants/Constant";
import reactotron from "reactotron-react-native";
import * as theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import NavigationUtil from "../../navigation/NavigationUtil";
import R from "@app/assets/R";
import DateUtil from "@app/utils/DateUtil";

export const OrderStatusEnum = {
  [ORDER.PAID]: {
    color: '#10AC84',
    title: R.strings().complete
  },
  [ORDER.REFUSE]: {
    color: '#D90429',
    title: R.strings().refuse
  },
  [ORDER.CANCELED]: {
    color: '#D90429',
    title: R.strings().canceled
  },
  [ORDER.CONFIRM]: {
    color: '#F39C12',
    title: R.strings().processing
  },
  [ORDER.PENDING]: {
    color: '#F39C12',
    title: R.strings().pending
  }
}

class OrderItem extends Component {

  render() {
    const { item, index } = this.props;
    return (
      <TouchableOpacity
        style={{
          marginTop: 5,
          backgroundColor: theme.colors.white,
          paddingVertical: 5,
          borderBottomWidth: 0.5,
          borderBottomColor: theme.colors.border
        }}
        onPress={() => {
          NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_ORDER, {
            orderID: item.orderID,
            code: item.code,
          });
        }}
      >
        <View
          style={{
            flexDirection: "row",
            paddingHorizontal: '2.5%',
            alignItems: 'center',
          }}
        >
          <FastImage
            style={{
              height: 60,
              aspectRatio: 1
            }}
            source={{ uri: item.image }}
          />
          <Block style={{ paddingLeft: '2.5%' }}>
            <View
              style={{
                flexDirection: "row",
              }}
            >
              <Text style={(theme.fonts.regular16, {
                flex: 1,
                color: theme.colors.black_title,
                paddingRight: 5,
              })}>
                {item.name}
              </Text>
              {(item.status == ORDER.PAID || item.status == ORDER.CANCELED || item.status == ORDER.REFUSE) ? <Text style={(theme.fonts.regular16,
                { color: OrderStatusEnum[item.status].color, ...theme.fonts.regular16 })}>
                {OrderStatusEnum[item.status].title}
              </Text> : null}
            </View>
            <NumberFormat
              style={{ marginVertical: 10 }}
              value={item.totalPrice}
              color={theme.colors.red_money}
              perfix="VNĐ"
              fonts={theme.fonts.regular15}
            />
            <View
              style={{
                flexDirection: "row",
                justifyContent: "space-between",
              }}
            >
              <Text style={[theme.fonts.regular15, { color: theme.colors.black1 }]}>
                {item.qty}
                {I18n.t("product2")}
              </Text>
              {/* <Text style={[theme.fonts.robotoBold16]}>
            {I18n.t("code_order")}
            {item.code}
          </Text> */}
              <Text style={[theme.fonts.regular15,
              { color: theme.colors.black1 }]}>{DateUtil.formatTime(item.createDate)} {DateUtil.formatShortDate(item.createDate)}</Text>
            </View>
          </Block>
        </View>
        {/* <View style={{ backgroundColor: theme.colors.border, height: 0.5, marginTop: 5 }} /> */}
      </TouchableOpacity>
    );
  }
}
export class ListOrderScreen extends Component {
  componentDidMount() {
    this._onRefresh();
  }
  _onRefresh() {
    const { status } = this.props;
    this.props.getListOrder(status, 1);
  }
  render() {
    const {
      status,
      confirmState,
      pendingState,
      historyState,
      refreshing
    } = this.props;
    var loading = pendingState.isLoading;
    var error = pendingState.error;
    var data = [];
    switch (status) {
      case ORDER.PENDING:
        loading = pendingState.isLoading;
        error = pendingState.error;
        data = pendingState.data;
        break;
      case ORDER.CONFIRM:
        loading = confirmState.isLoading;
        error = confirmState.error;
        data = confirmState.data;
        break;
      case ORDER.HISTORY:
        loading = historyState.isLoading;
        error = historyState.error;
        data = historyState.data;
        break;
    }
    if (loading) return <Loading />;
    if (error)
      return (
        <Error
          onPress={() => {
            this._onRefresh();
          }}
        />
      );
    if (data.length == 0) {
      return <Empty onRefresh={() => this._onRefresh()} />;
    }

    return (
      <FlatList
        style={[theme.styles.container]}
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={() => this._onRefresh()}
          />
        }
        data={data}
        keyExtractor={(item, index) => index.toString()}
        renderItem={({ item, index }) => {
          return <OrderItem item={item} index={index} />;
        }}
      />
    );
  }
}

const mapStateToProps = state => ({
  pendingState: state.orderReducer.pending,
  confirmState: state.orderReducer.confirm,
  historyState: state.orderReducer.history,
  refreshing: state.orderReducer.refreshing
});

const mapDispatchToProps = {
  getListOrder
};

export default connect(mapStateToProps, mapDispatchToProps)(ListOrderScreen);