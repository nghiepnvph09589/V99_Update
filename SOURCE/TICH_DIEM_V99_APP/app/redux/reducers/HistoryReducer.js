import {
  GET_HISTORY,
  GET_HISTORY_SUCCESS,
  GET_HISTORY_FAIL,
  UPDATE_USER,
  UPDATE_USER_FAIL,
  UPDATE_USER_SUCCESS
} from "../actions/type";

const initialState = {
  data: {},
  isLoading: false,
  error: null,
  refreshing: false
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_HISTORY: {
      return {
        ...state,
        isLoading: true,
        error: null,
        data: []
      };
    }
    case GET_HISTORY_SUCCESS: {
      return {
        ...state,
        isLoading: false,
        error: null,
        data: action.payload
      };
    }
    case GET_HISTORY_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false,
        data: []
      };
    }
    default:
      return state;
  }
}
