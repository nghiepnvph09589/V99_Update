import { Block, DCHeader, Empty, Loading } from '@app/components'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, SafeAreaView, FlatList, RefreshControl } from 'react-native'
import { connect } from 'react-redux'
import { getHistoryDrawPoints } from '@action'
import { ACTION_POINTS_TYPE, GET_HISTORY_POINT_TYPE, REDUCER, SCREEN_ROUTER } from '@app/constants/Constant'
import WalletPointsItem from '@app/components/WalletPointsItem'
import NavigationUtil from '@app/navigation/NavigationUtil'

export class HistoryDrawPointsScreen extends Component {

    componentDidMount() {
        this.getData()
    }

    getData = () => {
        const payload = {
            page: 1,
            type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
            typePoint: ACTION_POINTS_TYPE.DRAW_POINTS
        }
        this.props.getHistoryDrawPoints(payload)
    }

    renderFlastlist = () => {
        const { historyPointsState } = this.props
        if (historyPointsState.isLoading) return <Loading />
        if (historyPointsState.error) return <Error />
        if (!historyPointsState.data.length) return <Empty />

        return <FlatList
            refreshControl={<RefreshControl refreshing={false} onRefresh={this.getData} />}
            data={historyPointsState.data}
            keyExtractor={(item, index) => index}
            renderItem={this.renderItem}
        />
    }

    renderItem = ({ item, index }) => {
        
        return <WalletPointsItem item={item}
            index={index}
            onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_DRAW_POINTS, { id: item.historyID })} />

    }

    render() {
        return (
            <Block>
                <DCHeader isWhiteBackground
                    title={'Lịch sử rút điểm'} />
                <SafeAreaView style={theme.styles.container}>
                    {this.renderFlastlist()}
                </SafeAreaView>
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({
    historyPointsState: state[REDUCER.HISTORY_DRAW_POINTS]
})

const mapDispatchToProps = {
    getHistoryDrawPoints
}

export default connect(mapStateToProps, mapDispatchToProps)(HistoryDrawPointsScreen)
