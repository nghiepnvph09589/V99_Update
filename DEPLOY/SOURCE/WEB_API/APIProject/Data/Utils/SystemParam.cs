using Data.Business;
using Data.DB;
using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Utils
{
    public class SystemParam : GenericBusiness
    {
        //public const string PASS_DEFAULT = "daiichivn";
        //public const string HOST_DEFAULT = "smtp.gmail.com";
        //public const string EMAIL_CONFIG = "daiichisuport@gmail.com";
        //public const string PASS_CONFIG = "qqkbewqyqudknent";
        //public const string PASS_EMAIL = "windsoft123456@";

        public const string PASS_DEFAULT = "TD123456";
        public const string HOST_DEFAULT = "smtp.gmail.com";
        public const string EMAIL_CONFIG = "tichdiemtrieudo123@gmail.com";
        public const string PASS_CONFIG = "gfneiwfxmzdbpvhr";
        public const string PASS_EMAIL = "TichDiemTrieuDo@123";
        //public const string URL_WEB_SOCKET = "http://tdtd.winds.vn:8090/socketio/";
        public const string URL_WEB_SOCKET = "http://tichdiemv99.winds.vn:8090/socketio/";
        //public const string PASS_CONFIG = "sqsusteaeaztongt";
        //public const string PASS_EMAIL = "windsoft123456a@";

        //public const string APP_ID = "916a6532-ef76-4606-836c-16c1d3a7329f";
        public const string APP_ID = "9e9fbbde-ba5c-42af-9c08-7888f0709c4a";
        //public const string Authorization = "Basic :YmFjZTA0YjktNGU0ZC00N2RmLTljYWQtYjM4MjI2OTQwY2E4";
        public const string Authorization = "Basic :MmExMjU3ODQtYTY3Zi00N2RlLWJhMTQtODViNWM0MWQ3MWIx";
        public const string URL_ONESIGNAL = "https://onesignal.com/api/v1/notifications";
        public const string URL_BASE_https = "Basic ://onesignal.com/api/v1/notifications";
        //public const string ANDROID_CHANNEL_ID = "958b57c5-9726-4c2e-b4ac-0e3f085a957b";
        public const string ANDROID_CHANNEL_ID = "cf2a4a57-1d24-4a8d-8950-c853928144c3";

        // config VNPAY
        public const string vnp_Return_url = "http://tichdiemv99.winds.vn/VnPay/Index";
        public const string vnpay_api_url = "http://tichdiemv99.winds.vn/api/Payment/vnp_ipn";
        public const string vnp_Url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string vnp_TmnCode = "V99GROUP";
        public const string vnp_HashSecret = "CPJTEKIYPHZUVOMMTDFQRGMXBIOBEDPW";
        public const string Transaction_False = "Giao dịch thất bại";
        public const string Transaction_Success = "Giao dịch thành công";
        public const string customer_failed = "tichdiemv99://failed/";
        public const string customer_success = "tichdiemv99://success/";
        public const string vnp_CodeSuccess = "00";

        public const int STATUS_TRANSACTION_SUCCESS = 1;//Giao dịch thành công
        public const int STATUS_TRANSACTION_WAITING = 0;//Chờ xác nhận
        public const int STATUS_TRANSACTION_FALSE = -1;//Từ chối


        public const int TIME_DELAY = 30000;

        public const int NOTIFY_NAVIGATE_ORDER = 1; //xác nhận, hủy, thanh toán đơn hàng => báo về app, app => màn hình chi tiết đơn hàng
        public const int NOTIFY_NAVIGATE_REQUEST = 2; //Yêu cầu rút điểm
        public const int NOTIFY_NAVIGATE_NEWS = 3; //bài viết mới => báo về app, app => màn hình chi tiết bài viết
        public const int NOTIFY_NAVIGATE_HISTORY = 4; //Lịch sử ví point => Màn hình ví point
        public const int NOTIFY_NAVIGATE_HISTORY_POINT_RANKING = 5;//Lịch sử ví tích => Màn hình ví tích điểm

        public const int ONESIGNAL_NOTIFY_POINT_HISTORY = 6;//Push onsignal tới màn hình lịch sử tích điểm
        public const int ONESIGNAL_NOTIFY_REQUEST_DETAIL = 7;//push onsignal tới màn hình chi tiết yêu cầu
        public const int ONESIGNAL_NOTIFY_HISTORY_POINT_RANKING = 8;//Lịch sử tích điểm ví tích điểm
        public const int NOTIFY_ADD_POINT_VNPAY = 9;//Nạp điểm ví Point VNPAY

        // Customer
        public const int CUSTOMER_VIP = 1;
        public const int CUSTOMER_NORMAL = 0;

        public const int POINT_NEW_MEMBER = 500;
        public const int POINT_NEW_MEMBER_REF = 1000;

        public const int CONFIG_TIME = 5;
        public const int ROLL_ADMIN = 1;
        public const int ROLL_CUSTOMER = 0;
        public const int POINT_START = 10;
        public const int POINT_RANKING_START = 0;
        public const int POINT_V_START = 0;
        public const int QTY_CONTENT_HOME_SCREEN = 5;
        public const int TYPE_LOGIN_ACCOUNT = 4;
        public const int TYPE_LOGIN_PHONE = 3;
        public const int TYPE_LOGIN_FACE = 1;
        public const int TYPE_LOGIN_GOOGLE = 2;
        public const double TIME_EXPIRE_OTP = 60;
        public const int TYPE_LOGIN_APP_CUSTOMER = 1;
        public const int TYPE_LOGIN_DEFAULT = 1;
        public const int TYPE_REGISTER_ACCOUNT = 2;
        public const int TYPE_ERROR_UPDATE_PHONE = 1;
        public const int TYPE_ERROR_PHONE_EXIST = 2;
        public const int CHECK_OTP_FAIL = 0;
        public const int CHECK_OTP_TRUE = 1;

        public const int QTY_DEFAULT_ADD_TO_CART = 1;
        public const int GET_ALL_PRODUCT = 0;

        public const int GENDER_MEN = 0;
        public const int GENDER_WOMEN = 1;

        public const int PROVINCE_DEFAULT = 1;
        public const int DISTRICT_DEFAULT = 1;

        public const int NO_NEED_UPDATE = 0;
        public const int NEED_UPDATE = 1;

        public const string CONVERT_DATETIME = "dd/MM/yyyy";
        public const string CONVERT_DATETIME_HAVE_HOUR = "dd/MM/yyyy HH:mm";
        public const int MAX_ROW_IN_LIST = 30;
        public const int LIMIT_ORDER = 20;
        public const int ACTIVE = 1;
        public const int RETURN_TRUE = 1;
        public const int RETURN_FALSE = 0;
        public const int ACTIVE_FALSE = 0;
        public const int NO_ACTIVE = 2;
        public const int COUNT_NULL = 0;
        public const int DELETE_REQUEST_FAIL = 2;
        public const int CATEGORY_PRODUCT = 11;
        public const int OUT_OF_STOCK = 0;//het hang
        public const int IN_STOCK = 1; //con hang
        public const int ALL_ITEM = 3;

        //agent
        public const int TYPE_REQUEST_AGENT = 1;
        public const int STATUS_ACCEPT_REQUEST_AGENT = 1;
        public const int STATUS_CANCEL_REQUEST_AGENT = 0;
        public const int STATUS_PENDING_REQUEST_AGENT = 2;

        public const int EXIST_AGENT_CODE = 99;
        public const int EXIST_PHONE_AGENT = 98;
        public const string MESSAGE_EXIST_PHONE_AGENT = "Số điện thoại này đã được yêu cầu hoặc thuộc về 1 đại lý khác.";

        public const int TYPE_IMAGE = 1;
        public const int TYPE_VIDEO = 2;

        public const string TOKEN_INVALID = "Token invalid";
        public const string TOKEN_NOT_FOUND = "Token not found";
        public const string DEVICE_ID_NOT_FOUND_MESSAGE = "DeviceID not found";
        public const string SERVER_ERROR = "Hệ thống đang bảo trì";
        public const string MESSAGE_ERROR = "Thất bại";
        public const string MESSAGE_DRAW_POINT = "Đã gửi yêu cầu rút điểm thành công";
        public const string MESSAGE_DRAW_POINT_ERROR = "Không thể gửi yêu cầu rút điểm";
        public const string ERROR_ADD_TO_CART_MESSAGE = "Không thể thêm sản phẩm vào giỏ hàng.";
        public const string MESSAGE_CART_EMPTY = "Giỏ hàng đang trống.";
        public const string USERID_NOT_FOUND = "Vui lòng nhập đúng định dạng số điện thoại";
        public const string MESSAGE_INVALID_NUMBER = "Số lượng không được phép âm";
        public const string MESSAGE_REMOVE_CART_NO_ID = "Vui lòng chọn sản phẩm để xóa khỏi giỏ hàng";
        public const string MESSAGE_CREATE_ORDER_NO_ID = "Vui lòng chọn sản phẩm để đặt hàng.";
        public const string MESSAGE_CREATE_ORDER_FAIL = "Không thể tạo đơn hàng.";
        public const string MESSAGE_ERROR_CONDITION_POINT_CREATE_ORDER_FAIL = "Số điểm của bạn không đủ để tạo đơn hàng này";
        public const string MESSAGE_ERROR_CONDITION_STATUS_ACCOUNT_DEACTIVE = "Không thể thực hiện chức năng này khi tài khoản chưa được kích hoạt";
        public const string MESSAGE_UPDATE_CART_FAIL = "Không thể cập nhật giỏ hàng. Hệ thống không tìm thấy thông tin sản phẩm cần cập nhật";
        public const string MESSAGE_ERROR_USER_INFO = "Vui lòng nhập đầy đủ họ tên, số điện thoại, địa chỉ";
        public const string MESSAGE_ERROR_LASTREF_CODE = "Mã giới thiệu không hợp lệ";
        public const string MESSAGE_EXISTING_REQUEST_AGENT = "Đã tồn tại yêu cầu đăng ký. Vui lòng chờ hệ thống xác nhận";
        public const string MESSAGE_ERROR_UPDATE_PHONE = "Không thể thay đổi số điện thoại";
        public const string MESSAGE_ERROR_EXIST_PHONE = "Số điện thoại đã được sử dụng";
        public const string MESSAGE_ERROR_EXIST_EMAIL = "Email đã được sử dụng";
        public const string MESSAGE_CODE_COLOR_INVALID = "Vui lòng nhập đúng định dạng mã màu HEX";
        public const string MESSAGE_NOTIFY_NOT_FOUND = "Thông báo không tồn tại";
        public const string MESSAGE_EXIST_ACCOUNT = "Số điện thoại này đã được sử dụng.";
        public const string MESSAGE_LOGIN_ACCOUNT_FAIL = "Sai tài khoản hoặc mật khẩu";
        public const string MESSAGE_ERROR_CREATE_ORDER_NO_DATA = "Vui lòng chọn sản phẩm để tạo đơn hàng";
        public const string MESSAGE_ERROR_UPDATE_CART_NO_DATA = "Vui lòng chọn sản phẩm để cập nhật";
        public const string MESSAGE_REQUIRE_REGISTER = "Vui lòng nhập họ tên, số điện thoại và mật khẩu";
        public const string MESSAGE_NOT_EXIST_PRODUCT = "Hệ thống không tìm thấy sản phẩm";
        public const string MESSAGE_ADD_TO_CART_FAIL = "Sản phẩm không tồn tại hoặc đã hết hàng";
        public const string MESSAGE_ADD_TO_CART_SUCCESS = "Thêm sản phẩm vào giỏ hàng thành công";
        public const string MESSAGE_ERROR_REMOVE_CART_FAIL = "Không thể xóa sản phẩm khỏi giỏ hàng. Hệ thống không tìm thấy thông tin sản phẩm.";
        public const string MESSAGE_NOT_EXIST_INFO = "Hệ thống không tìm thấy thông tin.";
        public const string MESSAGE_LIST_EMPTY = "Danh sách dữ liệu hiện đang rỗng.";
        public const string MESSAGE_ERROR_INVALID_PHONE = "Vui lòng nhập đúng định dạng số điện thoại.";
        public const string MESSAGE_NOT_EXIST_USER = "Hệ thống không tìm thấy người dùng với số điện thoại này";
        public const string MESSAGE_ERROR_EXPIRE_OTP = "Mã xác nhận đã hết hiệu lực";
        public const string MESSAGE_LOGIN_APP_CUSTOMER_FALSE = "Vui lòng nhập lại mã OTP";
        public const string MESSAGE_CANT_GIVE_POINT_FOR_ME = "Bạn không thể tặng điểm cho chính mình";
        public const string MESSAGE_ERROR_GIVE_POINT_NO_POINT = "Vui lòng nhập số điểm cần tặng";
        public const string MESSAGE_KHONG_DU_DIEM_DE_TANG = "Số điểm khả dụng của bạn không đủ để chuyển điểm";
        public const string MESSAGE_VUOT_QUA_HAN_MUC_QUY_DINH = "Bạn không thể tặng điểm vì đã dùng hết điểm trong 1 ngày. Số điểm có thể dùng trong 1 ngày bằng ";
        public const string MESSAGE_GIVE_POINT_SUCCESS = "Tặng điểm thành công";
        public const string MESSAGE_ERROR_PHONE = "Vui lòng nhập lại số điện thoại";
        public const string MESSAGE_ERROR_WRONG_PASSWORD = "Mật khẩu cũ không đúng";
        public const string MESSAGE_ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD = "Mật khẩu mới không được phép giống mật khẩu hiện tại";
        public const string MESSAGE_CHANGE_PASSWORD_SUCCESS = "Đổi mật khẩu thành công";
        public const string MESSAGE_ERROR_NOT_EXIST_AGENT_CODE = "Mã đại lý không tồn tại trong hệ thống";
        public const string MESSAGE_ERROR_AGENT_CODE_USED = "Mã đại lý đã được sử dụng";
        public const string MESSAGE_ACTIVE_AGENT_SUCCESS = "Tài khoản của bạn đã được kích hoạt thành đại lý";
        public const string MESSAGE_ERROR_CUSTOMER_WAS_AGENT = "Tài khoản của bạn đã là đại lý. Không thể thực hiện thao tác này";
        public const string MESSAGE_WARRANTY_CODE_USED = "Mã bảo hành đã được sử dụng";
        public const string MESSAGE_WARRANTY_CODE_NOT_EXIST = "Mã kích hoạt bảo hành không tồn tại";
        public const string MESSAGE_SCAN_QR_FAIL = "Mã đã được sử dụng hoặc không tồn tại";
        public const string MESSAGE_ACTIVE_WARRANTY_SUCCESS = "Kích hoạt bảo hành thành công";
        public const string MESSAGE_ERROR_INVALID_EMAIL = "Sai định dạng Email";
        public const string MESSAGE_NEW_PASSWORD_SENT = "Mật khẩu đã mới được gửi về email của bạn. Vui lòng kiểm tra email";
        public const string MESSAGE_ERROR_NOT_EXIST_EMAIL = "Email không tồn tại trong hệ thống";
        public const string MESSAGE_ERROR_INVALID_POINT = "Vui lòng nhập đúng định dạng số điểm";
        public const string MESSAGE_ERROR_DISTRICT_NOT_IN_PROVINCE = "Quận huyện bạn chọn không phù hợp với Tỉnh thành";
        public const string MESSAGE_ERROR_PROVINCEID_NULL = "Vui lòng chọn Tỉnh thành";
        public const string MESSAGE_ERROR_NOT_EXIST_NEWS = "Bài viết không tồn tại";
        public const string MESSAGE_ERROR_PHONE_NOT_FOUND = "Số điện thoại chưa được đăng ký";
        public const string MESSAGE_PLEASE_UPDATE_PHONE = "Vui lòng cập nhật thông tin cho tài khoản";
        public const string MESSAGE_ERROR_USER_NOT_FOUND = "Không thể chuyển điểm đến số điện thoại này, vui lòng kiểm tra lại.";
        public const string MESSAGE_ERROR_CAN_NOT_GIFT_POINT = "Không thể chuyển điểm";
        public const string MESSAGE_ERROR_DUPLICATE_BANK_ACOUNT = "Không thể có  2 tài khoản chung một ngân hàng, vui lòng kiểm tra lại.";
        public const string MESSAGE_ERROR_EMAIL_NOT_FOUND = "Vui lòng nhập lại email";
        public const string MESSAGE_EMAIL_ERROR_LOGIN = "Tài khoản chưa được đăng ký";
        public const string MESSAGE_INVALID_INPUT_POINT = "Số điểm chuyển không hợp lệ";



        // thanh cong
        public const int SUCCESS_CODE = 200;
        public const string SUCCESS_MESSAGE = "Thành công";
        public const string SUCCESS_MESSAGE_UPDATE_CART = "Cập nhật giỏ hàng thành công";
        // sai mk
        public const int ERROR_PASS_API = 403;
        // loi quy trinh
        public const int PROCESS_ERROR = 500;
        public const int FAIL = 501;
        public const int ERROR_UPDATE_PHONE = 502;
        public const int ERROR_EXIST_EMAIL = 503;
        public const int ERROR_EXIST_PHONE = 505;
        public const int ERROR_ADD_TO_CART = 506;
        public const int CODE_INVALID_NUMBER = 507;
        public const int REMOVE_CART_NO_ID = 508;
        public const int CREATE_ORDER_NO_ID = 509;
        public const int CREATE_ORDER_FAIL = 510;
        public const int UPDATE_CART_FAIL = 511;
        public const int ERROR_USER_INFO = 512;
        public const int ERROR_LOGIN_ACCOUNT_FAIL = 513;
        public const int ERROR_CREATE_ORDER_NO_DATA = 514;
        public const int ERROR_UPDATE_CART_NO_DATA = 515;
        public const int REQUIRE_REGISTER = 516;
        public const int NOT_EXIST_PRODUCT = 517;
        public const int ERROR_REMOVE_CART_FAIL = 518;
        public const int KHONG_DU_DIEM_DE_TANG = 519;
        public const int VUOT_QUA_HAN_MUC_QUY_DINH = 520;
        public const int ERROR_WRONG_PASSWORD = 521;
        public const int ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD = 522;
        public const int ERROR_NOT_EXIST_AGENT_CODE = 523;
        public const int ERROR_AGENT_CODE_USED = 524;
        public const int ERROR_CUSTOMER_WAS_AGENT = 525;
        public const int ERROR_INVALID_EMAIL = 526;
        public const int ERROR_NOT_EXIST_EMAIL = 527;
        public const int ERROR_DISTRICT_NOT_IN_PROVINCE = 528;
        public const int ERROR_PROVINCEID_NULL = 529;
        public const int ERROR_PHONE = 530;
        public const int LOGIN_APP_CUSTOMER_FALSE = 531;
        public const int ERROR_EXPIRE_OTP = 532;
        public const int ERROR_LASTREF_CODE = 533;
        public const int ERROR_PHONE_NOT_FOUND = 534;
        public const int ERROR_ACOUNT_DEACTIVE = 535;
        public const int ERROR_CONDITION_POINT = 536;



        public const int EXIST_ACCOUNT = -500;
        public const int EXIST_PHONE_REGISTER = -501;
        public const int EXIST_EMAIL_REGISTER = -502;
        public const int ADD_TO_CART_FAIL = -503;
        public const int ERROR_INVALID_PHONE = -504;
        public const int WARRANTY_CODE_USED = -505;
        public const int WARRANTY_CODE_NOT_EXIST = -506;
        public const int ERROR_INVALID_POINT = -507;
        public const int ERROR_MINPOINT_DRAW = -536;


        public const int ERROR = 0;
        public const int SUCCESS = 1;
        public const int DUPLICATE_PHONE = 2;

        public const int NOT_FOUND = 404;
        public const int DATA_NOT_FOUND = 400;
        public const string DATA_NOT_FOUND_MESSAGE = "Kiểm tra dữ liệu đầu vào";
        public const int DEVICE_ID_NOT_FOUND = 300;
        // khong duoc phep
        public const int UNAUTHORIZED = 401;

        //đã tồn tại yêu cầu đăng ký đại lý
        public const int EXISTING_REQUEST_AGENT = 303;

        public const int STATUS_RUNNING = 1;
        public const int STATUS_REQUEST_WAITING = 0;
        public const int STATUS_REQUEST_SUCCESS = 1;
        public const int STATUS_REQUEST_CANCEl = 2;
        public const int STATUS_REQUEST_DELETE = 3;
        // Type đổi quà
        public const int TYPE_POINT_SAVE = 1;
        public const int TYPE_POINT_GIVE = 2;
        public const int TYPE_POINT_RECEIVE = 3;
        public const int TYPE_POINT_RECEIVE_GIFT = 4;
        public const int TYPE_CARD = 6;

        //type noti web

        //type noti order
        public const int TYPE_NOTI_ORDER = 1;
        //Gửi yêu cầu rút điểm
        public const int TYPE_NOTI_REQUEST_DRAW_POINT = 2;
        //Type Loại ví
        public const int TYPE_WALLET_POINT = 1;
        public const int TYPE_POINTS = 2;

        //Các type yêu cầu về điểm
        //Type yêu cầu rút điểm
        public const int TYPE_REQUEST_DRAW_POINT = 1;
        //Type tặng điểm
        public const int TYPE_REQUEST_GIFT_POINT = 2;
        //Type thu phí sử dụng app
        public const int TYPE_FEE_USE_APP = 3;
        //type Cộng điểm
        public const int TYPE_ADD_POINT = 4;
        //Type được tặng điểm
        public const int TYPE_AWARDED_POINT = 5;
        //Cộng điểm hàng ngày từ hệ thống
        public const int TYPE_SYSTEM_ADD_POINT = 6;//Nếu là ví point thì là tích điểm còn ví tích điểm thì là trừ điểm
        //Type trừ điểm khi mua hàng
        public const int TYPE_MINUS_POINT_ORDER = 7;
        //Type hoàn điểm từ đơn hàng
        public const int TYPEADD_POINT_FROM_BILL = 8;
        //type Cộng điểm vi tich
        public const int TYPE_ADD_POINT_RANKING = 9;
        //Type cộng điểm khi giới thiệu sản phẩm
        public const int TYPE_ADD_POINT_PRODUCT_INTRODUCTION = 10;
        //Type chuyển điểm ví V sang ví tích điểm
        public const int TYPE_CONVERT_POINT_V_TO_POINT_RANKING = 11;
        //Type chuyển điểm ví tích điểm sang ví V 
        public const int TYPE_CONVERT_POINT_RANKING_TO_POINT_V = 12;
        //Type cộng điểm ví V khi đăng ký tài khoản
        public const int TYPE_ADD_POINT_NEW_MEMBER = 13;
        //Type cộng điểm ví V khi giới thiệu thành viên mới
        public const int TYPE_ADD_POINT_NEW_MEMBER_REF = 14;
        //Type nạp tiền ví Point VNPAY
        public const int TYPE_ADD_POINT_VNPAY = 15;


        //Type ví điểm tích
        public const int TYPE_POINT_RANKING = 1;
        //Type ví point
        public const int TYPE_POINT = 2;        
        //Type ví V
        public const int TYPE_POINT_V = 3;

        public const string TYPE_POINT_SAVE_ICON = "https://image.flaticon.com/icons/png/512/2333/2333325.png";
        public const string TYPE_POINT_GIVE_ICON = "https://image.flaticon.com/icons/png/512/1643/1643098.png";
        public const string TYPE_POINT_RECEIVE_ICON = "https://image.flaticon.com/icons/png/512/1643/1643146.png";
        public const string TYPE_POINT_RECEIVE_GIFT_ICON = "https://image.flaticon.com/icons/png/512/2308/premium/2308818.png";
        public const string TYPE_ADD_POINT_ICON = "https://image.flaticon.com/icons/png/512/1415/premium/1415467.png";
        public const string TYPE_CARD_ICON = "https://image.flaticon.com/icons/png/512/1041/premium/1041555.png";


        public const int SIZE_CODE = 8;
        public const int LENGTH_AGENT_CODE = 8;
        public const int MIN_NUMBER = 100000;
        public const int MAX_NUMBER = 999999;

        // Status warranty 
        public const int W_STATUS_ACTIVE = 1;
        public const int W_STATUS_NO_ACTIVE = 2;
        public const int W_STATUS_ERROR = 3;

        // cách kiểu tích điểm
        public const int WARRANTY = 2;
        public const int PRODUCT = 1;

        //
        public const int MESS_BY_CUS = 1;
        public const int MESS_BY_ADMIN = 2;
        //
        public const int HISTORY_TYPE_ADD_ANOTHER = 3;
        public const int HISTORY_TYPE_ADD_PRODUCT = 1;
        public const int HISTORY_TYPE_ADD_WARRANTY = 2;
        //
        // danh mục tin tức
        public const int NEWS_TYPE_NEWS = 1;
        public const string NEWS_TYPE_NEWS_STRING = "Tin tức";
        // danh mục sự kiện
        public const int NEWS_TYPE_EVENT = 2;
        public const string NEWS_TYPE_EVENT_STRING = "Sự kiện";
        // danh mục banner
        public const int NEWS_TYPE_BANNER = 3;
        public const string NEWS_TYPE_BANNER_STRING = "Banner";
        // danh mục quảng cáo
        public const int NEWS_TYPE_ADS = 4;
        public const string NEWS_TYPE_ADS_STRING = "Quảng cáo";


        public const string COMMENT_HISTORY_ADD_POINT = "Tích điểm khuyễn mãi";
        public const string COMMENT_HISTORY_SAVE_POINT_PRODUCT = "Kích hoạt bảo hành sản phẩm";
        // link check access Token
        public const string LINK_URL_FACEBOOK = "https://graph.facebook.com/me?fields=name,picture.height(960).width(960)&access_token=";
        public const string LINK_URL_GOOGLE_MAIL = "https://www.googleapis.com/plus/v1/people/me?access_token=";
        public const string LINK_URL_GOOGLE_MAIL2 = "https://www.googleapis.com/plus/v2/people/me?access_token=";
        // Telecom
        public const int MAX_TELECOM = 4;
        public const string URL_VIETTEL = "https://upload.wikimedia.org/wikipedia/vi/thumb/e/e8/Logo_Viettel.svg/800px-Logo_Viettel.svg.png";
        public const string URL_MOBIPHONE = "https://upload.wikimedia.org/wikipedia/commons/d/de/Mobifone.png";
        public const string URL_VINAPHONE = "https://lozimom.com/wp-content/uploads/2017/04/vinaphone-logo.png";
        public const string URL_VIETNAMMOBILE = "https://upload.wikimedia.org/wikipedia/vi/thumb/a/a8/Vietnamobile_Logo.svg/1280px-Vietnamobile_Logo.svg.png";
        public const int TELECOM_TYPE_GIFT = 0;
        public const int TYPE_VIETTEL = 1;
        public const int TYPE_MOBIPHONE = 2;
        public const int TYPE_VINAPHONE = 3;
        public const int TYPE_VIETNAMMOBILE = 4;
        public const string TYPE_VIETTEL_STRING = "Viettel";
        public const string TYPE_MOBIPHONE_STRING = "Mobiphone";
        public const string TYPE_VINAPHONE_STRING = "Vinaphone";
        public const string TYPE_VIETNAMMOBILE_STRING = "VietnamMobile";
        public const string URL_FIRST = "https://graph.facebook.com/";
        public const string URL_LAST = "/picture?type=large&redirect=true&width=250&height=250";
        public const int STATUS_PRODUCT_ACTIVE = 1;
        public const int STATUS_PRODUCT_NO_ACTIVE = 2;
        public const string STATUS_PRODUCT_ACTIVE_STRING = "Đã sử dụng";
        public const string STATUS_PRODUCT_NO_ACTIVE_STRING = "Chưa sử dụng";

        public const int STATUS_REQUEST_PENDING = 0;
        public const int STATUS_REQUEST_ACCEPTED = 1;
        public const int STATUS_REQUEST_CANCEL = 2;
        public const string STATUS_REQUEST_PENDING_STRING = "Chờ xác nhận";
        public const string STATUS_REQUEST_ACCEPTED_STRING = "Đã xác nhận";
        public const string STATUS_REQUEST_CANCEL_STRING = "Hủy";

        public const int TYPE_REQUEST_GIFT = 1;
        public const int TYPE_REQUEST_VOUCHER = 2;
        public const int TYPE_REQUEST_CARD = 3;

        public const string TYPE_REQUEST_GIFT_STRING = "Quà tặng";
        public const string TYPE_REQUEST_VOUCHER_STRING = "Voucher";
        public const string TYPE_REQUEST_CARD_STRING = "Thẻ cào";

        public const int TYPE_GIFT_GIFT = 1;
        public const int TYPE_GIFT_VOUCHER = 2;
        public const int TYPE_GIFT_CARD = 3;

        public const int STATUS_GIFT_ACTIVE = 1;
        public const int STATUS_GIFT_PAUSE = 0;
        public const int STATUS_GIFT_CANCEL_AND_ADD = 2;
        public const int STATUS_GIFT_CANCEL = 3;

        public const int NO_ACTIVE_DELETE = 0;
        public const int MAX_ROW_IN_LIST_WEB = 20;
        public const bool BOOLEAN_TRUE = true;
        public const bool BOOLEAN_FALSE = false;
        public const int DUPLICATE_NAME = 2;

        public const int QRCODE_TYPE_PRODUCT = 1;
        public const int QRCODE_TYPE_WARRANTY = 2;

        public const int STATUS_CARD_ACTIVE = 1;
        public const int STATUS_CARD_NO_ACTIVE = 2;
        public const int ERROR_DATE = 3;


        //public const string App_ID = "6bb1c7ed-5ca7-4576-9fea-cd40523a2de8";
        //public const string Channel_ID = "a4509146-84c0-4deb-9c72-ca1397173a04";
        //public const string Rest_API_KEY = "M2M5NGNjZGYtNzYzZi00YTNiLWE5MTAtYjIyZDk0ZTkzYjg3";
        //public const string URL_Notification_POST = "https://onesignal.com/api/v1/notifications";

        public const string TICHDIEM_NOTI = "Tích điểm triệu đô thông báo";

        public const string REQUIRE_FIELD = "Vui lòng không để trống!";
        public const string PUSH_NOTIFY_ONESIGNAL = "https://onesignal.com/api/v1/notifications";
        public const string INVALID_NUMBER = "Chỉ được phép nhập số!";
        public const string LOGIN_FAIL = "Vui lòng nhập đúng số điện thoại";
        public const float KeyA = 11;
        public const float KeyB = 87;
        public const float KeyC = 48;
        public const string ICON_GIFT = "http://icons.iconarchive.com/icons/lovuhemant/merry-christmas/256/Gift-icon.png";
        public const string ICON_MONEY = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT1TeKukaS-4k9SggMYEP4VpmsTMwVUlhDeTIOSiNyBbhrVATdv";
        public const string ICON_BONUS = "https://image.flaticon.com/icons/png/512/536/536089.png";
        public const string ICON_ORDER_CANCEL = "https://image.flaticon.com/icons/png/512/172/172120.png";
        public const string ICON_ORDER_CONFIRM = "https://image.flaticon.com/icons/png/512/262/262807.png";
        public const string ICON_ORDER_PAID = "https://image.flaticon.com/icons/png/512/172/172165.png";
        public const string ICON_NOTIFY_TYPE_POINT_GIVE = "https://image.flaticon.com/icons/png/512/1643/1643098.png";
        public const string ICON_NOTIFY_TYPE_POINT_RECEIVE = "https://image.flaticon.com/icons/png/512/1643/1643146.png";
        public const string ICON_NOTIFY_TYPE_CREATE_NEWS = "https://image.flaticon.com/icons/png/512/1598/1598991.png";
        public const string ICON_NOTIFY_TYPE_REQUEST = "https://image.flaticon.com/icons/png/512/2308/premium/2308818.png";
        public const string ICON_BULLHORN = "https://image.flaticon.com/icons/png/512/172/172173.png";

        public const int NOTIFY_TYPE_ORDER_REFUSE = 5;
        public const int NOTIFY_TYPE_ORDER_CANCEL = 1;
        public const int NOTIFY_TYPE_ORDER_CONFIRM = 2;
        public const int NOTIFY_TYPE_ORDER_PAID = 3;
        public const int NOTIFY_TYPE_POINT_GIVE = 4;
        public const int NOTIFY_TYPE_POINT_RECEIVE = 5;
        public const int NOTIFY_TYPE_CREATE_NEWS = 6;
        public const int NOTIFY_TYPE_CANCEL_AGENT = 7;

        public const int TYPE_SEND_CUSTOMER = 1;
        public const int TYPE_SEND_AGENCY = 2;
        public const int TYPE_SEND_ALL = 0;

        public const int SEND_NOTIFY = 1;

        public const int SHOW_HOME_SCREEN = 1;
        public const int NO_SHOW_HOME_SCREEN = 0;

        public const int STATUS_NEWS_ACTIVE = 1;
        public const int STATUS_NEWS_DRAFT = 0;
        public const int UPDATE_NEWS_DEFAULT = 1;
        public const int UPDATE_NEWS_POST = 0;
        public const int LENGTH_QR_HASH = 15;
        public const int EXISTING = 2;
        public const int CAN_NOT_DELETE = 2;
        public const int ROLE_USER_ORDER = 3;
        public const int ROLE_USER = 2;
        public const int ROLE_ADMIN = 1;
        public const int ROLE_CUSTOMER = 1;
        public const int ROLE_AGENT = 2;
        public const int NOT_ADMIN = 3;
        public const int WRONG_PASSWORD = 2;
        public const int FAIL_LOGIN = 2;
        public const int TYPE_REQUEST_NOTIFY = 4;
        //public const int TYPE_ORDER_NOTIFY = 7;
        
        public const int MAX_PEOPLE = 90;

        //Số điểm tối thiểu để nạp vào ví Point VNpay
        public const int CHARGE_MIN_POINT = 10;
        public const string INVALID_CHARGE_MIN_POINT = "Số điểm mỗi lần nạp phải từ 10 điểm trở lên";
        public const int CHARGE_MAX_POINT = 10000;
        public const string INVALID_CHARGE_MAX_POINT = "Số điểm mỗi lần nạp phải từ 10000 điểm trở xuống";

        //Số điểm tối thiểu để có thể rút
        public const string MinPoint = "MinPoint";
        //Số điểm tối thiểu để được rút 
        public const string AddPoint = "AddPoint";
        public const string KHONG_DU_DIEM_DOI_QUA_MESSAGE = "Số điểm khả dụng của bạn không đủ để đổi quà. Vì số điểm tối thiểu trong tài khoản phải duy trì ở mức ";
        public const string DA_DUNG_HET_SO_DIEM_TRONG_NGAY_MESSAGE = "Bạn không thể đổi quà vì đã dùng hết điểm trong 1 ngày. Số điểm có thể dùng trong 1 ngày bằng ";
        public const string GIFT_REQUEST_NOT_FOUND_MESSAGE = "Đổi quà thất bại. Hệ thống không tim thấy thông tin món quà bạn yêu cầu.";
        public const string CONFIRM_FAIL = "Hệ thống đã hết thẻ. Vui lòng chọn thẻ khác";
        public const string CREATE_REQUEST_SUCCESS_MESSAGE = "Yêu cầu đã được gửi thành công. Vui lòng chờ phản hồi từ hệ thống";
        public const string DIEM_DOI_QUA_LON_HON_DIEM_MINH_MESSAGE = "Số điểm của bạn không đủ để đổi quà";
        public const string MESSAGE_ERROR_NOT_ENOUGH_CONDITION_DRAW_POINT = "Số điểm của bạn chưa đủ để yêu cầu rút điểm";
        public const string MESSAGE_ERROR_MAX_POINT_DRAW = "Số điểm yêu cầu không được vượt quá tổng điểm hiện có";
        public const string MESSAGE_ERROR_CONDITION_MOVE_POINT = "Số điểm muốn chuyển phải lớn hơn 0";

        public const int KHONG_DU_DIEM_DOI_QUA = -201;
        public const int DA_DUNG_HET_SO_DIEM_TRONG_NGAY = -202;
        public const int GIFT_REQUEST_NOT_FOUND = -203;
        public const int DIEM_DOI_QUA_LON_HON_DIEM_MINH = -204;

        //Card
        public const string IMPORT_CARD_VIETTEL = "viettel";
        public const string IMPORT_CARD_MOBIPHONE = "mobi";
        public const string IMPORT_CARD_VINAPHONE = "vina";
        public const string IMPORT_CARD_VIETNAMOBILE = "vnmobile";
        public const int TELECOMTYPE_VIETTEL = 1;
        public const int TELECOMTYPE_MOBIPHONE = 2;
        public const int TELECOMTYPE_VINAPHONE = 3;
        public const int TELECOMTYPE_VIETNAMOBILE = 4;
        public const int ERROR_IMPORT_DUPLICATE = 0;

        // status, type trong bảng Orders
        public const int TYPE_CART = 1;
        public const int TYPE_ORDER = 2;

        public const int MIX_COLOR = 1;
        public const int NO_MIX_COLOR = 0;


        public const int STATUS_CART_PENDING = 0;
        public const int STATUS_CART_BOUGHT = 1;

        // status order tuylips
        public const int STATUS_ORDER_CANCEL = 0; //Hủy
        public const int STATUS_ORDER_PENDING = 1; //Chờ xác nhận
        public const int STATUS_ORDER_PAID = 2; //đã thanh toán
        public const int STATUS_ORDER_CONFIRM = 3; //xác nhận
        public const int STATUS_ORDER_REFUSE = 4; //từ chối
        public const double PARAM_PLUS = 0.8;


        // Status for all
        public const int STATUS_ACTIVE = 1;
        public const int STATUS_NO_ACTIVE = 0;

        // Not Found
        public const int PHONE_NOT_FOUND = -1;

        // ExcelFile Error
        public const int FILE_NOT_FOUND = -1;
        public const int FILE_DATA_DUPLICATE = 0;
        public const int FILE_IMPORT_SUCCESS = 1;
        public const int FILE_FORMAT_ERROR = -2;
        public const int FILE_EMPTY = -3;
        public const int IMPORT_ERROR = -4;
        public const int MIN_LENGTH_VALIDATE = -5;
        public const int DATA_ERROR = -6;

        // check Length Card
        public const int MAX_LENGTH_CODE = -7;
        public const int MAX_LENGTH_SERI = -8;
        public const int CODE_EQUALS_SERI = -9;

        // type MemberPointHistory
        public const int HISPOINT_TICH_DIEM = 1;
        public const int HISPOINT_TANG_DIEM = 2;
        public const int HISPOINT_DUOC_TANG_DIEM = 3;
        public const int HISPOINT_DOI_QUA = 4;
        public const int HISPOINT_HE_THONG_CONG_DIEM = 5;
        public const int HISPOINT_DOI_THE = 6;
        public const int HISTORY_POINT_CANCEL_REQUEST = 7;

        //
        public const int ACTIVE_AGENT = 1;

        
        public const string SEND_TIME = "null";
        public const string USER = "dekko";
        public const string PASS = "Vmg@123456";
        public const string ALIAS = "DEKKO GROUP";
        public const string LINK_MESS = "http://brandsms.vn:8018/VMGAPI.asmx/BulkSendSms?";
        public const string CONTENT_MESS = "Cam on quy khach da dang ky. Ma xac nhan cua quy khach la :";

        //Nam ta 
        public const string MESSAGE_INSERT_SUCCESS = "Thêm mới dữ liệu thành công!";
        public const string MESSAGE_UPDATE_SUCCESS = "Cập nhật dữ liệu thành công!";

    }
}
