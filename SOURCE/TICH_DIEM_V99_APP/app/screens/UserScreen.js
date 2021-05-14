import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  Image,
  SafeAreaView,
  ScrollView,
  StyleSheet,
  RefreshControl,
  TextInput,
  ActivityIndicator,
  Alert
} from "react-native";
import { connect } from "react-redux";
import theme, * as Theme from "../constants/Theme";
import I18n from "../i18n/i18n";
import { getUserInfo } from "../redux/actions";
import reactotron from "reactotron-react-native";
import {
  Loading,
  Block,
  DCHeader,
  Error,
  FastImage,
  NumberFormat,
  LoadingProgress,
  requestLogin, FstImage
} from "../components";
import UserItem from "../components/UserItem";
import { Avatar } from "react-native-elements";
import StepIndicator from "react-native-step-indicator";
import NavigationUtil from "../navigation/NavigationUtil";
import { SCREEN_ROUTER, REDUCER, AGENT, USER_ACTIVATED } from "../constants/Constant";
import DialogInput from "react-native-dialog-input";
import { requestLogout } from "../constants/Api";
import { showConfirm, showMessages } from "../utils/Alert";
import AsyncStorage from "@react-native-community/async-storage";
import { activeAgent } from "../constants/Api";
import Modal from "react-native-modal";
import Toast, { BACKGROUND_TOAST } from "../utils/Toast";
import RequireLogin from "../components/RequireLogin";
import codePush from "react-native-code-push";
import R from "@app/assets/R";
import LinkingUtils, { LINKING_TYPE } from "@app/utils/LinkingUtils";

