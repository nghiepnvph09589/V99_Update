import {
  GET_USER,
  GET_USER_SUCCESS,
  GET_USER_FAIL,
  SOCIAL_LOGIN,
  SOCIAL_LOGIN_SUCCESS,
  SOCIAL_LOGIN_FAIL,
  EXCHANGE_GIFT_SUCCESS,
  DONATE_SUCCESS,
  ACTIVE_WARRANTY_SUCCESS,
  UPDATE_USER_SUCCESS, UPDATE_USER_LOCAL, GET_HOME_SUCCESS

} from "../actions/type";

const initialState = {
  data: {},
  isLoading: false,
  error: null,
  refreshing: false
};

export default function (state = initialState, action) {
  switch (action.type) {
    case SOCIAL_LOGIN:
    case GET_USER: {
      return { ...state, isLoading: true, error: null };
    }
    case SOCIAL_LOGIN_SUCCESS:
    case GET_USER_SUCCESS: {
      return {
        ...state,
        isLoading: false,
        error: null,
        data: action.payload
      };
    }
    case SOCIAL_LOGIN_FAIL:
    case GET_USER_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false
      };
    }
    case UPDATE_USER_SUCCESS: {
      return {
        ...state,
        data: {
          ...state.data,
          customerName: action.payload.customerName,
          phone: action.payload.phone,
          dobStr: action.payload.dobStr,
          sex: action.payload.sex,
          email: action.payload.email,
          provinceName: action.payload.provinceName,
          districtName: action.payload.districtName,
          address: action.payload.address,
          provinceID: action.payload.provinceID,
          districtID: action.payload.districtID,
        }
      }
    }

    case UPDATE_USER_LOCAL:
      return {
        ...state,
        data: {
          ...state.data,
          ...action.payload
        }
      };

    case DONATE_SUCCESS:
    case EXCHANGE_GIFT_SUCCESS:
      return {
        ...state,
        data: {
          ...state.data,
          point: action.payload.data.currentPoint,
          remain: action.payload.data.remain,
          pointRanking: action.payload.data.rankingPoint
        }
      };
    case ACTIVE_WARRANTY_SUCCESS:
      return {
        ...state,
        data: {
          ...state.data,
          point: action.payload.currentPoint,
          pointRanking: action.payload.rankingPoint
        }
      }

    case GET_HOME_SUCCESS:
      return {
        data: {
          ...state.data,
          ...action.payload.customerInfo
        }
      }
    default:
      return state;
  }
}
