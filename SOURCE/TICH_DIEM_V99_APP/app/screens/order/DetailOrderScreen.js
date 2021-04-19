import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  ScrollView,
  Image,
  SafeAreaView,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import {
  Block,
  DCHeader,
  Loading,
  Empty,
  Error,
  NumberFormat,
  OrderItem,
  FastImage, FstImage, LoadingProgress
} from "../../components";
import { ORDER, SCREEN_ROUTER } from "../../constants/Constant";
import reactotron from "reactotron-react-native";
import * as theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import { getOrderDetail } from "../../redux/actions";
import { themed } from "react-navigation";
import { cancelOrder, requestAddToCart, requestCreateOrder } from "../../constants/Api";
import { showConfirm, showMessages } from "../../utils/Alert";
import NavigationUtil from "../../navigation/NavigationUtil";
import { getListOrder, updateUserLocal, addToCart } from "../../redux/actions";
import DateUtil from "../../utils/DateUtil";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import R from "@app/assets/R";
import { OrderStatusEnum } from './ListOrderScreen'
import { NavigationActions, StackActions } from 'react-navigation';
import callAPI from "@app/utils/CallApiHelper";

export class DetailOrderScreen extends Component {

  state = {
    isLoading: false,
  }

  componentDidMount() {
    const orderID = this.props.navigation.getParam("orderID");
    this.props.getOrderDetail(orderID);
  }

  _cancel = async () => {
    const orderID = this.props.navigation.getParam("orderID");
    try {
      const response = await cancelOrder(orderID);
      Toast.show(response.message, BACKGROUND_TOAST.SUCCESS);
      this.props.getListOrder(ORDER.CANCELED, 1);
      this.props.getListOrder(ORDER.PENDING, 1);
      this.props.updateUserLocal({
        point: response.data.point
      })

      NavigationUtil.goBack();
    } catch (error) {
      Toast.show("Vui lòng thử lại", BACKGROUND_TOAST.FAIL);
    }
  };

