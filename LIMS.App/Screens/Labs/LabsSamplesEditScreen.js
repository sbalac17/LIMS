import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import DatePicker from 'react-native-datepicker';
import ErrorList from '../../Components/ErrorList';
import { updateSample } from '../../DataAccess/LabsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class LabsSamplesEditScreen extends React.Component {
    static navigationOptions = {
        title: 'Edit Lab Sample',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let { labSample, sample } = this.props.navigation.state.params;
        this.labId = labSample.LabId;
        this.sampleId = labSample.SampleId;

        this.state = {
            saving: false,
            errors: [],
            labSample,
            sample,
            notes: labSample.Notes,
        };
    }
    
    render() {
        let { saving, errors, labSample, sample, notes } = this.state;

        // TODO: the view needs to expand back when keyboard closes
        return (
            <KeyboardAvoidingView contentContainerStyle={styles.container} behavior="height" keyboardVerticalOffset={60}>
                <View>
                <ScrollView style={styles.wrap}>
                    <Text h4>Test Code</Text>
                    <Text>{sample.TestId}</Text>

                    <Text h4>Description</Text>
                    <Text>{sample.Description}</Text>

                    <Text h4>Taken</Text>
                    <Text>{sample.AddedDate}</Text>

                    <Text h4>Assigned</Text>
                    <Text>{labSample.AssignedDate}</Text>

                    <Text h4>Notes</Text>
                    <TextInput
                        value={notes}
                        multiline={true}
                        style={styles.input}
                        onChangeText={text => this.setState({ notes: text })} />

                    <ErrorList errors={this.state.errors} />

                    <View style={{ marginTop: 15 }}>
                        <Button title='Save'
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
        this.setState({ saving: true, errors: [] });

        try {
            await updateSample(this.labId, this.sampleId, {
                Notes: this.state.notes
            });

            this.goBack();
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e) });
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
