import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import ErrorList from '../../Components/ErrorList';
import { create } from '../../DataAccess/TestsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class TestsCreateScreen extends React.Component {
    static navigationOptions = {
        title: 'Create Test',
        drawerLabel: 'Tests'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack, popToTop } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;
        this.popToTop = popToTop;

        this.state = {
            saving: false,
            errors: [],
            test: {
                TestId: '',
                Name: '',
                Description: '',
            },
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let test = this.state.test;

        // TODO: the view needs to expand back when keyboard closes
        return (
            <KeyboardAvoidingView contentContainerStyle={styles.container} behavior="height" keyboardVerticalOffset={60}>
                <View>
                <ScrollView style={styles.wrap}>
                    <Text h4>Test Code</Text>
                    <TextInput
                        value={test.TestId}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, test: { TestId: text, Name: test.Name, Description: test.Description }})} />

                    <Text h4>Name</Text>
                    <TextInput
                        value={test.Name}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, test: { TestId: test.TestId, Name: text, Description: test.Description }})} />
                    
                    <Text h4>Description</Text>
                    <TextInput
                        value={test.Description}
                        multiline={true}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, test: { TestId: test.TestId, Name: test.Name, Description: text }})} />

                    <ErrorList errors={this.state.errors} />

                    <View style={{ marginTop: 15 }}>
                        <Button title='Create'
                            loading={saving}
                            buttonStyle={{ backgroundColor: '#3a3' }}
                            onPress={() => this._save()} />
                    </View>
                </ScrollView>
                </View>
            </KeyboardAvoidingView>
        );
    }

    async _save() {
        if (this.state.saving) return;
        
        let test = this.state.test;
        this.setState({ saving: true, errors: [], test });

        try {
            let newTest = await create(this.state.test);
            this.popToTop();
            this.navigate('TestsDetails', { testId: newTest.TestId })
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), test });
        }
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    wrap: {
        margin: 15,
    },
    input: {
        backgroundColor: '#ccc',
        margin: 0,
        padding: 10,
    }
});
