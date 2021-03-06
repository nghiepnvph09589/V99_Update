import React, { Component, useState } from "react";
import {
  Button,
  Pressable,
  TextInput,
  View,
  Text,
  FlatList,
  ImageBackground,
  Dimensions,
  //  Modal,
  ScrollView,
  TouchableOpacity,
  RefreshControl,
  StyleSheet,
  Keyboard
} from "react-native";
import { connect } from "react-redux";
import {
  getListGift,
  exchangeGift,
  getUserInfo,
  getWalletAccumulate
} from "../../redux/actions";
import {
  GIFT_TYPE,
  REDUCER,
  GET_HISTORY_POINT_TYPE,
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
import Modal from "react-native-modal";
import WalletPointsItem from "@app/components/WalleVItem";
import { getListPointV } from "../../redux/actions";
import callAPI, { callAPIHook } from "@app/utils/CallApiHelper";
import { requestVtoPoint } from "@app/constants/Api";
import NumberFormatTextInput from "react-number-format";

const EXCHANGE_RATE = 0.01;

export class PointVScreen extends Component {
  componentDidMount() {
    this.getData();
    // this.postDataVtoPoint()
  }

  getData = () => {
    const payload = {
      page: 1,
      type: GET_HISTORY_POINT_TYPE.LIST_POINT_V,
      typePoint: ""
    };
    const payloadPoint = {
      page: 1,
      type: GET_HISTORY_POINT_TYPE.WALLET_ACCUMULATE_POINTS,
      typePoint: ""
    };
    this.props.getListPointV(payload);
    this.props.getWalletAccumulate(payloadPoint);
  };

  renderViewPoint = () => {
    const { userState } = this.props;
    return (
      <FstImage
        style={{ width, height: width * 0.4 }}
        source={R.images.img_decor}
        resizeMode="contain"
      >
        <Block center middle>
          {/* <Text
            style={{
              color: theme.colors.active,
              marginBottom: 5,
              ...theme.fonts.semibold16
            }}
          >
            ??i???m c???a b???n
          </Text> */}
          <NumberFormat
            fonts={theme.fonts.semibold25}
            color={theme.colors.active}
            value={userState.pointV}
            perfix={R.strings().point_V}
          />
          <Text
            style={{
              color: theme.colors.black,
              marginBottom: 5,
              ...theme.fonts.regular12
            }}
          >
            ~
            {(userState.pointV * EXCHANGE_RATE)
              .toString()
              .replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,")}{" "}
            USD
          </Text>
        </Block>
      </FstImage>
    );
  };
  callBackGetData = () => {
    this.props.getUserInfo();
    this.getData();
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
  renderFlastlist = () => {
    const { walletPointsState } = this.props;

    if (walletPointsState.isLoading) return <Loading />;
    if (walletPointsState.error) return <Error onPress={this.getData} />;
    if (!walletPointsState.data.listHistoriesPointMember)
      return <Empty onRefresh={this.getData} />;
    return (
      <FlatList
        refreshControl={
          <RefreshControl refreshing={false} onRefresh={this.getData} />
        }
        style={{ backgroundColor: "white", marginTop: 15 }}
        data={walletPointsState.data.listHistoriesPointMember}
        keyExtractor={(item, index) => index.toString()}
        renderItem={this.renderItem}
      />
    );
  };

  renderItem = ({ item, index }) => {
    return <WalletPointsItem item={item} index={index} />;
  };

  render() {
    return (
      <Block color={theme.colors.primary_background}>
        {this.renderViewPoint()}
        {this.renderViewOptions()}
        {this.renderFlastlist()}
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
      const res = await requestVtoPoint(parseFloat(point.replace(/,/g, "")));
      console.log(parseFloat(point.replace(/,/g, "")));
      await this.props.callBack();
      if (res) {
        this.setState({ isLoading: false, point: "", inputvalue: "" }, () => {
          showMessages(
            R.strings().notification,
            "Chuy???n ??i???m th??nh c??ng!",
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
    const { point, modalVisible, isLoading } = this.state;
    return (
      <View style={styles.centeredView}>
        <Modal
          isVisible={modalVisible}
          onBackdropPress={() => {
            this.setModalVisible(!modalVisible);
          }}
        >
          <View style={styles.modalView}>
            <Text style={styles.modalText}>Chuy???n sang V?? t??ch ??i???m</Text>
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
                  placeholder="Nh???p s??? ??i???m"
                  onChangeText={text => this.onChangeText(text)}
                />
              )}
            />

            {/* <TextInput
              style={styles.input}
              keyboardType="number-pad"
              value={point}
              placeholder="Nh???p s??? ??i???m"
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
              {this.renderButton("Hu???", () => {
                this.setModalVisible(!modalVisible);
              })}
              {this.renderButton("X??c nh???n", () => {
                if (!point) {
                  showMessages(
                    R.strings().notification,
                    "Vui l??ng nh???p s??? ??i???m!"
                  );
                  return;
                }
                showConfirm(
                  R.strings().notification,
                  `B???n c?? ch???c ch???n mu???n chuy???n ${point} V kh??ng?`,
                  () => {
                    this.postDataVtoPoint(point);
                  },
                  "?????ng ??"
                );
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
          onPress={() => this.setModalVisible(true)}
        >
          <Text style={{ color: "white" }}>{text}</Text>
        </TouchableOpacity>
      </View>
    );
  }
}

const styles = StyleSheet.create({
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
  modalStyle: {
    justifyContent: "center",
    borderRadius: Platform.OS === "ios" ? 30 : 0,
    shadowRadius: 10,
    height: 280
    // backgroundColor: '#898989'
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
  userState: state[REDUCER.USER].data,
  walletPointsState: state[REDUCER.GET_LIST_POINT_V]
});

const mapDispatchToProps = {
  getListPointV,
  getUserInfo,
  getWalletAccumulate
};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(PointVScreen);
