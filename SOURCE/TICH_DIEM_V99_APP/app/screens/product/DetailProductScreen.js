import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  SafeAreaView,
  Image,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  Alert,
  Linking
} from "react-native";
import { connect } from "react-redux";
import {
  Block,
  PrimaryButton,
  LoadingProgress,
  NumberFormat,
  FastImage,
  FstImage
} from "../../components";
import * as theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import { addToCart, getUserInfo, getCountCart } from "../../redux/actions";
import NavigationUtil from "../../navigation/NavigationUtil";
import {
  SCREEN_ROUTER,
  REDUCER,
  USER_ACTIVATED
} from "../../constants/Constant";
import { showConfirm } from "../../utils/Alert";
import { requireLogin } from "../../utils/AlertRequireLogin";
import ObjectUtil from "../../utils/ObjectUtil";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import HTMLView from "react-native-render-html";
import reactotron from "reactotron-react-native";
import AutoHeightImage from "react-native-auto-height-image";
import AsyncStorage from "@react-native-community/async-storage";
import R from "@app/assets/R";
import SwiperFlatList from "react-native-swiper-flatlist";
import { requestAddToCart } from "@api";
import callAPI from "@app/utils/CallApiHelper";
import { formatNumber } from "@app/utils/NumberUtils";

export class DetailProductScreen extends Component {
  componentDidMount() {
    this.checkToken();
  }

  state = {
    token: null,
    isRequesting: false
  };

  checkToken = async () => {
    const { userInfoState } = this.props;
    var token = await AsyncStorage.getItem("token");
    if (!!token) this.props.getCountCart();
    if (ObjectUtil.isEmpty(userInfoState.data) && !!token)
      this.props.getUserInfo();
    this.setState({ token });
  };

  renderBody() {
    const { navigation, countCartState, addCartState } = this.props;
    const item = navigation.getParam("item");
    const checkAcc = navigation.getParam("checkAcc") || 0;
    return (
      <Block>
        <View style={{ paddingHorizontal: "2.5%" }}>
          <Text
            style={[
              theme.fonts.semibold14,
              {
                marginTop: 10
              }
            ]}
          >
            {item.name}
          </Text>
          {/* <View
            style={{
              flexDirection: "row",
              marginTop: 5,
              justifyContent: "space-between"
            }}
          >
            <NumberFormat
              value={item.price}
              color={theme.colors.red_money}
              fonts={theme.fonts.semibold18}
              perfix="đ"
            />
            <Text
              style={{
                color:
                  item.stockStatus == 1 ? "#00C48C" : theme.colors.red_money,
                ...theme.fonts.semibold15
              }}
            >
              {item.stockStatus == 1 ? "Còn hàng" : "Hết hàng"}
            </Text>
          </View> */}
          <View style={{ flexDirection: "column", padding: 5 }}>
            <View
              style={{
                flexDirection: "row",
                marginTop: 5,
                justifyContent: "space-between"
              }}
            >
              <Text
                style={{
                  color: "red",
                  textDecorationLine: !checkAcc ? "none" : "line-through",
                  ...theme.fonts.regular14
                }}
                children={"Giá thường: " + formatNumber(item.price) + "đ"}
              />
              <Text
                style={{
                  color:
                    item.stockStatus == 1 ? "#00C48C" : theme.colors.red_money,
                  ...theme.fonts.semibold15
                }}
              >
                {item.stockStatus == 1 ? "Còn hàng" : "Hết hàng"}
              </Text>
            </View>

            <Text
              style={{
                marginTop: 8,
                textDecorationLine: checkAcc ? "none" : "line-through",
                color: "red",
                ...theme.fonts.regular14
              }}
              children={"Giá VIP: " + formatNumber(item.priceVIP) + "đ"}
            />
          </View>

          <HTMLView
            html={item.description}
            containerStyle={{
              backgroundColor: theme.colors.white,
              width: width * 0.95,
              paddingVertical: "2.5%",
              alignSelf: "center"
            }}
            onLinkPress={(even, href) => Linking.openURL(href)}
            imagesInitialDimensions={{
              width: width,
              aspectRatio: 3
            }}
            ignoredStyles={["height", "width", "font-family"]}
            imagesMaxWidth={width * 0.95}
            alterChildren={node => {
              if (node.name === "iframe") {
                delete node.attribs.width;
                delete node.attribs.height;
              }
              return node.children;
            }}
          />
        </View>
      </Block>
    );
  }

