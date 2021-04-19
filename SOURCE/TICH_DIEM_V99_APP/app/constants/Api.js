import axios from "axios";
import { Alert } from "react-native";
import AsyncStorage from "@react-native-community/async-storage";
import NavigationUtil from "../navigation/NavigationUtil";
import I18n from "../i18n/i18n";
import { showMessages } from "../utils/Alert";
import { SCREEN_ROUTER } from "./Constant";

const BASE_URL = {
  DEV: "http://54.251.116.128:81/",
  RELEASE: "http://tdtd.winds.vn/"
};

function createAxios() {
  // AsyncStorage.setItem("token", 'FD41095A3D1CFCDA4D3BB3F2D2D40F17')

  var axiosInstant = axios.create();
  const url = "http://tichdiemv99.winds.vn/";
  // const url = BASE_URL.RELEASE

  axiosInstant.defaults.baseURL = url;

  axiosInstant.defaults.timeout = 20000;
  axiosInstant.defaults.headers = { "Content-Type": "application/json" };

  axiosInstant.interceptors.request.use(
    async config => {
      let token = await AsyncStorage.getItem("token");
      config.headers.token = token || "";
      return config;
    },
    error => Promise.reject(error)
  );

  axiosInstant.interceptors.response.use(response => {
    if (response.data && response.data.code == 403) {
      showMessages("Thông báo", I18n.t("relogin"));
      AsyncStorage.setItem(
        "token",
        "",
        NavigationUtil.navigate(SCREEN_ROUTER.AUTH_LOADING)
      );
    } else if (response.data && response.data.status != 1) {
      showMessages("Thông báo", response.data.message);
    }
    return response;
  });
  return axiosInstant;
}

export const getAxios = createAxios();

/* Support function */
function handleResult(api) {
  return api.then(res => {
    if (res.data.status != 1) {
      return Promise.reject(new Error("Co loi xay ra"));
    }
    return Promise.resolve(res.data);
  });
}

export const requestLogin = payload => {
  return handleResult(getAxios.post("api/Service/CheckLoginApp", payload));
};

export const requestCheckPhoneRegister = payload => {
  return handleResult(getAxios.post("api/Service/CheckPhoneRegister", payload));
};

export const requestHomeData = payload => {
  return handleResult(getAxios.get(`api/Service/GetHomeScreen`));
};

export const requestProduct = () => {
  return handleResult(getAxios.get(`api/Service/getListCategory`));
};

export const requestGetListGifts = payload => {
  return handleResult(
    getAxios.get("api/Service/GetListGifts", {
      params: {
        type: payload,
        GiftID: "",
        SearchKey: ""
      }
    })
  );
};
export const getUserInfo = () => {
  return handleResult(getAxios.get(`api/Service/GetUserInfor`));
};
export const requestAddressData = () => {
  return handleResult(
    getAxios.get(`api/Service/GetProvinceAndDistric?ProID=${""}`)
  );
};

export const requestLogout = () => {
  return handleResult(getAxios.get("api/Service/Logout"));
};

export const getCarts = () => {
  return handleResult(getAxios.get("api/Service/getCarts"));
};

export const requestAddToCart = payload => {
  return handleResult(
    getAxios.post("api/Service/AddToCart", { ListOrderItemID: payload })
  );
};

export const requestRemoveCart = payload => {
  return handleResult(
    getAxios.post("api/Service/RemoveCart", { listOrderItemID: payload })
  );
};

export const exchangeGift = payload => {
  return handleResult(
    getAxios.post("/api/Service/CreateRequest", {
      GiftID: payload.gift.giftID,
      Type: payload.type
    })
  );
};

export const requestDonate = payload => {
  return handleResult(getAxios.post("/api/Service/GivePoint", payload));
};

export const requestCreateOrder = payload => {
  return handleResult(getAxios.post("/api/Service/CreateOrder", payload));
};

export const requestGetCountCart = () => {
  return handleResult(getAxios.get("/api/Service/GetCountCart"));
};

export const requestListProduct = payload => {
  return handleResult(
    getAxios.get(`api/Service/getListProduct`, {
      params: {
        limit: 10,
        ...payload
      }
    })
  );
};
export const requestRegister = payload => {
  return handleResult(getAxios.post(`/api/Service/Register`, payload));
};

export const requestCheckOTP = payload => {
  return handleResult(getAxios.post(`/api/Service/CheckOTP`, payload));
};

