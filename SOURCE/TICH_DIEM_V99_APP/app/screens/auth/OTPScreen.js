import AsyncStorage from "@react-native-community/async-storage";
import {
    GoogleSignin,
    statusCodes
} from "@react-native-community/google-signin";
import React, { Component, useState } from "react";
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
import { requestCheckOTP } from '@api'
import {
    CodeField,
    Cursor,
    useBlurOnFulfill,
    useClearByFocusCell,
} from 'react-native-confirmation-code-field';


const CELL_COUNT = 6;

const PrimaryCodeField = ({ onFullFill }) => {
    const [value, setValue] = useState('');
    const ref = useBlurOnFulfill({ value, cellCount: CELL_COUNT, });
    const [props, getCellOnLayoutHandler] = useClearByFocusCell({
        value,
        setValue,
    });

    return (
        <View style={{ paddingHorizontal: '5%', marginVertical: 20 }}>
            <CodeField
                ref={ref}
                {...props}
                value={value}
                onChangeText={(text) => {
                    setValue(text)
                    if (text.length == CELL_COUNT && onFullFill)
                        onFullFill(text)
                }}
                cellCount={CELL_COUNT}
                rootStyle={styles.codeFieldRoot}
                keyboardType="number-pad"
                textContentType="oneTimeCode"
                renderCell={({ index, symbol, isFocused }) => (
                    <Text
                        key={index}
                        style={[styles.cell, isFocused && styles.focusCell]}
                        onLayout={getCellOnLayoutHandler(index)}>
                        {symbol || (isFocused ? <Cursor /> : null)}
                    </Text>
                )}
            />
        </View>
    );
};

export class OTPScreen extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isLoading: false
        };
        this.phone = props.navigation.getParam('phone')
        this.email = props.navigation.getParam('email')
    }

    componentDidMount() {
    }
    checkOtp = async (otp) => {
        this.setState({ isLoading: true })

        const payload = {
            otp,
            phone: this.phone,
            email: this.email,
        }
        try {
            const res = await requestCheckOTP(payload)
            NavigationUtil.navigate(SCREEN_ROUTER.REGISTER,
                { phone: this.phone, email: this.email })
        } catch (error) {
            console.log(error)
        } finally { this.setState({ isLoading: false }) }
    }

    render() {
        const { loginState } = this.props;
        return (
            <KeyboardAvoidingView style={{ flex: 1 }} enabled>
                {this.state.isLoading && <LoadingProgress />}
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
                        source={R.images.img_otp}
                        resizeMode='contain'
                    />

                    <Text
                        style={{
                            marginTop: 20,
                            textAlign: 'center',
                            color: theme.colors.primary,
                            ...theme.fonts.semibold25
                        }}>Nhập mã xác nhận</Text>
                    <Text
                        style={{
                            marginTop: 20,
                            textAlign: 'center',
                            color: theme.colors.black_title,
                            paddingHorizontal: '5%',
                            ...theme.fonts.regular16
                        }}>Hệ thống sẽ gửi mã xác nhận tới email bạn vừa nhập. Vui lòng nhập mã bạn vừa nhận được !</Text>
                    <PrimaryCodeField onFullFill={this.checkOtp} />
                    {/* <Button
                        text={'TIẾP THEO'}
                        top={30}
                        onPress={() => {
                            NavigationUtil.navigate(SCREEN_ROUTER.REGISTER)
                        }}
                    /> */}

                </ScrollView>
            </KeyboardAvoidingView>
        );
    }
}

const mapStateToProps = state => ({
});

const mapDispatchToProps = {
};

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(OTPScreen);

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
    root: { flex: 1, padding: 20 },
    title: { textAlign: 'center', fontSize: 30 },
    codeFieldRoot: { marginTop: 20 },
    cell: {
        width: 50,
        height: 50,
        // lineHeight: 38,
        justifyContent: 'center',
        alignItems: 'center',
        fontSize: 24,
        textAlign: 'center',
        backgroundColor: '#EEEEEE',
        borderRadius: 5,
        overflow: 'hidden',
    },
    focusCell: {
        borderColor: '#000',
    },
});