  handleAddToCart = () => {
    if (!this.state.token) {
      requireLogin();
      return;
    }

    const { navigation } = this.props;
    const item = navigation.getParam("item");

    if (item.stockStatus != 1) {
      Toast.show("Sản phẩm này đã hết hàng", BACKGROUND_TOAST.FAIL);
      return;
    }

    const itemID = item.id;
    this.props.addToCart([itemID]);
  };

  handleBuyNow = () => {
    if (!this.state.token) {
      requireLogin();
      return;
    }

    const { navigation } = this.props;
    const item = navigation.getParam("item");

    if (item.stockStatus != 1) {
      Toast.show("Sản phẩm này đã hết hàng", BACKGROUND_TOAST.FAIL);
      return;
    }

    const itemID = item.id;
    this.setState({ isRequesting: true });

    callAPI({
      API: requestAddToCart,
      payload: [itemID],
      onSuccess: res => {
        NavigationUtil.navigate(SCREEN_ROUTER.CART, {
          listItemIDSelected: [itemID]
        });
      },
      onError: err => {
        console.log(err);
      },
      onFinaly: () => {
        this.setState({ isRequesting: false });
      }
    });
  };

  render() {
    const { navigation, countCartState, addCartState } = this.props;
    const { isRequesting } = this.state;
    const item = navigation.getParam("item");

    listImage = item.image;
    return (
      <Block>
        {(addCartState.isLoading || isRequesting) && <LoadingProgress />}
        <SafeAreaView style={theme.styles.container}>
          <ScrollView showsVerticalScrollIndicator={false}>
            <View
              style={{
                alignItems: "center",
                backgroundColor: theme.colors.white
              }}
            >
              <SwiperFlatList
                autoplay
                autoplayDelay={3}
                autoplayLoop
                data={listImage || []}
                keyExtractor={(item, index) => index.toString()}
                showPagination
                paginationActiveColor={theme.colors.active}
                paginationDefaultColor={theme.colors.inactive}
                paginationStyle={{
                  marginBottom: 15
                }}
                paginationStyleItem={{
                  width: width * 0.025,
                  aspectRatio: 2
                }}
                renderItem={({ item, index }) => {
                  return (
                    <TouchableOpacity
                      onPress={() => {
                        NavigationUtil.navigate(SCREEN_ROUTER.IMAGE_VIEWER, {
                          images: {
                            listImage,
                            index
                          }
                        });
                      }}
                    >
                      <FstImage
                        source={{ uri: item }}
                        style={styles.image}
                        resizeMode="cover"
                      />
                    </TouchableOpacity>
                  );
                }}
              />
              <TouchableOpacity
                style={styles._imgBack}
                onPress={NavigationUtil.goBack}
              >
                <FstImage
                  style={{
                    height: 36,
                    width: 36
                  }}
                  source={R.images.ic_back2}
                  resizeMode="contain"
                />
              </TouchableOpacity>
              <TouchableOpacity
                style={[styles._imgBack, { left: "85%" }]}
                onPress={() => {
                  if (!!this.state.token)
                    NavigationUtil.navigate(SCREEN_ROUTER.CART);
                  else requireLogin();
                }}
              >
                <FstImage
                  style={{
                    height: 36,
                    width: 36
                  }}
                  source={R.images.ic_cart_round}
                  resizeMode="contain"
                />
              </TouchableOpacity>
            </View>
            {this.renderBody()}
          </ScrollView>
          <Block flex={false} row paddingHorizontal={10} paddingVertical={10}>
            <PrimaryButton
              onPress={this.handleAddToCart}
              border={theme.colors.primary}
              background={theme.colors.white}
              textColor={theme.colors.primary}
              style={[
                styles.button,
                {
                  marginRight: 5
                }
              ]}
              title={I18n.t("add_to_cart")}
            />
            <PrimaryButton
              onPress={this.handleBuyNow}
              style={styles.button}
              title={I18n.t("buy_now")}
            />
          </Block>
        </SafeAreaView>
      </Block>
    );
  }
}

const styles = StyleSheet.create({
  button: {
    flex: 1
  },
  image: {
    width,
    height: height * 0.4,
    marginBottom: 10,
    backgroundColor: "white"
  },
  _imgBack: {
    position: "absolute",
    left: "3%",
    top: 10
  }
});

const mapStateToProps = state => ({
  countCartState: state[REDUCER.COUNT_CART],
  addCartState: state[REDUCER.ADD_TO_CART],
  userInfoState: state[REDUCER.USER]
});

const mapDispatchToProps = {
  addToCart,
  getUserInfo,
  getCountCart
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DetailProductScreen);
