import React, { Component } from "react";
import {
  View,
  Text,
  Image,
  SafeAreaView,
  StatusBar,
  ActivityIndicator,
  ImageBackground, Platform
} from "react-native";
import NavigationUtil from "../../navigation/NavigationUtil";
import { ASYNCSTORAGE_KEY, SCREEN_ROUTER } from "../../constants/Constant";
import AsyncStorage from "@react-native-community/async-storage";
// import { connect } from 'react-redux'
import codePush from "react-native-code-push";
import reactotron from "reactotron-react-native";
import OneSignal from "react-native-onesignal";
import { connect } from "react-redux";
import { setIsLogin } from "../../redux/actions";
import SplashScreen from 'react-native-splash-screen'
import DeviceInfo from 'react-native-device-info';
import { requestGetAppVersion } from '@api'
import theme from "@app/constants/Theme";

export class AuthLoadingScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      update: false
    };
  }

  componentDidMount() {
    // AsyncStorage.setItem(ASYNCSTORAGE_KEY.TOKEN, '2DA2429B0528EE5285A92F20E72AA3C6')

    // this.checkAccount()
    // return
    
    if (__DEV__) {
      this.checkAccount()
    }
    else
      this.checkCodePushVer()
  }

  checkCodePushVer = async () => {

    const appVersion = DeviceInfo.getVersion()

    const code_push_version = await AsyncStorage.getItem(
      ASYNCSTORAGE_KEY.CODE_PUSH
    );

    const os = Platform.OS == "ios" ? 1 : 2;

    const payload = {
      typeOS: os
    }

    try {
      const res = await requestGetAppVersion(payload)

      const searchIndex = res.data
        .map(elem => elem.versionApp)
        .indexOf(appVersion);

      if (searchIndex == -1 || res.data[searchIndex].codePushVersion == code_push_version) {
        reactotron.log('not update')
        this.checkAccount()
        return
      }

      const itemSelect = res.data[searchIndex]
      reactotron.log(itemSelect, 'itemSelect')

      if (itemSelect.forceUpdate == 1) {
        reactotron.log('checkUpdate');
        this._checkUpdate(itemSelect.codePushVersion);
      }
      else this._checkUpdateNotForceUpdate(itemSelect.codePushVersion);

    } catch (error) {
      this.checkAccount()
      reactotron.log(error)
    }
  };

  _checkUpdateNotForceUpdate = async (codePushVersion) => {
    reactotron.log('_checkUpdateNotForceUpdate');
    codePush
      .checkForUpdate()
      .then(update => {
        if (!update) {
          this.checkAccount();
        } else {
          codePush.sync(
            {
              updateDialog: null,
              installMode: codePush.InstallMode.ON_NEXT_RESTART
            },
            status => {
              this.checkAccount();
            },
            progress => { }
          ).then(() => {
            AsyncStorage.setItem(
              ASYNCSTORAGE_KEY.CODE_PUSH,
              codePushVersion
            );
          })
        }
      })
      .catch(err => {
        console.log("error", error);
        codePush.allowRestart();
      });
  };

  _checkUpdate = async (codePushVersion) => {
    this.setState(
      {
        ...this.state,
        update: true
      },
      async () => {
        codePush
          .checkForUpdate()
          .then(update => {
            reactotron.log(update);
            this.setState({
              update: false
            });
            if (!update) {
              this.setState({ update: false }, () => this.checkAccount());
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
                      update: true
                    });
                  } else {
                    this.setState({
                      update: false
                    });
                  }
                },
                progress => {
                  reactotron.log(progress);
                }
              ).then((res) => {
                AsyncStorage.setItem(
                  ASYNCSTORAGE_KEY.CODE_PUSH,
                  codePushVersion
                );
              })
            }
          })
          .catch(err => {
            this.setState({ update: false }, () => this.checkAccount());
          });
      }
    );
    codePush.notifyAppReady();
  }

  checkAccount = async () => {
    SplashScreen.hide();
    const token = await AsyncStorage.getItem("token");
    this.props.setIsLogin(!!token);
    NavigationUtil.navigate(SCREEN_ROUTER.MAIN);
  };

  render() {
    return (
      <ImageBackground
        style={{
          width: "100%",
          height: "100%",
          alignItems: "center",
          justifyContent: "center"
        }}
        source={require("../../assets/images/splash.png")}
      >
        <ActivityIndicator size="large" color={theme.colors.active} />
      </ImageBackground>
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {
  setIsLogin
};

export default connect(mapStateToProps, mapDispatchToProps)(AuthLoadingScreen);
