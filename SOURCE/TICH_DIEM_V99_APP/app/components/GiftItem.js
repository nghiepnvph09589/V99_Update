import React, { Component } from 'react'
import {
    Text, View, TouchableOpacity,
    StyleSheet, Image
} from 'react-native'
import * as theme from '../constants/Theme'
import { SCREEN_ROUTER } from '../constants/Constant'
import NumberFormat from './NumberFormat'
import i18n from '../i18n/i18n'
import { Block, FastImage } from '.'
import NavigationUtil from '../navigation/NavigationUtil'

export class GiftItem extends Component {
    render() {
        const { item } = this.props
        return (
            <TouchableOpacity
                onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.GIFT_DETAIL, { item: item })}
                style={[styles.item]}>
                <FastImage
                    style={[styles.image]}
                    source={{
                        uri: item.urlImage,
                    }}
                    resizeMode='cover'
                />

                <Block paddingHorizontal={10} space='between'>
                    <Text style={[theme.fonts.robotoMedium16, {
                        marginBottom: 10
                    }]}> {item.giftName} </Text>
                    <NumberFormat
                        value={item.point}
                        color={theme.colors.red}
                        fonts={theme.fonts.robotoMedium16}
                        perfix={i18n.t('point')} />
                </Block>
            </TouchableOpacity>
        )
    }
}

export default GiftItem

const styles = StyleSheet.create({
    item: {
        flexDirection: 'row',
        marginHorizontal: 5,
        marginTop: 10,
        padding: 10,
        borderRadius: 5,
        backgroundColor: theme.colors.white,
        shadowColor: theme.colors.black,
        shadowOffset: {
            width: 0,
            height: 1,
        },
        shadowOpacity: 0.22,
        shadowRadius: 2.22,
        elevation: 3,
    },
    image: {
        width: 75,
        height: 75,
        borderRadius: 5,
    }
});