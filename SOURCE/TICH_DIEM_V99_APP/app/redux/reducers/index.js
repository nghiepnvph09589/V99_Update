import { combineReducers } from "redux";
import { REDUCER } from '../../constants/Constant';
import { RESET } from "../actions/type";
import AddressReducer from './AddressReducer';
import AddToCartReducer from './AddToCartReducer';
import CartReducer from './CartReducer';
import CountCartReducer from './CountCartReducer';
import GetImageReducer from './GetImageReducer';
import GetOrderDetailReducer from './GetOrderDetailReducer';
import GetStoreReducer from './GetStoreReducer';
import HistoryReducer from './HistoryReducer';
import HomeReducer from './HomeReducer';
import ListProductReducer from './ListProductReducer';
import NotificationReducer from './NotificationReducer';
import OrderReducer from './OrderReducer';
import ProductReducer from "./ProductReducer";
import UpdateUseReducer from './UpdateUseReducer';
import UserReducer from "./UserReducer";
import UtilityReducer from './UtilityReducer';
import WalletAccumulatePointsReducer from './WalletAccumulatePointsReducer';
import WarrantyReducer from './WarrantyReducer';
import WalletPointsReducer from './WalletPointsReducer';
import HistoryDrawPointsReducer from './HistoryDrawPointsReducer';
import BankSelectReducer from './BankSelectReducer';
import BankReducer from './BankReducer';
import HistoryMovingPointsReducer from './HistoryMovingPointsReducer';
import NavigateTabReducer from './NavigateTabReducer';

appReducer = combineReducers({
  [REDUCER.USER]: UserReducer,
  productReducer: ProductReducer,
  homeReducer: HomeReducer,
  [REDUCER.ADDRESS]: AddressReducer,
  cartReducer: CartReducer,
  [REDUCER.COUNT_CART]: CountCartReducer,
  [REDUCER.ADD_TO_CART]: AddToCartReducer,
  [REDUCER.WALLET_ACCUMULATE_POINTS]: WalletAccumulatePointsReducer,
  [REDUCER.WALLET_POINTS]: WalletPointsReducer,
  [REDUCER.HISTORY_DRAW_POINTS]: HistoryDrawPointsReducer,
  [REDUCER.BANK_SELECT]: BankSelectReducer,
  [REDUCER.BANK]: BankReducer,
  [REDUCER.HISTORY_MOVING_POINTS]: HistoryMovingPointsReducer,
  [REDUCER.NAVIGATE_TAB]: NavigateTabReducer,
  listProductReducer: ListProductReducer,
  orderReducer: OrderReducer,
  getOrderDetailReducer: GetOrderDetailReducer,
  getStoreReducer: GetStoreReducer,
  utilityReducer: UtilityReducer,
  historyReducer: HistoryReducer,
  updateUseReducer: UpdateUseReducer,
  notificationReducer: NotificationReducer,
  warrantyReducer: WarrantyReducer,
  imageReducer: GetImageReducer,
});

const initialState = appReducer({}, {})

export default rootReducer = (state, action) => {
  if (action.type === RESET) {
    state = initialState
  }

  return appReducer(state, action)
}
