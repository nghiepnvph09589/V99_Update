import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  Image,
  StyleSheet,
  SafeAreaView,
  KeyboardAvoidingView,
  Platform,
  TouchableOpacity
} from "react-native";
import { connect } from "react-redux";
import TextFieldInput from "../../components/TextFieldInput";
import I18n from "../../i18n/i18n";
import Button from "../../components/Button";
import Checked from "../../components/Checked";
import * as Theme from "../../constants/Theme";
import { SCREEN_ROUTER, GENDER, REDUCER } from "../../constants/Constant";
import Header from "../../components/DCHeader";
import DatePicker from "react-native-datepicker";
import NavigationUtil from "../../navigation/NavigationUtil";
import {
  getAddress,
  updateUser,
  getUserInfo,
  getHome
} from "../../redux/actions";
import { Dropdown } from "react-native-material-dropdown";
import reactotron from "reactotron-react-native";
import { Block, Loading } from "../../components";
import { showMessages } from "../../utils/Alert";
import FastImage from "../../components/FstImage";
import R from "@app/assets/R";
export class UpdateUserInfoScreen extends Component {
  componentDidMount() {
    const { addressState } = this.props;
    if (!addressState.listProvince.length) this.props.getAddress();
  }
  constructor(props) {
    super(props);
    const { UserInfoState } = this.props;
    this.state = {
      name: UserInfoState.data.customerName,
      phone: UserInfoState.data.phone,
      email: UserInfoState.data.email,
      city: UserInfoState.data.provinceName,
      district: UserInfoState.data.districtName,
      address: UserInfoState.data.address,
      birthday: UserInfoState.data.dobStr,
      gender: UserInfoState.data.sex,
      provinceID: UserInfoState.data.provinceID,
      districtID: UserInfoState.data.districtID
    };
  }
  filterListDistrict = proId => {
    const { addressState } = this.props;
    var result = addressState.listDistrict.filter(district => {
      return district.provinceID == proId;
    });
    this.setState({
      ...this.state,
      district: result[0].districtName,
      districtID: result[0].districtID
    });
  };
  _updateUser() {
    const {
      name,
      phone,
      birthday,
      email,
      provinceID,
      districtID,
      address,
      gender
    } = this.state;
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if (name == "" || name.trim().length == 0) {
      showMessages(I18n.t("notification"), I18n.t("error_name"));
    } else if (phone == "") {
      showMessages(I18n.t("notification"), I18n.t("not_input_phone"));
    } else if (email == "") {
      showMessages(I18n.t("notification"), I18n.t("not_email"));
    } else if (!filter.test(email)) {
      showMessages(I18n.t("notification"), I18n.t("email_wrong"));
    } else {
      this.props.updateUser({
        phone: phone,
        customerName: name.trim(),
        dobStr: birthday,
        sex: gender,
        email: email,
        provinceID: provinceID,
        districtID: districtID,
        address: address.trim()
      });
    }
  }
  _renderBody() {
    const {
      addressState,
      updateUseSate,
      homeState,
      UserInfoState
    } = this.props;
    if (
      updateUseSate.isLoading ||
      homeState.isLoading ||
      UserInfoState.isLoading
    )
      return <Loading />;
    return (
      <Block>
        <TextFieldInput
          top={20}
          label={I18n.t("name")}
          onChangeText={text => {
            this.setState({
              ...this.state,
              name: text
            });
          }}
          submit={() => this.phone.focus()}
          maxLength={50}
          obligatory
          value={this.state.name}
        />
        <TextFieldInput
          editable={false}
          top={20}
          label={I18n.t("phone")}
          keyboardType={"phone-pad"}
          maxLength={12}
          submit={() => this.email.focus()}
          refs={ref => (this.phone = ref)}
          onChangeText={text => {
            this.setState({
              ...this.state,
              phone: text.trim()
            });
          }}
          obligatory
          value={this.state.phone}
        />
        <TextFieldInput
          // editable={false}
          top={20}
          label={I18n.t("email")}
          keyboardType={"email-address"}
          onChangeText={text => {
            this.setState({
              ...this.state,
              email: text.trim()
            });
          }}
          obligatory
          value={this.state.email}
          refs={ref => (this.email = ref)}
          autoCapitalize="none"
        />
        <View style={styles._viewBirth}>
          <Text
            style={[
              Theme.fonts.robotoRegular16,
              { flex: 1, color: Theme.colors.black1 }
            ]}
          >
            {I18n.t("birthday")}
          </Text>
          <View style={{ flex: 2 }}>
            <View style={styles._viewDate}>
              <DatePicker
                date={this.state.birthday}
                mode="date"
                showIcon={true}
                androidMode="spinner"
                placeholderText={Theme.fonts.robotoRegular16}
                format="DD/MM/YYYY"
                minDate="01/01/1950"
                maxDate={new Date()}
                confirmBtnText={"Confirm"}
                cancelBtnText={"Cancel"}
                showIcon={false}
                customStyles={{
                  dateInput: {
                    borderLeftWidth: 0,
                    borderRightWidth: 0,
                    borderTopWidth: 0,
                    borderBottomColor: 0,
                    justifyContent: "center"
                  },
                  dateText: [
                    Theme.fonts.robotoRegular16,
                    {
                      color: Theme.colors.black
                    }
                  ],
                  placeholderText: [
                    Theme.fonts.robotoRegular16,
                    {
                      color: Theme.colors.black
                    }
                  ]
                }}
                onDateChange={date => {
                  this.setState({
                    ...this.state,
                    birthday: date
                  });
                }}
              />
              <FastImage
                style={{ height: 20, width: 20 }}
                source={require("../../assets/images/ic_date.png")}
              />
            </View>
            <View
              style={{
                height: 0.5,
                backgroundColor: Theme.colors.black1
              }}
            />
          </View>
        </View>
        <View style={styles._viewGender}>
          <Text
            style={[
              Theme.fonts.robotoRegular16,
              { color: Theme.colors.black1 }
            ]}
          >
            {I18n.t("gender")}
          </Text>

          <Checked
            left={60}
            text={I18n.t("male")}
            status={this.state.gender}
            onPress={() => {
              this.state.gender == GENDER.MALE
                ? this.setState({
                  ...this.state,
                  gender: GENDER.FEMALE
                })
                : this.setState({
                  ...this.state,
                  gender: GENDER.MALE
                });
            }}
          />
          <Checked
            left={40}
            text={I18n.t("female")}
            status={
              this.state.gender == GENDER.MALE ? GENDER.FEMALE : GENDER.MALE
            }
            onPress={() => {
              this.state.gender == GENDER.MALE
                ? this.setState({
                  ...this.state,
                  gender: GENDER.FEMALE
                })
                : this.setState({
                  ...this.state,
                  gender: GENDER.MALE
                });
            }}
          />
        </View>
        <Dropdown
          containerStyle={{ marginTop: 15, marginHorizontal: 20 }}
          itemCount={6}
          dropdownPosition={3}
          value={this.state.city}
          label={I18n.t("city")}
          data={addressState.listProvince}
          labelExtractor={item => item.provinceName}
          keyExtractor={item => item.provinceID}
          onChangeText={async (value, index, data) => {
            await this.setState(
              {
                ...this.state,
                provinceID: data[index].provinceID
                //city: data[index].provinceName
              },
              () => this.filterListDistrict(data[index].provinceID)
            );
            await this.setState({
              ...this.state,
              city: data[index].provinceName
            });
          }}
        />

        <Dropdown
          containerStyle={{ marginTop: 15, marginHorizontal: 20 }}
          itemCount={6}
          dropdownPosition={3}
          value={this.state.district}
          label={I18n.t("district")}
          data={addressState.listDistrict.filter(district => {
            return district.provinceID == this.state.provinceID;
          })}
          labelExtractor={item => item.districtName}
          keyExtractor={item => item.districtID}
          onChangeText={(value, index, data) => {
            this.setState({
              ...this.state,
              districtID: data[index].districtID,
              district: data[index].districtName
            });
          }}
        />
        <TextFieldInput
          top={20}
          label={I18n.t("address")}
          onChangeText={text => {
            this.setState({
              ...this.state,
              address: text
            });
          }}
          value={this.state.address}
        />

        <Button
          text={R.strings().save}
          top={30}
          onPress={() => {
            this._updateUser();
          }}
        />
      </Block>
    );
  }
  render() {
    return (
      <Block>
        <Header isWhiteBackground title={I18n.t("basic_info")} back />
        <SafeAreaView style={Theme.styles.container}>
          <KeyboardAvoidingView
            enabled
            behavior={Platform.OS === "ios" ? "padding" : null}
            style={{ flexGrow: 1 }}
          >
            <ScrollView
              contentContainerStyle={{
                paddingBottom: 20,
                flexGrow: 1
              }}
              showsVerticalScrollIndicator={false}
            >
              {this._renderBody()}
            </ScrollView>
          </KeyboardAvoidingView>
        </SafeAreaView>
      </Block>
    );
  }
}

const mapStateToProps = state => ({
  addressState: state[REDUCER.ADDRESS],
  UserInfoState: state[REDUCER.USER],
  updateUseSate: state.updateUseReducer,
  homeState: state.homeReducer,
  UserInfoState: state[REDUCER.USER]
});

const mapDispatchToProps = {
  getAddress,
  updateUser,
  getUserInfo,
  getHome
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(UpdateUserInfoScreen);

const styles = StyleSheet.create({
  _viewBirth: {
    marginTop: 20,
    marginHorizontal: 20,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center"
  },
  _viewDate: {
    flexDirection: "row",
    alignItems: "center",
    marginRight: 0,
    justifyContent: "flex-end"
  },
  _viewGender: {
    marginHorizontal: 20,
    marginTop: 20,
    flexDirection: "row",
    alignItems: "center"
  }
});
