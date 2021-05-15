import React, { Component, useState } from 'react'
import {
    Button, Pressable, TextInput,
    View, Text, FlatList, ImageBackground, Dimensions,
    //  Modal,
    ScrollView, TouchableOpacity, RefreshControl, StyleSheet, Keyboard
} from 'react-native'
import { connect } from 'react-redux'
import { getListGift, exchangeGift } from '../../redux/actions'
import { GIFT_TYPE, REDUCER, GET_HISTORY_POINT_TYPE, SCREEN_ROUTER } from '../../constants/Constant'
import {
    Block, NumberFormat, Loading,
    PrimaryButton, LoadingProgress,
    Error, Empty, FstImage
} from '../../components'
import reactotron from 'reactotron-react-native'
import * as theme from '../../constants/Theme'
import I18n from '../../i18n/i18n'
import { showConfirm, showMessages } from '../../utils/Alert'
import ObjectUtil from '../../utils/ObjectUtil'
import NavigationUtil from '../../navigation/NavigationUtil'
import Toast, { BACKGROUND_TOAST } from '../../utils/Toast'
import { requireLogin } from '../../utils/AlertRequireLogin'
import AsyncStorage from "@react-native-community/async-storage"
import R from '@app/assets/R'
import ScrollableTabView, { DefaultTabBar, ScrollableTabBar } from 'react-native-scrollable-tab-view'
import PolicyScreen from './PolicyScreen'
import Modal from 'react-native-modal'
import WalletPointsItem from '@app/components/WalleVItem'
import { getListPointV } from '../../redux/actions'
import callAPI from '@app/utils/CallApiHelper'
import { requestVtoPoint } from '@app/constants/Api'

export class PointVScreen extends Component {
    

    componentDidMount() {
        this.getData()
        // this.postDataVtoPoint()
    }

    getData = () => {
        const payload = {
            page: 1,
            type: GET_HISTORY_POINT_TYPE.LIST_POINT_V,
            typePoint: ''
        }
        this.props.getListPointV(payload)
    }



    renderViewPoint = () => {
        const { userState } = this.props
        return (<FstImage
            style={{ width, height: width * 0.4 }}
            source={R.images.img_decor}
            resizeMode='contain'>
            <Block center middle>
                <Text style={{
                    color: theme.colors.active,
                    marginBottom: 5,
                    ...theme.fonts.semibold16
                }}>Điểm của bạn</Text>
                <NumberFormat
                    fonts={theme.fonts.semibold25}
                    color={theme.colors.active}
                    value={userState.pointV} perfix={R.strings().point_V} />
            </Block>
        </FstImage>)
    }
    renderViewOptions = () => {
        const { userState } = this.props;
        return (
            <View style={{
                padding: '2%',
                paddingLeft: 240,
                marginHorizontal: '4%',
                marginTop: -50,
            }}>
                <Option
                    text={R.strings().moving_point}
                />
            </View>)
    }
    renderFlastlist = () => {
        const { walletPointsState } = this.props
        // reactotron.log('walletPointsState', walletPointsState.data.listHistoriesPointMember)
        // return
        if (walletPointsState.isLoading) return <Loading />
        if (walletPointsState.error) return <Error onPress={this.getData} />
        if (!walletPointsState.data.listHistoriesPointMember) return <Empty onRefresh={this.getData} />
        // return
        return <FlatList
            refreshControl={<RefreshControl refreshing={false} onRefresh={this.getData} />}
            style={{ backgroundColor: 'white', marginTop: 15 }}
            data={walletPointsState.data.listHistoriesPointMember}
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
    onChangeText = (text) => {
        this.setState({ point: text })
    }


    state = {
        modalVisible: false
    };

    setModalVisible = (visible) => {
        this.setState({ modalVisible: visible });
    }

    // constructor(props) {
    //     super(props)

    //     this.state = {
    //         isLoading: true,
    //         error: null,
    //         data: []
    //     }
    // }
    postDataVtoPoint = (point) => {
        this.setState({ error: null, isLoading: true })
        callAPI({
            API: requestVtoPoint(point),
            onSuccess: (res) => {
                this.setState({ data: res.data })
                this.setModalVisible(!modalVisible)
            },
            onError: (err) => {
                this.setState({ error: JSON.stringify(err) })
            },
            onFinaly: () => {
                this.setState({ isLoading: false })
            }
        })
    }

    render() {
        const { text } = this.props;
        const { modalVisible } = this.state;
        const { point, isPoint } = this.state
        return (
            <View style={styles.centeredView}>


                <Modal
                    style={styles.modalStyle}
                    animationType="slide"
                    visible={modalVisible}
                    // transparent={true}
                    
                    onRequestClose={() => {
                        this.setModalVisible(!modalVisible);
                    }}
                >
                    <View style={styles.centeredView}>
                        <View style={styles.modalView}>
                            <Text style={styles.modalText}>Số điểm muốn chuyển?</Text>
                            <TextInput
                                style={styles.input}
                                keyboardType='number-pad'
                                value={point}
                                placeholder='0'
                                onChangeText={this.onChangeText}
                            />
                            <View style={{
                                width: 100,
                                flexDirection: 'row',
                                justifyContent: 'space-between',
                            }}>
                                <Button
                                    title="Huỷ"
                                    onPress={() => this.setModalVisible(!modalVisible)}
                                />
                                <Button
                                    title="Ok"
                                    onPress={() => {
                                        this.setModalVisible(!modalVisible)
                                        this.postDataVtoPoint(point)
                                    }
                                    }
                                />
                            </View>
                        </View>
                    </View>
                </Modal>

                <TouchableOpacity
                    style={{
                        alignItems: "center",
                        backgroundColor: "#00bfff",
                        padding: 10,
                        width: 100,
                        height: 35,
                        paddingTop: 7,
                        paddingEnd: 7,
                        borderRadius: 8
                    }}
                    underlayColor='tomato'
                    onPress={() => this.setModalVisible(true)}>
                    <Text style={{ color: 'white' }}>{text}</Text>
                </TouchableOpacity>
            </View>



        );
    }
}



const styles = StyleSheet.create({
    input: {
        height: 40,
        width: 250,
        margin: 12,
        borderWidth: 1,
    },
    centeredView: {
        // flex: 1,
        // justifyContent: "center",
        // alignItems: "center",
        // // marginTop: 22
    },
    modalView: {
        margin: 20,
        backgroundColor: "white",
        borderRadius: 20,
        padding: 35,
        alignItems: "center",
        shadowColor: "#000",
        shadowOffset: {
            width: 0,
            height: 2
        },
        shadowOpacity: 0.25,
        shadowRadius: 4,
        elevation: 5,
    },
    modalStyle: {
        justifyContent: 'center',
        borderRadius: Platform.OS === 'ios' ? 30 : 0,
        shadowRadius: 10,
        height: 280,
        // backgroundColor: '#898989'
    },
    textStyle: {
        color: "white",
        fontWeight: "bold",
        textAlign: "center"
    },
    modalText: {
        marginBottom: 15,
        textAlign: "center"
    }
})
const mapStateToProps = (state) => ({
    userState: state[REDUCER.USER].data,
    walletPointsState: state[REDUCER.GET_LIST_POINT_V]
})

const mapDispatchToProps = {
    getListPointV
}
export default connect(mapStateToProps, mapDispatchToProps)(PointVScreen)
