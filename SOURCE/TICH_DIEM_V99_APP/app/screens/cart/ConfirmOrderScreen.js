import React, { Component } from 'react'
import { connect } from 'react-redux'
import {
    SafeAreaView, View, Text, TouchableOpacity,
    FlatList, ScrollView, StyleSheet, Image,
    TextInput, KeyboardAvoidingView, Platform
} from 'react-native'
import {
    DCHeader, Block, CartItem,
    NumberFormat, PrimaryButton, LoadingProgress, OrderItem, FstImage
} from '../../components'
import { SCREEN_ROUTER, REDUCER } from '../../constants/Constant'
import {
    getOrderDetail, getCart, getUserInfo,
    updateCountCart, updateUserLocal
} from '../../redux/actions'
import * as theme from '../../constants/Theme'
import I18n from '../../i18n/i18n'
import { Divider } from 'react-native-elements'
import NavigationUtil from '../../navigation/NavigationUtil'
import reactotron from 'reactotron-react-native'
import { showMessages, showConfirm } from '../../utils/Alert'
import Toast, { BACKGROUND_TOAST } from '../../utils/Toast'
import ObjectUtil from '../../utils/ObjectUtil'
import * as Api from '../../constants/Api'
import { NavigationActions, StackActions } from 'react-navigation';
import R from '@app/assets/R'

export class ConfirmOrderScreen extends Component {

    constructor(props) {
        super(props)
        const { userState, navigation } = props
        const data = navigation.getParam('data', {})
        this.state = {
            note: '',
            name: userState.customerName || '',
            phone: userState.phone || '',
            address: userState.address || '',
            provinceID: userState.provinceID || '',
            provinceName: userState.provinceName || '',
            districtID: userState.districtID || '',
            districtName: userState.districtName || '',
            isLoading: false,
            referralCode: userState.lastRefCode || '',
        }
    }

    componentDidMount() {
        const { userState } = this.props
        if (ObjectUtil.isEmpty(userState)) this.props.getUserInfo()
        if (!userState.customerName.trim() || !userState.phone || !userState.provinceID ||
            !userState.provinceName || !userState.districtID || !userState.districtName || !userState.address.trim()) {
            NavigationUtil.navigate(SCREEN_ROUTER.CUS_INFO, {
                userInfo: {
                    name: userState.customerName || '',
                    phone: userState.phone || '',
                    address: userState.address || '',
                    provinceID: userState.provinceID || '',
                    provinceName: userState.provinceName || '',
                    districtID: userState.districtID || '',
                    districtName: userState.districtName || '',
                },
                callback: this.updateUserInfoCallback
            })
            Toast.show('Vui lòng nhập đầy đủ thông tin đặt hàng', BACKGROUND_TOAST.FAIL)
        }
    }

    updateUserInfoCallback = callback => {
        this.setState((state) => {
            return {
                ...state,
                name: callback.name,
                phone: callback.phone,
                address: callback.address,
                provinceID: callback.provinceID,
                provinceName: callback.provinceName,
                districtID: callback.districtID,
                districtName: callback.districtName,
            }
        })
    }

