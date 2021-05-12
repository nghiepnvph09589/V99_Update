import { put, takeEvery, call } from "redux-saga/effects";
//import { AsyncStorage } from "react-native";
import AsyncStorage from "@react-native-community/async-storage";
import {
  GET_USER,
  GET_USER_SUCCESS,
  GET_USER_FAIL,
  GET_HOME_SUCCESS,
  GET_HOME_FAIL,
  GET_HOME,
  GET_GIFTS,
  GET_GIFTS_SUCCESS,
  GET_GIFTS_FAIL,
  GET_PRODUCT,
  GET_PRODUCT_FAIL,
  GET_PRODUCT_SUCCESS,
  GET_ADDRESS,
  GET_ADDRESS_SUCCESS,
  GET_ADDRESS_FAIL,
  SOCIAL_LOGIN_SUCCESS,
  SOCIAL_LOGIN_FAIL,
  SOCIAL_LOGIN,
  GET_CART,
  GET_CART_SUCCESS,
  GET_CART_FAIL,
  ADD_TO_CART,
  ADD_TO_CART_SUCCESS,
  ADD_TO_CART_FAIL,
  REMOVE_CART,
  REMOVE_CART_SUCCESS,
  REMOVE_CART_FAIL,
  EXCHANGE_GIFT,
  EXCHANGE_GIFT_SUCCESS,
  EXCHANGE_GIFT_FAIL,
  DONATE,
  DONATE_SUCCESS,
  DONATE_FAIL,
  GET_COUNT_CART,
  GET_COUNT_CART_SUCCESS,
  GET_COUNT_CART_FAIL,
  // REGISTER_SUCCESS,
  // REGISTER_FAIL,
  // REGISTER,
  UPDATE_USER_SUCCESS,
  UPDATE_USER_FAIL,
  UPDATE_USER,
  GET_LIST_PRODUCT,
  GET_LIST_PRODUCT_SUCCESS,
  GET_LIST_PRODUCT_FAIL,
  GET_LIST_ORDER,
  GET_LIST_ORDER_SUCCESS,
  GET_LIST_ORDER_FAIL,
  GET_OLDER_DETAIL_SUCCESS,
  GET_OLDER_DETAIL_FAIL,
  GET_OLDER_DETAIL,
  GET_STORE_SUCCESS,
  GET_STORE_FAIL,
  GET_STORE,
  GET_UTILITY_SUCCESS,
  GET_UTILITY_FAIL,
  GET_UTILITY,
  GET_HISTORY,
  GET_HISTORY_SUCCESS,
  GET_HISTORY_FAIL,
  CHANGE_PASS_SUCCESS,
  CHANGE_PASS,
  CHANGE_PASS_FAIL,
  GET_NOTIFICATION,
  GET_NOTIFICATION_SUCCESS,
  GET_NOTIFICATION_FAIL,
  GET_WARRANTY, GET_WARRANTY_SUCCESS, GET_WARRANTY_FAIL, GET_IMAGE_SUCCESS, GET_IMAGE_FAIL, GET_IMAGE,
  GET_WALLET_ACCUMULATE_POINTS, GET_WALLET_ACCUMULATE_POINTS_SUCCESS, GET_WALLET_ACCUMULATE_POINTS_FAIL,
  GET_WALLET_POINTS, GET_WALLET_POINTS_SUCCESS, GET_WALLET_POINTS_FAIL,
  GET_HISTORY_DRAW_POINTS, GET_HISTORY_DRAW_POINTS_SUCCESS, GET_HISTORY_DRAW_POINTS_FAIL, GET_BANK_SELECT, GET_BANK_SELECT_SUCCESS, GET_BANK_SELECT_FAIL, GET_BANK_SUCCESS, GET_BANK_FAIL, GET_BANK, GET_HISTORY_MOVING_POINTS, GET_HISTORY_MOVING_POINTS_SUCCESS, GET_HISTORY_MOVING_POINTS_FAIL

} from "../actions/type";
import * as API from "../../constants/Api";
import { SCREEN_ROUTER, GIFT_TYPE } from "../../constants/Constant";
import NavigationUtil from "../../navigation/NavigationUtil";
import reactotron from "reactotron-react-native";
import { showConfirm, showMessages } from "../../utils/Alert";
import I18n from "../../i18n/i18n";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";

