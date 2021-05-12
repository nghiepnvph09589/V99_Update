import React, { Component } from "react";
import {
  View,
  Text,
  Image,
  ScrollView,
  StyleSheet,
  ImageBackground,
  TouchableOpacity,
  FlatList,
  Platform,
  SafeAreaView, Linking
} from "react-native";
import { connect } from "react-redux";
import {
  Loading,
  Empty,
  Error,
  Block,
  ScrollableTabView,
  DCHeader,
  FastImage
} from "../../components";
import theme, * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import reactotron from "reactotron-react-native";
import NavigationUtil from "../../navigation/NavigationUtil";
import { SCREEN_ROUTER } from "../../constants/Constant";
import { getNewsDetail } from "../../constants/Api";
import DateUtil from "../../utils/DateUtil";
import HTMLView from "react-native-render-html";
import * as HTMLRenderers from '../../components/HTMLRenderers';

// import ImageViewer from 'react-native-image-zoom-viewer';
import {
  ifIphoneX,
  getBottomSpace,
  getStatusBarHeight
} from "react-native-iphone-x-helper";

class UtilityItem extends Component {
  render() {
    const { item, index } = this.props;
    return (
      <TouchableOpacity
        style={{
          marginHorizontal: 5,
          borderRadius: 5,
          flexDirection: "row",
          backgroundColor: Theme.colors.white,
          marginBottom: 5,
          marginTop: 5,
          shadowColor: "#000",
          shadowOffset: {
            width: 0,
            height: 1
          },
          shadowOpacity: 0.22,
          shadowRadius: 2.22,
          elevation: 3
        }}
        onPress={() => {
          NavigationUtil.push(SCREEN_ROUTER.DETAIL_UTILITY, { newsID: item.newsID });
        }}
      >
        <FastImage
          style={{
            height: 100,
            width: 100,
            marginHorizontal: 10,
            marginVertical: 10
          }}
          resizeMode="cover"
          source={{
            uri: item.urlImage
          }}
        />
        <Block>
          <Text
            style={[
              Theme.fonts.robotoMedium15,
              { marginTop: 10, paddingRight: 10 }
            ]}
            numberOfLines={2}
          >
            {item.title}
          </Text>
          <Text
            style={[Theme.fonts.robotoRegular12, { paddingRight: 10 }]}
            numberOfLines={2}
          >
            {item.description}
          </Text>

          <Text
            style={[
              Theme.fonts.robotoRegular12,
              {
                position: "absolute",
                bottom: 10,
                right: 10
              }
            ]}
          >
            {DateUtil.formatShortDate(item.createDate)}
          </Text>
        </Block>
      </TouchableOpacity>
    );
  }
}
export class DetailUtilityScreen extends Component {
  state = {
    newsDetail: [],
    newsRelative: [],
    isLoading: false,
    error: null,
    aspectRatio: 1.1
  };
  componentDidMount() {
    this._getNewsDetail();
  }

  getSizeImage = () => {
    const image = this.state.newsDetail.urlImage

    Image.getSize(
      image,
      (width, height) => {
        var aspectRatio = width / height

        this.setState({ aspectRatio })
      }, (error) => {
        this.setState({ aspectRatio: 1.1 })
      })
  }

  _getNewsDetail = async () => {
    const newsID = this.props.navigation.getParam("newsID");
    this.setState({
      ...this.state,
      isLoading: true,
      error: null
    });
    try {
      const response = await getNewsDetail(newsID);
      this.setState({
        newsDetail: response.data.newsDetail,
        newsRelative: response.data.newsRelative,
        isLoading: false
      }, this.getSizeImage);
    } catch (error) {
      this.setState({
        ...this.state,
        isLoading: false,
        error: error
      });
      reactotron.log(error);
    }
  };

