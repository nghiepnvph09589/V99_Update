import React from "react";
import {
  Image,
  Platform,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
  FlatList,
  RefreshControl
} from "react-native";
import NumberFormat from "react-number-format";
import * as theme from "../constants/Theme";
export default class FormatMoney extends React.Component {
  render() {
    const { value, color, fonts, style, perfix, title } = this.props;
    return (
      <NumberFormat
        value={value}
        displayType={"text"}
        thousandSeparator={true}
        renderText={value => (
          <Text
            numberOfLines={1}
            style={[
              fonts,
              {
                color: color,
                // padding: 5
              },
              style
            ]}
          >
            {title}{title && ': '}{value} {perfix}
          </Text>
        )}
      />
    );
  }
}