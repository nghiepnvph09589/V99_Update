import {
  GET_HOME,
  GET_HOME_FAIL,
  GET_HOME_SUCCESS,
  EXCHANGE_GIFT_SUCCESS,
  DONATE_SUCCESS,
  ACTIVE_WARRANTY_SUCCESS,
  UPDATE_USER_SUCCESS,
  SET_IS_LOGIN
} from "../actions/type";
const initialState = {
  userInfo: {},
  news: [],
  product: [],
  listBanner: [],
  listEvent: [],
  isLoading: false,
  error: null,
  refreshing: false,
  isLogin: false
};
export default function (state = initialState, action) {
  switch (action.type) {
    case GET_HOME:
      return { ...state, isLoading: true, error: null };

    case GET_HOME_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null,
        userInfo: action.payload.customerInfo,
        news: action.payload.listNews,
        product: action.payload.listProductHome,
        listBanner: action.payload.listBanner,
        // listEvent: action.payload.listEnvent,
        // listProductHome: action.payload.product
      };

    case DONATE_SUCCESS:
    case EXCHANGE_GIFT_SUCCESS:
      return {
        ...state,
        userInfo: {
          ...state.userInfo,
          point: action.payload.data.currentPoint
        }
      };

    case UPDATE_USER_SUCCESS:
      return {
        ...state,
        userInfo: {
          ...state.userInfo,
          customerName: action.payload.customerName
        }
      }

    case ACTIVE_WARRANTY_SUCCESS:
      return {
        ...state,
        userInfo: {
          ...state.userInfo,
          point: action.payload.currentPoint
        }
      }
    case GET_HOME_FAIL:
      return {
        ...state,
        isLoading: false,
        error: action.payload
      };
    case SET_IS_LOGIN:
      return {
        ...state,
        isLogin: action.payload
      };

    default:
      return state;
  }
}
