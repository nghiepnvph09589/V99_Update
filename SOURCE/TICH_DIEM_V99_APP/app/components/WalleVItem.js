import R from '@app/assets/R'
import { ACTION_POINTS_TYPE, SCREEN_ROUTER } from '@app/constants/Constant'
import theme, { colors } from '@app/constants/Theme'
import React, { Component } from 'react'
import { Text, TouchableOpacity, View } from 'react-native'
import { NumberFormat } from '.'
import Dash from 'react-native-dash';
import DateUtil from '@app/utils/DateUtil'
import NavigationUtil from '@app/navigation/NavigationUtil'



export const TitleWalletPointsEnum = {
    [ACTION_POINTS_TYPE.ADD_POINTS]: {
        color: '#00C48C',
        title: R.strings().add_point,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.AWARDED_POINTS]: {
        color: '#00C48C',
        title: R.strings().awarded,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.DRAW_POINTS]: {
        color: '#007AFF',
        title: R.strings().draw_points,
        sign: '-',
    },
    [ACTION_POINTS_TYPE.FEE_USE_APP]: {
        color: '#F39C12',
        title: R.strings().processing,
        sign: '-',
    },
    [ACTION_POINTS_TYPE.MOVING_POINTS]: {
        color: '#F39C12',
        title: R.strings().moving_point,
        sign: '-',
    },
    [ACTION_POINTS_TYPE.SYSTEM_ADD_POINTS]: {
        color: '#F39C12',
        title: R.strings().add_point,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.REFUNS_POINTS]: {
        color: '#F39C12',
        title: R.strings().refuns_points,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.MINUS_POINTS]: {
        color: '#F39C12',
        title: R.strings().minus_point,
        sign: '-',
    },
    [ACTION_POINTS_TYPE.REWARD_POINTS]: {
        color: '#00C48C',
        title: R.strings().reward_points,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.POINT_V]: {
        color: '#F39C12',
        title: R.strings().minus_point,
        sign: '-',
    },
    [ACTION_POINTS_TYPE.V_POINT]: {
        color: '#00C48C',
        title: R.strings().add_point,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.REFERENCE]: {
        color: '#00C48C',
        title: R.strings().add_point,
        sign: '+',
    },
    [ACTION_POINTS_TYPE.REGISTER]: {
        color: '#00C48C',
        title: R.strings().add_point,
        sign: '+',
    },
}

class WalletPointsItem extends Component {
    render() {
        const { item, index, onPress } = this.props
        const userAction = item.type == ACTION_POINTS_TYPE.AWARDED_POINTS ? 'Người gửi: ' : 'Người nhận: '
        // console.log(item)
        return (
            <TouchableOpacity
                onPress={onPress}
                style={{
                    paddingHorizontal: '2.5%',
                    marginTop: 10
                }}
                activeOpacity={!!!onPress && 1}>
                <Text style={{
                    color: theme.colors.black_title,
                    ...theme.fonts.regular16
                }}>
                    {DateUtil.formatTime(item.createDate)} {DateUtil.formatShortDate(item.createDate)}</Text>
                <View style={[{
                    flex: 1,
                    borderWidth: 0.5,
                    borderColor: theme.colors.border,
                    borderRadius: 5,
                    flexDirection: 'row',
                    marginTop: 5
                },
                (item.type == ACTION_POINTS_TYPE.AWARDED_POINTS || item.type == ACTION_POINTS_TYPE.MOVING_POINTS) && {
                    borderBottomLeftRadius: 0,
                    borderBottomRightRadius: 0,
                }]}>
                    <View style={{ flex: 1, paddingHorizontal: '2.5%', paddingVertical: 10, }}>
                        <Text style={{
                            color: TitleWalletPointsEnum[item.type].color,
                            marginBottom: 10,
                            ...theme.fonts.regular16
                        }}>{TitleWalletPointsEnum[item.type].title}</Text>
                        <NumberFormat
                            title={'Số dư'}
                            value={item.balance}
                            perfix='V'
                            color={theme.colors.black_title}
                            fonts={theme.fonts.regular16}
                        />
                    </View>
                    {/* <Dash dashColor={theme.colors.border} style={{ width: 1, height: '100%', flexDirection: 'column' }} /> */}
                    <View style={{
                        flexDirection: 'row',
                        justifyContent: 'center',
                        alignItems: 'center',
                        maxWidth: width * 0.2,
                        minWidth: width * 0.2,
                    }}>
                        <Text style={{
                            color: TitleWalletPointsEnum[item.type].color,
                            ...theme.fonts.regular16
                        }}>
                            {TitleWalletPointsEnum[item.type].sign}
                        </Text>
                        <NumberFormat
                            value={item.point}
                            perfix='V'
                            color={TitleWalletPointsEnum[item.type].color}
                            fonts={theme.fonts.regular16}
                        />
                    </View>

                </View>
                {(item.type == ACTION_POINTS_TYPE.AWARDED_POINTS || item.type == ACTION_POINTS_TYPE.MOVING_POINTS) && (
                    <View style={{
                        borderBottomWidth: 0.5,
                        borderLeftWidth: 0.5,
                        borderRightWidth: 0.5,
                        borderColor: colors.border,
                        borderBottomLeftRadius: 5,
                        borderBottomRightRadius: 5,
                        padding: '2%'
                    }}>
                        <Text
                            style={{
                                fontFamily: R.fonts.regular,
                                fontSize: 16,
                                color: colors.black_title
                            }}
                            children={userAction + item.userName + ' - ' + item.userPhone}
                        />
                    </View>
                )}

            </TouchableOpacity>
        )
    }
}

export default WalletPointsItem
