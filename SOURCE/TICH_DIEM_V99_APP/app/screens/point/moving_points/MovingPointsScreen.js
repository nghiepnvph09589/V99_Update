import R from '@app/assets/R'
import {
    Block, DCHeader, FstImage,
    LoadingProgress,
    NumberFormat, TDButton
} from '@app/components'
import { GET_HISTORY_POINT_TYPE, REDUCER, SCREEN_ROUTER, USER_ACTIVATED } from '@app/constants/Constant'
import theme from '@app/constants/Theme'
import NavigationUtil from '@app/navigation/NavigationUtil'
import React, { Component } from 'react'
import { ActivityIndicator, FlatList, ImageBackground, Keyboard } from 'react-native'
import {
    View, Text, SafeAreaView, TouchableOpacity,
    KeyboardAvoidingView, Platform, TextInput, StyleSheet,
    ScrollView, TouchableWithoutFeedback
} from 'react-native'
import { connect } from 'react-redux'
import { requestMovePoint, requestGetListMember } from '@api'
import Toast, { BACKGROUND_TOAST } from '@app/utils/Toast'
import { updateUserLocal, getWalletPoints } from '@action'

export class MovingPointsScreen extends Component {
    constructor(props) {
        super(props)
        this.state = {
            point: '',
            note: '',
            isRequesting: false,
            isLoading: true,
            showAutoComplete: false,
            searchData: [],
            searchKey: '',
            phone: ''
        }
    }


    renderImagePoint = () => {
        const { userState } = this.props

        return (
            <ImageBackground
                source={R.images.img_bg_action_points}
                style={{
                    width: width * 0.95,
                    aspectRatio: 4,
                    backgroundColor: 'red',
                    borderRadius: 5,
                    alignSelf: 'center',
                    flexDirection: 'row',
                    alignItems: 'center',
                    paddingHorizontal: '5%'
                }}
                resizeMode='cover'
            >
                <FstImage
                    source={R.images.ic_shape_star}
                    style={{ height: '70%', aspectRatio: 1, }}
                    resizeMode='contain'
                />
                <View style={{ paddingLeft: '5%' }}>
                    <Text style={{
                        ...theme.fonts.regular16,
                        color: 'white',
                        marginBottom: 10,
                    }}>Điểm hiện có</Text>
                    <NumberFormat
                        value={userState.point}
                        perfix={R.strings().point}
                        fonts={theme.fonts.regular18}
                        color={theme.colors.white}
                    />
                </View>
            </ImageBackground>
        )
    }

    onChangeText = (text) => {
        this.setState({ point: text })
    }

    onChangeNote = (text) => {
        this.setState({ note: text })
    }

    onCallback = item => {
        this.setState({
            searchKey: item.nameAndPhone,
            phone: item.phone
        })
    }

    renderViewAction = () => {
        const { point, note, searchKey, isLoading, searchData } = this.state

        return (
            <View style={{
                marginVertical: 10,
                paddingHorizontal: '2.5%',
            }}>
                <View style={styles.viewTextInput}>
                    <Text style={{
                        ...theme.fonts.regular20,
                        color: theme.colors.black1
                    }}>Chuyển điểm cho khách hàng:</Text>
                    <TextInput
                        style={styles.text_input}
                        value={searchKey}
                        placeholder='Nhập tên hoặc số điện thoại'
                        onTouchEnd={() => NavigationUtil.navigate(SCREEN_ROUTER.SEARCH_REFERRAL_CODE, {
                            callback: this.onCallback,
                            titleHeader: 'Chọn tài khoản'
                        })}
                    // onChangeText={this.onChangePhone}
                    />
                    {/* {(isLoading && searchData) ? this.renderViewAutoComplete() : null} */}
                    {/* {this.renderViewAutoComplete()} */}
                </View>

                <View style={styles.viewTextInput}>
                    <Text style={{
                        ...theme.fonts.regular20,
                        color: theme.colors.black1
                    }}>Nhập số tiền muốn chuyển:</Text>
                    <TextInput
                        keyboardType='number-pad'
                        style={styles.text_input}
                        value={point}
                        placeholder='0'
                        onChangeText={this.onChangeText}
                    />
                </View>

                <Text style={{
                    ...theme.fonts.regular18,
                    color: theme.colors.black1,
                    marginVertical: 10
                }}>Ghi chú</Text>

                <TextInput
                    style={{
                        ...styles.text_input,
                        ...theme.fonts.regular18
                    }}
                    value={note}
                    placeholder='Nhập ghi chú'
                    multiline
                    onChangeText={this.onChangeNote}
                />

                <TDButton
                    style={{ marginTop: 50 }}
                    title={R.strings().agree}
                    titleColor={theme.colors.white}
                    titleStyle={{ ...theme.fonts.regular20 }}
                    onPress={this.drawPointsPress}
                />
            </View>
        )
    }