export function* getUserInfo() {
  try {
    const response = yield call(API.getUserInfo);
    yield call(AsyncStorage.setItem, "userInfo", JSON.stringify(response.data));
    yield put({ type: GET_USER_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_USER_FAIL, payload: err });
  }
}

export function* getProduct() {
  try {
    const response = yield call(API.requestProduct);
    yield put({
      type: GET_PRODUCT_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_PRODUCT_FAIL, payload: err });
  }
}

export function* getHomeData(action) {

  try {
    const response = yield call(API.requestHomeData);
    yield put({ type: GET_HOME_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_HOME_FAIL, payload: err });
  }
}

export function* getListGifts(action) {
  try {
    const response = yield call(API.requestGetListGifts, action.payload);
    yield put({
      type: GET_GIFTS_SUCCESS,
      payload: { type: action.payload, data: response.data }
    });
  } catch (err) {
    yield put({
      type: GET_GIFTS_FAIL,
      payload: { err: err, type: action.payload }
    });
  }
}

export function* getAddressData() {
  try {
    const response = yield call(API.requestAddressData);
    yield put({ type: GET_ADDRESS_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_ADDRESS_FAIL, payload: err });
  }
}
export function* loginWithSocial(action) {

  try {
    const response = yield call(API.requestLogin, action.payload);

    yield call(AsyncStorage.setItem, "token", response.data.token);
    yield call(AsyncStorage.setItem, "userInfo", JSON.stringify(response.data));
    yield put({ type: SOCIAL_LOGIN_SUCCESS, payload: response.data });
    NavigationUtil.navigate(SCREEN_ROUTER.AUTH_LOADING);
  } catch (err) {
    yield put({ type: SOCIAL_LOGIN_FAIL, payload: err });
    if (err.message == "Network Error") {
      Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
    }
  }
}

export function* getCart(action) {
  try {
    const response = yield call(API.getCarts);
    yield put({
      type: GET_CART_SUCCESS, payload: {
        data: response.data,
        listItemIDSelected: action.payload.listItemIDSelected
      }
    });
  } catch (err) {
    yield put({ type: GET_CART_FAIL, payload: err });
  }
}

export function* getCountCart() {
  try {
    const response = yield call(API.requestGetCountCart);
    yield put({ type: GET_COUNT_CART_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_COUNT_CART_FAIL, payload: err });
  }
}

export function* addToCart(action) {
  try {
    const response = yield call(API.requestAddToCart, action.payload);
    yield put({ type: ADD_TO_CART_SUCCESS, payload: response.data });
    Toast.show("Đã thêm vào giỏ hàng");
  } catch (err) {
    Toast.show("Vui lòng thử lại", BACKGROUND_TOAST.FAIL);
    yield put({ type: ADD_TO_CART_FAIL, payload: err });
  }
}
export function* removeCart(action) {
  try {
    const response = yield call(API.requestRemoveCart, action.payload);
    yield put({ type: REMOVE_CART_SUCCESS, payload: response.data });
    Toast.show(
      "Đã xóa " + action.payload.length.toString() + " sản phẩm trong giỏ hàng"
    );
  } catch (err) {
    yield put({ type: REMOVE_CART_FAIL, payload: err });
  }
}

export function* exchangeGift(action) {
  try {
    const response = yield call(API.exchangeGift, action.payload);
    yield put({
      type: EXCHANGE_GIFT_SUCCESS,
      payload: {
        type: action.payload.type,
        data:
          action.payload.type == GIFT_TYPE.CARD
            ? response.data.changePoint
            : response.data
      }
    });
    if (action.payload.type == GIFT_TYPE.CARD) {
      NavigationUtil.navigate(SCREEN_ROUTER.TOP_UP, {
        cardDetail: response.data.cardDetail,
        gift: action.payload.gift
      });
    }
    Toast.show(response.message);
  } catch (err) {
    yield put({
      type: EXCHANGE_GIFT_FAIL,
      payload: { err: err, type: action.payload.type }
    });
  }
}

