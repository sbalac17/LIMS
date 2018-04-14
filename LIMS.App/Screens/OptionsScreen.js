import React from 'react';
import { StyleSheet, View, FlatList } from 'react-native';
import { Divider, Button, Text } from 'react-native-elements';
import { clearSavedToken } from '../Authentication/Token';

export default class OptionsScreen extends React.Component {
    static navigationOptions = {
        title: 'Options',
        drawerLabel: 'Options'
    };

    constructor(props) {
        super(props);

        const { navigate } = this.props.navigation;
        this.navigate = navigate;
    }
    
    render() {

        return (
            <View style={styles.container}>
                <Button title='Logout' onPress={() => this._logout()} />
            </View>
        );
    }

    async _logout() {
        await clearSavedToken();
        this.navigate('Auth');
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
        alignItems: 'stretch',
    }
});
