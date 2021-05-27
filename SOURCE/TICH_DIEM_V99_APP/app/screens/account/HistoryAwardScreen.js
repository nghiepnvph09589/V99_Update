import {
  Block,
  Loading,
  Error,
  Empty,
  DCHeader,
  NumberFormat
} from "@app/components";
import { requestGetRefPointHistory } from "@app/constants/Api";
import { fonts } from "@app/constants/Theme";
import { getHomeData } from "@app/redux/sagas/NetworkSaga";
import DateUtil from "@app/utils/DateUtil";
import { formatNumber, formatPrice } from "@app/utils/NumberUtils";
import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  SafeAreaView,
  KeyboardAvoidingView,
  ScrollView,
  FlatList
} from "react-native";
import Header from "../../components/DCHeader";
import I18n from "../../i18n/i18n";
export default class HistoryAwardScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      err: null,
      data: []
    };
  }
  componentDidMount() {
    this.getData();
  }
  getData = async () => {
    this.setState({
      isLoading: true
    });
    try {
      const res = await requestGetRefPointHistory();
      if (res) {
        this.setState({
          isLoading: false,
          data: res.data
        });
      }
    } catch (error) {
      this.setState({
        err: true,
        isLoading: false
      });
    }
  };

  _renderBody = () => {
    if (this.state.isLoading) return <Loading />;
    if (this.state.err)
      return (
        <Error
          onPress={() => {
            this.getData();
          }}
        />
      );
    if (!this.state.data?.length)
      return <Empty onRefresh={() => this.getData()} />;
    return (
      <FlatList
        refreshing={this.state.isLoading}
        onRefresh={() => this.getData()}
        contentContainerStyle={{ padding: 8 }}
        keyExtractor={(item, index) => index + ""}
        ListHeaderComponent={
          <View
            style={{
              flexDirection: "row",
              justifyContent: "center",
              alignContent: "center",
              borderBottomWidth: 0.8,
              borderBottomColor: "#DDD",
              paddingBottom: 5
            }}
          >
            <Text
              style={{
                flex: 0.7,
                textAlign: "center",
                ...fonts.robotoMedium14
              }}
              children={"STT"}
            />
            <Text
              style={{
                flex: 3,
                textAlign: "center",
                ...fonts.robotoMedium14
              }}
              children={"Người được giới thiệu"}
            />
            <Text
              style={{
                flex: 2.2,
                textAlign: "center",
                ...fonts.robotoMedium14
              }}
              children={"Số điện thoại"}
            />
            <Text
              style={{
                flex: 2.1,
                textAlign: "center",
                ...fonts.robotoMedium14
              }}
              children={"Thời gian"}
            />
            <Text
              style={{
                flex: 1.2,
                textAlign: "center",
                ...fonts.robotoMedium14
              }}
              children={"Điểm thưởng"}
            />
          </View>
        }
        style={{ flex: 1 }}
        data={this.state.data}
        renderItem={({ item, index }) => (
          <View
            style={{
              flexDirection: "row",
              marginTop: 10,
              justifyContent: "center",
              alignItems: "center",
              borderBottomWidth: 0.8,
              borderBottomColor: "#DDD",
              paddingBottom: 5
            }}
          >
            <Text
              style={{ flex: 0.7, textAlign: "center", ...fonts.regular14 }}
              children={index + 1}
            />
            <Text
              style={{
                flex: 3,
                textAlign: "center",
                ...fonts.regular14
              }}
              numberOfLines={3}
              children={item.customerRefName}
            />
            <Text
              style={{
                flex: 2.1,
                textAlign: "center",
                ...fonts.regular14
              }}
              children={item.customerRefPhone}
            />
            <Text
              style={{
                flex: 2.2,
                textAlign: "center",
                ...fonts.regular14
              }}
              children={DateUtil.formatDateTime(item.createdDate)}
            />
            <Text
              style={{
                flex: 1,
                alignSelf: "center",
                textAlign: "center",
                ...fonts.regular14
              }}
              children={formatNumber(item.point)}
            />
          </View>
        )}
      />
    );
  };
  render() {
    return (
      <Block>
        <DCHeader isWhiteBackground title={"Lịch sử nhận thưởng"} back />
        <SafeAreaView style={{ flex: 1 }}>{this._renderBody()}</SafeAreaView>
      </Block>
    );
  }
}

const styles = StyleSheet.create({});