    userInfo() {
        const { name, phone, address, districtName, provinceName, provinceID, districtID } = this.state
        return (
            <TouchableOpacity
                onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.CUS_INFO, {
                    userInfo: {
                        name: name,
                        phone: phone,
                        address: address,
                        provinceID: provinceID,
                        provinceName: provinceName,
                        districtID: districtID,
                        districtName: districtName,
                    },
                    callback: this.updateUserInfoCallback
                })}
                style={{
                    paddingHorizontal: 15,
                }}>
                {/* <Divider style={{ marginVertical: 5 }} /> */}
                <View style={{
                    flexDirection: 'row',
                    justifyContent: 'space-between'
                }}>
                    <Text style={[styles.text_user_info]}>{name}</Text>
                    <TouchableOpacity
                        onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.CUS_INFO, {
                            userInfo: {
                                name: name,
                                phone: phone,
                                address: address,
                                provinceID: provinceID,
                                provinceName: provinceName,
                                districtID: districtID,
                                districtName: districtName,
                            },
                            callback: this.updateUserInfoCallback
                        })}>
                        <FstImage
                            source={R.images.ic_edit}
                            style={{ width: 20, height: 20 }}
                            resizeMode='contain'
                        />
                    </TouchableOpacity>
                </View>
                <Text style={[styles.text_user_info, { marginVertical: 5 }]}>{phone}</Text>
                <Text style={[styles.text_user_info]}>
                    {address}{address && ', '}
                    {districtName}{districtName && ', '}
                    {provinceName}</Text>
            </TouchableOpacity>
        )
    }

    renderItem = ({ item, index }) => {
        return (<OrderItem item={item} />)
    }

    listOrder = () => {
        const { navigation } = this.props
        const data = navigation.getParam('data', {})

        return (
            <View style={{ paddingHorizontal: 15 }}>
                <ScrollView>
                    {data.map((item, index) => {
                        return <OrderItem key={index} item={item} index={index.toString()} />
                    })}
                </ScrollView>
            </View>
        )
    }

    getSumPrice = () => {
        const { navigation } = this.props
        const totalPrice = navigation.getParam('totalPrice', {})
        const listPrice = data.map(item => item.sumPrice);
        return listPrice.reduce((item, currentValue) => item + currentValue)
    }

    handleCreateOrder() {
        const { navigation, userState } = this.props
        const { name, phone, address, note, provinceID, districtID } = this.state
        if (name.trim() == '' || phone == '' || provinceID == 0 || districtID == 0 || address.trim() == '') {

            NavigationUtil.navigate(SCREEN_ROUTER.CUS_INFO, {
                userInfo: {
                    name: userState.customerName || '',
                    phone: userState.phone || '',
                    address: userState.address || '',
                    provinceID: userState.provinceID || '',
                    provinceName: userState.provinceName || '',
                    districtID: userState.districtID || '',
                    districtName: userState.districtName || '',
                },
                callback: this.updateUserInfoCallback
            })
            Toast.show('Vui lòng nhập đầy đủ thông tin đặt hàng', BACKGROUND_TOAST.FAIL)
            return
        }
        showConfirm(I18n.t('confirm'), 'Xác nhận đặt hàng', () => this.requestCreateOrder())
    }

    requestCreateOrder = async () => {
        const { navigation } = this.props
        const { name, phone, address, note, provinceID, districtID, referralCode } = this.state
        const data = navigation.getParam('data', {})
        const from = navigation.getParam('from', {})

        payload = {
            listOrderItem: data,
            ProvinceID: provinceID,
            DistrictID: districtID,
            BuyerName: name,
            BuyerPhone: phone,
            address,
            note,
            lastRefCode: referralCode
        }

        this.setState({
            isLoading: true
        })

        try {
            const res = await Api.requestCreateOrder(payload)
            if (from == SCREEN_ROUTER.CART) this.props.updateCountCart(data.length)
            this.props.updateUserLocal({
                lastRefCode: referralCode,
                point: res.data.point
            })
            this.props.navigation.reset(
                [
                    NavigationActions.navigate({
                        routeName: SCREEN_ROUTER.BOTTOM_BAR,
                        action: NavigationActions.navigate({
                            routeName: SCREEN_ROUTER.USER
                        })
                    }),
                    NavigationActions.navigate({ routeName: SCREEN_ROUTER.ORDER })
                ], 1)
            Toast.show('Đặt hàng thành công')
        } catch (error) {
            console.log(err)
            this.setState({ isLoading: false },
                () => Toast.show('Vui lòng thử lại', BACKGROUND_TOAST.FAIL)
            )
        } finally { this.setState({ isLoading: false }) }
    }

    titleView(icon, content, action) {
        return (
            <View style={styles.titleView}>
                <Block row center>
                    <Image source={icon} style={{ width: 20, height: 20 }} resizeMode='contain' />
                    <Text style={[theme.fonts.robotoRegular14, { marginLeft: 5 }]}>{content}</Text>
                </Block>
                {action && <TouchableOpacity style={{ marginRight: 8 }} onPress={action}>
                    <Text style={theme.fonts.robotoRegular14}>Sửa</Text>
                </TouchableOpacity>}
            </View>
        )
    }

    onCallback = item => {
        this.setState({ referralCode: item.phone })
    }

    render() {
        const { navigation, userState } = this.props
        const totalPrice = navigation.getParam('totalPrice', {})
        const { referralCode } = this.state

        return (
            <Block>
                {this.state.isLoading && <LoadingProgress />}
                <DCHeader isWhiteBackground title={I18n.t('confirm_order')} />
                <SafeAreaView style={[theme.styles.container, { backgroundColor: '#EEEEEE' }]}>
                    <KeyboardAvoidingView enabled behavior={Platform.OS === 'ios' ? 'padding' : null} style={{ flex: 1 }}>
                        <ScrollView contentContainerStyle={{ backgroundColor: '#EEEEEE' }}>
                            <View style={styles.block}>
                                {this.userInfo()}
                            </View>
                            <View style={styles.block}>
                                {this.listOrder()}
                            </View>
                            {/* <View style={styles.block}>
                                {this.titleView(require('../../assets/images/ic_note.png'), 'Ghi chú đơn hàng')}
                                <Divider style={{ marginHorizontal: 15, marginBottom: 10 }} />
                                <TextInput
                                    multiline
                                    style={[theme.fonts.regular14, {
                                        height: 80,
                                        padding: 5,
                                        textAlignVertical: 'top',
                                        marginHorizontal: 15
                                    }]} value={note}
                                    onChangeText={(value) => {
                                        this.setState({
                                            ...this.state,
                                            note: value
                                        })
                                    }} placeholder='Nội dung' />
                            </View> */}
                            {/* <View style={styles.block}>
                                {this.titleView(require('../../assets/images/ic_dolar.png'), 'Thông tin thanh toán')}
                                <Divider style={{ marginHorizontal: 15, marginBottom: 10 }} />
                                <View style={styles.payView}>
                                    <Text style={theme.fonts.robotoMedium14}>Tổng tiền:</Text>
                                    <NumberFormat value={totalPrice} perfix='đ'
                                        fonts={theme.fonts.robotoMedium16}
                                        color={theme.colors.red2} />
                                </View>
                            </View> */}

                        </ScrollView>
                        <View style={{
                            paddingVertical: 10,
                            backgroundColor: theme.colors.white,
                            paddingTop: 15,
                        }}>
                            <View style={{ flexDirection: 'row', alignItems: 'center', marginTop: 5, paddingHorizontal: '5%' }}>
                                <Text style={{ flex: 1, color: theme.colors.black_title, ...theme.fonts.regular16 }}>Mã giới thiệu</Text>
                                <TouchableOpacity
                                    activeOpacity={1}
                                    // onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.SEARCH_REFERRAL_CODE, {
                                    //     callback: this.onCallback
                                    // })}
                                    style={{
                                        // width: !referralCode && '30%',
                                        borderWidth: 0.5,
                                        padding: 5,
                                        borderRadius: 5,
                                        borderColor: theme.colors.border,
                                        alignItems: 'center',
                                        flexDirection: 'row',
                                    }}>

                                    <Text style={{
                                        color: theme.colors.black1,
                                        ...theme.fonts.regular18
                                    }}>{referralCode}</Text>
                                    {/* <FstImage source={R.images.ic_right}
                                        style={{ width: 15, height: 15, marginLeft: 10 }}
                                        resizeMode='contain'
                                    /> */}
                                </TouchableOpacity>
                            </View>
                            <View style={{ flexDirection: 'row', alignItems: 'center', marginVertical: 5, paddingHorizontal: '5%' }}>
                                <Text style={{ flex: 1, color: theme.colors.black_title, ...theme.fonts.regular16 }}>Tổng tiền</Text>
                                <NumberFormat
                                    value={totalPrice}
                                    // value={this.getTotalPrice() || 0}
                                    color={theme.colors.red2} perfix='đ' fonts={theme.fonts.robotoMedium20} />
                            </View>
                            <PrimaryButton
                                onPress={
                                    () => {
                                        this.handleCreateOrder()
                                    }
                                } title='Đặt hàng' />
                        </View>
                    </KeyboardAvoidingView>
                </SafeAreaView>
            </Block>
        )
    }


}

const styles = StyleSheet.create({
    block: {
        // marginBottom: 10,
        paddingVertical: 10,
        backgroundColor: theme.colors.white,
        marginVertical: 5
    },
    titleView: {
        paddingHorizontal: 15,
        flexDirection: 'row',
        marginVertical: 5,
        alignItems: 'center'
    },
    payView: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 25,
        marginBottom: 10,
    },
    text_user_info: {
        color: theme.colors.black_title,
        ...theme.fonts.regular15,
    }
})

const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
})

const mapDispatchToProps = {
    getOrderDetail,
    getCart,
    getUserInfo,
    updateCountCart,
    updateUserLocal
}

export default connect(mapStateToProps, mapDispatchToProps)(ConfirmOrderScreen)
