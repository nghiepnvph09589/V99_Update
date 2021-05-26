import React, { Component } from "react";
import {
    View,
    Text,
    SafeAreaView,
    Image,
    FlatList,
    TouchableOpacity,
    Dimensions,
    StyleSheet,
    ScrollView,
    RefreshControl,
    Alert,
    ActivityIndicator,
} from "react-native";
import { connect } from "react-redux";
import * as theme from "../../constants/Theme";
import {
    Block,
    DCHeader,
    NumberFormat,
    CartRightComponent,
    Loading,
    Empty,
    Error, FastImage
} from "../../components";
import reactotron from "reactotron-react-native";
import NavigationUtil from "../../navigation/NavigationUtil";
import { SCREEN_ROUTER, REDUCER } from "../../constants/Constant";
import { getListProduct, getProduct } from "../../redux/actions";
import * as Api from "../../constants/Api";
import AsyncStorage from "@react-native-community/async-storage"
import { SearchBar } from "react-native-elements";
import R from "@app/assets/R";
import { formatNumber } from "@app/utils/NumberUtils";

const formatData = (listProd, numberColumns) => {
    const numberOfFullRows = Math.floor(listProd.length / numberColumns);

    let numberOfElementsLastRow = listProd.length - numberOfFullRows * numberColumns;
    while (
        numberOfElementsLastRow !== numberColumns &&
        numberOfElementsLastRow !== 0
    ) {
        listProd.push({ key: `blank-${numberOfElementsLastRow}`, empty: true });
        numberOfElementsLastRow++;
    }

    return listProd;
};

const numberColumns = 2;

class ListProductScreen extends Component {
    constructor(props) {
        super(props);

        this.state = {
            data: [],
            listProd: [],
            isLoading: true,
            error: null,
            childID: -1,
            token: null,
            search: "",
            page: 1,
        };
    }

    timeout = null

    componentWillReceiveProps(preProps) {
        if (this.timeout) clearTimeout(this.timeout)
        this.setState({ search: preProps.text })
        this.timeout = setTimeout(() => this.getData(1, preProps.text), 300);
    }

    checkToken = async () => {
        var token = await AsyncStorage.getItem('token')
        this.setState({ token: token })
    }

    getData = (page, text) => {
        const { navigation, item } = this.props;
        payload = {
            page,
            cateID: item.id,
            text
        };

        Api.requestListProduct(payload)
            .then(result => {
                this.setState({
                    data: result.data.listProduct,
                    listProd: result.data.listProduct,
                });
            })
            .catch(error => {
                this.setState({
                    error: error
                });
            })
            .finally(() => {
                this.setState({
                    isLoading: false
                });
            });
    };

    onEndReached = (page) => {
        const { navigation, item } = this.props;
        const { childID, search } = this.state;
        payload = {
            page,
            cateID: item.id,
            text: search,
        };

        Api.requestListProduct(payload)
            .then(result => {
                if ((result.data.listProduct).length != 0) {
                    this.setState({
                        ...this.state,
                        data: this.state.data.concat(result.data.listProduct),
                        listProd: this.state.data.concat(result.data.listProduct),
                    });
                }
            })

            .catch(error => {
                this.setState({
                    ...this.state,
                    error: error
                });
            })
            .finally(() => {
                this.setState({
                    ...this.state,
                    isLoading: false
                });
            });
    };

    componentDidMount() {
        const { page, search } = this.state
        this.checkToken()
        this.getData(page, search);
    }

    _onRefresh = () => {
        const { page, search } = this.state;
        const { item } = this.props;
        this.props.getProduct()
        this.setState(
            {
                ...this.state,
                isLoading: true,
                error: null
            },
            () => this.getData(page, search)
        );
    };

    handleLoadMore = () => {
        this.setState(this.onEndReached(payload.page + 1));
    }

    renderFooter = () => {
        return (
            <View style={{
                marginTop: 10,
                textAlign: "center"
            }}>
                <ActivityIndicator animating size="large" />
            </View>
        )
    }

