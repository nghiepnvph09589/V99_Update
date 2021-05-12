import React, { Component } from "react";
import {
  Text,
  View,
  Modal,
  Image,
  TouchableOpacity,
  ActivityIndicator,
  StatusBar
} from "react-native";
import ImageViewer from "react-native-image-zoom-viewer";
import NavigationUtil from "../navigation/NavigationUtil";
import reactotron from "reactotron-react-native";
import { FastImage } from "../components";

export default class ImageViewerScreen extends Component {
  render() {
    const item = this.props.navigation.getParam("images", {});
    // console.log(item)
    const images = Array.from(item.listImage, item => {
      return {
        url: item
      };
    });

    return (
      <Modal
        visible
        transparent
        onRequestClose={() => {
          NavigationUtil.goBack();
        }}
      >
        <ImageViewer
          imageUrls={images}
          enableSwipeDown
          index={item.index}
          swipeDownThreshold={200}
          onSwipeDown={() => {
            NavigationUtil.goBack();
          }}
        />
        <TouchableOpacity
          style={{
            position: "absolute",
            top: 35,
            left: 30
          }}
          onPress={NavigationUtil.goBack}
        >
          <FastImage
            style={{ height: 25, width: 25 }}
            source={require("../assets/images/ic_back.png")}
            resizeMode='contain'
          />
        </TouchableOpacity>
      </Modal>
    );
  }
}
