import {
  GET_LIST_ORDER,
  GET_LIST_ORDER_SUCCESS,
  GET_LIST_ORDER_FAIL
} from "../actions/type";
import { ORDER } from "../../constants/Constant";

const initialState = {
  pending: {
    data: {},
    isLoading: false,
    error: null
  },
  confirm: {
    data: {},
    isLoading: false,
    error: null
  },
  history: {
    data: {},
    isLoading: false,
    error: null
  },
  refreshing: false
};

export default function (state = initialState, action) {
  switch (action.type) {
    case GET_LIST_ORDER: {
      switch (action.payload.status) {
        case ORDER.PENDING:
          return {
            ...state,
            pending: {
              ...state.pending,
              data: {},
              isLoading: true,
              error: null
            }
          };
        case ORDER.CONFIRM:
          return {
            ...state,
            confirm: {
              ...state.confirm,
              data: {},
              isLoading: true,
              error: null
            }
          };

        case ORDER.HISTORY:
          return {
            ...state,
            history: {
              ...state.confirm,
              data: {},
              isLoading: true,
              error: null
            }
          };

      }
    }
    case GET_LIST_ORDER_SUCCESS: {
      switch (action.payload.status) {
        case ORDER.PENDING:
          return {
            ...state,
            pending: {
              ...state.pending,
              data: action.payload.data.listOrder,
              isLoading: false,
              error: null
            }
          };
        case ORDER.CONFIRM:
          return {
            ...state,
            confirm: {
              ...state.confirm,
              data: action.payload.data.listOrder,
              isLoading: false,
              error: null
            }
          };
        case ORDER.HISTORY:
          return {
            ...state,
            history: {
              ...state.confirm,
              data: action.payload.data.listOrder,
              isLoading: false,
              error: null
            }
          };

      }
    }


    case GET_LIST_ORDER_FAIL:
      switch (action.payload.status) {
        case ORDER.PENDING:
          return {
            ...state,
            pending: {
              ...state.pending,
              data: {},
              isLoading: false,
              error: action.payload.error
            }
          };
        case ORDER.CONFIRM:
          return {
            ...state,
            confirm: {
              ...state.confirm,
              data: {},
              isLoading: false,
              error: action.payload.error
            }
          };
        case ORDER.HISTORY:
          return {
            ...state,
            history: {
              ...state.confirm,
              data: {},
              isLoading: false,
              error: action.payload.error
            }
          };
      }
    default:
      return state;
  }
}
