import {
  
    UPDATE_USER,
    UPDATE_USER_FAIL,
    UPDATE_USER_SUCCESS,
    CHANGE_PASS,
    CHANGE_PASS_SUCCESS,
    CHANGE_PASS_FAIL,
  } from "../actions/type";
  
  const initialState = {
    data: {},
    isLoading: false,
    error: null,
    refreshing: false
  };
  
  export default function(state = initialState, action) {
    switch (action.type) {
      case CHANGE_PASS:
      case UPDATE_USER: {
        return {
          ...state,
          isLoading: true,
          error: null,
          data: []
        };
      }
      case CHANGE_PASS_SUCCESS:
      case UPDATE_USER_SUCCESS:{
        return {
          ...state,
          isLoading: false,
          error: null,
          data: action.payload
        };
      }
      case CHANGE_PASS_FAIL:
      case UPDATE_USER_FAIL: {
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
  