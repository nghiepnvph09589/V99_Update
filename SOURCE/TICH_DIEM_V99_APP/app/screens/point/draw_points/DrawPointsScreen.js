import R from "@app/assets/R";
import {
  Block,
  DCHeader,
  FstImage,
  LoadingProgress,
  NumberFormat,
  TDButton
} from "@app/components";
import {
  GET_HISTORY_POINT_TYPE,
  REDUCER,
  SCREEN_ROUTER,
  USER_ACTIVATED
} from "@app/constants/Constant";
import theme from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import React, { Component } from "react";
import { ImageBackground, Alert } from "react-native";
import {
  View,
  Text,
  SafeAreaView,
  TouchableOpacity,
  KeyboardAvoidingView,
  Platform,
  TextInput,
  StyleSheet,
  ScrollView
} from "react-native";
import { connect } from "react-redux";
import { requestDrawPoint, requestDelBankAccount } from "@api";
import Toast, { BACKGROUND_TOAST } from "@app/utils/Toast";
import { updateUserLocal, getWalletPoints, getBank } from "@action";
import Modal from "react-native-modal";
import Icon from "react-native-vector-icons/FontAwesome";

export class DrawPointsScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      ID: -1,
      point: "",
      listBank: [],
      indexSelected: -1,
      isRequesting: false,
      isDelete: false
    };
  }

  renderImagePoint = () => {
    const { userState } = this.props;

    return (
      <ImageBackground
        source={R.images.img_bg_action_points}
        style={{
          width: width * 0.95,
          aspectRatio: 4,
          backgroundColor: "red",
          borderRadius: 5,
          alignSelf: "center",
          flexDirection: "row",
          alignItems: "center",
          paddingHorizontal: "5%"
        }}
        resizeMode="cover"
      >
        <FstImage
          source={R.images.ic_shape_star}
          style={{ height: "70%", aspectRatio: 1 }}
          resizeMode="contain"
        />
        <View style={{ paddingLeft: "5%" }}>
          <Text
            style={{
              ...theme.fonts.regular16,
              color: "white",
              marginBottom: 10
            }}
          >
            Điểm hiện có
          </Text>
          <NumberFormat
            value={userState.point}
            perfix={R.strings().point}
            fonts={theme.fonts.regular18}
            color={theme.colors.white}
          />
        </View>
      </ImageBackground>
    );
  };
  componentDidMount() {
    this.getData();
  }

  getData = async () => {
    this.props.getBank();
  };

  onChangeText = text => {
    this.setState({ point: text });
  };

  onSelectBank = (index, id) => {
    this.setState({ indexSelected: index, isDelete: true, ID: id });
  };

  renderBankSelect = () => {
    const { bankState } = this.props;
    const { itemSelected, indexSelected, ID, isDelete } = this.state;

    return bankState.map((item, index) => {
      const isSelected = indexSelected == index && ID == item.id;
      return (
        <TouchableOpacity
          onPress={() => {
            if (!isSelected) this.onSelectBank(index, item.id);
            // Toast.show(`Chủ tài khoản: ${item.userName} \n Số tài khoản: ${item.codeBankAccount}`,
            // BACKGROUND_TOAST.INFO)
            // this.setState({ID: item.id})
            // console.log(isSelected)
          }}
          key={index}
          style={{
            ...theme.styles.border,
            flexDirection: "row",
            alignItems: "center",
            paddingHorizontal: "2.5%",
            paddingVertical: 5,
            borderRadius: 5,
            marginBottom: 5
          }}
        >
          <Block row center>
            <FstImage
              source={{ uri: item.imageUrl }}
              style={{
                width: width * 0.1,
                aspectRatio: 1,
                borderRadius: width * 0.1
              }}
              resizeMode="stretch"
            />
            <Text
              style={{
                marginLeft: 10,
                color: theme.colors.black1,
                ...theme.styles.regular20,
                width: "85%"
              }}
              numberOfLines={1}
            >
              {item.shortName +
                " - ******" +
                item.codeBankAccount.substring(
                  item.codeBankAccount.length - 4,
                  item.codeBankAccount.length
                )}
            </Text>
          </Block>
          {isSelected && (
            <FstImage
              source={R.images.ic_tick_bank}
              style={{
                width: width * 0.05,
                aspectRatio: 1
              }}
              resizeMode="contain"
            />
          )}
        </TouchableOpacity>
      );
    });
  };

  renderViewAction = () => {
    const { point, isDelete } = this.state;
    const { bankState } = this.props;

    return (
      <View
        style={{
          marginVertical: 10,
          paddingHorizontal: "2.5%"
        }}
      >
        <View style={styles.viewTextInput}>
          <Text
            style={{
              ...theme.fonts.regular20,
              color: theme.colors.black1
            }}
          >
            Nhập số điểm muốn rút:
          </Text>
          <TextInput
            keyboardType="number-pad"
            style={styles.text_input}
            value={point}
            placeholder="0"
            onChangeText={this.onChangeText}
          />
        </View>
        {/* <View style={{
                    flexDirection: 'row',
                    alignItems: 'center',
                    paddingVertical: 10
                }}>
                    <Text style={{
                        flex: 1,
                        ...theme.fonts.regular20,
                        color: theme.colors.black1
                    }}>Số tiền nhận được: </Text>
                    <NumberFormat
                        value={'20000000'}
                        perfix='VNĐ'
                        color={theme.colors.black1}
                        fonts={theme.fonts.regular20}
                    />
                </View> */}

        <Text
          style={{
            flex: 1,
            ...theme.fonts.regular20,
            color: theme.colors.black1,
            marginBottom: 10
          }}
        >
          Chuyển điểm vào tài khoản:
        </Text>

        {this.renderBankSelect()}
        {isDelete && (
          <TDButton
            title={"Xóa tài khoản"}
            background={"#c0392b"}
            style={{ marginBottom: 5 }}
            titleColor={theme.colors.white}
            titleStyle={{ ...theme.fonts.regular20 }}
            onPress={this.alertDelBank}
          />
        )}
        <TDButton
          title={"Thêm tài khoản"}
          background={theme.colors.green}
          titleColor={theme.colors.white}
          titleStyle={{ ...theme.fonts.regular20 }}
          tintColour={theme.colors.white}
          image={R.images.ic_add}
          onPress={() => {
            NavigationUtil.navigate(SCREEN_ROUTER.ADD_BANK);
          }}
        />

        <Text
          style={{
            ...theme.fonts.regular16,
            color: theme.colors.black1,
            marginVertical: 10
          }}
        >
          Vui lòng chờ từ 1-3 ngày làm việc để yêu cầu rút điểm của bạn được xác
          nhận và số tiền nhận được đến tài khoản
        </Text>

        <TDButton
          style={{ marginTop: 50 }}
          title={R.strings().agree}
          titleColor={theme.colors.white}
          background={!isDelete && theme.colors.gray}
          titleStyle={{ ...theme.fonts.regular20 }}
          onPress={this.drawPointsPress}
          disabled={!isDelete ? true : false}
        />
      </View>
    );
  };

  alertDelBank = () => {
    const { bankState } = this.props;
    Alert.alert(
      "Thông báo",
      "Bạn chắc chắn muốn xóa tài khoản?",
      [
        {
          text: "Cancel",
          onPress: () => console.log("Cancel Pressed")
          // style: "cancel"
        },
        { text: "Đồng ý", onPress: this.delBankPress }
      ],
      { cancelable: false }
    );
  };

  delBankPress = async () => {
    const { ID } = this.state;
    const { bankState } = this.props;
    this.setState({ isRequesting: true });
    const payload = {
      ID: ID
      // indexSelected: -1,
    };
    try {
      const res = await requestDelBankAccount(payload);
      Toast.show("Xóa ngân hàng thành công");
      this.props.getBank();
      this.setState({ isDelete: false });
    } catch (error) {
      console.log(error);
    } finally {
      this.setState({ isRequesting: false });
    }
  };

  drawPointsPress = async () => {
    const { point, indexSelected } = this.state;
    const { bankState, userState } = this.props;

    if (!point.trim()) {
      Toast.show("Bạn chưa nhập điểm", BACKGROUND_TOAST.FAIL);
      return;
    }

    if (indexSelected == -1) {
      Toast.show("Bạn chưa chọn ngân hàng", BACKGROUND_TOAST.FAIL);
      return;
    }

    this.setState({ isRequesting: true });

    const payload = {
      point,
      bankID: bankState[indexSelected].id
    };

    try {
      const res = await requestDrawPoint(payload);
      Toast.show("Đã gửi yêu cầu rút điểm thành công");
      NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_DRAW_POINTS, {
        id: res.data.id
      });
      const payloadWalletPoint = {
        page: 1,
        type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
        typePoint: ""
      };
      this.props.getWalletPoints(payloadWalletPoint);
      this.props.updateUserLocal({ point: res.data.balance });
    } catch (error) {
      console.log(error);
    } finally {
      this.setState({ isRequesting: false });
    }
  };

  render() {
    const { userState } = this.props;
    const { isRequesting } = this.state;
    return (
      <Block>
        <DCHeader
          isWhiteBackground
          title={R.strings().draw_points}
          rightComponent={
            <TouchableOpacity
              onPress={() =>
                NavigationUtil.navigate(SCREEN_ROUTER.HISTORY_DRAW_POINTS)
              }
            >
              <FstImage
                source={R.images.ic_time_machine}
                style={{ width: 20, height: 20 }}
                resizeMode="contain"
              />
            </TouchableOpacity>
          }
        />
        <SafeAreaView style={theme.styles.container}>
          {isRequesting && <LoadingProgress />}
          <KeyboardAvoidingView
            behavior={Platform.OS === "ios" && "padding"}
            style={{
              flex: 1,
              backgroundColor: "white"
            }}
            enabled
            keyboardVerticalOffset={100}
          >
            <ScrollView>
              {this.renderImagePoint()}
              {this.renderViewAction()}
            </ScrollView>
          </KeyboardAvoidingView>
        </SafeAreaView>
      </Block>
    );
  }
}

const styles = StyleSheet.create({
  viewTextInput: {
    borderRadius: 5,
    borderWidth: 1,
    borderColor: theme.colors.border,
    // alignItems: 'center',
    padding: "2.5%"
  },
  text_input: {
    width: "100%",
    borderBottomWidth: 0.5,
    borderBottomColor: theme.colors.border,
    ...theme.fonts.regular25
  }
});

const mapStateToProps = state => ({
  userState: state[REDUCER.USER].data,
  bankState: state[REDUCER.BANK].data
});

const mapDispatchToProps = {
  updateUserLocal,
  getWalletPoints,
  getBank
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DrawPointsScreen);
