import { GET_ADDRESS, GET_ADDRESS_SUCCESS, GET_ADDRESS_FAIL, } from "../actions/type";

const initialState = {
    listProvince: [],
    listDistrict: [],
    isLoading: false,
    error: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case GET_ADDRESS: {
            return {
                ...state,
                isLoading: true,
                error: null,
            }
        }
        case GET_ADDRESS_SUCCESS: {
            return {
                ...state,
                listProvince: action.payload.province.sort((a, b) => convertVietnamese(a.provinceName) > convertVietnamese(b.provinceName)),
                listDistrict: action.payload.listDistrict
                    .sort((a, b) => convertVietnamese(a.districtName.replace(a.districtType, '')) > convertVietnamese(b.districtName.replace(b.districtType, '')))
                    .sort((a, b) => convertVietnamese(a.districtName) > convertVietnamese(b.districtName)),
                isLoading: false,
                error: null,
            }
        }
        case GET_ADDRESS_FAIL: {
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

function convertVietnamese(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_/g, "-");
    str = str.replace(/-+-/g, "-");
    str = str.replace(/^\-+|\-+$/g, "");

    return str;
}