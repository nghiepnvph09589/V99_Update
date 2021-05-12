import { GET_WALLET_ACCUMULATE_POINTS, GET_WALLET_ACCUMULATE_POINTS_SUCCESS, GET_WALLET_ACCUMULATE_POINTS_FAIL } from "../actions/type";

const initialState = {
    data: [],
    isLoading: true,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_WALLET_ACCUMULATE_POINTS:
            return {
                ...state,
                isLoading: true,
                error: null
            }
        case GET_WALLET_ACCUMULATE_POINTS_SUCCESS:
            return {
                ...state,
                isLoading: false,
                data: action.payload.listHistoriesPointMember
            }
        case GET_WALLET_ACCUMULATE_POINTS_FAIL:
            return {
                ...state,
                error: action.payload,
                isLoading: false
            }
    }
    return state;
}
