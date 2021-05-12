import React, { Component } from 'react'
import {
    View, Text, ImageBackground,
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

const bottomButton = 25
const padding_horizontal = 10
export class CardScreen extends Component {

    constructor(props) {
        const { cardState } = props
        super(props)
        this.state = {
            indexCarrierSelected: 0,
            indexPriceSeleted: null,
            priceSelected: null,
            token: null
        }
    }

    componentDidMount() {
    }

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
                    value={userState.pointRanking} perfix={R.strings().point} />
            </Block>
        </FstImage>)
    }

    renderScrollableTabView = () => {
        return (
            <Block>
                <ScrollableTabView
                    style={{
                        borderColor: theme.colors.border,
                    }}
                    tabBarBackgroundColor={theme.colors.white}
                    tabBarPosition='top'
                    tabBarActiveTextColor={theme.colors.primary}
                    tabBarInactiveTextColor={theme.colors.black1}
                    tabBarUnderlineStyle={{
                        height: 2,
                        backgroundColor: theme.colors.primary
                    }}
                    renderTabBar={() =>
                        <DefaultTabBar
                            style={{
                                alignSelf: 'center',
                                paddingTop: 8,
                            }} />
                    }
                    tabBarTextStyle={theme.fonts.semibold18}
                    onChangeTab={Keyboard.dismiss}>
                    <WalletAccumulatePointsScreen tabLabel={'Lịch sử'} key={1} />
                    <PolicyScreen tabLabel={'Chính sách'} key={0} />
                </ScrollableTabView>
            </Block>
        )
    }

    render() {
        return (
            <Block color={theme.colors.primary_background}>
                {this.renderViewPoint()}
                {this.renderScrollableTabView()}
            </Block>
        )
    }
}


const styles = StyleSheet.create({
    title: {
        marginVertical: 10,
        marginLeft: 5,
        ...theme.fonts.robotoMedium16,
    }
})

const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
})

const mapDispatchToProps = {
}

export default connect(mapStateToProps, mapDispatchToProps)(CardScreen)
