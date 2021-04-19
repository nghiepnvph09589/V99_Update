import AsyncStorage from "@react-native-community/async-storage";
import React, { Component } from "react";
import {
  Image,
  ImageBackground,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View
} from "react-native";
import { connect } from "react-redux";
import reactotron from "reactotron-react-native";
import Button from "../../components/Button";
import TextFieldInput from "../../components/TextFieldInput";
import { LOGIN_WITH, SCREEN_ROUTER, REDUCER } from "../../constants/Constant";
import theme, * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import NavigationUtil from "../../navigation/NavigationUtil";
import { getAddress, loginWithSocial } from "../../redux/actions";
import { showMessages } from "../../utils/Alert";
import {
  Loading, FastImage, LoadingProgress,
  Block, FstImage, TDTextInput
} from "../../components";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import { Icon } from "react-native-elements";
import OneSignal from "react-native-onesignal";
import R from "@app/assets/R";
import { requestCheckPhoneRegister } from '@api'

export class LoginScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      phone: "",
      password: "",
      deviceID: "",
      isLoading: false,
      isSelectLogin: true,
      email: ''
    };
  }

  componentDidMount() {
    this.getDevideID()
  }

  getDevideID = () => {
    OneSignal.getPermissionSubscriptionState(async status => {
      this.setState({
        deviceID: status.userId
      });
    });
  }

  renderViewSelect = () => {
    const { isSelectLogin } = this.state
    return (<View style={{
      flexDirection: 'row',
      marginLeft: '10%',
      marginBottom: 20
    }}>
      <TouchableOpacity
        onPress={() => this.setState({
          isSelectLogin: true,
        })}
        style={{
          ...styles.view_item_select,
          borderBottomColor: isSelectLogin && theme.colors.active || theme.colors.gray,
        }} activeOpacity={1}>
        <Text style={styles.text_item_select}>{R.strings().login}</Text>
      </TouchableOpacity>
      <TouchableOpacity
        onPress={() => this.setState({
          isSelectLogin: false, phone: ''
        })}
        style={{
          ...styles.view_item_select,
          borderBottomColor: !isSelectLogin && theme.colors.active || theme.colors.gray,
        }} activeOpacity={1}>
        <Text style={[styles.text_item_select, {
          marginLeft: 15,
        }]}>{R.strings().register}</Text>
      </TouchableOpacity>
    </View>)
  }

  renderFormLogin = () => {
    return (<>
      <TDTextInput
        fieldName='Số điện thoại'
        icon={R.images.ic_telephone}
        borderRadiusTop
        value={this.state.phone}
        keyboardType={"phone-pad"}
        onSubmitEditing={() => this.ref.edt_pass.focus()}
        maxLength={12}
        onChangeText={text => {
          this.setState({
            phone: text.trim()
          });
        }}
      />

      {/* <TDTextInput
        fieldName='Email'
        icon={R.images.ic_mail}
        value={this.state.email}
        borderRadiusTop
        keyboardType='email-address'
        onChangeText={text => {
          this.setState({
            email: text.trim()
          });
        }}
        autoCapitalize='none'
      /> */}

      <TDTextInput
        fieldName='Mật khẩu'
        icon={R.images.ic_feather_lock}
        borderRadiusBottom
        secureTextEntry={true}
        value={this.state.password}
        ref={(input) => this.edt_pass = input}
        onChangeText={text => {
          this.setState({
            ...this.state,
            password: text.trim()
          });
        }}
      />
    </>)
  }

  renderFormRegister = () => {
    return (<>
      {/* <TDTextInput
        fieldName='Số điện thoại'
        icon={R.images.ic_telephone}
        value={this.state.phone}
        borderRadiusTop
        keyboardType={"phone-pad"}
        onSubmitEditing={() => this.ref.edt_pass.focus()}
        maxLength={10}
        onChangeText={text => {
          this.setState({
            phone: text.trim()
          });
        }}
      /> */}
      <TDTextInput
        fieldName='Email'
        icon={R.images.ic_mail}
        value={this.state.email}
        borderRadiusTop
        borderRadiusBottom
        keyboardType='email-address'
        onChangeText={text => {
          this.setState({
            email: text.trim()
          });
        }}
        autoCapitalize='none'
      />
    </>)
  }

  render() {
    const { loginState } = this.props;
    const { phone, password, deviceID, isSelectLogin } = this.state
    return (
      <ImageBackground
        source={R.images.bg_login}
        style={{ width, height }}
        resizeMode='stretch'>
        <KeyboardAvoidingView
          behavior={Platform.OS === 'ios' && 'padding'}
          style={{
            flex: 1,
          }}
          enabled
          keyboardVerticalOffset={100}>
          {(loginState.isLoading || this.state.isLoading) && (
            <LoadingProgress />
          )}
          <ScrollView showsVerticalScrollIndicator={false}
            contentContainerStyle={{ flexGrow: 1, justifyContent: 'center' }}>
            <Icon
              containerStyle={{ position: "absolute", top: 50, right: 10 }}
              onPress={NavigationUtil.goBack}
              type="material"
              name="clear"
              size={30}
              color={Theme.colors.text_gray}
            />

            {this.renderViewSelect()}

            <View style={{
              maxHeight: height * 0.15,
              minHeight: height * 0.15,
            }}>
              {!!isSelectLogin ? this.renderFormLogin() : this.renderFormRegister()}

            </View>


            <Button
              text={isSelectLogin ? I18n.t("login").toLocaleUpperCase() : R.strings().register.toLocaleUpperCase()}
              top={30}
              onPress={() => {
                if (isSelectLogin) { this.loginPress() } else this.registerPress()
                // NavigationUtil.navigate(SCREEN_ROUTER.OTP)
              }}
            />

          </ScrollView>
        </KeyboardAvoidingView>
      </ImageBackground>
    );
  }

  loginPress = () => {
    const { phone, password, deviceID, email } = this.state

    // if (!email.trim()) {
    //   Toast.show('Bạn chưa nhập email', BACKGROUND_TOAST.FAIL)
    //   return
    // }

    if (phone == '') {
      Toast.show('Bạn chưa nhập số điện thoại', BACKGROUND_TOAST.FAIL)
      return
    }

    if (this.state.password == "") {
      Toast.show(I18n.t("error_pass"), BACKGROUND_TOAST.FAIL)
      return
    }
    const payload = {
      phone, email,
      password, deviceID
    }
    this.props.loginWithSocial(payload);
  }

  registerPress = async () => {
    const { phone, email } = this.state

    // if (phone == '') {
    //   showMessages(R.strings().notification, 'Bạn chưa nhập số điện thoại')
    //   return
    // }

    if (!email.trim()) {
      Toast.show('Bạn chưa nhập email', BACKGROUND_TOAST.FAIL)
      return
    }

    this.setState({ isLoading: true })

    const payload = { phone, email }

    try {
      const res = await requestCheckPhoneRegister(payload)
      if (res.data.isUpdate == 1) {
        NavigationUtil.navigate(SCREEN_ROUTER.REGISTER, { phone, email })
        showMessages('Thông báo', 'Vui lòng cập nhật thông tin tài khoản')
      } else
        NavigationUtil.navigate(SCREEN_ROUTER.OTP, { phone, email })
    } catch (error) {
      console.log(error)
    } finally { this.setState({ isLoading: false }) }
  }
}

const mapStateToProps = state => ({
  loginState: state[REDUCER.USER]
});

const mapDispatchToProps = {
  loginWithSocial,
  getAddress
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(LoginScreen);

const styles = StyleSheet.create({
  _imgLogo: {
    alignSelf: "center",
    resizeMode: "contain",
    width: width * 0.8,
    height: width * 0.3,
    marginTop: Theme.dimension.height * 0.05
  },
  _inpPhone: {
    alignItems: "center",
    justifyContent: "center"
  },
  _viewOr: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    marginHorizontal: 20,
    marginTop: 20,
  },

  _viewLine: {
    height: 1,
    width: "100%",
    backgroundColor: Theme.colors.black
  },
  _viewRegister: {
    flexDirection: "row",
    marginHorizontal: 20,
    marginTop: 10
  },
  view_item_select: {
    borderBottomColor: theme.colors.gray,
    borderBottomWidth: 3,
  },
  text_item_select: {
    ...theme.fonts.semibold18,
    marginBottom: 10,
    color: theme.colors.black1
  },
});
