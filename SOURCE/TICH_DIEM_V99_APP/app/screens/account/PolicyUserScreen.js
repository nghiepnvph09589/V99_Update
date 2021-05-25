import { Block } from '@app/components'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, ScrollView } from 'react-native'
import { connect } from 'react-redux'
import Header from "../../components/DCHeader";
import I18n from "../../i18n/i18n";

export class PolicyUserScreen extends Component {

    render() {
        return (
            <Block>
                <Header title={I18n.t("policy_user_screen")} back />

                <ScrollView
                    showsVerticalScrollIndicator={false}
                    style={{ flex: 1 }}>
                    <View style={{
                        paddingHorizontal: '5%',
                        paddingTop: 20
                    }}>
                        <Text style={{
                            marginBottom: 10,
                            ...theme.fonts.semibold18
                        }}> Chính sách và điều kiện</Text>

                        <Text style={{
                            marginBottom: 10,
                            ...theme.fonts.regular18
                        }}>1/. Thành viên VIP được mua sản phẩm giá ưu
                            đãi (Giảm giá từ 10% - 60%). Điều kiện để
                            trở thành thành viên VIP: hoàn thành 1 đơn
                            hàng có giá trị tối thiểu 500 điểm.
                    </Text>
                        <Text style={{
                            marginBottom: 10,
                            ...theme.fonts.regular18
                        }}>2/. Hoàn tiền thương hiệu 20% giá trị đơn
                            hàng.
                    </Text>
                        <Text style={{
                            marginBottom: 10,
                            ...theme.fonts.regular18
                        }}>3/. Hạn mức điểm tối thiểu được rút là 200
                            điểm / lần.
                    </Text>
                        <Text style={{
                            marginBottom: 10,
                            ...theme.fonts.regular18
                        }}>4/. Điểm thưởng giới thiệu là 10% giá trị mỗi
                            đơn hàng thành công của người được giới
                            thiệu.
                    </Text>
                    </View>

                </ScrollView>

            </Block>
        )
    }
}

const mapStateToProps = (state) => ({

})

const mapDispatchToProps = {

}

export default connect(mapStateToProps, mapDispatchToProps)(PolicyUserScreen)
