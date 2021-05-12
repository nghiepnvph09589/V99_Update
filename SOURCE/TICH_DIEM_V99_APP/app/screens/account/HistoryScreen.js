import React, { Component } from "react";
import {
  View,
  Text,
  SafeAreaView,
  FlatList,
  Image,
  Platform,
  StatusBar,
  RefreshControl,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import Header from "../../components/DCHeader";
import reactotron from "reactotron-react-native";
import Error from "../../components/Error";
import Loading from "../../components/Loading";
import * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import { Block, Empty } from "../../components";
import { getHistory } from "../../redux/actions";
import FastImage from "../../components/FstImage";
import { HISTORY } from "../../constants/Constant";
class HistoryItem extends Component {
  render() {
    const { item, index } = this.props;
    return (
      <View style={styles._viewItem}>
        <FastImage style={styles._img} source={{ uri: item.icon }} />
        <Block style={{ paddingRight: 20 }}>
          <Text
            style={[
              Theme.fonts.robotoMedium16,
              { marginTop: 10, paddingRight: 40 }
            ]}
            numberOfLines={2}
          >
            {item.title}
          </Text>
          <Text style={[Theme.fonts.robotoRegular14, { marginTop: 3 }]}>
            {I18n.t("surplus")}
            {item.balance}
          </Text>
          <Text style={[Theme.fonts.robotoRegular14, { marginTop: 3 }]}>
            {I18n.t("code_GD")}
            {item.addpointCode}
          </Text>
          <View
            style={{ flexDirection: "row", marginTop: 3, marginBottom: 10 }}
          >
            <FastImage
              style={{ height: 15, width: 15 }}
              source={require("../../assets/images/ic_clock.png")}
            />
            <Text
              style={[
                Theme.fonts.robotoRegular12,
                { color: Theme.colors.black2, marginLeft: 5 }
              ]}
            >
              {item.hour}
            </Text>
            <Text
              style={[
                Theme.fonts.robotoRegular12,
                { color: Theme.colors.black2, marginLeft: 10 }
              ]}
            >
              {item.date}
            </Text>
          </View>
        </Block>
        <Text
          style={[
            Theme.fonts.robotoMedium18,
            {
              position: "absolute",
              right: 15,
              color:
                item.type == HISTORY.ACCUMULATE ||
                  item.type == HISTORY.IS_DONATE ||
                  item.type == HISTORY.SYSTEM_DONATE ||
                  item.type == HISTORY.CANCELED_REQUEST
                  ? Theme.colors.primary
                  : Theme.colors.red
            }
          ]}
        >
          {item.type == HISTORY.ACCUMULATE ||
            item.type == HISTORY.IS_DONATE ||
            item.type == HISTORY.SYSTEM_DONATE ||
            item.type == HISTORY.CANCELED_REQUEST
            ? "+"
            : "-"}
          {item.point} {I18n.t("point")}
        </Text>
      </View>
    );
  }
}
export class HistoryScreen extends Component {
  componentDidMount() {
    this.props.getHistory();
  }

  _renderBody() {
    const { historyState } = this.props;
    reactotron.log(historyState);
    if (historyState.isLoading) return <Loading />;
    if (historyState.error)
      return (
        <Error
          // error={historyState.error.toString()}
          onPress={() => {
            this.props.getHistory();
          }}
        />
      );
    if (historyState.data.length == 0) {
      return <Empty onRefresh={() => this.props.getHistory()} />;
    }
    return (
      <FlatList
        style={{
          flex: 1
        }}
        data={historyState.data}
        refreshControl={
          <RefreshControl
            refreshing={historyState.refreshing}
            onRefresh={() => this.props.getHistory()}
          />
        }
        keyExtractor={(item, index) => index.toString()}
        renderItem={({ item, index }) => {
          return <HistoryItem item={item} index={index}></HistoryItem>;
        }}
      ></FlatList>
    );
  }
  render() {
    return (
      <Block>
        <Header title={I18n.t("his_deal")} back />
        <SafeAreaView style={Theme.styles.container}>
          {this._renderBody()}
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  historyState: state.historyReducer
});

const mapDispatchToProps = {
  getHistory
};

export default connect(mapStateToProps, mapDispatchToProps)(HistoryScreen);

const styles = StyleSheet.create({
  _viewItem: {
    marginTop: 5,
    marginHorizontal: 5,
    borderRadius: 2,
    flexDirection: "row",
    backgroundColor: Theme.colors.white,
    alignItems: "center",
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 1
    },
    shadowOpacity: 0.22,
    shadowRadius: 2.22,

    elevation: 3
  },
  _img: {
    height: 60,
    width: 60,
    marginHorizontal: 10
  }
});
