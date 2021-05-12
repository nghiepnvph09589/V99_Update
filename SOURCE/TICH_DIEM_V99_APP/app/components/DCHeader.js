import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  Platform,
  StatusBar, Image, StyleSheet
} from "react-native";
import { Header } from "react-native-elements";
import NavigationUtil from "../navigation/NavigationUtil";
import * as theme from "../constants/Theme";
import { SCREEN_ROUTER } from "../constants/Constant";
import PropTypes from "prop-types";
import { withNavigation } from "react-navigation";
import reactotron from "reactotron-react-native";
import { FastImage } from '.'
import Utils from '../utils/Utils'
import FstImage from "./FstImage";

class MainHeader extends Component {

  static propTypes = {
    title: PropTypes.string.isRequired,
    hideBackButton: PropTypes.bool,
    rightComponent: PropTypes.element,
  };
  static defaultProps = {
    hideBackButton: false,
    rightComponent: null
  };

  render() {
    const { title, hideBackButton, backAction,
      rightComponent, navigation, isWhiteBackground,
      ...props
    } = this.props;
    const parent = navigation.dangerouslyGetParent();
    const showBackButton = !hideBackButton && parent && parent.state && parent.state.routeName !== SCREEN_ROUTER.BOTTOM_BAR

    return (
      <Header
        containerStyle={[{
          marginTop: Platform.OS == "ios" ? 0 : -StatusBar.currentHeight,
        }, isWhiteBackground && styles.shadow]}
        placement="left"
        backgroundColor={isWhiteBackground ? theme.colors.white : theme.colors.primary}
        leftComponent={showBackButton &&
          <TouchableOpacity onPress={() => backAction ? backAction() : NavigationUtil.goBack()}>
            <Image
              style={{
                height: 20, width: 25,
                resizeMode: "contain",
                tintColor: isWhiteBackground && theme.colors.primary,
              }}
              source={require("../assets/images/ic_back.png")}
              resizeMode='contain'
            />
          </TouchableOpacity>
        }
        centerComponent={
          <Text
            style={[theme.fonts.semibold20,
            { color: isWhiteBackground ? theme.colors.black1 : theme.colors.white }]}
          >
            {title}
            {/* {Utils.toPascalCase(title)} */}
          </Text>
        }
        rightComponent={rightComponent}
        {...props}
      />
    );
  }
}

export default withNavigation(MainHeader);

const styles = StyleSheet.create({
  shadow: {
    overflow: 'hidden',
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.23,
    shadowRadius: 2.62,
    elevation: 4,
  },
});
