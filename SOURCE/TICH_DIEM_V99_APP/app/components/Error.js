import React, { Component } from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import I18n from '../i18n/i18n';
import * as Theme from '../constants/Theme'
import { Block, Icon } from './';

class Error extends Component {
    render() {
        const { error } = this.props
        return (
            <Block center middle>
                <TouchableOpacity
                    onPress={this.props.onPress}
                >
                    <Icon.MaterialIcons name='refresh' size={45} color='orange' />
                </TouchableOpacity>
                <Text style={{
                    textAlign: 'center',
                    color: Theme.colors.undoactionButtonBg
                }}>{error ? error : 'Vui lòng thử lại'}</Text>
            </Block>
        );
    }
}

export default Error;
