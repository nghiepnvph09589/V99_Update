import { ADD_TO_CART, ADD_TO_CART_SUCCESS, ADD_TO_CART_FAIL } from "../actions/type";

const initialState = {
    isLoading: false,
    error: null,
}

export default function (state = initialState, action) {
    switch (action.type) {
        case ADD_TO_CART:
            return { ...state, isLoading: true, error: null }
        case ADD_TO_CART_SUCCESS:
            return { ...state, isLoading: false, error: null }
        case ADD_TO_CART_FAIL:
            return { ...state, isLoading: false, error: action.payload, }
        default:
            return state
    }
}