import R from "@app/assets/R";
import {
  Block,
  DCHeader,
  FstImage,
  LoadingProgress,
  NumberFormat
} from "@app/components";
import { GET_HISTORY_POINT_TYPE, REDUCER } from "@app/constants/Constant";
import theme, { colors } from "@app/constants/Theme";
import React, { Component } from "react";
import {
  View,
  Text,
  ImageBackground,
  SafeAreaView,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  TouchableOpacity,
  TextInput,
  StyleSheet,
  Linking
} from "react-native";
import { connect } from "react-redux";
import Modal from "react-native-modal";
import NumberFormatTextInput from "react-number-format";
import { showConfirm, showMessages } from "@app/utils/Alert";
import { requestChargeMoneyToPoint } from "@app/constants/Api";
import reactotron from "reactotron-react-native";
import { getWalletPoints } from "@app/redux/actions";
import NavigationUtil from "@app/navigation/NavigationUtil";
export class LoadPointsScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isVisiable: false,
      point: "",
      error: null,
      isLoading: false
    };
  }
  componentDidMount() {
    Linking.getInitialURL()
      .then(url => {
        if (url) {
          console.log("getInitialURL: ", url);
        }
      })
      .catch(err => {});
    Linking.addEventListener("url", event => {
      this.handleNavigate(event.url);
    });
  }
  componentWillUnmount() {
    Linking.removeAllListeners("url");
  }

  handleNavigate = url => {
    const SUCCESS = "success";
    const FAILED = "failed";
    if (url.includes(SUCCESS)) {
      const payload = {
        page: 1,
        type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
        typePoint: 0
      };
      this.props.getWalletPoints(payload);
      showMessages(R.strings().notification, "Nạp điểm thành công!", () => {
        NavigationUtil.goBack();
      });
      return;
    }
    if (url.includes(FAILED)) {
      showMessages(
        R.strings().notification,
        "Nạp điểm thất bại! Xin vui lòng thử lại.",
        () => {
          console.log("FAIL!");
        }
      );
      return;
    }
  };
  requestRechargePoint = async point => {
    this.setState({ error: null, isLoading: true });
    try {
      const res = await requestChargeMoneyToPoint(
        parseFloat(point.replace(/,/g, ""))
      );
      if (res) {
        this.setState(
          { isLoading: false, point: "", isVisiable: false },
          () => {
            setTimeout(() => {
              Linking.openURL(res.data);
            }, 100);
            // Linking.addEventListener(res.data, event => {
            //   console.log("event", event);
            // });
            // reactotron.log("res", res);
          }
        );
      }
    } catch (error) {
    } finally {
      this.setState({ isLoading: false });
    }
  };
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
          paddingHorizontal: "5%",
          backgroundColor: "white"
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

  renderInfoLoadPoint = () => {
    return (
      <Block style={{ paddingHorizontal: "2.5%", backgroundColor: "white" }}>
        <Text
          style={{
            color: theme.colors.black_title,
            marginVertical: 10,
            ...theme.fonts.regular18
          }}
        >
          Chuyển điểm vào tài khoản
        </Text>

        {/* {this.renderBank('0691 000 405 546', 'Công ty TNHH Quốc tế Trà Tiên Thảo', 'Chi nhánh Tây Hà Nội')} */}
        {this.renderBank(
          "0451 000 380 385",
          "Nguyễn Thị Thúy",
          "Chi nhánh Thành Công"
        )}

        <Text
          style={{
            color: theme.colors.black_title,
            marginVertical: 10,
            ...theme.fonts.regular18
          }}
        >
          Nội dung chuyển - nhập cú pháp:
        </Text>

        <View
          style={{
            borderRadius: 5,
            padding: width * 0.025,
            ...theme.styles.border
          }}
        >
          <Text
            style={{
              textAlign: "center",
              marginVertical: 5,
              color: theme.colors.black_title,
              ...theme.fonts.regular18
            }}
          >
            Nạp điểm - Tên tài khoản - Số điện thoại
          </Text>
        </View>

        <Text
          style={{
            color: theme.colors.black_title,
            marginVertical: 10,
            ...theme.fonts.regular16
          }}
        >
          (Vui lòng nhập cú pháp trên khi chuyển khoản)
        </Text>
        <Text
          style={{
            color: theme.colors.black_title,
            marginBottom: 10,
            ...theme.fonts.regular16
          }}
        >
          VD: Nạp điểm - Nguyễn Minh Quang - 0793652678
        </Text>
      </Block>
    );
  };

  renderBank = (code, name, bank) => {
    return (
      <View
        style={{
          borderRadius: 5,
          padding: "2.5%",
          marginBottom: 10,
          ...theme.styles.border
        }}
      >
        <FstImage
          source={R.images.logo_vietcombank}
          style={{
            width: width * 0.3,
            aspectRatio: 3,
            marginBottom: 10
          }}
          resizeMode="contain"
        />
        <Text
          style={{
            color: theme.colors.black_title,
            ...theme.fonts.regular20
          }}
        >
          STK: {code}
        </Text>
        <Text
          style={{
            marginVertical: 5,
            color: theme.colors.black_title,
            ...theme.fonts.regular18
          }}
        >
          Tên: {name}
        </Text>
        <Text
          style={{
            color: theme.colors.black_title,
            ...theme.fonts.regular18
          }}
        >
          Chi nhánh: {bank}
        </Text>
      </View>
    );
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
  renderButtonModal = () => {
    return (
      <View
        style={{
          flexDirection: "row",
          marginTop: 20,
          justifyContent: "space-around",
          width: "100%"
        }}
      >
        {this.renderButton("Huỷ", () => {
          this.setState({
            isVisiable: !this.state.isVisiable
          });
        })}
        {this.renderButton("Xác nhận", () => {
          if (!this.state.point) {
            showMessages(R.strings().notification, "Vui lòng nhấp số điểm!");
            return;
          }
          showConfirm(
            R.strings().notification,
            "Bạn có chắc chắn muốn nạp số điểm này không?",
            () => {
              this.requestRechargePoint(this.state.point);
            },
            "Đồng ý"
          );
        })}
      </View>
    );
  };
  renderModal = () => {
    return (
      <Modal
        isVisible={this.state.isVisiable}
        onBackdropPress={() =>
          this.setState({ isVisiable: !this.state.isVisiable })
        }
      >
        <View style={styles.modalView}>
          <Text style={styles.modalText} children={"Nạp điểm"} />
          <NumberFormatTextInput
            value={this.state.point}
            displayType={"text"}
            thousandSeparator={true}
            renderText={input => (
              <TextInput
                style={styles.input}
                keyboardType="number-pad"
                thousandSeparator={true}
                value={input}
                placeholder="Nhập số điểm"
                onChangeText={value => {
                  this.setState({
                    ...this.state,
                    point: value
                  });
                }}
              />
            )}
          />
          {this.renderButtonModal()}
        </View>
        {this.state.isLoading && <LoadingProgress />}
      </Modal>
    );
  };
  render() {
    return (
      <Block>
        <DCHeader isWhiteBackground title={R.strings().load_point} />
        <SafeAreaView style={theme.styles.container}>
          <ScrollView>
            {this.renderImagePoint()}
            {this.renderInfoLoadPoint()}
            <View
              style={{
                backgroundColor: "white",
                paddingHorizontal: "2.5%",
                marginTop: 10,
                paddingVertical: 10
              }}
            >
              <Text
                style={{
                  marginBottom: 10,
                  color: theme.colors.black_title,
                  ...theme.fonts.regular16
                }}
              >
                Khi nạp điểm vui lòng chọn chuyển ngay lập tức để yêu cầu nạp
                điểm của bạn được xác nhận sớm nhất
              </Text>
              <Text
                style={{
                  color: theme.colors.black_title,
                  ...theme.fonts.regular16
                }}
              >
                Để nạp điểm vui lòng chuyển khoản đến số tài khoản bên trên và
                chờ từ 1-3 ngày làm việc để được cộng điểm
              </Text>
            </View>
            <TouchableOpacity
              style={{
                backgroundColor: colors.active,
                alignSelf: "center",
                marginTop: 10,
                borderRadius: 5,
                marginBottom: 20
              }}
              onPress={() => {
                this.setState({
                  isVisiable: !this.state.isVisiable
                });
              }}
              children={
                <Text
                  style={{
                    marginVertical: 8,
                    marginHorizontal: 25,
                    color: "white"
                  }}
                  children={"Nạp điểm"}
                />
              }
            />
            {this.renderModal()}
          </ScrollView>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  userState: state[REDUCER.USER].data
});

const mapDispatchToProps = {
  getWalletPoints
};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(LoadPointsScreen);

const styles = StyleSheet.create({
  input: {
    width: "100%",
    height: 42,
    borderRadius: 5,
    borderWidth: 1,
    paddingLeft: 8,
    borderColor: "#DDD"
  },
  modalText: {
    marginBottom: 15,
    textAlign: "center",
    ...theme.fonts.regular18
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
  }
});
