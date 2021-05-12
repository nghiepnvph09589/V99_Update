import React, { Component } from 'react';
import { View, Text, Image, ScrollView, RefreshControl } from 'react-native';
import * as theme from '../constants/Theme';
import { Block, FastImage } from '.'

class Empty extends Component {
    render() {
        const { title, description, urlImage, onRefresh } = this.props

        return (
            <Block>
                <ScrollView
                    showsVerticalScrollIndicator={false}
                    contentContainerStyle={{ flexGrow: 1 }}
                    style={{
                        backgroundColor: theme.colors.primary_background,
                    }}
                    refreshControl={
                        <RefreshControl
                            refreshing={false}
                            onRefresh={onRefresh}
                        />}>
                    <Block center middle paddingHorizontal='5%'>
                        <FastImage
                            source={urlImage && urlImage || require('../assets/images/img_empty.png')}
                            style={{
                                resizeMode: 'contain',
                                width: theme.dimension.width / 3,
                                height: theme.dimension.width / 3,
                            }} />
                        <Text style={[
                            theme.fonts.robotoRegular14,
                            {
                                marginTop: 8,
                                color: theme.colors.black1
                            }
                        ]}>{title && title || 'Danh sách trống'}</Text>
                        <Text style={[
                            theme.fonts.robotoRegular14,
                            {
                                marginTop: 10,
                                marginBottom: 10,
                                color: theme.colors.black2,
                                textAlign: 'center',
                            }
                        ]}>{description}</Text>
                    </Block>
                </ScrollView>
            </Block>
        );
    }
}

export default Empty;
