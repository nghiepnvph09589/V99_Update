import {
    GET_STORE,
    GET_STORE_SUCCESS,
    GET_STORE_FAIL
  } from "../actions/type";
  
  const initialState = {
    data: [],
    isLoading: false,
    error: null
  };
  
  export default function(state = initialState, action) {
    switch (action.type) {
      case GET_STORE:
        return {
          ...state,
          isLoading: true,
          error:null,
          data:[]
        };
      case GET_STORE_SUCCESS:
        return {
          ...state,
          isLoading: false,
          data: action.payload,
          error:null
        };
  
      case GET_STORE_FAIL:
        return {
          ...state,
          error: action.payload,
          isLoading: false,
          data:[]
        };
  
      default:
        return state;
    }
  }
  