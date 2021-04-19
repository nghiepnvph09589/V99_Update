import { Linking, Platform } from 'react-native'

export const LINKING_TYPE = {
    CALL: 1,
    ZALO: 2,
    WEB: 3,
}


export default LinkingUtils = (type, value) => {
    switch (type) {
        case LINKING_TYPE.WEB:
            Linking.openURL(value)
            break;
        case LINKING_TYPE.CALL:
            let number = value.trim().toString();
            if (Platform.OS !== "android") {
                number = `telprompt:${number}`;
            } else {
                number = `tel:${number}`;
            }
            Linking.openURL(number);
            break;
        case LINKING_TYPE.ZALO:
            // const number = value.trim().toString();
            Linking.openURL(`https://zalo.me/${value.trim().toString()}`);
            break;
        default:
            break;
    }
}