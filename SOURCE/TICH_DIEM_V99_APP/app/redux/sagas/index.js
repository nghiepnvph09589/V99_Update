import {
  watchGetUser,
  watchFetchHome,
  watchFetchGetGift,
  watchProduct,
  watchFetchAddress,
  watchSocialLogin,
  watchGetCart,
  watchAddToCart,
  watchRemoveCart,
  watchExchangeGift,
  watchFetchDonate,
  watchFetchGetCountCart,
  watchListProduct,
  //watchFetchRegister,
  watchFetchUpdateUser,
  watchFetchGetListOrder,
  watchFetchGetOrderDetail,
  watchFetchGetStore,
  watchFetchGetListUtility,
  watchFetchGetListNewRelative,
  watchFetchGetHistory,
  watchFetchChangePass,
  watchFetchGetNotification,
  watchFetchGetWarranty,
  watchFetchGetImage,
  watchFetchgetWalletAccumulate,
  watchFetchgetWalletPoints,
  watchFetchGetHistoryDrawPoints,
  watchFetchGetBankSelect,
  watchFetchGetBank,
  watchFetchGetHistoryMovingPoints,
  watchFetchGetListPontV,
} from "./NetworkSaga";

export default function* rootSaga() {
  yield watchGetUser
  yield watchFetchHome
  yield watchFetchGetGift
  yield watchProduct
  yield watchFetchAddress
  yield watchSocialLogin
  yield watchGetCart
  yield watchAddToCart
  yield watchRemoveCart
  yield watchExchangeGift
  yield watchFetchDonate
  yield watchFetchGetCountCart
  yield watchListProduct
  //yield watchFetchRegister
  yield watchFetchUpdateUser,
    yield watchFetchGetListOrder,
    yield watchFetchGetOrderDetail,
    yield watchFetchGetStore,
    yield watchFetchGetListUtility,
    yield watchFetchGetListNewRelative,
    yield watchFetchGetHistory
  yield watchFetchChangePass
  yield watchFetchGetNotification
  yield watchFetchGetImage
  yield watchFetchgetWalletAccumulate
  yield watchFetchgetWalletPoints
  yield watchFetchGetHistoryDrawPoints
  yield watchFetchGetBankSelect
  yield watchFetchGetBank
  yield watchFetchGetHistoryMovingPoints
  yield watchFetchGetListPontV
}
