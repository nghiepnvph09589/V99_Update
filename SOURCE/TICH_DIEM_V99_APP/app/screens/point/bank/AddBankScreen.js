import R from '@app/assets/R'
import {
    Block, DCHeader, FstImage,
    LoadingProgress, TDButton, TDDropDown, TDTextInput
} from '@app/components'
import theme from '@app/constants/Theme'
import NavigationUtil from '@app/navigation/NavigationUtil'
import React, { Component } from 'react'
import {
    View, Text, SafeAreaView,
    KeyboardAvoidingView, Platform, TouchableOpacity, ScrollView
} from 'react-native'
import { connect } from 'react-redux'
import { requestAddBankAccount } from '@api'
import { showMessages } from '@app/utils/Alert'
import Toast from '@app/utils/Toast'
import { REDUCER, SCREEN_ROUTER } from '@app/constants/Constant'
import reactotron from 'reactotron-react-native'
import { getBank } from '@action'

export class AddBankScreen extends Component {
    constructor(props) {
        super(props)
        this.state = {
            ID: '',
            bankNameSelected: '',
            codeBankAccount: "",
            userName: "",
            isRequesting: false,
        }
    }

    onChangeText = (key) => (text) => {
        this.setState({ [key]: text })
    }

    onChangeDropdown = (value, index, data) => {
        this.setState(
            {
                ID: value,
                bankNameSelected: data[index].shortName,
            },
        );
    };

    render() {
        const { ID, codeBankAccount, userName, isRequesting, bankNameSelected } = this.state
        const { bankSelectState } = this.props
        return (
            <Block>
                <DCHeader isWhiteBackground
                    title={'Thêm ngân hàng'}
                    rightComponent={<TouchableOpacity
                        onPress={() => NavigationUtil.navigate(SCREEN_ROUTER.BANK)}>
                        <FstImage source={R.images.ic_contacts}
                            style={{ width: 20, height: 20 }}
                            resizeMode='contain' />
                    </TouchableOpacity>}
                />
                {isRequesting && <LoadingProgress />}
                <SafeAreaView style={theme.styles.container}>
                    <KeyboardAvoidingView
                        behavior={Platform.OS === 'ios' && 'padding'}
                        style={{
                            flex: 1,
                        }}
                        enabled
                        keyboardVerticalOffset={100}>
                        <ScrollView>
                            <Text
                                style={{
                                    color: theme.colors.black_title,
                                    marginLeft: '5%',
                                    marginBottom: 20,
                                    ...theme.fonts.semibold18
                                }}>Thông tin ngân hàng</Text>
                            <TDDropDown
                                data={bankSelectState}
                                isBorder
                                value={ID || 'Chọn ngân hàng'}
                                labelExtractor={item => item.bankName + ' - ' + item.shortName}
                                valueExtractor={(item, index) => item.id}
                                onChangeText={this.onChangeDropdown}
                                style={{
                                    width: '90%'
                                }}
                            />
                            <TDTextInput
                                fieldName='Số tài khoản'
                                borderRadiusBottom
                                borderRadiusTop
                                containerStyle={{
                                    borderColor: theme.colors.border
                                }}
                                keyboardType='number-pad'
                                value={codeBankAccount}
                                onChangeText={this.onChangeText('codeBankAccount')}
                            />
                            <TDTextInput
                                fieldName='Tên chủ tài khoản'
                                borderRadiusBottom
                                borderRadiusTop
                                containerStyle={{
                                    borderColor: theme.colors.border
                                }}
                                value={userName}
                                onChangeText={this.onChangeText('userName')}
                            />

                            <TDButton
                                onPress={this.addBankPress}
                                title='Thêm'
                                style={{
                                    width: '90%',
                                    marginTop: 50
                                }}
                            />
                        </ScrollView>
                    </KeyboardAvoidingView>
                </SafeAreaView>
            </Block>
        )
    }

    addBankPress = async () => {
        const { ID, codeBankAccount, userName } = this.state

        this.setState({ isRequesting: true })

        const payload = {
            ID,
            codeBankAccount,
            userName
        }

        try {
            const res = await requestAddBankAccount(payload)
            this.props.getBank()
            Toast.show('Thêm ngân hàng thành công')
            NavigationUtil.reset(
                {
                    index: 1,
                    actions: [
                        NavigationUtil.navigate(SCREEN_ROUTER.DRAW_POINTS),
                        NavigationUtil.replace(SCREEN_ROUTER.DRAW_POINTS)
                    ]
                }
            )
            // NavigationUtil.navigate(SCREEN_ROUTER.DRAW_POINTS)
        } catch (error) {
            console.log(error)
        } finally { this.setState({ isRequesting: false }) }
    }
}

const mapStateToProps = (state) => ({
    bankSelectState: state[REDUCER.BANK_SELECT].data
})

const mapDispatchToProps = {
    getBank
}

export default connect(mapStateToProps, mapDispatchToProps)(AddBankScreen)
