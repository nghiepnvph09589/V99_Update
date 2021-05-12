import React, { Component } from "react";
import { View, Text, SafeAreaView } from "react-native";
import { connect } from "react-redux";
import reactotron from "reactotron-react-native";
import {
  Loading,
  Empty,
  Error,
  Block,
  ScrollableTabView,
  DCHeader
} from "../../components";
import * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import ListOrderScreen from "./ListOrderScreen";
import { ORDER } from "../../constants/Constant";
import R from "@app/assets/R";

export class OrderScreen extends Component {
  render() {
    return (
      <Block>
        <DCHeader title={I18n.t("bill")} back isWhiteBackground />
        <SafeAreaView style={Theme.styles.container}>
          <ScrollableTabView scrollableTabBar>
            <ListOrderScreen tabLabel={I18n.t("pending")} key={0} status={ORDER.PENDING} />
            <ListOrderScreen tabLabel={R.strings().processing} key={1} status={ORDER.CONFIRM} />
            <ListOrderScreen tabLabel={R.strings().history} key={2} status={ORDER.HISTORY} />
          </ScrollableTabView>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(mapStateToProps, mapDispatchToProps)(OrderScreen);
