import React from 'react';
import { View, StatusBar } from 'react-native';
import { StackNavigator, DrawerNavigator, SwitchNavigator } from 'react-navigation';
import { Header } from 'react-native-elements';
import * as NavigationService from './NavigationService';
import HomeScreen from './Screens/HomeScreen';
import LoginScreen from './Screens/LoginScreen';
import LoadingScreen from './Screens/LoadingScreen';
import TestsListScreen from './Screens/Tests/TestsListScreen';
import TestsDetailsScreen from './Screens/Tests/TestsDetailsScreen';
import TestsEditScreen from './Screens/Tests/TestsEditScreen';
import TestsDeleteScreen from './Screens/Tests/TestsDeleteScreen';
import TestsCreateScreen from './Screens/Tests/TestsCreateScreen';

function renderHeader(headerProps) {
    const options = headerProps.getScreenDetails(headerProps.scene).options;
    return (
        <Header
            centerComponent={{ text: options.title, style: { color: '#fff' } }} />);
}

const HomeStack = StackNavigator({
    Home: { screen: HomeScreen },
}, {
    initialRouteName: 'Home',
    navigationOptions: {
        header: renderHeader
    }
});

const TestsStack = StackNavigator({
    TestsList: { screen: TestsListScreen },
    TestsDetails: { screen: TestsDetailsScreen },
    TestsEdit: { screen: TestsEditScreen },
    TestsDelete: { screen: TestsDeleteScreen },
    TestsCreate: { screen: TestsCreateScreen },
}, {
    initialRouteName: 'TestsList',
    navigationOptions: {
        header: renderHeader
    }
});

const AppDrawer = DrawerNavigator({
    HomeStack: { screen: HomeStack },
    TestsStack: { screen: TestsStack },
}, {
    initialRouteName: 'HomeStack'
});

const AuthStack = StackNavigator({
    Login: { screen: LoginScreen }
}, {
    initialRouteName: 'Login'
});

let TopLevelNavigator = SwitchNavigator({
    Loading: LoadingScreen,
    App: AppDrawer,
    Auth: AuthStack
}, {
    initialRouteName: 'Loading'
});

export default class App extends React.Component {
    render() {
        return (
            <TopLevelNavigator ref={NavigationService.setTopLevelNavigator} />
        );
    }
}
