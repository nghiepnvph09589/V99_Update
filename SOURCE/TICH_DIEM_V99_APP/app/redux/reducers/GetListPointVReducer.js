import {
    GET_LIST_POINT_V,
    GET_LIST_POINT_V_FAIL,
    GET_LIST_POINT_V_SCUCCESS
} from "../actions/type";

const intialState = {
    data: {},
    isLoading: false,
    isErr: false
}

export default (state = intialState, action) => {
    switch (action.type) {
        case GET_LIST_POINT_V:
            return {
                ...state,
                isLoading: true,
                isErr:null
            }
        case GET_LIST_POINT_V_SCUCCESS:
            return {
                ...state,
                isLoading: false,
                data: action.payload
            }

        case GET_LIST_POINT_V_FAIL:
            return {
                ...state,
                isErr: true,
                isLoading: false
            }
            
    }
    return state
}