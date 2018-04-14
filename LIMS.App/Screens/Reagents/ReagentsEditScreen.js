import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import DatePicker from 'react-native-datepicker';
import ErrorList from '../../Components/ErrorList';
import { update } from '../../DataAccess/ReagentsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class ReagentsEditScreen extends React.Component {
    static navigationOptions = {
        title: 'Edit Reagent',
        drawerLabel: 'Reagents'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let reagent = this.props.navigation.state.params;
        this.reagentId = reagent.ReagentId;
        this.state = {
            saving: false,
            errors: [],
            reagent
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let reagent = this.state.reagent;

        // TODO: the view needs to expand back when keyboard closes
        return (
            <KeyboardAvoidingView contentContainerStyle={styles.container} behavior="height" keyboardVerticalOffset={60}>
                <View>
                <ScrollView style={styles.wrap}>
                    <Text h4>Name</Text>
                    <TextInput
                        value={reagent.Name}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, reagent: { Name: text, Quantity: reagent.Quantity, ExpiryDate: reagent.ExpiryDate, ManufacturerCode: reagent.ManufacturerCode }})} />

                    <Text h4>Quantity</Text>
                    <TextInput
                        value={reagent.Quantity.toString()}
                        keyboardType="numeric"
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, reagent: { Name: reagent.Name, Quantity: text, ExpiryDate: reagent.ExpiryDate, ManufacturerCode: reagent.ManufacturerCode }})} />
                                 
                    <Text h4>Expiry Date</Text>
                    <DatePicker date={reagent.ExpiryDate} style={{ width: '100%' }}
                        mode="date" format="DD/MM/YYYY"
                        minDate="01/01/2000" maxDate="01/01/2050"
                        onDateChange={text => this.setState({ saving, errors, reagent: { Name: reagent.Name, Quantity: reagent.Quantity, ExpiryDate: text, ManufacturerCode: reagent.ManufacturerCode }})} />

                    <Text h4>Manufacturer Code</Text>
                    <TextInput
                        value={reagent.ManufacturerCode}
                        style={styles.input}
                        onChangeText={text => this.setState({ saving, errors, reagent: { Name: reagent.Name, Quantity: reagent.Quantity, ExpiryDate: reagent.ExpiryDate, ManufacturerCode: text }})} />

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
        this.setState({ saving: true, errors: [], reagent: this.state.reagent });

        try {
            await update(this.reagentId, this.state.reagent);
            this.goBack();
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), reagent: this.state.reagent });
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
