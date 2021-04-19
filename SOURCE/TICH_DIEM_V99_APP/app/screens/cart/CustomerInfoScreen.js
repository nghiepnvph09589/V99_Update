import React, { Component } from 'react'
import { connect } from 'react-redux'
import {
    SafeAreaView, View, Text, TouchableOpacity,
    FlatList, ScrollView, StyleSheet,
    TextInput, KeyboardAvoidingView, Platform
} from 'react-native'
import {
    DCHeader, Block, PrimaryButton,
    Loading,
} from '../../components'
import * as theme from '../../constants/Theme'
import I18n from '../../i18n/i18n'
import { Divider } from 'react-native-elements'
import { Dropdown } from 'react-native-material-dropdown'
import NavigationUtil from '../../navigation/NavigationUtil'
import { REDUCER } from '../../constants/Constant'
import { getAddress } from "../../redux/actions";
import Toast, { BACKGROUND_TOAST } from '../../utils/Toast'


export class CustomerInfoScreen extends Component {

    constructor(props) {
        super(props)
        const { navigation } = props
        const userInfo = navigation.getParam('userInfo', {})
        this.state = {
            name: userInfo.name,
            phone: userInfo.phone,
            address: userInfo.address,
            districtName: userInfo.districtName,
            provinceName: userInfo.provinceName,
            provinceID: userInfo.provinceID,
            districtID: userInfo.districtID,
        }
    }

    filterListDistrict = (proId) => {
        const { addressState } = this.props
        var result = addressState.listDistrict.filter(district => {
            return (district.provinceID == proId)
        })
        this.setState({
            ...this.state,
            districtID: result[0].districtID,
            districtName: result[0].districtName
        })
    }

    componentDidMount() {
        const { addressState } = this.props
        if (!addressState.listProvince.length) this.props.getAddress()
    }


    prepareCallback() {
        const { navigation } = this.props
        const { name, phone, address, districtID, districtName, provinceID, provinceName } = this.state
        if (!phone || !name.trim() || !address.trim()) {
            Toast.show('Vui lòng nhập đầy đủ thông tin', BACKGROUND_TOAST.FAIL)
            return
        }
        const callback = navigation.getParam('callback', {})
        let callbackResult =
        {
            name: name.trim(),
            phone: phone,
            address: address.trim(),
            districtName: districtName,
            provinceName: provinceName,
            provinceID: provinceID,
            districtID: districtID,
        }
        callback(callbackResult)
        NavigationUtil.goBack()
    }

    renderBody() {
        const { name, phone, address, provinceName,
            provinceID, districtID, districtName } = this.state;
        const { addressState } = this.props
        if (addressState.isLoading) return <Loading />

        return (
            <KeyboardAvoidingView
                style={{ flex: 1 }}
                behavior={Platform.OS === 'ios' ? 'padding' : null}
                keyboardVerticalOffset={100}
                enabled>
                <ScrollView>
                    <Text style={styles.title}>Người đặt</Text>
                    <TextInput style={theme.styles.textInput} value={name}
                        onChangeText={(value) => {
                            this.setState({
                                ...this.state,
                                name: value
                            })
                        }}
                    />
                    <Text style={styles.title}>Số điện thoại</Text>
                    <TextInput style={theme.styles.textInput} value={phone}
                        onChangeText={(value) => {
                            this.setState({
                                ...this.state,
                                phone: value
                            })
                        }} keyboardType='number-pad'
                    />
                    <Text style={styles.title}>Tỉnh/Thành phố</Text>
                    <View style={theme.styles.dropDown}>
                        <Dropdown
                            itemCount={8}
                            containerStyle={{ paddingVertical: 0, marginTop: -23 }}
                            inputContainerStyle={{
                                borderBottomColor: 'transparent',
                            }}
                            data={addressState.listProvince}
                            value={provinceName}
                            labelExtractor={item => item.provinceName}
                            keyExtractor={item => item.provinceID}
                            placeholder="Chọn tỉnh / thành phố"
                            onChangeText={(value, index, data) => {
                                this.setState({
                                    ...this.state,
                                    provinceID: data[index].provinceID,
                                    provinceName: data[index].provinceName,
                                    address: ''
                                }, () => this.filterListDistrict(data[index].provinceID));
                            }}
                        />
                    </View>
                    <Text style={styles.title}>Quận/Huyện</Text>
                    <View style={theme.styles.dropDown}>
                        <Dropdown
                            itemCount={8}
                            containerStyle={{ paddingVertical: 0, marginTop: -23 }}
                            inputContainerStyle={{
                                borderBottomColor: 'transparent',
                            }}
                            data={addressState.listDistrict.filter(
                                district => {
                                    return (
                                        district.provinceID == provinceID
                                    );
                                })}
                            value={districtName}
                            labelExtractor={item => item.districtName}
                            keyExtractor={item => item.districtID}
                            placeholder="Chọn quận/ huyện"
                            onChangeText={(value, index, data) => {
                                this.setState({
                                    ...this.state,
                                    districtID: data[index].districtID,
                                    districtName: data[index].districtName,
                                });
                            }}
                        />
                    </View>
                    <Text style={styles.title}>Địa chỉ cụ thể</Text>
                    <TextInput style={theme.styles.textInput} value={address}
                        onChangeText={(value) => {
                            this.setState({
                                ...this.state,
                                address: value
                            })
                        }}
                    />
                    <PrimaryButton style={{ marginTop: 15 }} title='Lưu'
                        onPress={() => {
                            this.prepareCallback()
                        }} />
                </ScrollView>
            </KeyboardAvoidingView>
        )
    }

    render() {
        return (
            <Block>
                <DCHeader title='Chỉnh sửa thông tin đặt hàng' />
                <SafeAreaView style={theme.styles.container}>
                    {this.renderBody()}
                </SafeAreaView>
            </Block>
        )
    }
}

const styles = StyleSheet.create({
    title: {
        marginTop: 10,
        marginBottom: 5,
        paddingLeft: '5%',
        ...theme.fonts.robotoRegular14,
    }
})

const mapStateToProps = (state) => ({
    addressState: state[REDUCER.ADDRESS]
});

const mapDispatchToProps = {
    getAddress
};

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(CustomerInfoScreen);