export const updateUser = payload => {
  return handleResult(getAxios.post(`/api/Service/UpdateUser`, payload));
};
export const getListOrder = payload => {
  return handleResult(
    getAxios.get(
      `/api/Service/GetListOrder?status=${payload.status}&page=${
        payload.page
      }&limit=70`
    )
  );
};

export const requestGetNotify = () => {
  return handleResult(getAxios.get("/api/Service/GetNotify"));
};
export const getOrderDetail = payload => {
  return handleResult(
    getAxios.get(`/api/Service/GetOrderDetail?orderID=${payload}`)
  );
};
export const cancelOrder = payload => {
  return handleResult(
    getAxios.get(`/api/Service/CancelOrder?orderID=${payload}`)
  );
};
export const getStore = payload => {
  return handleResult(
    getAxios.get(
      `/api/Service/getListShop?provinceID=${payload}&latitude=20.08090&longitude=106.098098`
    )
  );
};

export const activeAgent = code => {
  return handleResult(
    getAxios.post(`/api/Service/ActiveAgent`, { AgentCode: code })
  );
};

export const getListUtility = payload => {
  return handleResult(
    getAxios.get(`/api/Service/GetNews?type=${payload.type}`)
  );
};

export const getNewsDetail = newsID => {
  return handleResult(
    getAxios.get(`/api/Service/getNewsDetail?newsID=${newsID}`)
  );
};
export const getHistory = () => {
  return handleResult(
    getAxios.get(`/api/Service/GetPointHistory?FromDate=12/07/2019`)
  );
};

export const changePass = payload => {
  return handleResult(getAxios.post(`/api/Service/changePassword`, payload));
};
export const requestGetItemByCode = ({ code }) => {
  return handleResult(
    getAxios.get(`/api/Service/getItemByCode`, {
      params: {
        code
      }
    })
  );
};

export const requestActiveWarranty = ({ code }) => {
  return handleResult(getAxios.post(`/api/Service/activeWarranty`, { code }));
};

export const requestGetListWarranty = payload => {
  return handleResult(
    getAxios.get(
      `/api/Service/getListWarranty?page=${payload.page}&limit=10&text=${
        payload.text
      }`
    )
  );
};
export const forgotPassword = payload => {
  return handleResult(getAxios.post(`/api/Service/forgotPassword`, payload));
};
export const getImage = () => {
  return handleResult(getAxios.get("/api/Service/getImages"));
};
export const deleteImage = listID => {
  return handleResult(
    getAxios.post("/api/Service/deleteImage", { listID: listID })
  );
};
export const uploadImage = payload => {
  return handleResult(getAxios.post("/api/Service/uploadImage", payload));
};
export const requestGetListMember = payload => {
  return handleResult(
    getAxios.get("/api/Service/GetListMember", { params: payload })
  );
};
export const requestListHistoryPointMember = payload => {
  return handleResult(
    getAxios.get("/api/Service/ListHistoryPointMember", {
      params: { limit: 200, ...payload }
    })
  );
};
export const requestGetListBank = () => {
  return handleResult(getAxios.get("/api/Service/GetListBank"));
};
export const requestAddBankAccount = payload => {
  return handleResult(getAxios.post("/api/Service/AddBankAccount", payload));
};
export const requestDelBankAccount = payload => {
  return handleResult(
    getAxios.get("/api/Service/DelBankAccount", { params: payload })
  );
};
export const requestGetListBankOfCus = () => {
  return handleResult(getAxios.get("/api/Service/GetListBankOfCus"));
};

export const requestDrawPoint = payload => {
  return handleResult(getAxios.post("/api/Service/DrawPoint", payload));
};

export const requestMovePoint = payload => {
  return handleResult(getAxios.post("/api/Service/MovePoint", payload));
};

export const requestGetHistyoriesDetail = payload => {
  return handleResult(
    getAxios.get("/api/Service/GetHistyoriesDetail", { params: payload })
  );
};

export const requestGetAppVersion = payload => {
  return handleResult(
    getAxios.get(`/api/Service/getAppVersion`, { params: payload })
  );
};

export const requestUpdateNoti = payload => {
  return handleResult(
    getAxios.get(`/api/Service/UpdateNoti`, { params: payload })
  );
};

export const requestGetListLastRefCode = () => {
  return handleResult(getAxios.get(`/api/Service/GetListLastRefCode`));
};