    drawPointsPress = async () => {
        const { point, note, phone, searchKey } = this.state
        const { userState } = this.props

        if (phone == '') {
            Toast.show('Bạn chưa chọn người nhận điểm', BACKGROUND_TOAST.FAIL)
            return
        }

        if (!point.trim()) {
            Toast.show('Bạn chưa nhập điểm', BACKGROUND_TOAST.FAIL)
            return
        }

        this.setState({ isRequesting: true })

        const payload = {
            point, phone, note
        }

        try {
            if (point > 0) {
                const res = await requestMovePoint(payload)
                Toast.show('Đã gửi yêu cầu chuyển điểm thành công')
                NavigationUtil.navigate(SCREEN_ROUTER.MOVING_POINTS_SUCCESS, {
                    item: res.data,
                    note, phone: searchKey
                })
                const payloadWalletPoint = {
                    page: 1,
                    type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
                    typePoint: ''
                }
                this.props.getWalletPoints(payloadWalletPoint)
                this.props.updateUserLocal({ point: res.data.balance })
            } else Toast.show('Số điểm cần chuyển phải lớn hơn 0', BACKGROUND_TOAST.FAIL)
        } catch (error) {
            console.log(error);
        }
        finally { this.setState({ isRequesting: false }) }
    }

    renderViewAutoComplete = () => {
        const { searchData, isLoading } = this.state;

        if (isLoading)
            return (
                <View
                    style={{
                        height: 30,
                        justifyContent: "center",
                        alignItems: "center",
                        ...styles.vSuggest
                    }}
                >
                    <ActivityIndicator size={"small"} />
                </View>
            );
        return (
            <FlatList
                keyboardShouldPersistTaps={"handled"}
                style={{
                    maxHeight: width / 4,
                    paddingLeft: 5,
                    paddingBottom: 8,
                    ...styles.vSuggest
                }}
                data={[0, 4, 4, 4]}
                // data={searchData}
                keyExtractor={(item, index) => index.toString()}
                renderItem={({ item, index }) => (
                    <TouchableOpacity
                        style={{ marginTop: 8 }}
                        onPress={() =>
                            this.setState({
                                searchCategoryProduct: item.name,
                                showSuggestKey: false
                            })
                        }
                    >
                        <Text>{'sdfhdsjf;'}</Text>
                    </TouchableOpacity>
                )}
            />
        );
    }

    timeout = null

    onChangePhone = (text) => {
        this.setState({ searchKey: text }, () => {
            if (this.timeout) clearTimeout(this.timeout)
            this.timeout = setTimeout(this.onSearch, 500);
        })
    }

    onSearch = async () => {
        const { searchKey } = this.state

        this.setState({ isLoading: true })

        const payload = { searchKey }

        try {
            const res = await requestGetListMember(payload)
            this.setState({
                isLoading: false,
                searchData: res.data,
                showAutoComplete: true
            })
        } catch (error) {
            console.log(error)
        } finally { this.setState({ isLoading: false }) }
    }

    render() {
        const { userState } = this.props
        const { isRequesting } = this.state
        return (
            <Block>
                <DCHeader isWhiteBackground
                    title={R.strings().moving_point}
                    rightComponent={<TouchableOpacity
                        onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.HISTORY_MOVING_POINTS)}>
                        <FstImage source={R.images.ic_time_machine}
                            style={{ width: 20, height: 20 }}
                            resizeMode='contain' />
                    </TouchableOpacity>} />
                <SafeAreaView style={theme.styles.container}>
                    {isRequesting && <LoadingProgress />}
                    <TouchableWithoutFeedback
                        style={{ flex: 1 }}
                        onPress={() => {
                            this.setState({ showAutoComplete: false });
                            Keyboard.dismiss();
                        }}>
                        <KeyboardAvoidingView
                            behavior={Platform.OS === 'ios' && 'padding' || null}
                            style={{
                                flex: 1,
                                backgroundColor: 'white'
                            }}
                            enabled
                            keyboardVerticalOffset={100}>
                            <ScrollView>
                                {this.renderImagePoint()}
                                {this.renderViewAction()}
                            </ScrollView>
                        </KeyboardAvoidingView>
                    </TouchableWithoutFeedback>
                </SafeAreaView>
            </Block>
        )
    }
}

const styles = StyleSheet.create({
    viewTextInput: {
        borderRadius: 5,
        borderWidth: 1,
        borderColor: theme.colors.border,
        padding: '2.5%',
        marginBottom: 10
    },
    text_input: {
        width: width * 0.975,
        borderBottomWidth: 0.5,
        borderBottomColor: theme.colors.border,
        ...theme.fonts.regular25
    },
    vSuggest: {
        backgroundColor: "red",
        position: "absolute",
        top:
            Platform.OS !== "ios" ? width * 0.2 + 8 : width * 0.2 + 5,
        // left: Platform.OS !== "ios" ? 18 : 12,
        // top: 0,
        left: -1,
        zIndex: 1,
        width: width * 0.95,
        borderBottomLeftRadius: 5,
        borderBottomRightRadius: 5
    },
})

const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
    bankState: state[REDUCER.BANK].data
})

const mapDispatchToProps = {
    updateUserLocal,
    getWalletPoints
}

export default connect(mapStateToProps, mapDispatchToProps)(MovingPointsScreen)
