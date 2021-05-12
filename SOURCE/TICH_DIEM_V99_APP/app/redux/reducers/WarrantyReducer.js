import {
    GET_WARRANTY,
    GET_WARRANTY_FAIL,
    GET_WARRANTY_SUCCESS
} from "../actions/type";

const initialState = {
    data: [],
    isLoading: false,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_WARRANTY:
            return {
                ...state,
                isLoading: true,
                data: [],
                error: null
            };
        case GET_WARRANTY_SUCCESS:
            return {
                ...state,
                isLoading: false,
                data: action.payload,
                error: null
            };
        case GET_WARRANTY_FAIL:
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
