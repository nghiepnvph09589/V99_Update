import { Block, DCHeader, Empty, Loading } from '@app/components'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, SafeAreaView, FlatList, RefreshControl } from 'react-native'
import { connect } from 'react-redux'
import { getHistoryDrawPoints } from '@action'
import { ACTION_POINTS_TYPE, GET_HISTORY_POINT_TYPE, REDUCER, SCREEN_ROUTER } from '@app/constants/Constant'
import WalletPointsItem from '@app/components/WalletPointsItem'
import NavigationUtil from '@app/navigation/NavigationUtil'
import { GET_HISTORY_MOVING_POINTS } from '@app/redux/actions/type'
import { getHistoryPoints } from '@action'

export class HistoryMovingPointsScreen extends Component {

    componentDidMount() {
        this.getData()
    }

    getData = () => {
        const payload = {
            page: 1,
            type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
            typePoint: ACTION_POINTS_TYPE.MOVING_POINTS
        }
        const type = {
            type: GET_HISTORY_MOVING_POINTS
        }
        this.props.getHistoryPoints(type, payload)
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
            onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.MOVING_POINTS_SUCCESS, { id: item.historyID })} />

    }

    render() {
        return (
            <Block>
                <DCHeader isWhiteBackground
                    title={'Lịch sử chuyển điểm'} />
                <SafeAreaView style={theme.styles.container}>
                    {this.renderFlastlist()}
                </SafeAreaView>
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({
    historyPointsState: state[REDUCER.HISTORY_MOVING_POINTS]
})

const mapDispatchToProps = {
    getHistoryPoints
}

export default connect(mapStateToProps, mapDispatchToProps)(HistoryMovingPointsScreen)
