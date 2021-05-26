import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  SafeAreaView,
  TouchableOpacity,
  Image,
  ImageBackground,
  RefreshControl, Alert, StatusBar
} from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import reactotron from "reactotron-react-native";
import { getProduct, getCountCart, getUserInfo } from "../../redux/actions";
import {
  Loading, DCHeader, Block,
  CartRightComponent, Error, ScrollableTabView
} from "../../components";
import theme from "@theme";
import { SCREEN_ROUTER, REDUCER } from "../../constants/Constant";
import I18n from '../../i18n/i18n'
import NavigationUtil from "../../navigation/NavigationUtil";
import AsyncStorage from "@react-native-community/async-storage"
import { requireLogin } from '../../utils/AlertRequireLogin'
import ListProductScreen from "./ListProductScreen";
import { Header, SearchBar } from "react-native-elements";
import { requestListProduct } from '@api'

export class ProductScreen extends Component {
  componentDidMount() {
    this.onRefresh()
  }

  constructor(props) {
    super(props)
    this.state = {
      token: null,
      text: '',
    }
  }

  onRefresh = async () => {
    this.props.getProduct()
    var token = await AsyncStorage.getItem('token')
    if (!!token) {
      this.setState({ token })
      this.props.getCountCart()
    }
  }

  renderBody() {
    const { text } = this.state
    const {homeState, productState, countCartState } = this.props
    if (productState.isLoading) return <Loading />
    if (productState.error)
      return <Error
        onPress={() => this.props.getProduct()} />;
        
    return (
      <Block>
        <SafeAreaView style={{ flex: 1, backgroundColor: theme.colors.white }}>
          <ScrollableTabView
            scrollableTabBar
            onChangeTab={(value) => {
              this.setState({ text: '' })
            }}
          >
            {
              productState.data.map((item, index) => {
                return <ListProductScreen
                checkAcc = {homeState.userInfo.isVip}
                  tabLabel={item.name}
                  key={index}
                  item={item}
                  text={text || ''}
                />
              })
            }
          </ScrollableTabView>
        </SafeAreaView>
      </Block>
    )
  }


  render() {
    const { countCartState } = this.props
    return (
      <Block>
        <Header
          style={{ width }}
          containerStyle={{
            width: width,
            marginTop: Platform.OS == "ios" ? 0 : -StatusBar.currentHeight,
          }}
          placement='left'
          backgroundColor={theme.colors.primary}
          leftContainerStyle={{ width: 0 }}
          centerComponent={<SearchBar
            round
            containerStyle={{
              width: '100%',
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
            onChangeText={this.searchFilter}
            placeholder={'Nhập từ khóa tìm kiếm'}
            value={this.state.text}
          />}
          rightComponent={
            <View style={{ width: width * 0.1 }}>
              <CartRightComponent
                quantity={countCartState.countCart}
                onPress={() => {
                  if (!!this.state.token)
                    NavigationUtil.navigate(SCREEN_ROUTER.CART)
                  // NavigationUtil.navigate(SCREEN_ROUTER.CART, { listItemIDSelected: [93, 92] })
                  else requireLogin()
                }} />
            </View>
          }
        />
        <SafeAreaView style={theme.styles.container}>
          {this.renderBody()}
        </SafeAreaView>
      </Block>
    );
  }

  searchFilter = async (text) => {
    this.setState({ text })
  }
}

const mapStateToProps = state => ({
  homeState: state.homeReducer,
  productState: state.productReducer,
  countCartState: state[REDUCER.COUNT_CART],
});

const mapDispatchToProps = {
  getProduct,
  getCountCart,
  getUserInfo
};

export default connect(mapStateToProps, mapDispatchToProps)(ProductScreen);
