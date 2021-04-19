import { Block, Empty, Loading } from '@app/components'
import WalletAccumulatePointsItem from '@app/components/WalletAccumulatePointsItem'
import { GET_HISTORY_POINT_TYPE, REDUCER } from '@app/constants/Constant'
import React, { Component } from 'react'
import { View, Text, RefreshControl, FlatList } from 'react-native'
import { connect } from 'react-redux'
import { getWalletAccumulate } from '../../redux/actions'

export class WalletAccumulatePointsScreen extends Component {

    componentDidMount() {
        this.getData()
    }

    getData = () => {
        const payload = {
            page: 1,
            type: GET_HISTORY_POINT_TYPE.WALLET_ACCUMULATE_POINTS,
            typePoint: ''
        }
        this.props.getWalletAccumulate(payload)
    }

    renderFlastlist = () => {
        const { walletPointsState } = this.props
        if (walletPointsState.isLoading) return <Loading />
        if (walletPointsState.error) return <Error onPress={this.getData} />
        if (!walletPointsState.data.length) return <Empty onRefresh={this.getData} />

        return <FlatList
            refreshControl={<RefreshControl refreshing={false} onRefresh={this.getData} />}
            data={walletPointsState.data}
            keyExtractor={(item, index) => index}
            renderItem={this.renderItem}
        />
    }

    renderItem = ({ item, index }) => {
        return <WalletAccumulatePointsItem item={item} index={index} />
    }

    render() {
        return (
            <Block>
                {this.renderFlastlist()}
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({
    walletPointsState: state[REDUCER.WALLET_ACCUMULATE_POINTS]
})

const mapDispatchToProps = {
    getWalletAccumulate
}

export default connect(mapStateToProps, mapDispatchToProps)(WalletAccumulatePointsScreen)
