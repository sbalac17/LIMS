import React from 'react';
import { StyleSheet, View, FlatList } from 'react-native';
import { Divider, Button, Text } from 'react-native-elements';
import { tryGetSavedToken, clearSavedToken } from '../Authentication/Token';

export default class OptionsScreen extends React.Component {
    static navigationOptions = {
        title: 'Options',
        drawerLabel: 'Options'
    };

    constructor(props) {
        super(props);

        const { navigate } = this.props.navigation;
        this.navigate = navigate;

        this.state = {
            userName: '',
        };

        this.refresh();
    }
    
    render() {
        return (
            <View style={styles.container}>
                <Text h4>Logged in as:</Text>
                <Text>{this.state.userName}</Text>
                <Button title='Logout' containerViewStyle={styles.spacing}
                    onPress={() => this._logout()} />
            </View>
        );
    }

    async refreshImpl() {
        try {
            let token = await tryGetSavedToken();
            this.setState({ userName: token.userName });
        } catch(e) {
            console.error(e);
        }
    }

    async _logout() {
        await clearSavedToken();
        this.navigate('Auth');
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 15,
        backgroundColor: '#fff',
        alignItems: 'stretch',
    },
    spacing: {
        marginTop: 15,
    },
});
