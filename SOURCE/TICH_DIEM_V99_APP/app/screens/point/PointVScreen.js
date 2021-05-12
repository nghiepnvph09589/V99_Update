import React, { Component } from 'react'
import {
    Button,
    View, Text,FlatList, ImageBackground,
    ScrollView, TouchableOpacity, RefreshControl, StyleSheet, Keyboard
} from 'react-native'
import { connect } from 'react-redux'
import { getListGift, exchangeGift } from '../../redux/actions'
import { GIFT_TYPE, REDUCER, SCREEN_ROUTER } from '../../constants/Constant'
import {
    Block, NumberFormat, Loading,
    PrimaryButton, LoadingProgress,
    Error, Empty, FstImage
} from '../../components'
import reactotron from 'reactotron-react-native'
import * as theme from '../../constants/Theme'
import I18n from '../../i18n/i18n'
import { showConfirm, showMessages } from '../../utils/Alert'
import ObjectUtil from '../../utils/ObjectUtil'
import NavigationUtil from '../../navigation/NavigationUtil'
import Toast, { BACKGROUND_TOAST } from '../../utils/Toast'
import { requireLogin } from '../../utils/AlertRequireLogin'
import AsyncStorage from "@react-native-community/async-storage"
import R from '@app/assets/R'
import ScrollableTabView, { DefaultTabBar, ScrollableTabBar } from 'react-native-scrollable-tab-view'
import PolicyScreen from './PolicyScreen'
import WalletAccumulatePointsScreen from './WalletAccumulatePointsScreen'
// import WalletPointsItem from '@app/components/WalletPointsItem'

export class PointVScreen extends Component {
    renderViewPoint = () => {
        const { userState } = this.props
        return (<FstImage
            style={{ width, height: width * 0.4 }}
            source={R.images.img_decor}
            resizeMode='contain'>
            <Block center middle>
                <Text style={{
                    color: theme.colors.active,
                    marginBottom: 5,
                    ...theme.fonts.semibold16
                }}>Điểm của bạn</Text>
                <NumberFormat
                    fonts={theme.fonts.semibold25}
                    color={theme.colors.active}
                    value={userState.pointV} perfix={R.strings().point_V} />
            </Block>
        </FstImage>)
    }
    renderViewOptions = () => {
        const { userState } = this.props
        return (
            <View style={{
                padding: '2%',
                paddingLeft: 240,
                marginHorizontal: '4%',
                marginTop: -60,
            }}>
                <Option
                    text={R.strings().moving_point}
                    onPress={() => { }}
                />
            </View>)
    }

    // renderFlastlist = () => {
    //     const { walletPointsState } = this.props
    //     if (walletPointsState.isLoading) return <Loading />
    //     if (walletPointsState.error) return <Error onPress={this.getData} />
    //     if (!walletPointsState.data.length) return <Empty />

    //     return <FlatList
    //         refreshControl={<RefreshControl refreshing={false} onRefresh={this.getData} />}
    //         style={{ backgroundColor: 'white', }}
    //         data={walletPointsState.data}
    //         keyExtractor={(item, index) => index.toString()}
    //         renderItem={this.renderItem}
    //     />
    // }

    // renderItem = ({ item, index }) => {
    //     return <WalletPointsItem item={item} index={index} />

    // }
    render() {
        return (
            <Block color={theme.colors.primary_background}>
                {this.renderViewPoint()}
                {this.renderViewOptions()}
                {this.renderFlastlist()}
            </Block>
        )
    }
}
class Option extends Component {
    render() {
        const { text, onPress } = this.props;
        return (
            <TouchableOpacity
                style={{
                    alignItems: "center",
                    backgroundColor: "#00bfff",
                    padding: 8,
                    paddingTop: 5,
                    paddingEnd: 5,
                    borderRadius: 8
                }}
                onPress={onPress}>
                <Text style={{ color: 'white' }}>{text}</Text>
            </TouchableOpacity>
        );
    }
}
const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
    // walletPointsState: state[REDUCER.WALLET_POINTS]
    //
})

const mapDispatchToProps = {
}
export default connect(mapStateToProps, mapDispatchToProps)(PointVScreen)