export function* donate(action) {
  try {
    const response = yield call(API.requestDonate, action.payload);
    yield put({
      type: DONATE_SUCCESS,
      payload: { data: response.data } //không viết (payload: response.data) để lấy currentPoint cùng kiểu trong UserReducer
    });
    Toast.show(response.message);
  } catch (err) {
    yield put({ type: DONATE_FAIL, payload: err });
  }
}

export function* updateUser(action) {
  try {
    const response = yield call(API.updateUser, action.payload);
    yield put({ type: UPDATE_USER_SUCCESS, payload: response.data });
    yield call(AsyncStorage.mergeItem, "userInfo", JSON.stringify(response.data));
    Toast.show(I18n.t("update_user_success"), BACKGROUND_TOAST.SUCCESS);
    NavigationUtil.goBack();
  } catch (err) {
    yield put({ type: UPDATE_USER_FAIL, payload: err });
    if (err.message == "Network Error") {
      Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
    }
  }
}

export function* getListProduct(action) {
  try {
    const response = yield call(API.requestListProduct, action.payload);
    yield put({ type: GET_LIST_PRODUCT_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_LIST_PRODUCT_FAIL, payload: err });
  }
}
export function* getListOrder(action) {
  try {
    const response = yield call(API.getListOrder, action.payload);
    yield put({
      type: GET_LIST_ORDER_SUCCESS,
      payload: { status: action.payload.status, data: response.data }
    });
  } catch (err) {
    yield put({
      type: GET_LIST_ORDER_FAIL,
      payload: { error: err, status: action.payload.status }
    });
  }
}
export function* getOrderDetail(action) {
  try {
    const response = yield call(API.getOrderDetail, action.payload);
    yield put({ type: GET_OLDER_DETAIL_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_OLDER_DETAIL_FAIL, payload: err });
  }
}
export function* getStore(action) {
  try {
    const response = yield call(API.getStore, action.payload);
    yield put({ type: GET_STORE_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_STORE_FAIL, payload: err });
  }
}
export function* getListUtility(action) {
  try {
    const response = yield call(API.getListUtility, action.payload);
    yield put({
      type: GET_UTILITY_SUCCESS,
      payload: { type: action.payload.type, data: response.data }
    });
  } catch (err) {
    yield put({
      type: GET_UTILITY_FAIL,
      payload: { error: err, type: action.payload.type }
    });
  }
}

export function* getHistory() {
  try {
    const response = yield call(API.getHistory);
    yield put({ type: GET_HISTORY_SUCCESS, payload: response.data });
  } catch (err) {
    yield put({ type: GET_HISTORY_FAIL, payload: err });
  }
}

export function* changePass(action) {
  try {
    const response = yield call(API.changePass, action.payload);
    yield put({ type: CHANGE_PASS_SUCCESS, payload: response.data });
  } catch (err) {
    if (err.message == "Network Error") {
      Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
    }
    yield put({ type: CHANGE_PASS_FAIL, payload: err });
  }
}

export function* getNotification() {
  try {
    const response = yield call(API.requestGetNotify);
    yield put({
      type: GET_NOTIFICATION_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_NOTIFICATION_FAIL, payload: err });
  }
}

export function* getWarranty() {
  try {
    const response = yield call(API.requestGetListWarranty);
    yield put({
      type: GET_WARRANTY_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_WARRANTY_FAIL, payload: err });
  }
}

export function* getImage(action) {
  try {
    const response = yield call(API.getImage);
    yield put({
      type: GET_IMAGE_SUCCESS,
      payload: response.data //không viết (payload: response.data) để lấy currentPoint cùng kiểu trong UserReducer
    });
    // reactotron.log(response.data)
  } catch (err) {
    yield put({ type: GET_IMAGE_FAIL, payload: err });
  }
}

export function* getWalletAccumulate(action) {
  try {
    const response = yield call(API.requestListHistoryPointMember, action.payload);
    yield put({
      type: GET_WALLET_ACCUMULATE_POINTS_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_WALLET_ACCUMULATE_POINTS_FAIL, payload: err });
  }
}

