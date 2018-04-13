import { NavigationActions } from 'react-navigation';

let savedNavigator = null;

export function getTopLevelNavigator() {
    return savedNavigator;
}

export function setTopLevelNavigator(navigator) {
    savedNavigator = navigator;
}

export function navigate(routeName, params) {
    savedNavigator.dispatch(NavigationActions.navigate({
        type: NavigationActions.NAVIGATE,
        routeName,
        params
    }));
}
