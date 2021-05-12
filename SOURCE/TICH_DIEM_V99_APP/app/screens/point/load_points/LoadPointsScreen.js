import R from '@app/assets/R'
import { Block, DCHeader, FstImage, NumberFormat } from '@app/components'
import { REDUCER } from '@app/constants/Constant'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, ImageBackground, SafeAreaView, KeyboardAvoidingView, Platform, ScrollView } from 'react-native'
import { connect } from 'react-redux'

export class LoadPointsScreen extends Component {

    renderImagePoint = () => {
        const { userState } = this.props

        return (
            <ImageBackground
                source={R.images.img_bg_action_points}
                style={{
                    width: width * 0.95,
                    aspectRatio: 4,
                    backgroundColor: 'red',
                    borderRadius: 5,
                    alignSelf: 'center',
                    flexDirection: 'row',
                    alignItems: 'center',
                    paddingHorizontal: '5%',
                    backgroundColor: 'white'
                }}
                resizeMode='cover'
            >
                <FstImage
                    source={R.images.ic_shape_star}
                    style={{ height: '70%', aspectRatio: 1, }}
                    resizeMode='contain'
                />
                <View style={{ paddingLeft: '5%' }}>
                    <Text style={{
                        ...theme.fonts.regular16,
                        color: 'white',
                        marginBottom: 10,
                    }}>Điểm hiện có</Text>
                    <NumberFormat
                        value={userState.point}
                        perfix={R.strings().point}
                        fonts={theme.fonts.regular18}
                        color={theme.colors.white}
                    />
                </View>
            </ImageBackground>
        )
    }

    renderInfoLoadPoint = () => {
        return (
            <Block style={{ paddingHorizontal: '2.5%', backgroundColor: 'white' }}>
                <Text style={{
                    color: theme.colors.black_title,
                    marginVertical: 10,
                    ...theme.fonts.regular18
                }}>Chuyển điểm vào tài khoản</Text>

                {/* {this.renderBank('0691 000 405 546', 'Công ty TNHH Quốc tế Trà Tiên Thảo', 'Chi nhánh Tây Hà Nội')} */}
                {this.renderBank('0451 000 380 385', 'Nguyễn Thị Thúy', 'Chi nhánh Thành Công')}

                <Text style={{
                    color: theme.colors.black_title,
                    marginVertical: 10,
                    ...theme.fonts.regular18
                }}>Nội dung chuyển - nhập cú pháp:</Text>

                <View style={{
                    borderRadius: 5,
                    padding: width * 0.025,
                    ...theme.styles.border
                }}>
                    <Text style={{
                        textAlign: 'center',
                        marginVertical: 5,
                        color: theme.colors.black_title,
                        ...theme.fonts.regular18
                    }}>Nạp điểm - Tên tài khoản - Số điện thoại</Text>
                </View>

                <Text style={{
                    color: theme.colors.black_title,
                    marginVertical: 10,
                    ...theme.fonts.regular16
                }}>(Vui lòng nhập cú pháp trên khi chuyển khoản)</Text>
                <Text style={{
                    color: theme.colors.black_title,
                    marginBottom: 10,
                    ...theme.fonts.regular16
                }}>VD: Nạp điểm - Nguyễn Minh Quang - 0793652678</Text>
            </Block>
        )
    }

    renderBank = (code, name, bank) => {
        return (
            <View style={{
                borderRadius: 5,
                padding: '2.5%',
                marginBottom: 10,
                ...theme.styles.border
            }}>
                <FstImage source={R.images.logo_vietcombank}
                    style={{
                        width: width * 0.3, aspectRatio: 3,
                        marginBottom: 10,
                    }}
                    resizeMode='contain'
                />
                <Text style={{
                    color: theme.colors.black_title,
                    ...theme.fonts.regular20
                }}>STK: {code}</Text>
                <Text style={{
                    marginVertical: 5,
                    color: theme.colors.black_title,
                    ...theme.fonts.regular18
                }}>Tên: {name}</Text>
                <Text style={{
                    color: theme.colors.black_title,
                    ...theme.fonts.regular18
                }}>Chi nhánh: {bank}</Text>
            </View>
        )
    }

    render() {
        return (
            <Block>
                <DCHeader isWhiteBackground
                    title={R.strings().load_point} />
                <SafeAreaView style={theme.styles.container}>
                    <ScrollView>
                        {this.renderImagePoint()}
                        {this.renderInfoLoadPoint()}
                        <View style={{
                            backgroundColor: 'white',
                            paddingHorizontal: '2.5%',
                            marginTop: 10,
                            paddingVertical: 10
                        }}>
                            <Text style={{
                                marginBottom: 10,
                                color: theme.colors.black_title,
                                ...theme.fonts.regular16
                            }}>Khi nạp điểm vui lòng chọn chuyển ngay lập tức để yêu cầu nạp điểm của bạn được xác nhận sớm nhất</Text><Text style={{
                                color: theme.colors.black_title,
                                ...theme.fonts.regular16
                            }}>Để nạp điểm vui lòng chuyển khoản đến số tài khoản bên trên và chờ từ 1-3 ngày làm việc để được cộng điểm</Text>
                        </View>
                    </ScrollView>
                </SafeAreaView>
            </Block>
        )
    }
}

const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data

})

const mapDispatchToProps = {

}

export default connect(mapStateToProps, mapDispatchToProps)(LoadPointsScreen)
