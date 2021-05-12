import {
    GET_COUNT_CART, GET_COUNT_CART_SUCCESS,
    GET_COUNT_CART_FAIL, REMOVE_CART_SUCCESS,
    ADD_TO_CART_SUCCESS,
    UPDATE_COUNT_CART,
} from "../actions/type";

const initialState = {
    isLoading: false,
    error: null,
    countCart: 0,
    isShow: false,
}

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_COUNT_CART:
            return {
                ...state,
                isLoading: true, error: null,
                isShow: false,
            }
        case GET_COUNT_CART_SUCCESS:
            return {
                ...state,
                isLoading: false, error: null,
                countCart: action.payload,
                isShow: true,
            }
        case GET_COUNT_CART_FAIL:
            return {
                ...state, isLoading: false,
                error: action.payload,
                isShow: false
            }

        case REMOVE_CART_SUCCESS:
            return {
                ...state,
                countCart: action.payload && action.payload.length || 0
            }

        case ADD_TO_CART_SUCCESS:
            return {
                ...state,
                countCart: action.payload,
            }
        case UPDATE_COUNT_CART:
            return {
                ...state,
                countCart: state.countCart - action.payload
            }
        default:
            return state
    }
}