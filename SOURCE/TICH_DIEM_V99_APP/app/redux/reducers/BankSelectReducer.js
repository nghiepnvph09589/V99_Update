import { GET_BANK_SELECT, GET_BANK_SELECT_SUCCESS, GET_BANK_SELECT_FAIL, } from "../actions/type";

const initialState = {
    data: [],
    isLoading: false,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_BANK_SELECT: {
            return {
                ...state,
                isLoading: true,
                error: null,
            }
        }
        case GET_BANK_SELECT_SUCCESS: {
            return {
                ...state,
                data: action.payload,
                isLoading: false,
                error: null,
            }
        }
        case GET_BANK_SELECT_FAIL: {
            return {
                ...state,
                error: action.payload,
                isLoading: false,
            }
        }
        default:
            return state
    }
}
