import { NavigationActions, StackActions } from "react-navigation";

let _navigator; // eslint-disable-line

function setTopLevelNavigator(navigatorRef) {
  _navigator = navigatorRef;
}

function navigate(routeName, params) {
  _navigator.dispatch(
    NavigationActions.navigate({
      routeName,
      params
    })
  );
}
function goBack() {
  // _navigator.goBack();
  _navigator.dispatch(NavigationActions.back());
}

function popToTop() {
  _navigator.dispatch(StackActions.popToTop());
}

function replace(routeName, params) {
  _navigator.dispatch(StackActions.replace({ routeName, params }));
}

function reset(index, actions) {
  _navigator.dispatch(
    StackActions.reset({ index, actions })
  )
}

function pop(count) {
  _navigator.dispatch(
    StackActions.pop({
      n: count || 1
    })
  );
}
function push(routeName, params) {
  _navigator.dispatch(
    StackActions.push({
      routeName,
      params
    })
  );
}


export default {
  navigate,
  setTopLevelNavigator,
  goBack,
  popToTop,
  replace,
  push,
  reset,
  pop
};
