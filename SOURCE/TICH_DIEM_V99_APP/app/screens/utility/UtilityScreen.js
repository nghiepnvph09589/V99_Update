import React, { Component } from "react";
import { Text, View, SafeAreaView } from "react-native";
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
import ListUtilitiesScreen from "./ListUtilitiesScreen";
import { UTILITY } from "../../constants/Constant";
import R from "@app/assets/R";
export default class UtilityScreen extends Component {
  render() {
    const { navigation } = this.props
    const initialPage = navigation.getParam('initial_page');
    return (
      <Block>
        <DCHeader title={R.strings().news_event} back />
        <SafeAreaView style={{ flex: 1, backgroundColor: Theme.colors.white }}>
          <ScrollableTabView
            initialPage={initialPage}
          >
            <ListUtilitiesScreen
              tabLabel={I18n.t("news")}
              key={0}
              type={UTILITY.NEWS}
            />
            <ListUtilitiesScreen
              tabLabel={R.strings().event}
              key={1}
              type={UTILITY.EVENT}
            />

          </ScrollableTabView>
        </SafeAreaView>
      </Block>
    );
  }
}
