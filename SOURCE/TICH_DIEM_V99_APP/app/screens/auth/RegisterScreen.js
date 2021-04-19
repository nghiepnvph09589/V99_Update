import React, { Component } from "react";
import { View, Text, ScrollView, KeyboardAvoidingView } from "react-native";
import Header from "../../components/DCHeader";
import I18n from "../../i18n/i18n";
import TextFieldInput from "../../components/TextFieldInput";
import Button from "../../components/Button";
import * as Theme from "../../constants/Theme";
import { SCREEN_ROUTER, ASYNCSTORAGE_KEY } from "../../constants/Constant";
import NavigationUtil from "../../navigation/NavigationUtil";
import { Dropdown } from "react-native-material-dropdown";
import { connect } from "react-redux";
//import { register } from "../../redux/actions";
import reactotron from "reactotron-react-native";
import { showMessages } from "../../utils/Alert";
import { Block, LoadingProgress } from "../../components";
import SafeAreaView from "react-native-safe-area-view";
import { requestRegister } from "../../constants/Api";
import AsyncStorage from "@react-native-community/async-storage";
import { Loading, Error } from "../../components";
import { getAddress } from "../../redux/actions";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import { updateUserLocal } from '@action'
import R from "@app/assets/R";
export class RegisterScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      name: "",
      phone: props.navigation.getParam('phone'),
      email: props.navigation.getParam('email'),
      password: "",
      re_password: "",
      referralCode: ''
    };
  }

  _register = async () => {
    const {
      name,
      phone,
      password,
      re_password, referralCode, email
    } = this.state;
    if (name == "" || name.trim().length == 0) {
      showMessages(I18n.t("notification"), I18n.t("error_name"));
    } else if (phone == "") {
      showMessages(I18n.t("notification"), I18n.t("not_input_phone"));
    } else if (referralCode == "") {
      showMessages(I18n.t("notification"), 'Bạn chưa nhập mã giới thiệu');
    } else if (password == "") {
      showMessages(I18n.t("notification"), I18n.t("error_pass"));
    } else if (re_password == "") {
      showMessages(I18n.t("notification"), I18n.t("not_re_pass"));
    } else if (re_password != password) {
      showMessages(I18n.t("notification"), I18n.t("error_re_pass"));
    } else {
      this.setState({
        isLoading: true
      });

      const payload = {
        name: name.trim(),
        phone, password, email,
        lastRefCode: referralCode,
      }

      try {
        const response = await requestRegister(payload);
        AsyncStorage.setItem(ASYNCSTORAGE_KEY.TOKEN, response.data.token, () => {
          NavigationUtil.navigate(SCREEN_ROUTER.AUTH_LOADING)
        })
      } catch (error) {
        console.log(error)
      } finally {
        this.setState({ isLoading: false })
      }
    }
  };
  _renderBody() {
    const { isLoading, referralCode } = this.state;

    return (
      <View style={{ flex: 1 }}>
        <TextFieldInput
          icon={R.images.ic_feather_user}
          top={20}
          label={I18n.t("name")}
          value={this.state.name}
          onChangeText={text => {
            this.setState({
              name: text
            });
          }}
          submit={() => this.phone.focus()}
          obligatory
          maxLength={50}
        />

        <TextFieldInput
          icon={R.images.ic_telephone}
          top={20}
          label={I18n.t("phone")}
          keyboardType={"phone-pad"}
          value={this.state.phone}
          maxLength={10}
          submit={() => this.referralCode.focus()}
          refs={ref => (this.phone = ref)}
          onChangeText={text => {
            this.setState({
              phone: text.trim()
            });
          }}
          obligatory
        />

        <TextFieldInput
          icon={R.images.ic_mail}
          editable={false}
          top={20}
          label={'Email'}
          keyboardType={"phone-pad"}
          value={this.state.email}
          // submit={() => this.referralCode.focus()}
          // refs={ref => (this.phone = ref)}
          // onChangeText={text => {
          //   this.setState({
          //     ...this.state,
          //     phone: text.trim()
          //   });
          // }}
          obligatory
        />

        <TextFieldInput
          icon={R.images.ic_telephone}
          top={20}
          label={'Mã giới thiệu'}
          keyboardType={"phone-pad"}
          value={referralCode}
          maxLength={10}
          submit={() => this.password.focus()}
          refs={ref => (this.referralCode = ref)}
          onChangeText={text => {
            this.setState({
              referralCode: text.trim()
            });
          }}
          obligatory
        />

        <TextFieldInput
          icon={R.images.ic_feather_lock}
          top={20}
          label={I18n.t("pass")}
          secureTextEntry={true}
          value={this.state.password}
          submit={() => this.re_password.focus()}
          refs={ref => (this.password = ref)}
          onChangeText={text => {
            this.setState({
              ...this.state,
              password: text.trim()
            });
          }}
          obligatory
        />

        <TextFieldInput
          icon={R.images.ic_feather_lock}
          top={20}
          label={I18n.t("re_password")}
          value={this.state.re_password}
          secureTextEntry={true}
          refs={ref => (this.re_password = ref)}
          onChangeText={text => {
            this.setState({
              ...this.state,
              re_password: text.trim()
            });
          }}
          obligatory
        />

        <Button
          text={I18n.t("register")}
          top={50}
          onPress={this._register}
        />
      </View>
    );
  }

  render() {
    const { isLoading } = this.state
    return (
      <Block>
        <Header title={'Nhập thông tin'} back />
        <SafeAreaView style={Theme.styles.container}>
          <KeyboardAvoidingView
            enabled
            behavior={Platform.OS === "ios" ? "padding" : null}
            style={{ flexGrow: 1 }}
          >
            <ScrollView
              contentContainerStyle={{
                paddingBottom: 20,
                flexGrow: 1
              }}
              showsVerticalScrollIndicator={false}
            >
              {!!isLoading && <LoadingProgress />}
              {this._renderBody()}
            </ScrollView>
          </KeyboardAvoidingView>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
});

const mapDispatchToProps = {
  updateUserLocal
};

export default connect(mapStateToProps, mapDispatchToProps)(RegisterScreen);
