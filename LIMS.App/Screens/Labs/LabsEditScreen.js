import React from 'react';
import { StyleSheet, View, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import DatePicker from 'react-native-datepicker';
import ErrorList from '../../Components/ErrorList';
import AutoScrollingView from '../../Components/AutoScrollingView';
import { update } from '../../DataAccess/LabsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class LabsEditScreen extends React.Component {
    static navigationOptions = {
        title: 'Edit Lab',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let lab = this.props.navigation.state.params;
        this.labId = lab.LabId;
        this.state = {
            saving: false,
            errors: [],
            lab
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let lab = this.state.lab;

        return (
            <AutoScrollingView>
                <Text h4>College Name</Text>
                <TextInput
                    value={lab.CollegeName}
                    style={styles.input}
                    onChangeText={text => this.setState({ saving, errors, lab: { CollegeName: text, CourseCode: lab.CourseCode, WeekNumber: lab.WeekNumber, TestId: lab.TestId, Location: lab.Location }})} />

                <Text h4>Course Code</Text>
                <TextInput
                    value={lab.CourseCode}
                    style={styles.input}
                    onChangeText={text => this.setState({ saving, errors, lab: { CollegeName: lab.CollegeName, CourseCode: text, WeekNumber: lab.WeekNumber, TestId: lab.TestId, Location: lab.Location }})} />

                <Text h4>Week Number</Text>
                <TextInput
                    value={lab.WeekNumber.toString()}
                    keyboardType="numeric"
                    style={styles.input}
                    onChangeText={text => this.setState({ saving, errors, lab: { CollegeName: lab.CollegeName, CourseCode: lab.CourseCode, WeekNumber: text, TestId: lab.TestId, Location: lab.Location }})} />

                <Text h4>Test Code</Text>
                <TextInput
                    value={lab.TestId}
                    style={styles.input}
                    onChangeText={text => this.setState({ saving, errors, lab: { CollegeName: lab.CollegeName, CourseCode: lab.CourseCode, WeekNumber: lab.WeekNumber, TestId: text, Location: lab.Location }})} />

                <Text h4>Location</Text>
                <TextInput
                    value={lab.Location}
                    style={styles.input}
                    onChangeText={text => this.setState({ saving, errors, lab: { CollegeName: lab.CollegeName, CourseCode: lab.CourseCode, WeekNumber: lab.WeekNumber, TestId: lab.TestId, Location: text }})} />

                <ErrorList errors={this.state.errors} />

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
        this.setState({ saving: true, errors: [], lab: this.state.lab });

        try {
            await update(this.labId, this.state.lab);
            this.goBack();
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), lab: this.state.lab });
        }
    }
}

const styles = StyleSheet.create({
    input: {
        backgroundColor: '#ccc',
        margin: 0,
        padding: 10,
    }
});
