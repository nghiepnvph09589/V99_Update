import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  ImageBackground,
  TouchableOpacity,
  RefreshControl,
  SafeAreaView,
  PermissionsAndroid,
  Platform,
  Linking,
  StatusBar
} from "react-native";
import { connect } from "react-redux";
import I18n from "../i18n/i18n";
import NavigationUtil from "../navigation/NavigationUtil";
import {
  REDUCER,
  SCREEN_ROUTER,
  USER_ACTIVATED,
  UTILITY
} from "../constants/Constant";
import {
  getHome,
  getUserInfoAction,
  getUtility,
  getBank,
  getBankSelect
} from "../redux/actions";
import {
  Block,
  NumberFormat,
  PrimaryButton,
  FastImage,
  Loading,
  Error,
  FstImage
} from "../components";
import AsyncStorage from "@react-native-community/async-storage";
import SwiperFlatList from "react-native-swiper-flatlist";
import DateUtil from "../utils/DateUtil";
import ObjectUtil from "../utils/ObjectUtil";
import { requireLogin } from "../utils/AlertRequireLogin";
import theme from "@theme";
import R from "@app/assets/R";
import Toast, { BACKGROUND_TOAST } from "@app/utils/Toast";
import reactotron from "reactotron-react-native";

class Option extends Component {
  render() {
    const { text, img, onPress } = this.props;
    return (
      <TouchableOpacity
        style={{
          flex: 1,
          backgroundColor: "white",
          alignItems: "center",
          shadowColor: "#000",
          marginBottom: 20,
          shadowOffset: {
            width: 0,
            height: 1
          },
          shadowOpacity: 0.2,
          shadowRadius: 2.0,
          elevation: 2,
          padding: 5,
          borderRadius: 5,
          marginHorizontal: "2.5%"
        }}
        onPress={onPress}
      >
        <FstImage
          style={{ height: 50, width: 50 }}
          resizeMode="contain"
          source={img}
        />
        <Text
          style={[
            theme.fonts.regular14,
            {
              width: "80%",
              textAlign: "center",
              marginTop: 5,
              color: theme.colors.black_title
            }
          ]}
          numberOfLines={2}
        >
          {text}
        </Text>
      </TouchableOpacity>
    );
  }
}
class Product extends Component {
  render() {
    const { item, index } = this.props;
    return (
      <TouchableOpacity
        style={styles._viewProduct}
        onPress={() => {
          NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_PRODUCT, { item });
        }}
      >
        <View>
          <FastImage
            style={{ height: 120, width: 120, alignSelf: "center" }}
            resizeMode="contain"
            source={{ uri: item.imageUrl }}
          />

          <Text style={[theme.fonts.robotoRegular15, { marginLeft: 10 }]}>
            {item.itemName}
          </Text>
        </View>

        <View style={{ flexDirection: "row", padding: 5 }}>
          <FastImage
            style={{ height: 15, width: 15 }}
            resizeMode="cover"
            source={R.images.ic_box}
          />

          <NumberFormat
            style={{ marginLeft: 8 }}
            value={item.price}
            color={theme.colors.red_money}
            perfix="đ"
            fonts={theme.fonts.robotoRegular12}
          />
        </View>
      </TouchableOpacity>
    );
  }
}
class News extends Component {
  render() {
    const { item } = this.props;
    return (
      <TouchableOpacity
        style={styles._viewProduct}
        onPress={() => {
          NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_UTILITY, {
            newsID: item.newsID
          });
        }}
      >
        <FstImage
          style={{
            width: "100%",
            aspectRatio: 1.3,
            alignSelf: "center",
            borderRadius: 5,
            overflow: "hidden"
          }}
          resizeMode="cover"
          source={{ uri: item.urlImage }}
        />

        <Text numberOfLines={2} style={[theme.fonts.regular16, { margin: 10 }]}>
          {item.title}
        </Text>
      </TouchableOpacity>
    );
  }
}
export class HomeScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      token: null
    };
  }

  checkLogin = async () => {
    let token = await AsyncStorage.getItem("token");
    this.setState({ token }, () => {
      if (!!token) {
        this.props.getBank();
        this.props.getBankSelect();
      }
    });
  };

  _onRefresh = async () => {
    this.props.getHome();
  };

  componentDidMount() {
    const { homeState, advertisementState } = this.props;
    this.checkLogin();
    this._onRefresh();
    reactotron.log('pro' + homeState.listProductHome)
  }

  _renderImageSlider() {
    const { homeState } = this.props;
    return (
      <View>
        <SwiperFlatList
          autoplay
          autoplayDelay={5}
          autoplayLoop
          data={homeState.listBanner || []}
          keyExtractor={(item, index) => index.toString()}
          renderItem={({ item, index }) => {
            return (
              <TouchableOpacity
                onPress={() => {
                  NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_UTILITY, {
                    newsID: item.newsID
                  });
                }}
              >
                <FstImage
                  source={{ uri: item.urlImage }}
                  style={styles._viewSlider}
                  resizeMode="cover"
                />
              </TouchableOpacity>
            );
          }}
        />
      </View>
    );
  }

  _renderBody() {
    const { homeState, advertisementState, userState } = this.props;
    if (homeState.isLoading || advertisementState.isLoading) return <Loading />;
    if (homeState.error || advertisementState.error)
      return <Error onPress={() => this._onRefresh()} />;
      reactotron.log('homeState',homeState)
    return (
      <Block
        style={{
          backgroundColor: theme.colors.backgroundColor
        }}
      >
        {!!homeState.listBanner.length && this._renderImageSlider()}

        <View style={{ flexDirection: "row", marginVertical: 10 }}>
          <Option
            onPress={() => {
              if (!!!this.state.token) {
                requireLogin();
                return;
              }
              NavigationUtil.navigate(SCREEN_ROUTER.CART);
            }}
            text={R.strings().buy}
            img={R.images.img_cart}
          />
          <Option
            onPress={() => {
              if (!!!this.state.token) {
                requireLogin();
                return;
              }
              NavigationUtil.navigate(SCREEN_ROUTER.LOAD_POINTS);
            }}
            text={R.strings().load_point}
            img={R.images.img_draw_points}
          />
          <Option
            text={R.strings().draw_points}
            img={R.images.img_recharge}
            onPress={() => {
              if (!!!this.state.token) {
                requireLogin();
                return;
              }
              if (userState.status !== USER_ACTIVATED) {
                Toast.show(
                  "Tài khoản chưa được kích hoạt, vui lòng nạp 300 điểm để được kích hoạt và sử dụng điểm",
                  BACKGROUND_TOAST.FAIL
                );
                return;
              }
              NavigationUtil.navigate(SCREEN_ROUTER.DRAW_POINTS);
            }}
          />
        </View>
        <View style={{ backgroundColor: theme.colors.white }}>
          {!!homeState.product.length && (
            <View style={styles.titleView}>
              <Text
                style={[
                  theme.fonts.semibold18,
                  { color: theme.colors.black_title, marginBottom: 5 }
                ]}
              >
                {R.strings().rank_product}
              </Text>
              <TouchableOpacity
                onPress={() => {
                  NavigationUtil.navigate(SCREEN_ROUTER.UTILITY, {
                    initial_page: 1
                  });
                }}
              >
                <Text style={styles.all_title}>{R.strings().see_more}</Text>
              </TouchableOpacity>
            </View>
          )}
          <ScrollView
            contentContainerStyle={{ paddingLeft: 15 }}
            horizontal
            showsHorizontalScrollIndicator={false}
          >
            {homeState.product.map((item, index) => {
              return <Product item={item} index={index} key={index} />;
            })}
          </ScrollView>
        </View>
        <View style={{ backgroundColor: theme.colors.white }}>
          {!!homeState.news.length && (
            <View style={styles.titleView}>
              <Text
                style={[
                  theme.fonts.semibold18,
                  { color: theme.colors.black_title, marginBottom: 5 }
                ]}
              >
                {I18n.t("new_event")}
              </Text>
              <TouchableOpacity
                onPress={() => {
                  NavigationUtil.navigate(SCREEN_ROUTER.UTILITY);
                }}
              >
                <Text style={styles.all_title}>{R.strings().see_more}</Text>
              </TouchableOpacity>
            </View>
          )}
          <ScrollView
            contentContainerStyle={{ paddingLeft: 15 }}
            horizontal
            showsHorizontalScrollIndicator={false}
          >
            {homeState.news.map((item, index) => {
              return <News item={item} index={index} key={index} />;
            })}
          </ScrollView>
        </View>

      </Block>
    );
  }

  render() {
    const { homeState, advertisementState } = this.props;
    const news = 0;

    return (
      <Block>
        <ImageBackground
          style={[
            {
              flex: 1
            },
            Platform.OS !== "android" && { paddingTop: 20 }
          ]}
          resizeMode="stretch"
          source={require("../assets/images/img_bg_home.png")}
        >
          <ScrollView
            showsVerticalScrollIndicator={false}
            contentContainerStyle={[
              {
                justifyContent: "center"
              }
            ]}
            refreshControl={
              <RefreshControl
                refreshing={this.props.homeState.refreshing}
                onRefresh={() => this._onRefresh()}
              />
            }
          >
            <View style={styles._viewHello}>
              {!!!this.state.token ? (
                <Block>
                  <Text style={[theme.fonts.semibold17, { color: "white" }]}>
                    Chào bạn
                  </Text>
                  <Text
                    style={[
                      theme.fonts.semibold17,
                      { color: "white", marginVertical: 5 }
                    ]}
                  >
                    Đăng nhập để có trải nghiệm tốt hơn
                  </Text>
                  <PrimaryButton
                    onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.LOGIN)}
                    background={theme.colors.orange}
                    style={{
                      borderRadius: 10,
                      alignSelf: "flex-start",
                      width: "35%",
                      height: 35
                    }}
                    title="Đăng nhập"
                  />
                </Block>
              ) : (
                <>
                  <Block row center>
                    <Text
                      style={[
                        theme.fonts.semibold17,
                        { color: theme.colors.headerTextColor, marginTop: 4 }
                      ]}
                    >
                      {I18n.t("hello")}
                      {", "}
                      {!ObjectUtil.isEmpty(homeState.userInfo) &&
                        homeState.userInfo.customerName}
                    </Text>
                  </Block>
                  <TouchableOpacity
                    style={{ padding: 5 }}
                    onPress={() =>
                      NavigationUtil.navigate(SCREEN_ROUTER.NOTIFY)
                    }
                  >
                    <FstImage
                      source={R.images.ic_bell}
                      style={{ width: 20, height: 30, marginLeft: 10 }}
                      resizeMode="contain"
                    />
                  </TouchableOpacity>
                </>
              )}
            </View>
            {this._renderBody()}
          </ScrollView>
        </ImageBackground>
      </Block>
    );
  }
}
const mapStateToProps = state => ({
  homeState: state.homeReducer,
  advertisementState: state.utilityReducer.advertisement,
  userState: state[REDUCER.USER].data
});

