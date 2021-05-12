import React, { PureComponent } from 'react'
import { Text, View, Image, StyleSheet } from 'react-native'
import { Divider } from 'react-native-elements'
import { Block, NumberFormat, FastImage } from '.'
import * as theme from '../constants/Theme'

export class OrderItem extends PureComponent {
    render() {
        const { item, index } = this.props
        return (
            <Block marginTop={10}>
                <Block row center marginTop={5} paddingHorizontal={'5%'}>
                    <FastImage source={{ uri: item.image }} style={styles.image} resizeMode='contain' />
                    <Block paddingLeft={10}>
                        <Text numberOfLines={1}
                            style={[theme.fonts.regular16]}>
                            {item.itemName}</Text>
                        {/* <Text style={theme.fonts.robotoRegular12}>SL: {item.qty}</Text> */}
                        <View style={{
                            flexDirection: 'row',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            marginTop: 20
                        }}>
                            <NumberFormat
                                style={{
                                    // alignSelf: 'flex-end'
                                }}
                                fonts={theme.fonts.regular15} color={theme.colors.red_money}
                                value={item.sumPrice} perfix='VNĐ' />
                            <Text style={{
                                color: theme.colors.black1,
                                ...theme.fonts.regular16
                            }}>x {item.qty}</Text>

                        </View>

                    </Block>
                </Block>
                <Divider style={{ marginTop: 5, }} />
            </Block>
        )
    }
}

const styles = StyleSheet.create({
    image: {
        width: 75,
        height: 75,
    }
})

export default OrderItem
