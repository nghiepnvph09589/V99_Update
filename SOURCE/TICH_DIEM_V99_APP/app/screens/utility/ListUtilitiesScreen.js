import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  ScrollView,
  Image,
  FlatList,
  RefreshControl,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import { getUtility } from "../../redux/actions";
import reactotron from "reactotron-react-native";
import { UTILITY, SCREEN_ROUTER } from "../../constants/Constant";
import theme, * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import { Loading, Empty, Error, Block, FstImage, FastImage } from "../../components";
import NavigationUtil from "../../navigation/NavigationUtil";
import DateUtil from "../../utils/DateUtil";
class UtilityItem extends Component {
  render() {
    const { item, index } = this.props;
    return (
      <TouchableOpacity
        style={styles._viewItem}
        onPress={() => {
          NavigationUtil.navigate(SCREEN_ROUTER.DETAIL_UTILITY, { newsID: item.newsID });
        }}
      >
        <FstImage
          style={{
            height: 100,
            width: 100,
            marginHorizontal: 10,
            marginVertical: 10,
            borderRadius: 5,
          }}
          resizeMode="cover"
          source={{
            uri: item.urlImage
          }}
        />
        <Block>
          <Text
            style={[
              Theme.fonts.semibold16,
              {
                marginTop: 10,
                paddingRight: 10,
                color: Theme.colors.black_title
              }
            ]}
            numberOfLines={2}
          >
            {item.title}
          </Text>
          <Text
            style={[
              Theme.fonts.regular14,
              { paddingRight: 10, color: Theme.colors.text_gray }
            ]}
            numberOfLines={2}
          >
            {item.description}
          </Text>

          <Text
            style={[
              Theme.fonts.regular13,
              {
                position: "absolute",
                bottom: 10,
                right: 10
              }
            ]}
          >{DateUtil.formatTime(item.createDate)} {DateUtil.formatShortDate(item.createDate)}
          </Text>
        </Block>
      </TouchableOpacity>
    );
  }
}

export class ListUtilitiesScreen extends Component {
  componentDidMount() {
    const { type } = this.props;
    this.props.getUtility(type);
  }
  render() {
    const {
      newsState,
      saleState,
      guaranteeState,
      type,
      refreshing
    } = this.props;
    reactotron.log(newsState);
    var isLoading = newsState.isLoading;
    var error = newsState.error;
    var data = [];
    switch (type) {
      case UTILITY.NEWS:
        isLoading = newsState.isLoading;
        error = newsState.error;
        data = newsState.data;
        break;
      case UTILITY.EVENT:
        isLoading = saleState.isLoading;
        error = saleState.error;
        data = saleState.data;
        break;

      case UTILITY.GUARANTY:
        isLoading = guaranteeState.isLoading;
        error = guaranteeState.error;
        data = guaranteeState.data;
        break;
    }
    if (isLoading) return <Loading />;
    if (error)
      return (
        <Error
          //error={error.toString()}
          onPress={() => {
            this.props.getUtility(type);
          }}
        />
      );
    if (data.length == 0) {
      return <Empty onRefresh={() => this.props.getUtility(type)} />;
    }

    return (
      <Block>
        <FlatList
          style={{
            flex: 1,
            backgroundColor: Theme.colors.white,
            paddingBottom: 20,
          }}
          refreshControl={
            <RefreshControl
              refreshing={refreshing}
              onRefresh={() => this.props.getUtility(type)}
            />
          }
          data={data}
          keyExtractor={(item, index) => index.toString()}
          renderItem={({ item, index }) => {
            return <UtilityItem item={item} index={index} />;
          }}
        ></FlatList>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  newsState: state.utilityReducer.news,
  saleState: state.utilityReducer.sale,
  guaranteeState: state.utilityReducer.guarantee,
  refreshing: state.utilityReducer.refreshing
});

const mapDispatchToProps = {
  getUtility
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ListUtilitiesScreen);

const styles = StyleSheet.create({
  _viewItem: {
    marginTop: 5,
    flexDirection: "row",
    backgroundColor: Theme.colors.white,
    marginBottom: 5,
  }
})