import {
  GET_UTILITY,
  GET_UTILITY_SUCCESS,
  GET_UTILITY_FAIL
} from "../actions/type";
import { UTILITY } from "../../constants/Constant";
const initialState = {
  news: {
    data: {},
    isLoading: false,
    error: null
  },
  sale: {
    data: {},
    isLoading: false,
    error: null
  },
  guarantee: {
    data: {},
    isLoading: false,
    error: null
  },
  advertisement: {
    data: {},
    isLoading: false,
    error: null
  },
  refreshing: false
};

export default function (state = initialState, action) {
  switch (action.type) {
    case GET_UTILITY: {
      switch (action.payload.type) {
        case UTILITY.NEWS:
          return {
            ...state,
            news: {
              ...state.news,
              isLoading: true
            }
          };
        case UTILITY.EVENT:
          return {
            ...state,
            sale: {
              ...state.sale,
              data: {},
              isLoading: true,
              error: null
            }
          };
        case UTILITY.GUARANTY:
          return {
            ...state,
            guarantee: {
              ...state.guarantee,
              data: {},
              isLoading: true,
              error: null
            }
          };
        case UTILITY.ADVERTISEMENT:
          return {
            ...state,
            advertisement: {
              ...state.advertisement,
              data: {},
              isLoading: true,
              error: null
            }
          };
      }
    }
    case GET_UTILITY_SUCCESS: {
      switch (action.payload.type) {
        case UTILITY.NEWS:
          return {
            ...state,
            news: {
              ...state.news,
              data: action.payload.data,
              isLoading: false,
              error: null
            }
          };
        case UTILITY.EVENT:
          return {
            ...state,
            sale: {
              ...state.sale,
              data: action.payload.data,
              isLoading: false,
              error: null
            }
          };
        case UTILITY.GUARANTY:
          return {
            ...state,
            guarantee: {
              ...state.guarantee,
              data: action.payload.data,
              isLoading: false,
              error: null
            }
          };
        case UTILITY.ADVERTISEMENT:
          return {
            ...state,
            advertisement: {
              ...state.advertisement,
              data: action.payload.data,
              isLoading: false,
              error: null
            }
          };
      }
    }

    case GET_UTILITY_FAIL:
      switch (action.payload.type) {
        case UTILITY.NEWS:
          return {
            ...state,
            news: {
              ...state.news,
              data: {},
              isLoading: false,
              error: action.payload.error
            }
          };
        case UTILITY.EVENT:
          return {
            ...state,
            sale: {
              ...state.sale,
              data: {},
              isLoading: false,
              error: action.payload.error
            }
          };
        case UTILITY.GUARANTY:
          return {
            ...state,
            guarantee: {
              ...state.guarantee,
              data: {},
              isLoading: false,
              error: action.payload.error
            }
          };
        case UTILITY.ADVERTISEMENT:
          return {
            ...state,
            advertisement: {
              ...state.advertisement,
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
