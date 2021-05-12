import React, { Component } from 'react'
import { Text, View, Keyboard } from 'react-native'
import ScrollableTabView, { DefaultTabBar, ScrollableTabBar } from 'react-native-scrollable-tab-view'
import * as theme from '../constants/Theme'
import PropTypes from "prop-types"

export class ScrollableTab extends Component {

    static propTypes = {
        scrollableTabBar: PropTypes.bool,
    };
    static defaultProps = {
        scrollableTabBar: false,
    };

    render() {
        const { children, scrollableTabBar, ...props } = this.props
        return (
            <ScrollableTabView
                style={{
                    borderColor: theme.colors.border,
                }}
                tabBarBackgroundColor={theme.colors.white}
                tabBarPosition='top'
                tabBarActiveTextColor={theme.colors.primary}
                tabBarInactiveTextColor={theme.colors.black1}
                tabBarUnderlineStyle={{
                    height: 2,
                    backgroundColor: theme.colors.primary
                }}
                renderTabBar={() =>
                    scrollableTabBar ?
                        <ScrollableTabBar />
                        :
                        <DefaultTabBar
                            style={{
                                alignSelf: 'center',
                                paddingTop: 8,
                            }} />
                }
                tabBarTextStyle={theme.fonts.semibold18}
                {...props}>
                {children}
            </ScrollableTabView>
        )
    }
}

export default ScrollableTab
