import React, { PureComponent } from 'react'
import { Text, View, TouchableOpacity } from 'react-native'
import { Block, FastImage } from '.'
import * as theme from '../constants/Theme'

export class CartRightComponent extends PureComponent {

    render() {
        const { onPress, quantity, ...props } = this.props
        return (
            <TouchableOpacity
                onPress={onPress}
                style={{
                    flexDirection: 'row',
                    marginRight: 10,
                }} {...props}>
                {/* <Text style={[theme.fonts.robotoRegular12, {
                    color: theme.colors.white,
                    alignSelf: 'center'
                }]}>({quantity})</Text> */}
                <FastImage
                    resizeMode='contain'
                    style={{
                        width: 25,
                        height: 25
                    }}
                    tintColor='white'
                    source={require('../assets/images/ic_cart.png')} />
            </TouchableOpacity>
        )
    }
}

export default CartRightComponent
