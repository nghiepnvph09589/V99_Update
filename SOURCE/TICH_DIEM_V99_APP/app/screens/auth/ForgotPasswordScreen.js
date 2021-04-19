import React, { Component } from "react";
import { View, Text } from "react-native";
import Header from "../../components/DCHeader";
import I18n from "../../i18n/i18n";
import TextFieldInput from "../../components/TextFieldInput";
import { SCREEN_ROUTER } from "../../constants/Constant";
import * as Theme from "../../constants/Theme";
import Button from "../../components/Button";
import NavigationUtil from "../../navigation/NavigationUtil";
import { showMessages } from "../../utils/Alert";
import { connect } from "react-redux";
import { Block, Loading } from "../../components";
import SafeAreaView from "react-native-safe-area-view";
import { forgotPassword } from "../../constants/Api";
//import { TextField } from "react-native-materialui-textfield";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";

export class ForgotPasswordScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      email: "",
      isLoading: false
    };
  }
  _forgotPassword = async () => {
    this.setState({
      ...this.state,
      isLoading: true
    });
    try {
      const response = await forgotPassword({ email: this.state.email });
      this.setState({
        ...this.state,
        isLoading: false
      });
      showMessages(I18n.t("notification"), response.message);
      NavigationUtil.navigate(SCREEN_ROUTER.AUTH_LOADING);
    } catch (error) {
      if(error.message=="Network Error"){
        Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
      }
      this.setState({
        ...this.state,
        isLoading: false
      });
    }
  };
  _renderBody() {
    if (this.state.isLoading) return <Loading />;
    const filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return (
      <View>
        <Text style={[Theme.fonts.robotoItalic14, { paddingHorizontal: 20 }]}>
          {I18n.t("guide_input")}
        </Text>
        <TextFieldInput
          top={20}
          label={I18n.t("email")}
          keyboardType={"email-address"}
          onChangeText={text => {
            this.setState({
              ...this.state,
              email: text.trim()
            });
          }}
          autoCapitalize="none"
        />
        <Button
          text={I18n.t("get_pass")}
          top={50}
          onPress={() => {
            if (this.state.email == "") {
              showMessages(I18n.t("notification"), I18n.t("not_email"));
            } else if (!filter.test(this.state.email)) {
              showMessages(I18n.t("notification"), I18n.t("email_wrong"));
            } else {
              this._forgotPassword();
            }
          }}
        />
      </View>
    );
  }
  render() {
  
    return (
      <Block>
        <Header title={I18n.t("forgot_pass")} back />
        <SafeAreaView
          style={[
            Theme.styles.container,
            { justifyContent: "center" }
          ]}
        >
          {this._renderBody()}
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ForgotPasswordScreen);
