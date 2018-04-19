import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, ActivityIndicator, TextInput } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import ErrorList from '../../Components/ErrorList';
import AutoScrollingView from '../../Components/AutoScrollingView';
import { extractErrorMessages } from '../../DataAccess/HttpClient';
import { removeReagent } from '../../DataAccess/LabsDao';

export default class LabsReagentDeleteScreen extends React.Component {
    static navigationOptions = {
        title: 'Delete Lab Reagent',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { goBack } = this.props.navigation;
        this.goBack = goBack;

        let { labId, usedReagent } = this.props.navigation.state.params;
        this.labId = labId;
        this.usedReagentId = usedReagent.UsedReagentId;
        
        this.state = {
            loading: false,
            errors: null,
            usedReagent,
            returnQuantity: '0'
        };
    }
    
    render() {
        let { usedReagent, returnQuantity } = this.state;

        return (
            <AutoScrollingView>
                <Text h4>Delete Lab Reagent</Text>
                <Text>Are you sure you want to delete the following lab reagent?</Text>
                <Divider style={{ margin: 10 }} />

                <Text h4>Name</Text>
                <Text>{usedReagent.ReagentName}</Text>

                <Text h4>Quantity</Text>
                <Text>{usedReagent.Quantity}</Text>
                
                <Text h4>Expires</Text>
                <Text>{usedReagent.ReagentExpiryDate}</Text>
                
                <Text h4>Manufacturer Code</Text>
                <Text>{usedReagent.ReagentManufacturerCode}</Text>

                <Text h4>Return Quantity</Text>
                <TextInput
                    value={returnQuantity}
                    keyboardType="numeric"
                    style={styles.input}
                    onChangeText={text => this.setState({ returnQuantity: text })} />

                <ErrorList errors={this.state.errors} />

                <View style={{ marginTop: 15 }}>
                    <Button title='Delete'
                        buttonStyle={{ backgroundColor: '#a33' }}
                        loading={this.state.loading}
                        onPress={() => this._delete()} />
                </View>
            </AutoScrollingView>
        );
    }

    async _delete() {
        this.setState({ loading: true, errors: null });

        try {
            await removeReagent(this.labId, this.usedReagentId, this.state.returnQuantity);
            this.goBack();
        } catch(e) {
            this.setState({ loading: false, errors: extractErrorMessages(e) });
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
    },
});
