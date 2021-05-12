import { Block } from '@app/components'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, ScrollView } from 'react-native'
import { connect } from 'react-redux'

export class PolicyScreen extends Component {

    render() {
        return (
            <Block
                style={{
                    paddingHorizontal: '5%',
                    paddingTop: 20
                }}>
                <ScrollView
                    showsVerticalScrollIndicator={false}
                    style={{ flex: 1 }}>
                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.semibold18
                    }}> Chính sách</Text>

                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.regular18
                    }}>1/. Được mua hàng đúng chất lượng, đúng giá thành, đảm bảo pháp lý của sản phẩm và được hoàn tiền thương hiệu 80% giá trị từng đơn hàng.</Text>
                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.regular18
                    }}>2/. Trong 120 ngày đầu hoàn 0,2%/ngày ( 6%/tháng) từ ngày 121 trở đi hoàn 0,1%/ngày (3% /tháng).</Text>
                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.regular18
                    }}>3/. Rút tiền vào thứ 3 hàng tuần.</Text>
                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.regular18
                    }}>4/. Hạn mức tối thiều rút tiền là 200.000đ/lần.</Text>
                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.regular18
                    }}>5/. Kết nối thành viên mua hàng là 5% trên doanh thu của người mụa.</Text>
                    <Text style={{
                        marginBottom: 10,
                        ...theme.fonts.regular18
                    }}>6/. Phí duy trì: 20.000đ/tháng.</Text>
                </ScrollView>
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({

})

const mapDispatchToProps = {

}

export default connect(mapStateToProps, mapDispatchToProps)(PolicyScreen)
