import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, ActivityIndicator, TextInput } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import ErrorList from '../../Components/ErrorList';
import AutoScrollingView from '../../Components/AutoScrollingView';
import { addReagent } from '../../DataAccess/LabsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

export default class LabsReagentsAddScreen extends React.Component {
    static navigationOptions = {
        title: 'Add Reagent to Lab',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let { labId, reagent } = this.props.navigation.state.params;
        this.labId = labId;
        
        this.state = {
            saving: false,
            reagent,
            errors: null,
            quantity: '1',
        };
    }
    
    render() {
        let { reagent, quantity } = this.state;

        return (
            <AutoScrollingView>
                <Text h4>Name</Text>
                <Text>{reagent.Name}</Text>

                <Text h4>Quantity</Text>
                <Text>{reagent.Quantity}</Text>

                <Text h4>Expires</Text>
                <Text>{reagent.ExpiryDate}</Text>

                <Text h4>Manufacturer Code</Text>
                <Text>{reagent.ManufacturerCode}</Text>

                <Text h4>Quantity to Use</Text>
                <TextInput
                    value={quantity}
                    keyboardType="numeric"
                    style={styles.input}
                    onChangeText={text => this.setState({ quantity: text })} />

                <ErrorList errors={this.state.errors} />

                <View style={{ marginTop: 15 }}>
                    <Button title='Add'
                        buttonStyle={{ backgroundColor: '#3c3' }}
                        loading={this.state.saving}
                        onPress={() => this._save()} />
                </View>
            </AutoScrollingView>
        );
    }

    async _save() {
        if (this.state.saving) return;

        this.setState({ saving: true, errors: null });

        try {
            await addReagent(this.labId, this.state.reagent.ReagentId, this.state.quantity);
            this.goBack();
        } catch(e) {
            this.setState({ saving: true, errors: extractErrorMessages(e) });
        }
    }
}

const styles = StyleSheet.create({
    input: {
        backgroundColor: '#ccc',
        margin: 0,
        padding: 10,
    },
});
