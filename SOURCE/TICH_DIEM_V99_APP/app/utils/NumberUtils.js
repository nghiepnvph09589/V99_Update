export function formatNumber(num) {
    if (num === null || num === undefined || num == 0 || num == NaN) return 0
    var result = num.toString().replace(/,/g, '')
    return result.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}

export const parseFloatComas = value => {
    if (!value) return 0
    return parseFloat(value.toString().replace(/,/g, ''))
}

export const parseIntComas = value => {
    if (!value) return 0
    return parseInt(value.toString().replace(/,/g, ''))
}

export function formatNumberToString(valu) {
    if (!value) return ""
    return value.toString().split(",").join("")
  }
export function formatPrice(value) {
    if (Platform.OS === "androi") {
      require("intl") // import intl object
      require("intl/locale-data/jsonp/en") // load the required locale details
    }
    return new Intl.NumberFormat("en-ES").format(formatNumberToString(value))
  }