import React, { Component } from 'react'
import {
    View, Text, ImageBackground,
    ScrollView, TouchableOpacity,TextInput,Button, RefreshControl, StyleSheet, Keyboard
} from 'react-native'
import { connect } from 'react-redux'
import { getListGift, exchangeGift } from '../../redux/actions'
import { GIFT_TYPE, REDUCER, SCREEN_ROUTER } from '../../constants/Constant'
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
import WalletAccumulatePointsScreen from './WalletAccumulatePointsScreen'
import Modal from 'react-native-modal'
import callAPI from '@app/utils/CallApiHelper'
import { requestPointToV } from '@app/constants/Api'

const bottomButton = 25
const padding_horizontal = 10
export class CardScreen extends Component {

    constructor(props) {
        const { cardState } = props
        super(props)
        this.state = {
            indexCarrierSelected: 0,
            indexPriceSeleted: null,
            priceSelected: null,
            token: null
        }
    }

    componentDidMount() {
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
                    value={userState.pointRanking} perfix={R.strings().point} />
            </Block>
        </FstImage>)
    }

    renderScrollableTabView = () => {
        return (
            <Block>
                <ScrollableTabView
                    style={{
                        borderColor: theme.colors.border,
                    }}
                    tabBarBackgroundColor={theme.colors.white}
                    tabBarPosition='top'
                    tabBarActiveTextColor={theme.colors.primary}
                    tabBarInactiveTextColor={theme.colors.black1}
                    tabBarUnderlineStyle={{
                        height: 2,
                        backgroundColor: theme.colors.primary
                    }}
                    renderTabBar={() =>
                        <DefaultTabBar
                            style={{
                                alignSelf: 'center',
                                paddingTop: 8,
                            }} />
                    }
                    tabBarTextStyle={theme.fonts.semibold18}
                    onChangeTab={Keyboard.dismiss}>
                    <WalletAccumulatePointsScreen tabLabel={'Lịch sử'} key={1} />
                    <PolicyScreen tabLabel={'Chính sách'} key={0} />
                </ScrollableTabView>
            </Block>
        )
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

    render() {
        return (
            <Block color={theme.colors.primary_background}>
                {this.renderViewPoint()}
                {this.renderViewOptions()}
                {this.renderScrollableTabView()}
                
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
    postDataVtoPoint = (point) => {
        this.setState({ error: null, isLoading: true })
        callAPI({
            API: requestPointToV(point),
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
                        padding: 8,
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
    title: {
        marginVertical: 10,
        marginLeft: 5,
        ...theme.fonts.robotoMedium16,
    },

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
    modalStyle:{
        justifyContent: 'center',
        borderRadius: Platform.OS === 'ios' ? 30 : 0,
        shadowRadius: 10,
        height: 280,
        // backgroundColor: '#898989'
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
        elevation: 5
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
})

const mapDispatchToProps = {
}

export default connect(mapStateToProps, mapDispatchToProps)(CardScreen)
