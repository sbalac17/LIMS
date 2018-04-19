import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import DatePicker from 'react-native-datepicker';
import ErrorList from '../../Components/ErrorList';
import AutoScrollingView from '../../Components/AutoScrollingView';
import { update } from '../../DataAccess/SamplesDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class SamplesEditScreen extends React.Component {
    static navigationOptions = {
        title: 'Edit Sample',
        drawerLabel: 'Samples'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let sample = this.props.navigation.state.params;
        this.sampleId = sample.SampleId;
        this.state = {
            saving: false,
            errors: [],
            sample
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let sample = this.state.sample;

        return (
            <AutoScrollingView>
                <Text h4>Test Code</Text>
                <Text>{sample.TestId}</Text>

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
        this.setState({ saving: true, errors: [], sample: this.state.sample });

        try {
            await update(this.sampleId, this.state.sample);
            this.goBack();
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), sample: this.state.sample });
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
