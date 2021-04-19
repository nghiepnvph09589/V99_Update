import { Block, DCHeader, Empty, Error, FstImage, Loading } from '@app/components'
import theme from '@app/constants/Theme'
import React, { Component } from 'react'
import { View, Text, SafeAreaView, ScrollView, Image } from 'react-native'
import { connect } from 'react-redux'
import Accordion from 'react-native-collapsible/Accordion';
import { requestGetListBankOfCus } from '@api'
import { RefreshControl } from 'react-native'
import { REDUCER } from '@app/constants/Constant'
import { getBank } from '@action'

export class BankScreen extends Component {
    componentDidMount() {
        this.getData()
    }

    state = {
        isLoading: true,
        error: null,
        data: [],
        activeSections: [],
    };

    getData = async () => {
        this.props.getBank()
    }

    renderFlastlist = () => {
        const { bankState } = this.props
        if (bankState.isLoading) return <Loading />
        if (bankState.error) return <Error onPress={this.getData} />
        if (!bankState.data.length) return <Empty />

        return <Accordion
            sections={bankState.data}
            activeSections={this.state.activeSections}
            // renderSectionTitle={this._renderSectionTitle}
            renderHeader={this._renderHeader}
            renderContent={this._renderContent}
            onChange={this._updateSections}
            containerStyle={{
                // borderRadius: 5,
                // borderWidth: 1,
                padding: '2.5%',
                // borderColor: theme.colors.border
            }}
        />
    }


    render() {

        return (
            <Block color={theme.colors.primary_background}>
                <DCHeader title={'Danh sách ngân hàng'} isWhiteBackground />
                <SafeAreaView style={theme.styles.container}>
                    <ScrollView style={{
                        flex: 1,
                        paddingHorizontal: '2.5%'
                    }}
                        contentContainerStyle={{ flexGrow: 1 }}
                        refreshControl={<RefreshControl
                            refreshing={false}
                            onRefresh={this.getData}
                        />}>
                        <Text style={{
                            color: theme.colors.black1,
                            marginBottom: 10,
                            ...theme.fonts.semibold18
                        }}>Thông tin ngân hàng</Text>
                        <Block>
                            {this.renderFlastlist()}
                        </Block>
                    </ScrollView>

                </SafeAreaView>
            </Block>
        )
    }

    _renderSectionTitle = section => {
        return (
            <View>
                <Text>_renderSectionTitle</Text>
            </View>
        );
    };

    _renderHeader = (section, index, isActive) => {
        return (
            <View style={{
                flexDirection: 'row',
                alignItems: 'center',
                borderRadius: isActive ? 0 : 5,
                borderTopLeftRadius: 5,
                borderTopRightRadius: 5,
                borderBottomLeftRadius: isActive ? 0 : 5,
                borderBottomRightRadius: isActive ? 0 : 5,
                borderLeftWidth: 1,
                borderRightWidth: 1,
                borderTopWidth: 1,
                borderBottomWidth: isActive ? 0 : 1,
                borderColor: theme.colors.border,
                marginBottom: isActive ? 0 : 10,
                paddingHorizontal: '2.5%'
            }}>
                <FstImage
                    source={{ uri: section.imageUrl }}
                    style={{
                        width: width * 0.1,
                        aspectRatio: 1,
                        borderRadius: 50,
                        marginRight: '5%'
                    }}
                    resizeMode='contain'
                />
                <Text style={{
                    color: theme.colors.black1,
                    ...theme.fonts.regular20
                }}>{section.shortName}</Text>
            </View>
        );
    };

    _renderContent = (section, index, isActive) => {
        return (
            <View style={{
                paddingTop: 5,
                paddingHorizontal: '2.5%',
                borderTopColor: theme.colors.border,
                borderTopWidth: 0.5,
                marginBottom: 10,
                borderLeftWidth: 1,
                borderRightWidth: 1,
                borderBottomWidth: 1,
                borderColor: theme.colors.border,
                marginBottom: 10,
                borderBottomLeftRadius: 5,
                borderBottomRightRadius: 5
            }}>
                <Text style={{
                    color: theme.colors.black_title,
                    marginBottom: 5,
                    ...theme.fonts.regular18
                }}>STK: {section.codeBankAccount}</Text>
                <Text style={{
                    color: theme.colors.black_title,
                    ...theme.fonts.regular18
                }}>Tên: {section.userName}</Text>
            </View>
        );
    };

    _updateSections = activeSections => {
        this.setState({ activeSections });
    };

}

const mapStateToProps = (state) => ({
    bankState: state[REDUCER.BANK]
})

const mapDispatchToProps = {
    getBank
}

export default connect(mapStateToProps, mapDispatchToProps)(BankScreen)
