import React, { Component } from "react";
import {
  Text,
  View,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  TextInput,
  StyleSheet
} from "react-native";
import Header from "../../components/DCHeader";
import * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import Button from "../../components/Button";
import { connect } from "react-redux";
import SafeAreaView from "react-native-safe-area-view";
import { Block, Loading, Error } from "../../components";
import reactotron from "reactotron-react-native";
import NavigationUtil from "../../navigation/NavigationUtil";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import { changePass } from "../../constants/Api";
import { showMessages } from "../../utils/Alert";

export class ChangePassScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      oldPass: "",
      newPass: "",
      reNewPass: "",
      isLoading: false
    };
  }
  _changePass = async () => {
    const { oldPass, newPass } = this.state;
    this.setState({
      ...this.state,
      isLoading: true
    });
    try {
      const response = await changePass({
        password: oldPass,
        newPassword: newPass
      });
      reactotron.log(response, "response");
      Toast.show(I18n.t("change_pass_success"), BACKGROUND_TOAST.SUCCESS);

      this.setState({
        ...this.state,
        isLoading: false
      });
      NavigationUtil.goBack();
    } catch (error) {
      if (error.message == "Network Error") {
        Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
      }
      reactotron.log(error, "error");
      this.setState({
        ...this.state,
        isLoading: false
      });
      //Toast.show('Vui lòng thử lại', BACKGROUND_TOAST.FAIL)
    }
  };

  _textInput(title, placeholder, onChangeText, onSubmitEditing, refs) {
    return (
      <View style={{ marginTop: 20 }}>
        <Text style={styles.text}>{title}</Text>
        <TextInput
          style={[Theme.styles.textInput, Theme.fonts.robotoRegular16]}
          onChangeText={onChangeText}
          placeholder={placeholder}
          secureTextEntry={true}
          onSubmitEditing={onSubmitEditing}
          ref={refs}
        />
      </View>
    );
  }
  _renderBody() {
    // const {changePassState}=this.props
    // const { oldPass, newPass } = this.state;
    const { isLoading } = this.state;
    if (isLoading) return <Loading />;
    return (
      <View>
        {this._textInput(
          I18n.t("old_pass"),
          I18n.t("old_pass1"),
          text => {
            this.setState({
              ...this.state,
              oldPass: text.trim()
            });
          },
          () => this.newPass.focus()
        )}

        {this._textInput(
          I18n.t("new_pass"),
          I18n.t("new_pass1"),
          text => {
            this.setState({
              ...this.state,
              newPass: text.trim()
            });
          },
          () => this.reNewPass.focus(),
          ref => (this.newPass = ref)
        )}
        {this._textInput(
          I18n.t("re_new_pass"),
          I18n.t("re_new_pass"),
          text => {
            this.setState({
              ...this.state,
              reNewPass: text.trim()
            });
          },
          () => {},
          ref => (this.reNewPass = ref)
        )}

        <Button
          text={I18n.t("save")}
          top={60}
          onPress={() => {
            if (this.state.oldPass == "") {
              showMessages("", I18n.t("error_old_pass"));
            } else if (this.state.newPass == "") {
              showMessages("", I18n.t("not_new_pass"));
            } else if (this.state.reNewPass == "") {
              showMessages("", I18n.t("not_re_new_pass"));
            } else if (this.state.newPass != this.state.reNewPass) {
              showMessages("", I18n.t("error_re_pass"));
            } else {
              this._changePass();
            }
          }}
        />
      </View>
    );
  }
  render() {
    return (
      <Block>
        <Header title={I18n.t("change_pass")} back />
        <SafeAreaView style={{ flex: 1 }}>
          <KeyboardAvoidingView
            enabled
            behavior={Platform.OS === "ios" ? "padding" : null}
            style={{ flex: 1 }}
          >
            <ScrollView
              showsVerticalScrollIndicator={false}
              contentContainerStyle={{ flexGrow: 1 }}
            >
              {this._renderBody()}
            </ScrollView>
          </KeyboardAvoidingView>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  //changePassState:state.updateUseReducer
});

const mapDispatchToProps = {
  // changePass
};

export default connect(mapStateToProps, mapDispatchToProps)(ChangePassScreen);
const styles = StyleSheet.create({
  text: {
    marginTop: 15,
    marginBottom: 5,
    marginLeft: "5%",
    ...Theme.fonts.robotoRegular16
  }
});
