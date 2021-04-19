import { GET_IMAGE, GET_IMAGE_SUCCESS, GET_IMAGE_FAIL } from "../actions/type";

const initialState = {
    data: [],
    isLoading: false,
    error: null,
    refreshing: false,
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_IMAGE: {
            return {
                ...state,
                isLoading: true,
                error: null,
            }
        }
        case GET_IMAGE_SUCCESS: {
            return {
                ...state,
                data: action.payload,
                isLoading: false,
                error: null,
            }
        }
        case GET_IMAGE_FAIL: {
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
