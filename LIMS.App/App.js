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

import SamplesListScreen from './Screens/Samples/SamplesListScreen';
import SamplesDetailsScreen from './Screens/Samples/SamplesDetailsScreen';
import SamplesEditScreen from './Screens/Samples/SamplesEditScreen';
import SamplesDeleteScreen from './Screens/Samples/SamplesDeleteScreen';
import SamplesCreateScreen from './Screens/Samples/SamplesCreateScreen';

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

const SamplesStack = StackNavigator({
    SamplesList: { screen: SamplesListScreen },
    SamplesDetails: { screen: SamplesDetailsScreen },
    SamplesEdit: { screen: SamplesEditScreen },
    SamplesDelete: { screen: SamplesDeleteScreen },
    SamplesCreate: { screen: SamplesCreateScreen },
}, {
    initialRouteName: 'SamplesList',
    navigationOptions: {
        header: renderHeader
    }
});

const AppDrawer = DrawerNavigator({
    HomeStack: { screen: HomeStack },
    TestsStack: { screen: TestsStack },
    SamplesStack: { screen: SamplesStack },
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
