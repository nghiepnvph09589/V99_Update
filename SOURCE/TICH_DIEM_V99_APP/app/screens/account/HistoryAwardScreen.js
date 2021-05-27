import { Block, Loading, Error } from "@app/components";
import { requestGetRefPointHistory } from "@app/constants/Api";
import { getHomeData } from "@app/redux/sagas/NetworkSaga";
import DateUtil from "@app/utils/DateUtil";
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
              alignContent: "center"
            }}
          >
            <Text style={{ flex: 0.7, textAlign: "center" }} children={"STT"} />
            <Text
              style={{ flex: 3, textAlign: "center" }}
              children={"Người được giới thiệu"}
            />
            <Text
              style={{ flex: 2.2, textAlign: "center" }}
              children={"Số điện thoại"}
            />
            <Text
              style={{ flex: 2.1, textAlign: "center" }}
              children={"Thời gian"}
            />
            <Text
              style={{ flex: 1.2, textAlign: "center" }}
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
              alignItems: "center"
            }}
          >
            <Text
              style={{ flex: 0.7, textAlign: "center" }}
              children={index + 1}
            />
            <Text
              style={{
                flex: 3,
                textAlign: "center"
              }}
              numberOfLines={3}
              children={item.customerRefName}
            />
            <Text
              style={{
                flex: 2.1,
                textAlign: "center"
              }}
              children={item.customerRefPhone}
            />
            <Text
              style={{
                flex: 2.2,
                textAlign: "center"
              }}
              children={DateUtil.formatDateTime(item.createdDate)}
            />
            <Text
              style={{ flex: 1, alignSelf: "center", textAlign: "center" }}
              children={item.point}
            />
          </View>
        )}
      />
    );
  };
  render() {
    return (
      <Block>
        <Header title={"Lịch sử nhận thưởng"} back />
        <SafeAreaView style={{ flex: 1 }}>
          <KeyboardAvoidingView
            enabled
            behavior={Platform.OS === "ios" ? "padding" : null}
            style={{ flex: 1 }}
          >
            {this._renderBody()}
          </KeyboardAvoidingView>
        </SafeAreaView>
      </Block>
    );
  }
}

const styles = StyleSheet.create({});
