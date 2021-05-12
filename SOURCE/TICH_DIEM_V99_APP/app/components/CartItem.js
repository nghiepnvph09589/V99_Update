import React, { Component } from 'react'
import { connect } from 'react-redux'
import { View, Text, Image, TouchableOpacity, TextInput, Platform, StyleSheet } from 'react-native'
import { Block, NumberFormat, FastImage } from '.'
import * as theme from '../constants/Theme'
import I18n from '../i18n/i18n'
import reactotron from 'reactotron-react-native'
import CircleCheckBox, { LABEL_POSITION } from 'react-native-circle-checkbox'
import { updateCartItem, cartItemTouch, selectAllCartItem, removeCartSuccess } from '../redux/actions'
import R from '@app/assets/R'
import callAPI from '@app/utils/CallApiHelper'
import { requestRemoveCart } from '@app/constants/Api'
import LoadingProgress from './LoadingProgress'
import { showConfirm } from '@app/utils/Alert'

MAX_QTY = 999
MIN_QTY = 1

class CartItem extends Component {

    state = {
        isRequesting: false
    }

    updateQuantity = (qty) => {
        const { item, index, isChecked, cartState } = this.props
        tmpItem = {
            ...item,
            qty: qty,
            sumPrice: qty * item.itemPrice
        }
        this.props.updateCartItem(tmpItem, index, isChecked)
        if (qty == 0) this.props.cartItemTouch(index, true)
    }

    cartItemTouch = () => {
        const { item, index, isChecked, cartState } = this.props
        this.props.cartItemTouch(index, isChecked)
    }

    onLongPressRemoveCart = (item) => {
        showConfirm(R.strings().confirm, 'Bạn muốn xóa sản phẩm này?', () => this.handleRemoveCart(item))
    }

    handleRemoveCart = async (item) => {
        this.setState({ isRequesting: true })
        const payload = [item.orderItemID]

        callAPI({
            API: requestRemoveCart,
            payload,
            onSuccess: res => {
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

    render() {
        const { item, index, isChecked, cartState } = this.props
        if (typeof item.qty == 'undefined') {
            item.qty = 0;
        }
        return (
            <TouchableOpacity
                // disabled={!item.status}
                activeOpacity={item.status !== 0 ? 0.2 : 1}
                onPress={() => {
                    if (item.status !== 0) this.cartItemTouch()
                }}
                onLongPress={() => {
                    if (item.status == 0) this.onLongPressRemoveCart(item)
                }}
                style={[{
                    paddingHorizontal: 10,
                    paddingVertical: 10,
                    flexDirection: 'row',
                    alignItems: 'center',
                    marginTop: 5,
                    backgroundColor: theme.colors.white,
                    opacity: item.status == 1 && 1 || 0.6
                },
                ]}>
                <CircleCheckBox
                    outerColor={theme.colors.primary}
                    innerColor={theme.colors.primary}
                    outerSize={13}
                    filterSize={11}
                    innerSize={9}
                    checked={isChecked}
                    onToggle={(checked) => this.cartItemTouch()}
                />
                {this.state.isRequesting && <LoadingProgress />}
                <FastImage source={{ uri: item.image }} style={{
                    width: 85,
                    height: 85,
                    marginHorizontal: 10,
                }} resizeMode='contain' />
                <Block>
                    <Text numberOfLines={2} style={[theme.fonts.regular16]}>{item.itemName}</Text>

                    <NumberFormat fonts={theme.fonts.regular15}
                        color={theme.colors.red_money}
                        value={item.sumPrice} perfix='VNĐ' />
                    <View style={{ flexDirection: 'row', }}>
                        <View style={{
                            width: 120,
                            height: 35,
                            borderColor: theme.colors.border,
                            borderWidth: 0.25,
                            flexDirection: 'row',
                            alignItems: 'center',
                            borderRadius: 3,
                            marginTop: 5,
                            marginRight: 15,
                            // marginLeft: 10
                        }}>
                            <TouchableOpacity
                                disabled={!item.status}
                                style={[styles.viewAction, {
                                    borderRightWidth: 0.25
                                }]}
                                onPress={() => {
                                    if (item.qty == MIN_QTY) return
                                    this.updateQuantity(item.qty - 1)
                                }}>
                                <Text style={styles.actionText}>-</Text>
                            </TouchableOpacity>
                            <Block flex={1.5}
                                padding={5} center middle
                            >
                                <TextInput
                                    editable={item.status != 0}
                                    style={[theme.fonts.robotoRegular14, {
                                        width: '100%',
                                        textAlign: 'center',
                                        textAlignVertical: 'center',
                                        height: Platform.OS === 'ios' ? '100%' : 40,
                                        marginTop: Platform.OS === 'ios' ? 0 : 25,
                                    }]}
                                    keyboardType="number-pad"
                                    type='number'
                                    min='1'
                                    returnKeyType='done'
                                    onChangeText={(text) => {
                                        var tmp = parseInt(text, 10)
                                        if (tmp > MAX_QTY) tmp = MAX_QTY
                                        var value = (isNaN(tmp) ? 0 : tmp)
                                        this.updateQuantity(value)
                                    }}
                                    value={item.qty.toString()}
                                />
                            </Block>
                            <TouchableOpacity
                                disabled={!item.status}
                                style={[styles.viewAction, {
                                    borderLeftWidth: 0.25,
                                }]}
                                onPress={() => {
                                    if (item.qty == MAX_QTY) return
                                    this.updateQuantity(item.qty + 1)
                                }}>
                                <Text style={styles.actionText}>+</Text>
                                {/* <Icon.AntDesign name="plus" size={20} color={theme.colors.primaryDark} outline /> */}
                            </TouchableOpacity>
                        </View>


                    </View>
                    {!item.status && <Text
                        style={{
                            textAlign: 'right',
                            textAlignVertical: 'bottom',
                            color: theme.colors.red
                        }}
                        children={'Hết hàng (Nhấn giữ để xóa)'} />}
                </Block>
            </TouchableOpacity>
        )
    }
}

const styles = StyleSheet.create({
    actionText: {
        color: theme.colors.text_gray,
        ...theme.fonts.robotoRegular16,
        fontSize: 30,
        fontWeight: 'normal',
    },
    viewAction: {
        flex: 1,
        height: '100%',
        justifyContent: 'center',
        alignItems: 'center',
        borderColor: theme.colors.border,
    }
})

const mapStateToProps = (state) => ({
    cartState: state.cartReducer
})

const mapDispatchToProps = {
    updateCartItem,
    cartItemTouch,
    selectAllCartItem,
    removeCartSuccess
}

export default connect(mapStateToProps, mapDispatchToProps)(CartItem)
