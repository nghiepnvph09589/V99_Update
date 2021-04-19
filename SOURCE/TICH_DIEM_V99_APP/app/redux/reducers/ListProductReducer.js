import { GET_LIST_PRODUCT, GET_LIST_PRODUCT_SUCCESS, GET_LIST_PRODUCT_FAIL } from "../actions/type";

const initialState = {
    data: [],
    isLoading: true,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_LIST_PRODUCT:
            return {
                ...state,
                isLoading: true
            }
        case GET_LIST_PRODUCT_SUCCESS:
            return {
                ...state,
                isLoading: false,
                data: action.payload.listProduct
            }
        case GET_LIST_PRODUCT_FAIL:
            return {
                ...state,
                error: action.payload,
                isLoading: false
            }
    }
    return state;
}
