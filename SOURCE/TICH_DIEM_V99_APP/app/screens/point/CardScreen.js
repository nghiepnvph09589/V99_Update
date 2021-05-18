import React, { Component } from "react";
import NumberFormatTextInput from "react-number-format";
import {
  View,
  Text,
  ImageBackground,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Button,
  RefreshControl,
  StyleSheet,
  Keyboard
} from "react-native";
import { connect } from "react-redux";
import {
  getListGift,
  exchangeGift,
  getUserInfo,
  getWalletAccumulate,
  getListPointV
} from "../../redux/actions";
import {
  GET_HISTORY_POINT_TYPE,
  GIFT_TYPE,
  REDUCER,
  SCREEN_ROUTER
} from "../../constants/Constant";
import {
  Block,
  NumberFormat,
  Loading,
  PrimaryButton,
  LoadingProgress,
  Error,
  Empty,
  FstImage
} from "../../components";
import reactotron from "reactotron-react-native";
import * as theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import { showConfirm, showMessages } from "../../utils/Alert";
import ObjectUtil from "../../utils/ObjectUtil";
import NavigationUtil from "../../navigation/NavigationUtil";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import { requireLogin } from "../../utils/AlertRequireLogin";
import AsyncStorage from "@react-native-community/async-storage";
import R from "@app/assets/R";
import ScrollableTabView, {
  DefaultTabBar,
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
import PolicyScreen from "./PolicyScreen";
import WalletAccumulatePointsScreen from "./WalletAccumulatePointsScreen";
import Modal from "react-native-modal";
import callAPI from "@app/utils/CallApiHelper";
import { requestPointToV } from "@app/constants/Api";

const bottomButton = 25;
const padding_horizontal = 10;
export class CardScreen extends Component {
  constructor(props) {
    const { cardState } = props;
    super(props);
    this.state = {
      indexCarrierSelected: 0,
      indexPriceSeleted: null,
      priceSelected: null,
      token: null
    };
  }

  componentDidMount() {}

  renderViewPoint = () => {
    const { userState } = this.props;
    return (
      <FstImage
        style={{ width, height: width * 0.4 }}
        source={R.images.img_decor}
        resizeMode="contain"
      >
        <Block center middle>
          <Text
            style={{
              color: theme.colors.active,
              marginBottom: 5,
              ...theme.fonts.semibold16
            }}
          >
            Điểm của bạn
          </Text>
          <NumberFormat
            fonts={theme.fonts.semibold25}
            color={theme.colors.active}
            value={userState.pointRanking}
            perfix={R.strings().point}
          />
        </Block>
      </FstImage>
    );
  };

  renderScrollableTabView = () => {
    return (
      <Block>
        <ScrollableTabView
          style={{
            borderColor: theme.colors.border
          }}
          tabBarBackgroundColor={theme.colors.white}
          tabBarPosition="top"
          tabBarActiveTextColor={theme.colors.primary}
          tabBarInactiveTextColor={theme.colors.black1}
          tabBarUnderlineStyle={{
            height: 2,
            backgroundColor: theme.colors.primary
          }}
          renderTabBar={() => (
            <DefaultTabBar
              style={{
                alignSelf: "center",
                paddingTop: 8
              }}
            />
          )}
          tabBarTextStyle={theme.fonts.semibold18}
          onChangeTab={Keyboard.dismiss}
        >
          <WalletAccumulatePointsScreen tabLabel={"Lịch sử"} key={1} />
          <PolicyScreen tabLabel={"Chính sách"} key={0} />
        </ScrollableTabView>
      </Block>
    );
  };
  callBackGetData = () => {
    const payload = {
      page: 1,
      type: GET_HISTORY_POINT_TYPE.WALLET_ACCUMULATE_POINTS,
      typePoint: ""
    };
    const payloadPointV = {
      page: 1,
      type: GET_HISTORY_POINT_TYPE.LIST_POINT_V,
      typePoint: ""
    };
    this.props.getListPointV(payloadPointV);
    this.props.getUserInfo();
    this.props.getWalletAccumulate(payload);
  };
  renderViewOptions = () => {
    const { userState } = this.props;
    return (
      <View
        style={{
          padding: "2%",
          paddingLeft: 240,
          marginHorizontal: "4%",
          marginTop: -50
        }}
      >
        <Option
          text={R.strings().moving_point}
          callBack={this.callBackGetData}
        />
      </View>
    );
  };

  render() {
    return (
      <Block color={theme.colors.primary_background}>
        {this.renderViewPoint()}
        {this.renderViewOptions()}
        {this.renderScrollableTabView()}
      </Block>
    );
  }
}
class Option extends Component {
  onChangeText = text => {
    this.setState({ point: text, inputvalue: text });
  };

  state = {
    modalVisible: false,
    isLoading: false,
    error: null,
    inputvalue: ""
  };

  setModalVisible = visible => {
    this.setState({ modalVisible: visible });
  };
  postDataVtoPoint = async point => {
    this.setState({ error: null, isLoading: true });
    try {
      const res = await requestPointToV(parseFloat(point.replace(/,/g, "")));
      await this.props.callBack();
      if (res) {
        this.setState({ isLoading: false, point: "", inputvalue: "" }, () => {
          showMessages(
            R.strings().notification,
            "Chuyển điểm thành công!",
            () => {
              this.setState({ modalVisible: false });
            }
          );
        });
      }
    } catch (error) {
    } finally {
      this.setState({ isLoading: false });
    }
  };
  renderButton = (title, onPress) => {
    return (
      <TouchableOpacity
        onPress={onPress}
        style={{
          backgroundColor: theme.colors.active,
          width: "40%",
          justifyContent: "center",
          alignItems: "center",
          borderRadius: 5
        }}
      >
        <Text
          style={{
            marginVertical: 8,
            color: "white",
            ...theme.fonts.robotoMedium14
          }}
          children={title}
        />
      </TouchableOpacity>
    );
  };
  render() {
    const { text } = this.props;
    const { point, isLoading, modalVisible } = this.state;
    return (
      <View style={styles.centeredView}>
        <Modal
          isVisible={modalVisible}
          onBackdropPress={() => {
            this.setModalVisible(!modalVisible);
          }}
        >
          <View style={styles.modalView}>
            <Text style={styles.modalText}>Chuyển từ sang Ví V</Text>
            <NumberFormatTextInput
              value={this.state.inputvalue}
              displayType={"text"}
              thousandSeparator={true}
              renderText={inputvalue => (
                <TextInput
                  style={styles.input}
                  keyboardType="number-pad"
                  thousandSeparator={true}
                  value={inputvalue}
                  placeholder="Nhập số điểm"
                  onChangeText={text => this.onChangeText(text)}
                />
              )}
            />
            {/* <TextInput
              style={styles.input}
              keyboardType="number-pad"
              value={point}
              placeholder="Nhập số điểm"
              onChangeText={this.onChangeText}
            /> */}
            <View
              style={{
                flexDirection: "row",
                marginTop: 20,
                justifyContent: "space-around",
                width: "100%"
              }}
            >
              {this.renderButton("Huỷ", () => {
                this.setModalVisible(!modalVisible);
              })}
              {this.renderButton("Xác nhận", () => {
                if (!point) {
                  showMessages(
                    R.strings().notification,
                    "Vui lòng nhấp số điểm!"
                  );
                  return;
                }
                this.postDataVtoPoint(point);
              })}
            </View>
          </View>
          {isLoading && <LoadingProgress />}
        </Modal>

        <TouchableOpacity
          style={{
            alignItems: "center",
            justifyContent: "center",
            backgroundColor: "#00bfff",
            height: 35,
            borderRadius: 8
          }}
          underlayColor="tomato"
          onPress={() => this.setModalVisible(true)}
        >
          <Text style={{ color: "white" }}>{text}</Text>
        </TouchableOpacity>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  title: {
    marginVertical: 10,
    marginLeft: 5,
    ...theme.fonts.robotoMedium16
  },

  input: {
    width: "100%",
    height: 42,
    borderRadius: 5,
    borderWidth: 1,
    paddingLeft: 8,
    borderColor: "#DDD"
  },
  centeredView: {
    // flex: 1,
    // justifyContent: "center",
    // alignItems: "center",
    // // marginTop: 22
  },
  modalStyle: {
    justifyContent: "center",
    borderRadius: Platform.OS === "ios" ? 30 : 0,
    shadowRadius: 10,
    height: 280
    // backgroundColor: '#898989'
  },
  modalView: {
    backgroundColor: "white",
    borderRadius: 10,
    padding: 20,
    alignItems: "center",
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5
  },
  textStyle: {
    color: "white",
    fontWeight: "bold",
    textAlign: "center"
  },
  modalText: {
    marginBottom: 15,
    textAlign: "center",
    ...theme.fonts.regular18
  }
});

const mapStateToProps = state => ({
  userState: state[REDUCER.USER].data
});

const mapDispatchToProps = {
  getUserInfo,
  getWalletAccumulate,
  getListPointV
};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CardScreen);