export function* getWalletPoints(action) {
  try {
    const response = yield call(API.requestListHistoryPointMember, action.payload);
    yield put({
      type: GET_WALLET_POINTS_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_WALLET_POINTS_FAIL, payload: err });
  }
}

export function* getHistoryDrawPoints(action) {
  try {
    const response = yield call(API.requestListHistoryPointMember, action.payload);
    yield put({
      type: GET_HISTORY_DRAW_POINTS_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_HISTORY_DRAW_POINTS_FAIL, payload: err });
  }
}

export function* getHistoryMovingPoints(action) {
  try {
    const response = yield call(API.requestListHistoryPointMember, action.payload);
    yield put({
      type: GET_HISTORY_MOVING_POINTS_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_HISTORY_MOVING_POINTS_FAIL, payload: err });
  }
}

export function* getBankSelect() {
  try {
    const response = yield call(API.requestGetListBank);
    yield put({
      type: GET_BANK_SELECT_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_BANK_SELECT_FAIL, payload: err });
  }
}

export function* getBank() {
  try {
    const response = yield call(API.requestGetListBankOfCus);
    yield put({
      type: GET_BANK_SUCCESS,
      payload: response.data
    });
  } catch (err) {
    yield put({ type: GET_BANK_FAIL, payload: err });
  }
}

export const watchGetUser = takeEvery(GET_USER, getUserInfo);
export const watchFetchHome = takeEvery(GET_HOME, getHomeData);
export const watchFetchGetGift = takeEvery(GET_GIFTS, getListGifts);
export const watchProduct = takeEvery(GET_PRODUCT, getProduct);
export const watchFetchAddress = takeEvery(GET_ADDRESS, getAddressData);
export const watchSocialLogin = takeEvery(SOCIAL_LOGIN, loginWithSocial);
export const watchGetCart = takeEvery(GET_CART, getCart);
export const watchAddToCart = takeEvery(ADD_TO_CART, addToCart);
export const watchRemoveCart = takeEvery(REMOVE_CART, removeCart);
export const watchExchangeGift = takeEvery(EXCHANGE_GIFT, exchangeGift);
export const watchFetchDonate = takeEvery(DONATE, donate);
export const watchFetchGetCountCart = takeEvery(GET_COUNT_CART, getCountCart);
export const watchListProduct = takeEvery(GET_LIST_PRODUCT, getListProduct);
//export const watchFetchRegister = takeEvery(REGISTER, register);
export const watchFetchUpdateUser = takeEvery(UPDATE_USER, updateUser);
export const watchFetchGetListOrder = takeEvery(GET_LIST_ORDER, getListOrder);
export const watchFetchGetOrderDetail = takeEvery(
  GET_OLDER_DETAIL,
  getOrderDetail
);
export const watchFetchGetStore = takeEvery(GET_STORE, getStore);
export const watchFetchGetListUtility = takeEvery(GET_UTILITY, getListUtility);
export const watchFetchGetHistory = takeEvery(GET_HISTORY, getHistory);
export const watchFetchChangePass = takeEvery(CHANGE_PASS, changePass);
export const watchFetchGetNotification = takeEvery(GET_NOTIFICATION, getNotification);
export const watchFetchGetWarranty = takeEvery(GET_WARRANTY, getWarranty);
export const watchFetchGetImage = takeEvery(GET_IMAGE, getImage);
export const watchFetchgetWalletAccumulate = takeEvery(GET_WALLET_ACCUMULATE_POINTS, getWalletAccumulate);
export const watchFetchgetWalletPoints = takeEvery(GET_WALLET_POINTS, getWalletPoints);
export const watchFetchGetHistoryDrawPoints = takeEvery(GET_HISTORY_DRAW_POINTS, getHistoryDrawPoints);
export const watchFetchGetBankSelect = takeEvery(GET_BANK_SELECT, getBankSelect);
export const watchFetchGetBank = takeEvery(GET_BANK, getBank);
export const watchFetchGetHistoryMovingPoints = takeEvery(GET_HISTORY_MOVING_POINTS, getHistoryMovingPoints);