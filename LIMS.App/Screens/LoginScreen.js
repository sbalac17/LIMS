import React from 'react';
import { StyleSheet, Text, TextInput, View, KeyboardAvoidingView } from 'react-native';
import { Button } from 'react-native-elements';
import AutoScrollingView from '../Components/AutoScrollingView';
import { createAccessToken } from '../Authentication/Token';

export default class LoginScreen extends React.Component {
    static navigationOptions = {
        title: 'Login',
        header: null
    };

    constructor(props) {
        super(props);

        this.state = { 
            username: '',
            password: '',
            working: false
        };
    }
    
    render() {
        return (
            <View style={styles.container}>
                <KeyboardAvoidingView contentContainerStyle={styles.scrollContainer} behavior="padding" keyboardVerticalOffset={0}>
                    <View style={styles.container}>
                        <Text style={styles.title}>LIMS</Text>
                        <Text style={styles.subtitle}>Centennial College</Text>

                        <TextInput placeholder="Username"
                            keyboardType="email-address"
                            value={this.state.username}
                            editable={!this.state.working}
                            onChangeText={text => this.setState({ username: text, password: this.state.password, working: this.state.working })}
                            style={styles.textbox} />

                        <TextInput placeholder="Password"
                            secureTextEntry={true}
                            value={this.state.password}
                            editable={!this.state.working}
                            onChangeText={text => this.setState({ username: this.state.username, password: text, working: this.state.working })}
                            style={styles.textbox} />

                        <Button title="Login"
                            disabled={this.state.working}
                            loading={this.state.working}
                            onPress={() => this.login()}
                            style={styles.submit} />
                    </View>
                </KeyboardAvoidingView>
            </View>
        );
    }

    async login() {
        const username = this.state.username;
        const password = this.state.password;

        const { navigate } = this.props.navigation;

        this.setState({ username, password, working: true });

        try {
            let token = await createAccessToken(username, password);
            navigate('App');
        } catch (e) {
            // TODO: show error message
            this.setState({ username, password, working: false });
        }
    }
}

const styles = StyleSheet.create({
    scrollContainer: {
        flex: 1,
        backgroundColor: '#fff',
    },
    container: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },
    title: {
        fontFamily: 'sans-serif',
        fontSize: 32,
    },
    subtitle: {
        fontFamily: 'sans-serif',
        fontSize: 20,
        color: '#888',
        marginBottom: 10,
    },
    textbox: {
        width: 200,
        height: 40,
    },
    submit: {
        
    }
});