    renderBody() {
        const { indexSelected, data, isLoading, error, listProd } = this.state;
        const {homeState} = this.props;
        if (isLoading) return <Loading />;
        if (error)
            return (
                <Error
                    reload={() => this._onRefresh()}
                />
            );
        if (!listProd.length) return <Empty onRefresh={() => this._onRefresh()} />;
        return (
            <Block
                style={{
                    backgroundColor: theme.colors.primary_background
                }}
            >
                <FlatList
                    data={formatData(listProd, numberColumns) || []}
                    keyExtractor={(item, index) => index.toString()}
                    refreshControl={
                        <RefreshControl
                            refreshing={data.isRefresh}
                            onRefresh={() => this._onRefresh()}
                        />
                    }
                    renderItem={({ item, index }) => {
                        if (item.empty === true) {
                            return <View style={[styles.item, styles.itemInvisible]} />;
                        }
                        return <ProductItem item={item} index={index} checkAcc = {homeState.userInfo.isVip}/>;
                    }}
                    numColumns={numberColumns}
                    onEndReached={this.handleLoadMore}
                    onEndReachedThreshold={1}
                    enableEmptySections={true}
                />
            </Block>
        );
    }

    render() {
        return (
            <Block>
                {this.renderBody()}
            </Block>
        );
    }
}

class ProductItem extends Component {
    render() {
        const { item, checkAcc } = this.props;
        return (
            <TouchableOpacity
                style={{
                    flex: 1,
                    marginVertical: 4,
                    marginHorizontal: 3,
                    backgroundColor: theme.colors.white,
                    borderColor: "#707070",

                    shadowColor: "#000",
                    shadowOffset: {
                        width: 0,
                        height: 2
                    },
                    shadowOpacity: 0.23,
                    shadowRadius: 2.62,
                    elevation: 4,
                    borderRadius: 5
                }}
                onPress={() => {
                    NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_PRODUCT, {
                        item: item
                    });
                }}
            >
                <FastImage
                    resizeMode="cover"
                    style={{
                        width: '100%',
                        aspectRatio: 1.5,
                        marginVertical: 10,
                        alignSelf: "center"
                    }}
                    source={{ uri: item.image[0] }}
                />

                <View style={{ height: 52 }}>
                    <Text
                        style={[
                            theme.fonts.semibold14,
                            {
                                marginHorizontal: 10,
                                padding: 5
                            }
                        ]}
                        numberOfLines={2}
                    >
                        {item.name}
                    </Text>
                </View>

                <View
                    style={{
                        flexDirection: 'column',
                        marginHorizontal: 16,
                        marginBottom: 10
                    }}
                >
                    {/* <NumberFormat
                        value={item.priceVIP}
                        color={theme.colors.red_money}
                        fonts={theme.fonts.robotoRegular14}
                        perfix="VNĐ"
                    /> */}
                    <Text style={{
            color: "red",
            textDecorationLine:(checkAcc===0)?'none': 'line-through',
            fontSize: 15
          }}
            children={'Giá thường: ' + formatNumber(item.price) + 'đ'} />

          <Text style={{
            textDecorationLine:(checkAcc===1)?'none': 'line-through',
            color: "red",
            fontSize: 15
          }}
            children={'Giá VIP: ' + formatNumber(item.priceVIP) + 'đ'} />
                </View>
            </TouchableOpacity>
        );
    }
}

const mapStateToProps = state => ({
    homeState: state.homeReducer,
    cartState: state.cartReducer,
    countCartState: state[REDUCER.COUNT_CART],
    listProductState: state.listProductReducer,
    
});

const mapDispatchToProps = {
    getListProduct, getProduct
};

const styles = StyleSheet.create({
    item: {
        alignItems: "center",
        justifyContent: "center",
        flex: 1,
        margin: 1,
        height: Dimensions.get("window").width / numberColumns // approximate a square
    },
    itemInvisible: {
        backgroundColor: "transparent"
    }
});

export default connect(mapStateToProps, mapDispatchToProps)(ListProductScreen);
