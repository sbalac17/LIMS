import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, TextInput } from 'react-native';
import { Text, Button } from 'react-native-elements';
import DatePicker from 'react-native-datepicker';
import ErrorList from '../../Components/ErrorList';
import AutoScrollingView from '../../Components/AutoScrollingView';
import { create } from '../../DataAccess/ReagentsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class ReagentsCreateScreen extends React.Component {
    static navigationOptions = {
        title: 'Create Reagent',
        drawerLabel: 'Reagents'
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
            reagent: {
                Name: '',
                Quantity: '',
                ExpiryDate: '',
                ManufacturerCode: '',
            },
        };
    }
    
    render() {
        let saving = this.state.saving;
        let errors = this.state.errors;
        let reagent = this.state.reagent;

        // TODO: the view needs to expand back when keyboard closes
        return (
            <AutoScrollingView>
                <Text h4>Name</Text>
                <TextInput
                    value={reagent.Name}
                    style={styles.input}
                    onChangeText={text => this.setState({ saving, errors, reagent: { Name: text, Quantity: reagent.Quantity, ExpiryDate: reagent.ExpiryDate, ManufacturerCode: reagent.ManufacturerCode }})} />

                <Text h4>Quantity</Text>
                <TextInput
                    value={reagent.Quantity}
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
                    <Button title='Create'
                        loading={saving}
                        buttonStyle={{ backgroundColor: '#3a3' }}
                        onPress={() => this._save()} />
                </View>
            </AutoScrollingView>
        );
    }

    async _save() {
        if (this.state.saving) return;
        
        let reagent = this.state.reagent;
        this.setState({ saving: true, errors: [], reagent });

        try {
            let newReagent = await create(this.state.reagent);
            this.popToTop();
            this.navigate('ReagentsDetails', { reagentId: newReagent.ReagentId })
        } catch (e) {
            this.setState({ saving: false, errors: extractErrorMessages(e), reagent });
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