const HOTLINE = '0977.380.806'
export class UserScreen extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isModalVisible: false,
      code: "",
      isLoading: false,
      update: false,
    };
  }
  componentDidMount = async () => {
    this.props.getUserInfo();
  };

  _checkUpdate() {
    this.setState(
      {
        ...this.state,
        update: true
      },
      () => {
        codePush
          .checkForUpdate()
          .then(update => {
            reactotron.log(update);
            this.setState({
              ...this.state,
              update: false
            });
            if (!update) {
              setTimeout(() => {
                Alert.alert("Thông báo", "Bạn đang dùng phiên bản mới nhất", [
                  { text: "Đóng" }
                ]);
              }, 500);
            } else {
              codePush.notifyAppReady();
              codePush.sync(
                {
                  updateDialog: null,
                  installMode: codePush.InstallMode.IMMEDIATE
                },
                status => {
                  reactotron.log(status);
                  if (
                    status == codePush.SyncStatus.DOWNLOADING_PACKAGE ||
                    status == codePush.SyncStatus.CHECKING_FOR_UPDATE ||
                    status == codePush.SyncStatus.SYNC_IN_PROGRESS ||
                    status == codePush.SyncStatus.INSTALLING_UPDATE
                  ) {
                    this.setState({
                      ...this.state,
                      update: true
                    });
                  } else {
                    this.setState({
                      ...this.state,
                      update: false
                    });
                  }
                },
                progress => {
                  reactotron.log(progress);
                }
              );
            }
          })
          .catch(err => {
            this.setState(
              { update: false },
              setTimeout(() => {
                Alert.alert("Thông báo", "Bạn đang dùng phiên bản mới nhất", [
                  { text: "Đóng" }
                ]);
              }, 500)
            );
          });
      }
    );
    codePush.notifyAppReady();
  }

  _logout = async () => {
    const { userInfoState } = this.props;
    try {
      const response = await requestLogout();
      if (response) {
        await AsyncStorage.setItem("token", "");
        userInfoState.data.typeLogin == 4
          ? await AsyncStorage.setItem("phone", userInfoState.data.phone)
          : await AsyncStorage.setItem("phone", "");
        NavigationUtil.navigate("AuthLoading");
      }
    } catch (error) {
      Toast.show("Vui lòng thử lại", BACKGROUND_TOAST.FAIL);
    }
  };


  renderViewPoint = () => {
    const { userInfoState } = this.props;

    return (<>
      {userInfoState.data.status !== USER_ACTIVATED ? <Text style={[Theme.fonts.regular16, {
        marginVertical: 20,
        textAlign: 'center',
        paddingHorizontal: '5%',
        color: theme.colors.red_money
      }]}>Tài khoản chưa được kích hoạt, vui lòng nạp 300 điểm để được kích hoạt</Text>
        : <View style={{
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'center',
          marginVertical: 10
        }}>
          <Block middle center style={{ borderRightWidth: 0.5, borderRightColor: theme.colors.border }}>
            <Text style={{
              color: theme.colors.active,
              ...theme.fonts.regular18
            }}>Point</Text>
            <NumberFormat fonts={theme.fonts.regular18} value={userInfoState.data.point}
              perfix={R.strings().point} />
          </Block>
          <Block middle center style={{ borderLeftWidth: 0.5, borderLeftColor: theme.colors.border }}>
            <Text style={{
              color: theme.colors.active,
              ...theme.fonts.regular18
            }}>Điểm tích lũy</Text>
            <NumberFormat fonts={theme.fonts.regular18} value={userInfoState.data.pointRanking}
              perfix={R.strings().point} />
          </Block>
          <Block middle center style={{ borderLeftWidth: 0.5, borderLeftColor: theme.colors.border }}>
            <Text style={{
              color: theme.colors.active,
              ...theme.fonts.regular18
            }}>Point V</Text>
            <NumberFormat fonts={theme.fonts.regular18} value={userInfoState.data.pointV}
              perfix={R.strings().point_V} />
          </Block>
        </View>}
    </>)
  }

  _renderBody() {
    const { userInfoState } = this.props;
    const { isLoading, update } = this.state;
    if (isLoading || update) return <LoadingProgress />;

    // console.log(userInfoState)
    if (userInfoState.isLoading) return <Loading />;
    if (userInfoState.error)
      return (
        <Error
          onPress={() => {
            this.props.getUserInfo();
          }}
        />
      );
    return (
      <ScrollView
        style={{ flex: 1 }}
        refreshControl={
          <RefreshControl
            refreshing={userInfoState.data.refreshing}
            onRefresh={() => this.props.getUserInfo()}
          />
        }
      >
        <View
          style={styles._viewUser}
        >
          <Avatar
            rounded
            source={{
              uri: userInfoState.data.urlAvatar
            }}
            size={65}
            renderPlaceholderContent={<ActivityIndicator />}
            placeholderStyle={{ backgroundColor: "white" }}
          />

          <Block style={{ flex: 1, marginLeft: 20 }}>
            <View style={{ flexDirection: "row", alignItems: "center" }}>
              <Text
                style={[Theme.fonts.regular18, { flex: 1 }]}
                numberOfLines={1}
              >
                {userInfoState.data.customerName}
              </Text>
            </View>

            <Text style={[Theme.fonts.regular16, { marginTop: 3 }]}>
              {userInfoState.data.phone}
            </Text>

          </Block>
        </View>

        {this.renderViewPoint()}

        <View
          style={{
            marginVertical: 5,
            backgroundColor: Theme.colors.white,
            paddingBottom: 15,
            marginBottom: 5
          }}
        >
          <UserItem
            text={'Thông tin cá nhân'}
            img={R.images.ic_feather_user}
            onPress={() => {
              NavigationUtil.navigate(SCREEN_ROUTER.UPDATE_USER);
            }}
          />
          {/* <UserItem
            text={'Chính sách tích điểm và đổi điểm'}
            img={R.images.ic_bookmark}
          /> */}
          <UserItem
            text={I18n.t("bill")}
            img={R.images.ic_notepad}
            onPress={() => {
              NavigationUtil.navigate(SCREEN_ROUTER.ORDER);
            }}
          />
          <UserItem
            text={R.strings().cart}
            img={R.images.ic_cart}
            onPress={() => {
              NavigationUtil.navigate(SCREEN_ROUTER.CART);
            }}
            tintColor={theme.colors.red_money}
          />
          <UserItem
            text={R.strings().introduce_customers}
            img={R.images.ic_loan}
            onPress={() => {
              NavigationUtil.navigate(SCREEN_ROUTER.INTRODUCE_CUSTOMERS);
            }}
          />
          <UserItem
            text={R.strings().change_pass}
            img={R.images.ic_change_pass}
            onPress={() => {
              NavigationUtil.navigate(SCREEN_ROUTER.CHANGE_PASS);
            }}
          />
          <UserItem
            text="Kiểm tra phiên bản"
            img={require("../assets/images/img_check_update.png")}
            onPress={() => this._checkUpdate()}
          />
          <UserItem
            text={I18n.t("logout")}
            img={require("../assets/images/ic_log_out.png")}
            onPress={() => {
              showConfirm(
                I18n.t("logout"),
                I18n.t("confirm_logout"),
                this._logout
              );
            }}
          />
        </View>
        {/* {this._renderPoint()} */}
        <TouchableOpacity
          onPress={() => {
            LinkingUtils(LINKING_TYPE.CALL, HOTLINE)
          }}
          style={{
            flexDirection: 'row',
            padding: 10,
            justifyContent: 'center',
            alignItems: 'center',
            backgroundColor: theme.colors.active,
            alignSelf: 'center',
            borderRadius: 5
          }}>
          <FstImage
            source={R.images.ic_constulation}
            style={{ width: 20, height: 20, marginRight: 5 }}
            resizeMode='contain' />
          <Text style={{ color: 'white', ...theme.fonts.regular18 }}>{HOTLINE}</Text>
        </TouchableOpacity>
      </ScrollView>
    );
  }

  render() {
    return (
      <Block>
        <SafeAreaView style={Theme.styles.container}>
          {this._renderBody()}
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  userInfoState: state[REDUCER.USER],
});

const mapDispatchToProps = {
  getUserInfo
};

export default connect(mapStateToProps, mapDispatchToProps)(UserScreen);
const styles = StyleSheet.create({
  _viewPoint: {
    width: "100%",
    alignItems: "center",
    backgroundColor: Theme.colors.white
  },
  _rankName: {
    width: "50%",
    alignItems: "center",
    justifyContent: "center",
    flexDirection: "row"
  },
  _textRankName: {
    color: Theme.colors.white,
    backgroundColor: Theme.colors.red,
    position: "absolute",
    paddingHorizontal: 10,
    paddingVertical: 2,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,

    elevation: 5
  },
  _viewUser: {
    alignItems: "center",
    flexDirection: "row",
    marginTop: 10,
    paddingHorizontal: 20,
    paddingVertical: 10,
    backgroundColor: Theme.colors.white,
    flex: 1
  },
  _viewConfirm: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingHorizontal: 45,
    width: "100%",
    marginTop: 40,
    paddingBottom: 20
  }
});
