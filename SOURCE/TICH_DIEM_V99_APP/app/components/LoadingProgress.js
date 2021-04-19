import React from 'react';
import { Modal, View, Text, ActivityIndicator } from 'react-native';
import * as theme from '../constants/Theme'

const LoadingProgress = () => (
    // <ProgressDialog
    //     visible={true}
    //     title="Đang xử lý."
    //     message="Vui lòng đợi..."
    // />
    <Modal transparent>
        <View style={{
            flex: 1,
            justifyContent: 'center',
            alignItems: 'center',
            backgroundColor: theme.colors.modal
        }}>
            <View style={{
                backgroundColor: theme.colors.white,
                width: '80%',
                height: 120,
                justifyContent: 'center',
                alignItems: 'center',
                borderRadius: 10,
            }}>
                <ActivityIndicator size="large" color={theme.colors.primary} />
                <Text style={[theme.fonts.robotoRegular15, {
                    marginTop: 10
                }]}>Đang xử lý...</Text>
            </View>
        </View>
    </Modal>
);

export default LoadingProgress;