import {
    GET_CART, GET_CART_SUCCESS, GET_CART_FAIL,
    SELECT_ALL_CART_ITEM, UPDATE_CART_ITEM, CART_ITEM_TOUCH, REMOVE_CART_SUCCESS, REMOVE_CART, REMOVE_CART_FAIL
} from "../actions/type"
import Mockup from "../../constants/Mockup"
import reactotron from "reactotron-react-native"


const initialState = {
    isLoading: false,
    error: false,
    data: [],
    dataSelected: [],
    dataUnSelected: [],
    countDataActive: [],
    dataActive: [],
    totalPrice: 0,
    quantityCart: 0,
    isRemoving: false,
}

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_CART:
            return {
                ...state,
                isLoading: true,
                error: false,
                data: [],
                dataSelected: [],
                dataUnSelected: [],
                dataActive: [],
                totalPrice: 0,
            }

        case GET_CART_SUCCESS: {
            const listCart = action.payload.data.listCart || []
            const listItemIDSelected = action.payload.listItemIDSelected || []
            let dataSelectedTmp = []
            let dataUnSelectedTmp = []
            let totalPriceTmp = 0
            let countDataActive = 0
            let listDataActive = []

            if (listCart.length) {
                listDataActive = listCart.filter(value => value.status != 0)
                countDataActive = listDataActive.length
            }

            if (listItemIDSelected?.length) {
                dataSelectedTmp = listItemIDSelected.map((valueItemID, index) => {
                    return listCart.findIndex((value, index) => (value.itemID == valueItemID && value.status != 0))
                }).filter(value => value != -1)

                listCart.forEach((valueCart, index) => {
                    if (valueCart.status != 0 && !dataSelectedTmp.some((value) => (value == index))) {
                        dataUnSelectedTmp.push(index)
                    }
                })
            } else {
                if (listCart.length) {
                    listCart.forEach((value, index) => {
                        if (value.status != 0) dataSelectedTmp.push(index)
                    })
                }
            }

            if (dataSelectedTmp.length)
                totalPriceTmp = dataSelectedTmp.reduce((previousValue, currentValue) => {
                    const sumPrice = listCart[currentValue].qty * listCart[currentValue].itemPrice
                    return previousValue + sumPrice
                }, 0) || 0

            return {
                ...state,
                isLoading: false,
                error: false,
                data: listCart,
                dataSelected: dataSelectedTmp,
                totalPrice: totalPriceTmp,
                dataUnSelected: dataUnSelectedTmp,
                countDataActive,
                dataActive: listDataActive
            }
        }

        case REMOVE_CART:
            return {
                ...state,
                isRemoving: true,
            }

        case REMOVE_CART_SUCCESS: {
            const listCart = action.payload || []
            let dataUnSelectedTmp = []
            let listDataActive = []
            let countDataActive = 0

            if (listCart.length) {
                listDataActive = listCart.filter(value => value.status != 0)
                countDataActive = listDataActive.length
                listCart.forEach((valueCart, index) => {
                    if (valueCart.status != 0) {
                        dataUnSelectedTmp.push(index)
                    }
                })
            }

            return {
                ...state,
                isRemoving: false,
                data: listCart,
                dataSelected: [],
                totalPrice: 0,
                countDataActive,
                dataActive: listDataActive,
                dataUnSelected: dataUnSelectedTmp,
            }
        }

        case REMOVE_CART_FAIL:
            return {
                ...state,
                isRemoving: false,
            }

        case GET_CART_FAIL:
            return {
                ...state,
                isLoading: false,
                error: action.payload,
                data: [],
                totalPrice: 0,
                dataSelected: [],
            }

        case SELECT_ALL_CART_ITEM:
            {
                if (action.payload) {
                    var currentValue = 0
                    state.dataUnSelected.map(item => currentValue += state.data[item].sumPrice)
                    return {
                        ...state,
                        dataSelected: [...state.dataSelected, ...state.dataUnSelected],
                        dataUnSelected: [],
                        totalPrice: state.totalPrice + currentValue,
                    }
                } else {
                    return {
                        ...state,
                        dataSelected: [],
                        dataUnSelected: [...state.dataSelected, ...state.dataUnSelected],
                        totalPrice: 0,
                    }
                }
            }

        case CART_ITEM_TOUCH: {
            if (action.payload.isChecked)
                return {
                    ...state,
                    dataSelected: state.dataSelected.filter(item => item != action.payload.index),
                    dataUnSelected: state.dataUnSelected.concat(action.payload.index),
                    totalPrice: state.totalPrice - state.data[action.payload.index].sumPrice,
                }
            else
                return {
                    ...state,
                    dataSelected: state.dataSelected.concat(action.payload.index),
                    dataUnSelected: state.dataUnSelected.filter(item => item != action.payload.index),
                    totalPrice: state.totalPrice + state.data[action.payload.index].sumPrice,
                }
        }

        case UPDATE_CART_ITEM: {
            let tmpTotalPrice = state.totalPrice
            if (action.payload.isChecked) tmpTotalPrice = state.totalPrice - state.data[action.payload.index].sumPrice

            return {
                ...state,
                data: changeItem(state.data, action.payload.item, action.payload.index),
                totalPrice: action.payload.isChecked && tmpTotalPrice + action.payload.item.sumPrice || tmpTotalPrice,
            }
        }

        default:
            return state
    }
}

function mergeArrays(...arrays) {
    let jointArray = [];

    arrays.forEach(array => {
        jointArray = [...jointArray, ...array];
    });
    return Array.from(new Set([...jointArray]));
}

changeItem = (array, item, index) => {
    tempData = array,
        tempData[index] = item
    return tempData
}