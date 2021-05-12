import { GET_BANK, GET_BANK_SUCCESS, GET_BANK_FAIL, } from "../actions/type";

const initialState = {
    data: [],
    isLoading: false,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_BANK: {
            return {
                ...state,
                isLoading: true,
                error: null,
            }
        }
        case GET_BANK_SUCCESS: {
            return {
                ...state,
                data: action.payload,
                isLoading: false,
                error: null,
            }
        }
        case GET_BANK_FAIL: {
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
