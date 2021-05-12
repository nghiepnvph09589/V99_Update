import React, { Component } from 'react'
import { View, Text, SafeAreaView, Keyboard } from 'react-native'
import { connect } from 'react-redux'
import i18n from '../../i18n/i18n'
import { Block, DCHeader, ScrollableTabView } from '../../components'
import * as theme from '../../constants/Theme'
import ListGiftScreen from './ListGiftScreen'
import CardScreen from './CardScreen'
import { GIFT_TYPE, REDUCER } from '../../constants/Constant'
import { getBankSelect, getBank, navigateTab } from "@action";
import reactotron from 'reactotron-react-native'

export class UsePointScreen extends Component {

    initialPage = this.props.navigation.getParam('initialPage')

    componentDidMount() {
        this.props.getBankSelect()
        this.props.getBank()
    }

    onChangeTab = ({ i, from }) => this.props.navigateTab(i);

    render() {
        return (
            <Block>
                <SafeAreaView style={theme.styles.container}>
                    <ScrollableTabView
                        onChangeTab={this.onChangeTab}
                        page={this.props.navigationState.initialPage}
                        initialPage={this.initialPage}
                    >
                        <ListGiftScreen tabLabel={'Ví Point'} key={GIFT_TYPE.GIFT} />
                        <CardScreen tabLabel={'Ví điểm tích'} key={GIFT_TYPE.CARD} />
                    </ScrollableTabView>
                </SafeAreaView>
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
    navigationState: state[REDUCER.NAVIGATE_TAB]
})

const mapDispatchToProps = {
    getBankSelect,
    getBank,
    navigateTab
}

export default connect(mapStateToProps, mapDispatchToProps)(UsePointScreen)
