import { GET_HISTORY_MOVING_POINTS, GET_HISTORY_MOVING_POINTS_SUCCESS, GET_HISTORY_MOVING_POINTS_FAIL } from "../actions/type";

const initialState = {
    data: [],
    isLoading: true,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_HISTORY_MOVING_POINTS:
            return {
                ...state,
                isLoading: true,
                error: null
            }
        case GET_HISTORY_MOVING_POINTS_SUCCESS:
            return {
                ...state,
                isLoading: false,
                data: action.payload.listHistoriesPointMember
            }
        case GET_HISTORY_MOVING_POINTS_FAIL:
            return {
                ...state,
                error: action.payload,
                isLoading: false
            }
    }
    return state;
}
