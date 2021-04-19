import React, { Component } from 'react'
import {
    FlatList, KeyboardAvoidingView, Platform,
    RefreshControl, SafeAreaView, Text,
    TouchableOpacity, View, ScrollView
} from 'react-native'
import CircleCheckBox from 'react-native-circle-checkbox'
import { Divider } from 'react-native-elements'
import { connect } from 'react-redux'
import reactotron from 'reactotron-react-native'
import {
    Block, CartItem, DCHeader, Empty,
    Error, Loading, LoadingProgress, NumberFormat, PrimaryButton
} from '../../components'
import { REDUCER, SCREEN_ROUTER, USER_ACTIVATED } from '../../constants/Constant'
import * as theme from '../../constants/Theme'
import I18n from '../../i18n/i18n'
import NavigationUtil from '../../navigation/NavigationUtil'
import {
    getCart, getCountCart, getUserInfo,
    removeCart, selectAllCartItem, removeCartSuccess
} from '../../redux/actions'
import { showConfirm } from '../../utils/Alert'
import ObjectUtil from '../../utils/ObjectUtil'
import Toast, { BACKGROUND_TOAST } from '../../utils/Toast'
import { requestRemoveCart } from '@api'
import callAPI from '@app/utils/CallApiHelper'

class CartScreen extends Component {

    constructor(props) {
        super(props)
        this.state = {
            isRequesting: false,
            listItemIDSelected: props.navigation.state.params?.listItemIDSelected || []
        }
    }

    getData = () => {
        const { listItemIDSelected } = this.state
        this.props.getCart(listItemIDSelected)
    }


    componentDidMount() {
        const { userInfoState } = this.props
        if (ObjectUtil.isEmpty(userInfoState.data)) this.props.getUserInfo()
        this.getData()
    }

    onRefresh = () => {
        this.getData()
    }

    handleBuyNow() {
        const { cartState, userInfoState } = this.props

        if (userInfoState.data.status !== USER_ACTIVATED) {
            Toast.show('Tài khoản chưa được kích hoạt, vui lòng nạp 300 điểm để được kích hoạt và mua hàng', BACKGROUND_TOAST.FAIL)
            return
        }

        if (cartState.dataSelected.length == 0 || cartState.totalPrice == 0) {
            Toast.show('Vui lòng chọn sản phẩm với đầy đủ số lượng', BACKGROUND_TOAST.FAIL)
            return
        }
        listSelected = cartState.dataSelected.map((item, index) => cartState.data[item]) || []
        NavigationUtil.navigate(SCREEN_ROUTER.CONFIRM_ORDER, {
            from: SCREEN_ROUTER.CART,
            data: listSelected,
            totalPrice: cartState.totalPrice
        })
    }

    handleRemoveCart = async () => {
        const { cartState } = this.props
        const payload = cartState.dataSelected.map((item, index) => cartState.data[item].orderItemID)
        // this.props.removeCart(payload)
        this.setState({ isRequesting: true })

        callAPI({
            API: requestRemoveCart,
            payload,
            onSuccess: res => {
                this.setState({ listItemIDSelected: [] })
                this.props.removeCartSuccess(res.data.listCart)
            },
            onError: err => {
                console.log(err)
            },
            onFinaly: () => {
                this.setState({ isRequesting: false })
            }
        })
    }

    selectAllCartItem() {
        const { cartState } = this.props
        if (cartState.data && cartState.data.length)
            this.props.selectAllCartItem(!(cartState.dataSelected.length == cartState.countDataActive))
    }

