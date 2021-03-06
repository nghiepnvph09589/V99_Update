import R from '@app/assets/R'
import { Block, DCHeader, Error, FstImage, Loading, NumberFormat } from '@app/components'
import theme from '@app/constants/Theme'
import DateUtil from '@app/utils/DateUtil'
import React, { Component } from 'react'
import { View, Text, SafeAreaView, ScrollView } from 'react-native'
import { connect } from 'react-redux'
import { DRAW_POINTS_STATUS } from '@constant'
import { REDUCER } from '@app/constants/Constant'
import { requestGetHistyoriesDetail } from '@api'
import { getBank } from '@action'

export const DrawPointsStatusEnum = {
    [DRAW_POINTS_STATUS.PENDING]: {
        title: R.strings().pending,
        color: theme.colors.active
    },
    [DRAW_POINTS_STATUS.CONFIRMED]: {
        title: R.strings().confirm,
        color: '#00C48C'
    },
    [DRAW_POINTS_STATUS.REFUSE]: {
        title: R.strings().refuse,
        color: theme.colors.red_money
    },
    [null]: {
        title: '',
        color: theme.colors.red_money
    },
}


export class DetailDrawPointsScreen extends Component {

    constructor(props) {
        super(props)
        this.item = props.navigation.getParam('item')
        this.id = props.navigation.getParam('id')

        this.state = {
            data: !!this.id ? {} : this.item,
            isLoading: !!this.id,
            error: null
        }
    }

    componentDidMount() {
        this.getData()
    }

    getData = async () => {
        if (!!!this.id) {
            return
        }

        this.props.getBank()
        this.setState({ isLoading: true, error: null })

        const payload = {
            id: this.id
        }

        try {
            const res = await requestGetHistyoriesDetail(payload)
            this.setState({ data: res.data })
        } catch (error) {
            this.setState({ error: 'ddd' })
            console.log(error);
        }
        finally { this.setState({ isLoading: false }) }
    }

    renderData = () => {
        const { isLoading, data, error } = this.state
        const { bankState } = this.props

        // console.log(data)
        // console.log(item)
        if (isLoading) return <Loading />
        if (error) return <Error reload={this.getData} />
        
        return (
            <Block>
                <View style={{
                    marginHorizontal: '2.5%',
                    borderBottomWidth: 1,
                    borderBottomColor: '#B2BEC3',
                    paddingBottom: 10,
                    flexDirection: 'row',
                    alignItems: 'center',
                }}>
                    <Text style={{
                        flex: 1,
                        color: theme.colors.black_title,
                        ...theme.fonts.regular18
                    }}>Tr???ng th??i</Text>
                    <Text style={{
                        color: DrawPointsStatusEnum[data.status].color,
                        ...theme.fonts.regular16
                    }}>{DrawPointsStatusEnum[data.status].title}</Text>
                </View>

                <View style={{
                    flexDirection: 'row',
                    alignItems: 'center',
                    paddingHorizontal: '2.5%',
                    marginTop: 10
                }}>
                    <Text style={{
                        flex: 1,
                        color: theme.colors.black_title,
                        ...theme.fonts.regular18
                    }}>Th???i gian</Text>
                    <Text style={{
                        color: theme.colors.black_title,
                        ...theme.fonts.regular16
                    }}>{DateUtil.formatTime(data.createDate)} {DateUtil.formatShortDate(data.createDate)}</Text>
                </View>

                <Text style={{
                    color: theme.colors.black_title,
                    marginTop: 20,
                    marginBottom: 5,
                    marginLeft: '2.5%',
                    ...theme.fonts.regular16
                }}>Chuy???n ti???n v??o t??i kho???n</Text>

                <View style={{
                    flexDirection: 'row',
                    alignItems: 'center',
                    paddingHorizontal: '2.5%',
                    marginBottom: 10,
                }}>
                    <FstImage
                        source={{ uri: data.bankInfo?.urlImg }}
                        style={{
                            width: width * 0.1,
                            aspectRatio: 1,
                            borderRadius: width * 0.1
                        }}
                        resizeMode='stretch'
                    />
                    <Text style={{
                        marginLeft: 10,
                        color: theme.colors.black1,
                        ...theme.styles.regular20
                    }}>{data.bankInfo?.shortName}</Text>

                </View>
                {
                    bankState.data.map((item, index) => {
                        return item.id === data.bankInfo?.bankID &&
                            <View style={{marginLeft: 10, marginBottom: 20}}>
                                <Text style={{color: theme.colors.black1}}>S??? t??i kho???n: {item.codeBankAccount}</Text>
                                <Text style={{color: theme.colors.black1}}>Ch??? t??i kho???n: {item.userName}</Text>
                            </View>
                    })
                }

                <View style={{
                    marginHorizontal: '2.5%',
                    borderBottomWidth: 1,
                    borderBottomColor: '#B2BEC3',
                    paddingBottom: 5,
                    marginBottom: 10,
                    flexDirection: 'row',
                    alignItems: 'center',
                }}>
                    <Text style={{
                        flex: 1,
                        color: theme.colors.black_title,
                        marginBottom: 5,
                        ...theme.fonts.regular18
                    }}>??i???m ???? r??t</Text>
                    <NumberFormat
                        value={data.point}
                        fonts={theme.fonts.regular16}
                        color={theme.colors.black_title}
                        perfix={R.strings().point}
                    />
                </View>

                <View style={{
                    marginHorizontal: '2.5%',
                    marginBottom: 20,
                    flexDirection: 'row',
                    alignItems: 'center',
                }}>
                    <Text style={{
                        flex: 1,
                        color: theme.colors.black_title,
                        marginBottom: 5,
                        ...theme.fonts.regular18
                    }}>S??? ti???n nh???n ???????c</Text>
                    <NumberFormat
                        value={data.totalMoney}
                        fonts={theme.fonts.regular16}
                        color={theme.colors.black_title}
                        perfix={'VN??'}
                    />
                </View>


                <Text style={{
                    color: theme.colors.black_title,
                    marginBottom: 5,
                    textAlign: 'center',
                    ...theme.fonts.regular16
                }}>Vui l??ng ch??? 1-3 ng??y  l??m vi???c ????? nh???n ???????c chuy???n kho???n</Text>
            </Block>
        )

    }

    render() {
        const { data } = this.state
        return (
            <Block>
                <DCHeader title={'Y??u c???u r??t ??i???m'} isWhiteBackground />
                <SafeAreaView style={{
                    ...theme.styles.container,
                }}>
                    {this.renderData()}
                </SafeAreaView>
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({
    bankState: state[REDUCER.BANK]
})

const mapDispatchToProps = {
    getBank
}

export default connect(mapStateToProps, mapDispatchToProps)(DetailDrawPointsScreen)