  _renderBody() {
    const orderID = this.props.navigation.getParam("orderID");
    const { orderDetailState } = this.props;
    if (orderDetailState.isLoading) return <Loading />;
    if (orderDetailState.error)
      return (
        <Error
          onPress={() => {
            this.props.getOrderDetail(orderID);
          }}
        />
      );
    if (orderDetailState.data == 0) return <Loading />;

    return (
      <Block style={theme.styles.container}>
        <View style={styles._viewCode}>
          <Text
            style={[
              theme.fonts.regular18,
              { color: theme.colors.black_title }
            ]}
          >
            Mã đơn hàng:  {orderDetailState.data.code}
          </Text>
          <Text
            style={[
              theme.fonts.regular16,
              {
                color: OrderStatusEnum[orderDetailState.data.status].color
              }
            ]}
          >
            {OrderStatusEnum[orderDetailState.data.status].title}
          </Text>
        </View>

        <View style={{
          backgroundColor: theme.colors.white,
          paddingHorizontal: '5%',
          marginTop: 10
        }}>
          <View style={styles.view_user_info}>
            <FstImage source={R.images.ic_single} style={styles.ic_info} resizeMode='contain' />
            <Text
              style={styles.text_user_info}>{orderDetailState.data.buyerName}</Text>
          </View>
          <View style={[styles.view_user_info, { marginVertical: 5 }]}>
            <FstImage source={R.images.ic_phone} style={styles.ic_info} resizeMode='contain' />
            <Text
              style={styles.text_user_info}>
              {orderDetailState.data.buyerPhone}</Text>
          </View>
          <View style={styles.view_user_info}>
            <FstImage source={R.images.ic_pin} style={styles.ic_info} resizeMode='contain' />
            <Text
              style={styles.text_user_info}
            >
              {orderDetailState.data.address},{" "}
              {orderDetailState.data.districtName},{" "}
              {orderDetailState.data.provinceName}
            </Text>
          </View>
        </View>

        <View style={{ marginTop: 10, backgroundColor: theme.colors.white }}>
          {orderDetailState.data.listOrderItem.map((item, index) => {
            return <OrderItem item={item} index={index} key={index} />;
          })}
        </View>
        {/* {orderDetailState.data.status != ORDER.PAID && (
          <View style={{ marginTop: 10, backgroundColor: theme.colors.white }}>
            {this._renderHeader(
              R.images.ic_box,
              I18n.t("note_bill")
            )}
            <Text
              style={[
                theme.fonts.robotoItalic14,
                { paddingHorizontal: 15, paddingVertical: 10 }
              ]}
            >
              {orderDetailState.data.note}
            </Text>
          </View>
        )} */}
        <View style={{ marginTop: 10, backgroundColor: theme.colors.white }}>
          <View style={styles._viewCode}>
            <Text style={[theme.fonts.regular18, { color: theme.colors.black1 }]}>
              Mã giới thiệu
            </Text>
            <Text style={{
              color: theme.colors.black1,
              ...theme.fonts.regular18
            }}>{orderDetailState.data.lastRefCode || R.strings().not_update}</Text>
          </View>
          <View style={styles._viewCode}>
            <Text style={[theme.fonts.regular18, { color: theme.colors.black1 }]}>
              {I18n.t("sum")}
            </Text>
            <NumberFormat
              value={orderDetailState.data.totalPrice}
              color={theme.colors.red_money}
              perfix="đ"
              fonts={theme.fonts.regular18}
            />
          </View>
        </View>

        {/* <View
          style={{
            backgroundColor: theme.colors.white,
            marginTop: 10,
            paddingHorizontal: 15,
            paddingBottom: 20,
            paddingTop: 5
          }}
        >
          {orderDetailState.data.createDate != null &&
            this._renderTime(
              I18n.t("time_order"),
              orderDetailState.data.createDate
            )}
          {orderDetailState.data.cancelDate != null &&
            this._renderTime(
              I18n.t("time_cancel"),
              orderDetailState.data.cancelDate
            )}
          {orderDetailState.data.confirmDate != null &&
            this._renderTime(
              I18n.t("time_confirm"),
              orderDetailState.data.confirmDate
            )}
          {orderDetailState.data.paymentDate != null &&
            this._renderTime(
              I18n.t("time_payment"),
              orderDetailState.data.paymentDate
            )}
        </View> */}
        {orderDetailState.data.status == ORDER.PENDING && (
          <View style={{ marginTop: 50, backgroundColor: theme.colors.white }}>
            <TouchableOpacity
              onPress={() => {
                showConfirm(
                  I18n.t("cancel_order"),
                  I18n.t("confirm_cancel"),
                  () => this._cancel()
                );
              }}
              style={styles._viewCancel}
            >
              <Text
                style={[
                  theme.fonts.semibold18,
                  { color: theme.colors.white }
                ]}
              >
                {I18n.t("cancel_order")}
              </Text>
            </TouchableOpacity>
          </View>
        )}
        {(orderDetailState.data.status == ORDER.REFUSE || orderDetailState.data.status == ORDER.CANCELED) && (
          <View style={{ marginTop: 50, backgroundColor: theme.colors.white }}>
            <TouchableOpacity
              onPress={() => {
                showConfirm(
                  'Đặt lại',
                  'Đặt lại đơn hàng này!',
                  this.handRe_orderPress
                );
              }}
              style={[styles._viewCancel, { backgroundColor: theme.colors.active }]}
            >
              <Text
                style={[
                  theme.fonts.semibold18,
                  { color: theme.colors.white }
                ]}
              >
                Đặt lại
              </Text>
            </TouchableOpacity>
          </View>
        )}
      </Block>
    );
  }

