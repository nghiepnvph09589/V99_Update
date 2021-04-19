import {
  GET_PRODUCT,
  GET_PRODUCT_SUCCESS,
  GET_PRODUCT_FAIL,
} from "../actions/type";

const initialState = {
  data: [],
  isLoading: true,
  error: null
};

export default function (state = initialState, action) {
  switch (action.type) {
    case GET_PRODUCT:
      return {
        ...state,
        isLoading: true,
        data: [],
        error: null
      };
    case GET_PRODUCT_SUCCESS:
      return {
        ...state,
        isLoading: false,
        data: action.payload,
      };
    case GET_PRODUCT_FAIL:
      return {
        ...state,
        error: action.payload,
        isLoading: false,
        data: []
      };

    default:
      return state;
  }
}
