import R from "@app/assets/R";
import { requestGetListLastRefCode } from "@app/constants/Api";
import theme from "@app/constants/Theme";
import callAPI from "@app/utils/CallApiHelper";
import React, { Component } from "react";
import {
  FlatList,
  RefreshControl,
  SafeAreaView,
  View,
  Text,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import reactotron from "reactotron-react-native";
import {
  Block,
  CartItem,
  DCHeader,
  Empty,
  Error,
  Loading
} from "../../components";

export class IntroduceCustomersScreen extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: true,
      error: null,
      data: []
    };
  }

  componentDidMount() {
    this.getData();
  }

  getData = () => {
    this.setState({ error: null, isLoading: true });

    callAPI({
      API: requestGetListLastRefCode,
      onSuccess: res => {
        this.setState({ data: res.data });
      },
      onError: err => {
        this.setState({ error: JSON.stringify(err) });
      },
      onFinaly: () => {
        this.setState({ isLoading: false });
      }
    });
  };

  renderBody() {
    const { isLoading, error, data } = this.state;
    if (isLoading) return <Loading />;
    if (error) return <Error onPress={this.getData} />;
    if (!data.length) {
      return <Empty onRefresh={this.getData} />;
    }
    return (
      <FlatList
        style={{
          paddingHorizontal: "3%"
        }}
        ListHeaderComponent={
          <View style={styles.header_list}>
            <Text
              style={{
                flex: 1,
                ...styles.text_header
              }}
            >
              STT
            </Text>
            <Text
              style={{
                flex: 6,
                ...styles.text_header
              }}
            >
              Họ tên
            </Text>
            <Text
              style={{
                flex: 3,
                ...styles.text_header
              }}
            >
              Số điện thoại
            </Text>
          </View>
        }
        data={data}
        refreshControl={
          <RefreshControl refreshing={false} onRefresh={this.getData} />
        }
        keyExtractor={(item, index) => index.toString()}
        renderItem={this.renderItem}
      />
    );
  }

  renderItem = ({ item, index }) => {
    return (
      <View
        style={{
          alignItems: "center",
          ...styles.header_list,
          marginTop: 15
        }}
      >
        <Text
          style={{
            flex: 1,
            ...styles.text_item
          }}
        >
          {index + 1}
        </Text>
        <Text
          style={{
            flex: 6,
            ...styles.text_item
          }}
        >
          {item.name}
        </Text>
        <Text
          style={{
            flex: 3,
            ...styles.text_item
          }}
        >
          {item.phone}
        </Text>
      </View>
    );
  };

  render() {
    return (
      <Block color={theme.colors.primary_background}>
        <DCHeader isWhiteBackground title={R.strings().introduce_customers} />
        <SafeAreaView style={theme.styles.container}>
          {this.renderBody()}
        </SafeAreaView>
      </Block>
    );
  }
}

const styles = StyleSheet.create({
  header_list: {
    flexDirection: "row",
    paddingBottom: 10,
    borderBottomColor: theme.colors.border,
    borderBottomWidth: 0.5,
    marginTop: 10
  },
  text_header: {
    ...theme.fonts.semibold16
  },
  text_item: {
    ...theme.fonts.regular16
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(IntroduceCustomersScreen);