  render() {
    const { isLoading, newsDetail, newsRelative, error } = this.state;
    if (isLoading) return <Loading />;
    if (error)
      return (
        <Block>
          <Error
            onPress={() => {
              this._getNewsDetail();
            }}
          />
          <TouchableOpacity
            style={styles._imgBack}
            onPress={() => {
              NavigationUtil.goBack();
            }}
          >
            <FastImage
              style={{
                height: 36,
                width: 36
              }}
              source={require("../../assets/images/ic_back2.png")}
            />
          </TouchableOpacity>
        </Block>
      );
    return (
      <Block>
        <SafeAreaView style={{ flex: 1 }}>
          <ScrollView
            style={{
              backgroundColor: Theme.colors.white
            }}
          >
            <TouchableOpacity
              onPress={() => {
                NavigationUtil.navigate(SCREEN_ROUTER.IMAGE_VIEWER, {
                  images: {
                    listImage: [newsDetail.urlImage],
                    index: 0
                  }
                });
              }}
            >
              <FastImage
                style={{
                  ...styles._imgItem,
                  aspectRatio: this.state.aspectRatio
                }}
                resizeMode="cover"
                source={{ uri: newsDetail.urlImage }}
              />
            </TouchableOpacity>
            <View style={{ paddingHorizontal: '2.5%' }}>
              <Text style={[Theme.fonts.robotoMedium16, { paddingTop: 10 }]}>
                {newsDetail.title}
              </Text>

              <Text
                style={[
                  Theme.fonts.robotoItalic12,
                  {
                    flexDirection: "row",
                    marginVertical: 5
                  }
                ]}
              >
                {DateUtil.formatTime(newsDetail.createDate)} {DateUtil.formatShortDate(newsDetail.createDate)}
              </Text>
              <View
                style={{ height: 0.5, backgroundColor: Theme.colors.border }}
              />
            </View>
            <View style={{ marginHorizontal: 25 }}>
              <HTMLView
                renderers={HTMLRenderers}
                html={newsDetail.content || ''}
                containerStyle={{
                  backgroundColor: theme.colors.white,
                  width: width * 0.95,
                  paddingVertical: 15,
                  alignSelf: 'center'
                }}
                imagesMaxWidth={width * 0.95}
                ignoredStyles={['height', 'width', 'font-family']}
                onLinkPress={(even, href) => Linking.openURL(href)}
                alterChildren={(node) => {
                  if (node.name === 'iframe') {
                    delete node.attribs.width;
                    delete node.attribs.height;
                  }
                  return node.children;
                }}

              />
              <Text style={[Theme.fonts.robotoRegular14]}>
                {newsDetail.description}
              </Text>
            </View>
            {/* <View
              style={{
                flexDirection: "row",
                marginHorizontal: 10,
                alignItems: "center"
              }}
            >
              <Text
                style={[
                  Theme.fonts.robotoMedium16,
                  { color: Theme.colors.red }
                ]}
              >
                {I18n.t("articles_relative")}
              </Text>
              <View
                style={{
                  height: 0.5,
                  marginLeft: 5,
                  backgroundColor: Theme.colors.red,
                  marginTop: 14,
                  flex: 1
                }}
              />
            </View> */}

            {/* <View
              style={{
                flex: 1,
                backgroundColor: Theme.colors.backgroundColor,
                marginBottom: 20,
                paddingTop: 5
              }}
            >
              {newsRelative.map((item, index) => {
                return <UtilityItem item={item} index={index} key={index} />;
              })}
            </View> */}
          </ScrollView>
          <TouchableOpacity
            style={styles._imgBack}
            onPress={() => {
              NavigationUtil.goBack();
            }}
          >
            <FastImage
              style={{
                height: 36,
                width: 36
              }}
              source={require("../../assets/images/ic_back2.png")}
            />
          </TouchableOpacity>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DetailUtilityScreen);

const styles = StyleSheet.create({
  _imgItem: {
    width: "100%",
    // height: height * 0.4,
    backgroundColor: Theme.colors.white,
  },
  _imgBack: {
    height: 36,
    width: 36,
    position: "absolute",
    left: 10,
    top: ifIphoneX ? getStatusBarHeight() + 5 : 5
  }
});