    renderBody() {
        const { cartState } = this.props
        if (cartState.isLoading) return (<Loading />)
        if (cartState.error) return (<Error error={cartState.error.toString()} />)
        if (cartState.data.length == 0) return (<Empty title={I18n.t('cart_empty')} urlImage={require('../../assets/images/img_cart_empty.png')}
            onRefresh={() => this.onRefresh()} />)
        return (
            <Block>
                {/* <View style={{
                    flexDirection: 'row',
                    paddingHorizontal: 10,
                    height: 40,
                    alignItems: 'center'
                }}>
                    <TouchableOpacity
                        onPress={() => this.selectAllCartItem()}
                        style={{
                            flexDirection: 'row', alignItems: 'center'
                        }}>
                        <CircleCheckBox
                            outerColor={theme.colors.primary}
                            innerColor={theme.colors.primary}
                            outerSize={13}
                            filterSize={11}
                            innerSize={9}
                            checked={cartState.data.length && cartState.dataSelected.length == cartState.data.length}
                            onToggle={(checked) => this.selectAllCartItem()}
                        />
                        <View style={{ flexDirection: 'row' }}>
                            <Text style={
                                [theme.fonts.robotoRegular14,
                                { marginLeft: 10 }]}>{I18n.t('select_all')} ({cartState.data.length.toString()})</Text>
                        </View>
                    </TouchableOpacity>
                    <View style={{ alignItems: 'flex-end', justifyContent: 'center', flex: 1 }}>
                        <TouchableOpacity
                            onPress={() => {
                                if (!cartState.dataSelected.length) {
                                    // showMessages(I18n.t('notification'), ("Vui lòng chọn sản phẩm muốn xóa!"))
                                    Toast.show("Vui lòng chọn sản phẩm muốn xóa!", BACKGROUND_TOAST.FAIL)
                                }
                                else {
                                    showConfirm(I18n.t('confirm'), (`Bạn muốn xóa ${cartState.dataSelected.length} sản phẩm đã chọn!`),
                                        () => this.handleRemoveCart())
                                }
                            }}>
                            <Text style={[theme.fonts.robotoRegular14, { color: theme.colors.red2 }]}>Xóa</Text>
                        </TouchableOpacity>
                    </View>

                </View> */}
                <Divider />
                <KeyboardAvoidingView
                    style={{ flex: 1, backgroundColor: theme.colors.gray }}
                    behavior={Platform.OS === 'ios' ? 'padding' : null}
                    keyboardVerticalOffset={100} enabled>
                    <ScrollView
                        refreshControl={<RefreshControl refreshing={false} onRefresh={this.onRefresh} />}>
                        {cartState.data.map((item, index) => {
                            return <CartItem
                                index={index} item={item} key={index}
                                isChecked={cartState.dataSelected.includes(index)}
                            />
                        })}
                    </ScrollView>
                </KeyboardAvoidingView>


                <View style={{
                    paddingHorizontal: '2.5%',
                    paddingBottom: 10
                }}>
                    <View style={{
                        flexDirection: 'row',
                        alignItems: 'center'
                    }}>
                        <TouchableOpacity
                            onPress={() => this.selectAllCartItem()}
                            style={{
                                flexDirection: 'row', alignItems: 'center', marginVertical: 5,
                                flex: 1
                            }}>
                            <CircleCheckBox
                                outerColor={theme.colors.primary}
                                innerColor={theme.colors.primary}
                                outerSize={13}
                                filterSize={11}
                                innerSize={9}
                                checked={!!cartState.countDataActive && cartState.dataSelected.length == cartState.countDataActive}
                                onToggle={(checked) => this.selectAllCartItem()}
                            />
                            <View style={{ flexDirection: 'row' }}>
                                <Text style={
                                    [theme.fonts.regular16,
                                    { marginLeft: 10, color: theme.colors.active }]}>{I18n.t('select_all')} ({cartState.countDataActive})</Text>
                            </View>
                        </TouchableOpacity>
                        <TouchableOpacity
                            style={{ alignItems: 'flex-end', justifyContent: 'center', }}
                            onPress={() => {
                                if (!cartState.dataSelected.length) {
                                    Toast.show("Vui lòng chọn sản phẩm muốn xóa!", BACKGROUND_TOAST.FAIL)
                                }
                                else {
                                    showConfirm(I18n.t('confirm'), (`Bạn muốn xóa ${cartState.dataSelected.length} sản phẩm đã chọn!`),
                                        () => this.handleRemoveCart())
                                }
                            }}>
                            <Text style={[theme.fonts.regular16, { color: theme.colors.red2 }]}>Xóa</Text>
                        </TouchableOpacity>
                    </View>

                    <View style={{ flexDirection: 'row', alignItems: 'center', marginVertical: 5 }}>
                        <Text style={{ flex: 1, color: theme.colors.black_title, ...theme.fonts.regular16 }}>Tổng tiền</Text>
                        <NumberFormat
                            style={{ marginTop: 5 }}
                            value={cartState.totalPrice}
                            // value={this.getTotalPrice() || 0}
                            color={theme.colors.red2} perfix='đ' fonts={theme.fonts.robotoMedium20} />
                    </View>
                    <PrimaryButton
                        onPress={() => this.handleBuyNow()}
                        title='Đặt hàng'
                        style={{ width: '99%' }}
                    />
                </View>

            </Block>
        )
    }

    render() {
        const { cartState } = this.props
        const { isRequesting } = this.state
        return (
            <Block color={theme.colors.primary_background}>
                {(cartState.isRemoving || isRequesting) && <LoadingProgress />}
                <DCHeader isWhiteBackground title={I18n.t('cart')} />
                <SafeAreaView style={theme.styles.container}>
                    {this.renderBody()}
                </SafeAreaView>
            </Block>
        )
    }

}

const mapStateToProps = (state) => ({
    cartState: state.cartReducer,
    userInfoState: state[REDUCER.USER],
})

const mapDispatchToProps = {
    getCart,
    selectAllCartItem,
    getCountCart,
    removeCart,
    getUserInfo,
    removeCartSuccess
}

export default connect(mapStateToProps, mapDispatchToProps)(CartScreen)
