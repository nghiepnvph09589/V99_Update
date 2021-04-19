import React, { Component } from 'react'
import {
    View, Text,
    FlatList, RefreshControl,
    TouchableOpacity
} from 'react-native'
import { connect } from 'react-redux'
import { getWalletAccumulate, getWalletPoints } from '../../redux/actions'
import { GET_HISTORY_POINT_TYPE, GIFT_TYPE, REDUCER, SCREEN_ROUTER, USER_ACTIVATED } from '../../constants/Constant'
import {
    GiftItem, Block, Loading, Error,
    Empty, FstImage, NumberFormat
} from '../../components'
import reactotron from 'reactotron-react-native'
import * as theme from '../../constants/Theme'
import R from '@app/assets/R'
import NavigationUtil from '@app/navigation/NavigationUtil'
import WalletPointsItem from '@app/components/WalletPointsItem'
import Toast, { BACKGROUND_TOAST } from '@app/utils/Toast'
export class ListGiftScreen extends Component {

    componentDidMount() {
        this.getData()
    }

    getData = () => {
        const payload = {
            page: 1,
            type: GET_HISTORY_POINT_TYPE.WALLET_POINTS,
            typePoint: 0
        }
        this.props.getWalletPoints(payload)
    }



    renderViewPoint = () => {
        const { userState } = this.props
        return (<FstImage
            style={{ width, height: width * 0.4 }}
            source={R.images.img_decor}
            resizeMode='contain'>
            <Block center style={{ marginTop: 30 }}>
                <Text style={{
                    color: theme.colors.active,
                    marginBottom: 5,
                    ...theme.fonts.semibold16
                }}>Điểm của bạn</Text>
                <NumberFormat
                    fonts={theme.fonts.semibold25}
                    color={theme.colors.active}
                    value={userState.point} perfix={R.strings().point} />
            </Block>
        </FstImage>)
    }

    renderViewOptions = () => {
        const { userState } = this.props
        return (
            <View style={{
                backgroundColor: 'white',
                flexDirection: 'row',
                padding: '2%',
                shadowOffset: {
                    width: 0,
                    height: 1
                },
                shadowOpacity: 0.2,
                shadowRadius: 2,
                elevation: 3,
                marginHorizontal: '5%',
                borderRadius: 5,
                marginTop: -50
            }}>
                <Option
                    onPress={() => {
                        NavigationUtil.navigate(SCREEN_ROUTER.LOAD_POINTS);
                    }}
                    text={R.strings().load_point}
                    img={R.images.img_draw_points}
                />
                <Option
                    onPress={() => {
                        if (userState.status !== USER_ACTIVATED) {
                            Toast.show('Tài khoản chưa được kích hoạt, vui lòng nạp 300 điểm để được kích hoạt và sử dụng điểm', BACKGROUND_TOAST.FAIL)
                            return
                        }
                        NavigationUtil.navigate(SCREEN_ROUTER.DRAW_POINTS);
                    }}
                    text={R.strings().draw_points}
                    img={R.images.draw_points}
                />
                <Option
                    text={R.strings().moving_point}
                    img={R.images.moving_point}
                    onPress={() => {
                        if (userState.status !== USER_ACTIVATED) {
                            Toast.show('Tài khoản chưa được kích hoạt, vui lòng nạp 300 điểm để được kích hoạt và sử dụng điểm', BACKGROUND_TOAST.FAIL)
                            return
                        }
                        NavigationUtil.navigate(SCREEN_ROUTER.MOVING_POINTS);
                    }}
                />
            </View>)
    }

    renderFlastlist = () => {
        const { walletPointsState } = this.props
        if (walletPointsState.isLoading) return <Loading />
        if (walletPointsState.error) return <Error onPress={this.getData} />
        if (!walletPointsState.data.length) return <Empty />

        return <FlatList
            refreshControl={<RefreshControl refreshing={false} onRefresh={this.getData} />}
            style={{ backgroundColor: 'white', }}
            data={walletPointsState.data}
            keyExtractor={(item, index) => index.toString()}
            renderItem={this.renderItem}
        />
    }

    renderItem = ({ item, index }) => {
        return <WalletPointsItem item={item} index={index} />

    }

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
        const { text, img, onPress } = this.props;
        return (
            <TouchableOpacity
                style={{
                    alignItems: "center",
                    flex: 1
                }}
                onPress={onPress}
            >
                <FstImage
                    style={{ height: 50, width: 50 }}
                    resizeMode="contain"
                    source={img}
                />
                <Text
                    style={[
                        theme.fonts.regular14,
                        {
                            width: "80%",
                            textAlign: "center",
                            marginTop: 5,
                            color: theme.colors.black_title
                        }
                    ]}
                    numberOfLines={2}
                >
                    {text}
                </Text>
            </TouchableOpacity>
        );
    }
}


const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
    walletPointsState: state[REDUCER.WALLET_POINTS]
})

const mapDispatchToProps = {
    getWalletPoints
}

export default connect(mapStateToProps, mapDispatchToProps)(ListGiftScreen)
