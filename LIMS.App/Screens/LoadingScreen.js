import React from 'react';
import { StyleSheet, Text, View, Button } from 'react-native';
import { tryGetSavedToken } from '../Authentication/Token';

export default class LoadingScreen extends React.Component {
    render() {
        this._checkAuthentication();

        return (
            <View style={styles.container}>
                <Text>Loading...</Text>
            </View>
        );
    }

    async _checkAuthentication() {
        const { navigate } = this.props.navigation;

        const token = await tryGetSavedToken();
        navigate(token ? 'App' : 'Auth');
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
        alignItems: 'center',
        justifyContent: 'center',
    },
});
