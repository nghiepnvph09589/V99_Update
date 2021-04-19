import React, { PureComponent } from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import I18n from '../i18n/i18n'
import * as theme from '../constants/Theme'

class PrimaryButton extends PureComponent {

    render() {
        const {
            title, background, textColor,
            border, style, onPress,
            ...props } = this.props
        return (
            <TouchableOpacity
                onPress={onPress}
                style={[
                    {
                        justifyContent: 'center',
                        alignItems: 'center',
                        alignSelf: 'center',
                        width: '90%',
                        height: 45,
                        backgroundColor: background ? background : theme.colors.primary,
                        borderRadius: 5,
                        borderWidth: border ? 1 : 0,
                        borderColor: border && border,
                    }, style
                ]} {...props}>
                <Text style={[theme.fonts.semibold18, {
                    color: textColor ? textColor : theme.colors.white
                }]}>{title}</Text>
            </TouchableOpacity>
        );
    }
}

export default PrimaryButton;
