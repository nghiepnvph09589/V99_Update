import {
  GET_USER,
  GET_PRODUCT,
  GET_HOME,
  GET_GIFTS,
  GET_ADDRESS,
  SOCIAL_LOGIN,
  GET_CART,
  SELECT_CART_ITEM,
  SELECT_ALL_CART_ITEM,
  UNSELECT_CART_ITEM,
  UPDATE_CART_ITEM,
  ADD_TO_CART,
  REMOVE_CART,
  EXCHANGE_GIFT,
  DONATE,
  GET_COUNT_CART,
  CART_ITEM_TOUCH,
  UPDATE_COUNT_CART,
  UPDATE_USER,
  GET_LIST_PRODUCT,
  GET_LIST_ORDER,
  GET_OLDER_DETAIL,
  GET_STORE,
  GET_UTILITY,
  GET_LIST_NEW_RELATIVE,
  ACTIVE_WARRANTY_SUCCESS,
  GET_HISTORY,
  RELOAD,
  CHANGE_PASS,
  GET_NOTIFICATION,
  GET_WARRANTY,
  SET_IS_LOGIN,
  GET_IMAGE, UPDATE_USER_LOCAL, GET_WALLET_ACCUMULATE_POINTS, GET_WALLET_POINTS, GET_HISTORY_DRAW_POINTS, GET_BANK_SELECT, GET_BANK, GET_HISTORY_MOVING_POINTS, UPDATE_NOTIFICATION, NAVIGATE_TAB, REMOVE_CART_SUCCESS
} from "./type";

export const getUserInfo = () => ({
  type: GET_USER,
  payload: {}
});

//Goi dispatch
export const getProduct = () => ({
  type: GET_PRODUCT,
  payload: {}
});
export const getHome = () => ({
  type: GET_HOME,
  payload: {}
});

export const getListGift = type => ({
  type: GET_GIFTS,
  payload: type
});

export const exchangeGift = (type, gift) => ({
  type: EXCHANGE_GIFT,
  payload: {
    type: type,
    gift: gift
  }
});

export const getAddress = () => ({
  type: GET_ADDRESS,
  payload: {}
});
export const loginWithSocial = payload => ({
  type: SOCIAL_LOGIN,
  payload
});

export const getCart = (listItemIDSelected = []) => ({
  type: GET_CART,
  payload: { listItemIDSelected }
});

export const getCountCart = () => ({
  type: GET_COUNT_CART,
  payload: {}
});

export const updateCountCart = payload => ({
  type: UPDATE_COUNT_CART,
  payload: payload
});

export const addToCart = payload => ({
  type: ADD_TO_CART,
  payload: payload
});

export const removeCart = payload => ({
  type: REMOVE_CART,
  payload: payload
});

export const selectCartItem = index => ({
  type: SELECT_CART_ITEM,
  payload: index
});
export const unselectCartItem = index => ({
  type: UNSELECT_CART_ITEM,
  payload: index
});

export const cartItemTouch = (index, isChecked) => ({
  type: CART_ITEM_TOUCH,
  payload: {
    index,
    isChecked
  }
});

export const selectAllCartItem = type => ({
  type: SELECT_ALL_CART_ITEM,
  payload: type
});

export const updateCartItem = (item, index, isChecked) => ({
  type: UPDATE_CART_ITEM,
  payload: { item, index, isChecked }
});

export const removeCartSuccess = (payload) => ({
  type: REMOVE_CART_SUCCESS,
  payload
});

export const donate = payload => ({
  type: DONATE,
  payload: payload
});

export const getListProduct = (page, parentID, childID) => ({
  type: GET_LIST_PRODUCT,
  payload: { page, parentID, childID }
});

export const updateUser = payload => ({
  type: UPDATE_USER,
  payload: payload
});

export const updateUserLocal = payload => ({
  type: UPDATE_USER_LOCAL,
  payload
});

export const getListOrder = (status, page) => ({
  type: GET_LIST_ORDER,
  payload: {
    status: status,
    page: page
  }
});
export const getOrderDetail = payload => ({
  type: GET_OLDER_DETAIL,
  payload: payload
});
export const getStore = payload => ({
  type: GET_STORE,
  payload: payload
});

export const getUtility = type => ({
  type: GET_UTILITY,
  payload: {
    type: type
  }
});

export const activeWarrantySuccess = payload => ({
  type: ACTIVE_WARRANTY_SUCCESS,
  payload: payload
});

export const getHistory = () => ({
  type: GET_HISTORY,
  payload: {}
});


export const changePass = (payload) => ({
  type: CHANGE_PASS,
  payload: payload
});

export const getNotification = () => ({
  type: GET_NOTIFICATION,
  payload: {}
});

export const getWarranty = () => ({
  type: GET_WARRANTY,
  payload: {}
});
export const setIsLogin = (payload) => ({
  type: SET_IS_LOGIN,
  payload: payload
});
export const getImages = () => ({
  type: GET_IMAGE,
  payload: {}
});

export const getWalletAccumulate = payload => ({
  type: GET_WALLET_ACCUMULATE_POINTS,
  payload
});

export const getWalletPoints = payload => ({
  type: GET_WALLET_POINTS,
  payload
});

export const getHistoryDrawPoints = payload => ({
  type: GET_HISTORY_DRAW_POINTS,
  payload
});

export const getHistoryPoints = (type, payload) => ({
  ...type,
  payload
});

export const getBankSelect = () => ({
  type: GET_BANK_SELECT,
  payload: {}
});

export const getBank = () => ({
  type: GET_BANK,
  payload: {}
});

export const updateNotiItem = (index) => ({
  type: UPDATE_NOTIFICATION,
  payload: index
});

export const navigateTab = payload => ({
  type: NAVIGATE_TAB,
  payload
});