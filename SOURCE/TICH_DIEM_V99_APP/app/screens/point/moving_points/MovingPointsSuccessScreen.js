import R from '@app/assets/R'
import { Block, DCHeader, Error, Loading, NumberFormat } from '@app/components'
import theme from '@app/constants/Theme'
import DateUtil from '@app/utils/DateUtil'
import React, { Component } from 'react'
import { View, Text, SafeAreaView, ScrollView } from 'react-native'
import { connect } from 'react-redux'
import { requestGetHistyoriesDetail } from '@api'

export class MovingPointsSuccessScreen extends Component {
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
        if (isLoading) return <Loading />
        if (error) return <Error reload={this.getData} />

        return (
            <Block>
                <View style={{
                    flexDirection: 'row',
                    alignItems: 'center',
                    paddingHorizontal: '2.5%',
                }}>
                    <Text style={{
                        flex: 1,
                        color: theme.colors.black1,
                        ...theme.fonts.regular18
                    }}>Thời gian</Text>
                    <Text style={{
                        color: theme.colors.black_title,
                        ...theme.fonts.regular16
                    }}>{DateUtil.formatTime(data.createDate)} {DateUtil.formatShortDate(data.createDate)}</Text>
                </View>

                <View style={{
                    marginHorizontal: '2.5%',
                    borderBottomWidth: 2,
                    borderBottomColor: '#B2BEC3',
                    paddingBottom: 5,
                    marginTop: 20,
                }}>
                    <Text style={{
                        color: theme.colors.black1,
                        marginBottom: 5,
                        ...theme.fonts.regular18
                    }}>Chuyển điểm cho khách hàng</Text>
                    <Text style={{
                        color: theme.colors.black_title,
                        ...theme.fonts.regular16
                    }}>{!this.id ? this.props.navigation.getParam('phone') : data.userName + ' - ' + data.userPhone}</Text>
                </View>
                <View style={{
                    marginHorizontal: '2.5%',
                    borderBottomWidth: 2,
                    borderBottomColor: '#B2BEC3',
                    paddingBottom: 5,
                    marginTop: 20,
                }}>
                    <Text style={{
                        color: theme.colors.black1,
                        marginBottom: 5,
                        ...theme.fonts.regular18
                    }}>Số điểm đã chuyển</Text>
                    <NumberFormat
                        value={data.point}
                        fonts={theme.fonts.regular16}
                        color={theme.colors.black_title}
                        perfix={R.strings().point}
                    />
                </View>
                <View style={{
                    marginHorizontal: '2.5%',
                    paddingBottom: 5,
                    marginTop: 20,
                }}>
                    <Text style={{
                        color: theme.colors.black1,
                        marginBottom: 5,
                        ...theme.fonts.regular18
                    }}>Ghi chú</Text>
                    <Text style={{
                        color: theme.colors.black_title,
                        ...theme.fonts.regular16
                    }}>{!this.id ? this.props.navigation.getParam('note') : data.comment}</Text>
                </View>
            </Block>
        )

    }


    render() {
        return (
            <Block>
                <DCHeader title={'Thông tin chuyển điểm'} isWhiteBackground />
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

})

const mapDispatchToProps = {

}

export default connect(mapStateToProps, mapDispatchToProps)(MovingPointsSuccessScreen)