const mapDispatchToProps = {
  getHome,
  getUtility,
  getBank,
  getBankSelect
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(HomeScreen);

const styles = StyleSheet.create({
  _viewHello: {
    flexDirection: "row",
    marginTop: 15,
    paddingHorizontal: 15,
    alignItems: "center"
    // backgroundColor: "red",
    // flex:1
  },
  _viewPoint: {
    marginVertical: 10,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center"
  },
  _viewOption: {
    marginHorizontal: 15,
    marginTop: 10,
    backgroundColor: theme.colors.white,
    borderRadius: 2,
    shadowOffset: {
      width: 0,
      height: 1
    },
    shadowOpacity: 0.2,
    shadowRadius: 2,
    elevation: 3,
    paddingHorizontal: 10
  },
  _viewProduct: {
    width: width * 0.4,
    backgroundColor: theme.colors.white,
    marginRight: 10,
    borderRadius: 5,
    // overflow: 'hidden',
    shadowColor: "#000",
    marginBottom: 20,
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.23,
    shadowRadius: 2.62,
    elevation: 4
  },
  titleView: {
    paddingHorizontal: 15,
    flexDirection: "row",
    justifyContent: "space-between",
    marginTop: 10,
    alignItems: "baseline"
  },
  all_title: {
    color: theme.colors.red_more,
    ...theme.fonts.regular13
  },
  _viewSlider: {
    marginVertical: 10,
    width: width * 0.95,
    marginHorizontal: theme.dimension.width * 0.025,
    aspectRatio: 2.5,
    borderRadius: 3,
    overflow: "hidden"
  },

  _button: {
    height: 40,
    backgroundColor: theme.colors.red,
    alignItems: "center",
    // justifyContent: "center",
    flexDirection: "row",
    borderRadius: 25,
    marginHorizontal: 40
  },
  _icPhone: {
    height: 60,
    width: 60,
    position: "absolute",
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 40,
    borderColor: "red",
    borderWidth: 1,
    backgroundColor: theme.colors.white,
    marginLeft: 40
  }
});
