import { Block, DCHeader, ScreenComponent } from '@app/components'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, FlatList } from 'react-native'
import { SafeAreaView } from 'react-navigation'
import { connect } from 'react-redux'
import { requestGetListMember } from '@api'
import { SearchBar } from 'react-native-elements'
import { TouchableOpacity } from 'react-native'
import NavigationUtil from '@app/navigation/NavigationUtil'
import { ActivityIndicator } from 'react-native'

export class SearchReferralCodeScreen extends Component {

    constructor(props) {
        super(props)

        this.state = {
            searchKey: '',
            isLoading: false,
            data: []
        }

        this.onCallBack = props.navigation.getParam('callback', {})
        this.titleHeader = props.navigation.getParam('titleHeader', '')
    }

    componentDidMount() {
        // this.onSearch()
    }

    timeout = null

    render() {
        return (
            <Block>
                <DCHeader title={this.titleHeader || 'Chọn mã giới thiệu'} />
                <SafeAreaView style={theme.styles.container}>
                    <SearchBar
                        autoFocus={true}
                        round
                        containerStyle={{
                            width: width,
                            backgroundColor: theme.colors.primary,
                            borderBottomColor: theme.colors.primary,
                            borderTopColor: theme.colors.primary,
                            justifyContent: 'center',
                        }}
                        inputContainerStyle={{
                            backgroundColor: theme.colors.white,
                            borderRadius: 5,
                        }}
                        inputStyle={{
                            ...theme.fonts.regular15
                        }}
                        searchIcon={{ size: 24 }}
                        onChangeText={this.onChangeText}
                        placeholder={'Nhập mã giới thiệu'}
                        value={this.state.searchKey}
                        onClear={() => this.setState({ data: [] })}
                    />
                    <FlatList
                        data={this.state.data}
                        keyExtractor={(item, index) => index}
                        renderItem={this.renderItem}
                        ListHeaderComponent={
                            this.state.isLoading && <ActivityIndicator style={{ marginTop: 50 }} size='large' />
                        }
                    />
                </SafeAreaView>
            </Block>
        )
    }

    renderItem = ({ item, index }) => {
        return (
            <TouchableOpacity
                onPress={() => {
                    this.onCallBack(item)
                    NavigationUtil.goBack()
                }}
                style={{ padding: 10 }}>
                <Text>{item.nameAndPhone}</Text>
            </TouchableOpacity>)
    }


    onChangeText = (text) => {
        this.setState({ searchKey: text }, () => {
            if (text.length < 5) return
            if (this.timeout) clearTimeout(this.timeout)
            this.timeout = setTimeout(this.onSearch, 500);
        })
    }

    onSearch = async () => {
        const { searchKey } = this.state

        this.setState({ isLoading: true })

        const payload = { searchKey }

        try {
            const res = await requestGetListMember(payload)
            this.setState({
                isLoading: false,
                data: res.data
            })
        } catch (error) {
            console.log(error)
        } finally { this.setState({ isLoading: false }) }
    }
}

const mapStateToProps = (state) => ({

})

const mapDispatchToProps = {

}

export default connect(mapStateToProps, mapDispatchToProps)(SearchReferralCodeScreen)
