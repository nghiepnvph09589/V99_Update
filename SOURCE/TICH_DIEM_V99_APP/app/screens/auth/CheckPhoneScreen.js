import AsyncStorage from "@react-native-community/async-storage";
import {
    GoogleSignin,
    statusCodes
} from "@react-native-community/google-signin";
import React, { Component } from "react";
import {
    Image,
    ImageBackground,
    KeyboardAvoidingView,
    ScrollView,
    StyleSheet,
    Text,
    TouchableOpacity,
    View
} from "react-native";
import { AccessToken, LoginManager } from "react-native-fbsdk";
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
import { Loading, FastImage, LoadingProgress, Block, FstImage } from "../../components";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import { Icon } from "react-native-elements";
import OneSignal from "react-native-onesignal";
import R from "@app/assets/R";
import { requestCheckPhoneRegister } from '@api'

export class CheckPhoneScreen extends Component {
    constructor(props) {
        super(props);
        this.state = {
            phone: "",
            isLoading: '',
        };
    }

    componentDidMount() {
    }

    render() {
        const { isLoading, phone } = this.state
        return (
            <KeyboardAvoidingView style={{ flex: 1 }} enabled>
                {!!isLoading && <LoadingProgress />}
                <ScrollView showsVerticalScrollIndicator={false}>
                    <Icon
                        containerStyle={{ position: "absolute", top: 50, left: 10 }}
                        onPress={() => NavigationUtil.goBack()}
                        type="antdesign"
                        name="leftcircle"
                        size={30}
                        color={Theme.colors.text_gray}
                    />
                    <FstImage
                        style={styles._imgLogo}
                        source={R.images.logo}
                        resizeMode='contain'
                    />
                    <Text
                        style={{
                            marginTop: 20,
                            textAlign: 'center',
                            color: theme.colors.primary,
                            ...theme.fonts.semibold25
                        }}>{R.strings().register}</Text>
                    <TextFieldInput
                        icon={R.images.ic_telephone}
                        top={40}
                        label={I18n.t("phone_number")}
                        keyboardType={"phone-pad"}
                        value={phone}
                        maxLength={12}
                        onChangeText={text => {
                            this.setState({
                                phone: text
                            });
                        }}
                    />

                    <Button
                        text={R.strings().register.toLocaleUpperCase()}
                        top={30}
                        onPress={this.checkPhoneRegister}
                    />

                </ScrollView>
            </KeyboardAvoidingView>
        );
    }

    checkPhoneRegister = async () => {
        const { phone } = this.state
        this.setState({ isLoading: true })

        const payload = { phone }

        try {
            const res = await requestCheckPhoneRegister(payload)
            if (res.data.isUpdate == 1) {
                NavigationUtil.navigate(SCREEN_ROUTER.REGISTER, { phone })
                showMessages('Thông báo', 'Vui lòng cập nhật thông tin tài khoản')
            } else
                NavigationUtil.navigate(SCREEN_ROUTER.OTP, { phone })
        } catch (error) {
            console.log(error)
        } finally { this.setState({ isLoading: false }) }
    }
}

const mapStateToProps = state => ({
});

const mapDispatchToProps = {
    loginWithSocial,
    getAddress
};

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(CheckPhoneScreen);

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
    }
});
