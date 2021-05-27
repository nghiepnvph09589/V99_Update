import {
  GET_NOTIFICATION,
  GET_NOTIFICATION_SUCCESS,
  GET_NOTIFICATION_FAIL,
  UPDATE_NOTIFICATION,
  CHECK_NOTI
} from "../actions/type";

const initialState = {
  data: [],
  isLoading: false,
  error: null,
  checkNoti: false
};

export default function(state = initialState, action) {
  switch (action.type) {
    case CHECK_NOTI:
      return {
        ...state,
        checkNoti: action.payload
      };

    case GET_NOTIFICATION:
      return {
        ...state,
        isLoading: true,
        data: [],
        error: null
      };
    case GET_NOTIFICATION_SUCCESS:
      return {
        ...state,
        isLoading: false,
        data: action.payload,
        error: null
      };
    case GET_NOTIFICATION_FAIL:
      return {
        ...state,
        error: action.payload,
        isLoading: false,
        data: []
      };

    case UPDATE_NOTIFICATION:
      var itemSelected = {
        ...state.data[action.payload],
        viewed: 1
      };

      return {
        ...state,
        data: changeItem(state.data, itemSelected, action.payload)
      };

    default:
      return state;
  }
}

changeItem = (array, item, index) => {
  (tempData = array), (tempData[index] = item);
  return tempData;
};
