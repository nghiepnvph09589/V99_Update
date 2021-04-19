import {
  GET_OLDER_DETAIL,
  GET_OLDER_DETAIL_FAIL,
  GET_OLDER_DETAIL_SUCCESS
} from "../actions/type";

const initialState = {
  data: [],
  isLoading: false,
  error: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_OLDER_DETAIL:
      return {
        ...state,
        isLoading: true,
        error: null,
        data: []
      };

    case GET_OLDER_DETAIL_SUCCESS:
      return {
        ...state,
        isLoading: false,
        // data : Mockup.Product
        data: action.payload,
        error:null
      };

    case GET_OLDER_DETAIL_FAIL:
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
