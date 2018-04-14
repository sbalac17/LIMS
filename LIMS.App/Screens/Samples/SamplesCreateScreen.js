import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import DatePicker from 'react-native-datepicker';
import ErrorList from '../../Components/ErrorList';
import { create } from '../../DataAccess/SamplesDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class SamplesCreateScreen extends React.Component {
    static navigationOptions = {
        title: 'Create Sample',
        drawerLabel: 'Samples'
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
            sample: {
                TestId: '',
                Description: '',
                AddedDate: '',
            },
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let sample = this.state.sample;

        // TODO: the view needs to expand back when keyboard closes
        return (
            <KeyboardAvoidingView contentContainerStyle={styles.container} behavior="height" keyboardVerticalOffset={60}>
                <View>
                <ScrollView style={styles.wrap}>
                    <Text h4>Test Code</Text>
                    <TextInput
                        value={sample.TestId}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, sample: { TestId: text, Description: sample.Description, AddedDate: sample.AddedDate }})} />

                    <Text h4>Description</Text>
                    <TextInput
                        value={sample.Description}
                        multiline={true}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, sample: { TestId: sample.TestId, Description: text, AddedDate: sample.AddedDate }})} />
                                 
                    <Text h4>Taken</Text>
                    <DatePicker date={sample.AddedDate} style={{ width: '100%' }}
                        mode="datetime" format="DD/MM/YYYY h:mm:ss A"
                        minDate="01/01/2000 12:00:00 AM" maxDate="01/01/2050 12:00:00 AM"
                        onDateChange={text => this.setState({ saving, errors, sample: { TestId: sample.TestId, Description: sample.Description, AddedDate: text }})} />

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
        let sample = this.state.sample;
        this.setState({ saving: true, errors: [], sample });

        try {
            let newSample = await create(this.state.sample);
            this.popToTop();
            this.navigate('SamplesDetails', { sampleId: newSample.SampleId })
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), sample });
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
