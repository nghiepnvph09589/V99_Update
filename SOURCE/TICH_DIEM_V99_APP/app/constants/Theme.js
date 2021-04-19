import { Dimensions, Platform, StatusBar, StyleSheet } from "react-native";
const dimension = ({ width, height } = Dimensions.get("window"));
import R from '@R'

const colors = {
  primary: "#F39C12",
  primaryDark: "#125183",
  primaryDark1: "#2E384D",
  textColor: "black",
  bottombarBg: 'white',
  defaultBg: '#f3f4f6',
  inactive: '#B2BEC3',
  indicator: "#B5BCB8",
  borderTopColor: "#dedede",
  green: "#12A74E",
  gray: "#EEEEEE",
  black: "#000",
  active: "#F39C12",
  defaultBg: "#f3f4f6",
  white: '#FFFFFF',
  primary_background: '#FFFFFF',
  red: "#EA4335",
  red2: "#EC1C25",
  red_money: "#D90429",
  headerColor: "#1b75bc",
  headerTextColor: "#FFFF",
  black1: "#666666",
  border: '#B2BEC3',
  white1: "#EFEFEF",
  green1: "#09AC29",
  black2: "#969696",
  modal: '#70707044',
  text_gray: '#979797',
  text_red: '#EC1C25',
  orange: '#F3753C',
  blue: "#1640E8",
  black_title: "#333333",
  yellow_more: "#FFBA08",
  red_more: "#D90429",
  button: "#FFBA08",
};

const sizes = {
  font: 15,
  h1: 48,
  h2: 34,
  h3: 28,
  h4: 15,
  paragraph: 15,
  caption: 13,
  captionMedium: 12,

  // global sizes
  base: 16,
  font: 14,
  border: 15,
  padding: 25
};

const fonts = {
  robotoRegular14: {
    fontSize: 14,
    fontFamily: "Roboto-Regular"
  },
  robotoBold16: {
    fontSize: 16,
    fontFamily: "Roboto-Bold"
  },
  robotoRegular12: {
    fontSize: 12,
    fontFamily: "Roboto-Regular"
  },
  robotoRegular16: {
    fontSize: 16,
    fontFamily: "Roboto-Regular"
  },
  robotoRegular18: {
    fontFamily: "Roboto-Regular",
    fontSize: 18,
    // lineHeight: 21
  },
  robotoMedium12: {
    fontSize: 12,
    fontFamily: "Roboto-Medium"
  },
  robotoMedium16: {
    fontSize: 16,
    fontFamily: "Roboto-Medium"
  },
  robotoLight12: {
    fontSize: 12,
    fontFamily: "Roboto-Light"
  },
  robotoMedium18: {
    fontSize: 18,
    fontFamily: "Roboto-Medium"
  },
  robotoMedium20: {
    fontSize: 20,
    fontFamily: "Roboto-Medium"
  },
  robotoItalic12: {
    fontSize: 12,
    fontFamily: "Roboto-Italic"
  },
  robotoMedium14: {
    fontSize: 14,
    fontFamily: "Roboto-Medium"
  },
  robotoMedium15: {
    fontSize: 15,
    fontFamily: "Roboto-Medium"
  },
  robotoLight16: {
    fontSize: 16,
    fontFamily: "Roboto-Light"
  },
  robotoRegular15: {
    fontSize: 15,
    fontFamily: "Roboto-Regular"
  },
  robotoMedium22: {
    fontSize: 22,
    fontFamily: "Roboto-Medium"
  },
  robotoRegular5: {
    fontSize: 5,
    fontFamily: "Roboto-Regular"
  },
  robotoRegular9: {
    fontSize: 9,
    fontFamily: "Roboto-Regular"
  },
  robotoItalic14: {
    fontSize: 14,
    fontFamily: "Roboto-Italic"
  },
  robotoRegular10: {
    fontSize: 10,
    fontFamily: "Roboto-Regular"
  },

  regular13: {
    fontSize: 13,
    fontFamily: R.fonts.regular
  },
  regular14: {
    fontSize: 14,
    fontFamily: R.fonts.regular
  },
  regular15: {
    fontSize: 15,
    fontFamily: R.fonts.regular
  },
  regular16: {
    fontSize: 16,
    fontFamily: R.fonts.regular
  },
  regular18: {
    fontSize: 18,
    fontFamily: R.fonts.regular
  },
  regular20: {
    fontSize: 20,
    fontFamily: R.fonts.regular
  },
  regular25: {
    fontSize: 25,
    fontFamily: R.fonts.regular
  },

  semibold12: {
    fontSize: 12,
    fontFamily: R.fonts.semibold
  },
  semibold14: {
    fontSize: 14,
    fontFamily: R.fonts.semibold
  },
  semibold15: {
    fontSize: 15,
    fontFamily: R.fonts.semibold
  },
  semibold16: {
    fontSize: 16,
    fontFamily: R.fonts.semibold
  },
  semibold17: {
    fontSize: 17,
    fontFamily: R.fonts.semibold
  },
  semibold18: {
    fontSize: 18,
    fontFamily: R.fonts.semibold
  },
  semibold25: {
    fontSize: 25,
    fontFamily: R.fonts.semibold
  },
};

const styles = StyleSheet.create({

  test: {
    flex: 1,
    backgroundColor: colors.primary,
    justifyContent: "center",
    alignItems: "center"
  },

  container: {
    flex: 1,
    backgroundColor: colors.primary_background,
    // marginTop: Platform.OS == "ios" ? 0 : -StatusBar.currentHeight,
  },

  menu: {
    flex: 1,
    marginVertical: 10,
    justifyContent: "center"
  },

  scrollHoz: {
    width: width * 0.9,
    height: height * 0.3,
    backgroundColor: colors.white,
    borderRadius: 15
  },
  border: {
    borderWidth: 0.5,
    borderColor: colors.border,
  },

  textInput: {
    height: 45,
    borderRadius: 2,
    borderWidth: 0.25,
    borderColor: colors.border,
    width: '90%',
    alignSelf: 'center',
    paddingHorizontal: 15
  },
  dropDown: {
    height: 45,
    borderRadius: 2,
    borderWidth: 0.25,
    borderColor: colors.border,
    width: '90%',
    alignSelf: 'center',
    paddingHorizontal: 15
  }
});

export { colors, sizes, fonts, styles, dimension };
const theme = { colors, sizes, fonts, styles, dimension };
export default theme;