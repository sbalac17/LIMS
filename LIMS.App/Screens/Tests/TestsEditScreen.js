import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import ErrorList from '../../Components/ErrorList';
import AutoScrollingView from '../../Components/AutoScrollingView';
import { update } from '../../DataAccess/TestsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class TestsEditScreen extends React.Component {
    static navigationOptions = {
        title: 'Edit Test',
        drawerLabel: 'Tests'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let test = this.props.navigation.state.params;
        this.testId = test.TestId;
        this.state = {
            saving: false,
            errors: [],
            test
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let test = this.state.test;

        return (
            <AutoScrollingView>
                <Text h4>Test Code</Text>
                <Text>{test.TestId}</Text>

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

                <ErrorList errors={errors} />

                <View style={{ marginTop: 15 }}>
                    <Button title='Save'
                        loading={saving}
                        buttonStyle={{ backgroundColor: '#3a3' }}
                        onPress={() => this._save()} />
                </View>
            </AutoScrollingView>
        );
    }

    async _save() {
        this.setState({ saving: true, errors: [], test: this.state.test });

        try {
            await update(this.testId, this.state.test);
            this.goBack();
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), test: this.state.test });
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
