import React, { Component } from 'react'
import {
    View,
    Text,
    SafeAreaView,
    TouchableOpacity,
    FlatList,
    Dimensions,
    RefreshControl,
    Platform,
    ScrollView
} from 'react-native'
import { connect } from 'react-redux'
import {
    Loading,
    Block,
    DCHeader,
    Error,
    FastImage,
    NumberFormat,
    requestLogin,
    Button,
    Empty
} from "../../components";
import * as Theme from "../../constants/Theme";
import I18n from "../../i18n/i18n";
import { getImages } from "../../redux/actions";
import reactotron from 'reactotron-react-native';
import ImagePicker from 'react-native-image-picker';
import ImageResizer from 'react-native-image-resizer';
import NavigationUtil from "../../navigation/NavigationUtil";
import { Icon } from "react-native-elements";
import { deleteImage, uploadImage } from "../../constants/Api";
import Toast, { BACKGROUND_TOAST } from "../../utils/Toast";
import { showConfirm, showMessages } from "../../utils/Alert";

const maxWidth = Dimensions.get('screen').width;
const maxHeight = Dimensions.get('screen').height;


export class UpdateImage extends Component {
    constructor(props) {
        super(props);
        // const { imageState } = this.props
        // reactotron.log(imageState.data,"ưqeweqw")
        // this.state = {
        //     listImage: imageState.data.length == 0 ? [] : this._arrayImage(imageState.data)
        // };
        this.state = {
            isLoading: false,
            image: null
        }
    }
    // _arrayImage(data) {
    //     var arrImage=[]
    //     for (let index = 0; index < data.length; index++) {
    //         arrImage.push(data[index].image);
    //     }
    //     return arrImage
    // }
    componentDidMount() {
        this._getdata()
    }
    _getdata() {
        this.props.getImages()
    }
    _deleteImage = async (id) => {
        this.setState({
            ...this.state,
            isLoading: true
        });
        try {
            var listID = []
            reactotron.log(listID.concat(id), "listID")
            const response = await deleteImage(listID.concat(id));
            Toast.show(response.message, BACKGROUND_TOAST.SUCCESS);
            this.setState({
                ...this.state,
                isLoading: false,
            });
            this.props.getImages();
        } catch (error) {
            if (error.message == "Network Error") {
                Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
            }
            this.setState({
                ...this.state,
                isLoading: false
            });
            //   reactotron.log(error);
        }
    };
    _uploadImage = async (image) => {
        // reactotron.log("loadImage")
        if (image == null) {
            Toast.show("Bạn chưa chọn ảnh", BACKGROUND_TOAST.FAIL);
        } else {
            this.setState({
                ...this.state,
                isLoading: true
            });
            try {
                const response = await uploadImage(image);
                //  reactotron.log(response)
                Toast.show(response.message, BACKGROUND_TOAST.SUCCESS);
                this.setState({
                    ...this.state,
                    isLoading: false,
                    image: null
                });
                this.props.getImages();
            } catch (error) {
                reactotron.log(error, "error");
                if (error.message == "Network Error") {
                    Toast.show(I18n.t("network_err"), BACKGROUND_TOAST.FAIL);
                }
                this.setState({
                    ...this.state,
                    isLoading: false
                });
                //   reactotron.log(error);
            }
        }
    };
    _renderBody() {
        const { imageState } = this.props
        // reactotron.log(imageState.data,"data")
        // reactotron.log(this.state.listImage,"listImage")
        if (imageState.isLoading) return <Loading />
        if (imageState.error) return
        <Error
            onPress={() => {
                this._getdata();
            }} />
        // if (imageState.data.length == 0) {
        //     return <Empty onRefresh={() => this.props._getdata()} title={"Chưa có ảnh"} />;
        // }
        return (
            <FlatList
                contentContainerStyle={{
                    // flex: 1,
                    paddingBottom: 20,
                    //  marginTop:10
                }}
                showsVerticalScrollIndicator={false}
                data={imageState.data}
                refreshControl={
                    <RefreshControl
                        refreshing={imageState.refreshing}
                        onRefresh={() => this.props.getImages()}
                    />
                }
                keyExtractor={(item, index) => index.toString()}
                renderItem={({ item, index }) => {
                    return (
                        // <TouchableOpacity
                        //     onPress={() => {
                        //         NavigationUtil.navigate(SCREEN_ROUTER.IMAGE_VIEWER, {
                        //             images: {
                        //                 listImage: imageState.data,
                        //                 index: index
                        //             }
                        //         });
                        //     }}
                        // >
                        <View style={{ marginTop: 20 }}>
                            <FastImage
                                style={{
                                    width: Theme.dimension.width * 0.9,
                                    height: 200,
                                    margin:10
                                }}
                                source={{ uri: item.image }}
                                resizeMode="cover"
                            />
                            <Icon
                                containerStyle={{
                                    position: "absolute",
                                    top: 0,
                                    right: 0,
                                    // borderColor: Theme.colors.text_gray,
                                    // borderWidth: 0.5,
                                    borderRadius: 15,
                                    backgroundColor:Theme.colors.text_gray
                                }}
                                onPress={() => {
                                    // reactotron.log(item.id)
                                    showConfirm(I18n.t('confirm'), 'Bạn muốn xóa ảnh này',
                                        () => this._deleteImage(item.id))
                                }}
                                type="material"
                                name="clear"
                                size={25}
                                color={Theme.colors.white}
                            />
                        </View>
                        // </TouchableOpacity>
                    );
                }}
            />
        )
    }
    render() {
        return (
            <Block>
                <DCHeader title={I18n.t("update_image_agent")} />
                <SafeAreaView style={[Theme.styles.container, { alignItems: "center" }]}>
                    <TouchableOpacity style={{
                        width: Theme.dimension.width * 0.9,
                        height: 130,
                        marginVertical: 10
                    }}
                        onPress={() => {
                            this._pickImage()
                        }}
                    >
                        <FastImage
                            style={{ flex: 1 }}
                            source={require("../../assets/images/ic_add_image.png")}
                        />
                    </TouchableOpacity>
                    {this._renderBody()}
                </SafeAreaView>
            </Block>
        )
    }
    _pickImage = async () => {
        const options = {};
        try {
            ImagePicker.launchImageLibrary(options, response => {
                if (response.didCancel) {
                    // console.log("User cancelled photo picker");
                } else if (response.error) {
                    // console.log("ImagePicker Error: ", response.error);
                } else if (response.customButton) {
                    // console.log("User tapped custom button: ", response.customButton);
                } else {
                    reactotron.log(JSON.stringify(response))
                    const data = new FormData();
                    data.append("img", {
                        name: response.fileName,
                        type: response.type,
                        uri:
                            Platform.OS === "android"
                                ? response.uri
                                : response.uri.replace("file://", "")
                    });
                    this.setState({
                        ...this.state,
                        // avatar_img: response.uri,
                        image: data
                    })

                }
                this._uploadImage(this.state.image)
            })
        } catch (error) {
            reactotron.log('ImagePicker Err', error);
        }
    }
}

const mapStateToProps = (state) => ({
    imageState: state.imageReducer
})

const mapDispatchToProps = {
    getImages
}

export default connect(mapStateToProps, mapDispatchToProps)(UpdateImage)