  handRe_orderPress = () => {
    const { navigation, orderDetailState, } = this.props
    const orderDetail = orderDetailState.data
    const listItemID = orderDetail.listOrderItem.map(value => value.itemID)

    this.setState({ isLoading: true })

    callAPI({
      API: requestAddToCart,
      payload: listItemID,
      onSuccess: res => {
        NavigationUtil.navigate(SCREEN_ROUTER.CART, { listItemIDSelected: listItemID })
      },
      onError: err => {
        console.log(err);
      },
      onFinaly: () => {
        this.setState({ isLoading: false })
      }
    })
  }

  re_orderPress = async () => {
    const { navigation, orderDetailState, } = this.props
    const orderDetail = orderDetailState.data

    payload = {
      listOrderItem: orderDetail.listOrderItem,
      ProvinceID: orderDetail.provinceID,
      DistrictID: orderDetail.districtID,
      BuyerName: orderDetail.buyerName,
      BuyerPhone: orderDetail.buyerPhone,
      address: orderDetail.address,
      note: orderDetail.note,
      lastRefCode: orderDetail.lastRefCode
    }

    this.setState({
      isLoading: true
    })

    try {
      const res = requestCreateOrder(payload)
      this.props.navigation.reset(
        [
          NavigationActions.navigate({
            routeName: SCREEN_ROUTER.BOTTOM_BAR,
            action: NavigationActions.navigate({
              routeName: SCREEN_ROUTER.USER
            })
          }),
          NavigationActions.navigate({ routeName: SCREEN_ROUTER.ORDER })
        ], 1)
      Toast.show('Đặt hàng thành công')
    } catch (error) {
      console.log(error);
      this.setState({ isLoading: false },
        () => Toast.show('Vui lòng thử lại', BACKGROUND_TOAST.FAIL)
      )
    } finally { this.setState({ isLoading: false }) }
  }

  _renderTime(title, time) {
    return (
      <View
        style={{
          flexDirection: "row",
          justifyContent: "space-between",
          paddingTop: 5
        }}
      >
        <Text
          style={[
            theme.fonts.robotoRegular14,
            { color: theme.colors.text_gray }
          ]}
        >
          {title}
        </Text>
        <Text
          style={[
            theme.fonts.robotoRegular14,
            { color: theme.colors.text_gray }
          ]}
        >
          {DateUtil.formatDateTime(time)}
        </Text>
      </View>
    );
  }
  render() {
    const { orderDetailState } = this.props;
    const { isLoading } = this.state
    //const code = navigation.getParam('code')
    return (
      <Block>
        <DCHeader title={'Chi tiết đơn hàng'} isWhiteBackground />
        {/* <DCHeader title={I18n.t("bill") + " " + orderDetailState.data.code} /> */}
        <SafeAreaView style={theme.styles.container}>
          <ScrollView contentContainerStyle={{ flexGrow: 1 }}>
            {isLoading && <LoadingProgress />}
            {this._renderBody()}
          </ScrollView>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  orderDetailState: state.getOrderDetailReducer
});

const mapDispatchToProps = {
  getOrderDetail,
  getListOrder,
  updateUserLocal,
  addToCart
};

export default connect(mapStateToProps, mapDispatchToProps)(DetailOrderScreen);
const styles = StyleSheet.create({
  _viewCode: {
    flexDirection: "row",
    alignItems: 'center',
    justifyContent: "space-between",
    marginTop: 10,
    paddingHorizontal: '5%',
    backgroundColor: theme.colors.white
  },
  _viewCancel: {
    paddingVertical: 15,
    marginTop: 20,
    backgroundColor: theme.colors.red,
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 5,
    marginBottom: 30,
    marginHorizontal: '5%'
  },
  text_user_info: {
    ...theme.fonts.regular18,
    color: theme.colors.black_title,
    marginLeft: 10
  },
  ic_info: {
    width: 20,
    height: 20
  },
  view_user_info: {
    flexDirection: 'row',
    alignItems: 'center'
  }
});
