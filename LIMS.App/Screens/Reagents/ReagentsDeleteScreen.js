import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import { remove } from '../../DataAccess/ReagentsDao';

export default class ReagentsDeleteScreen extends React.Component {
    static navigationOptions = {
        title: 'Delete Reagent',
        drawerLabel: 'Reagents'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let reagent = this.props.navigation.state.params;
        this.reagentId = reagent.ReagentId;
        this.state = reagent;
    }
    
    render() {
        return (
            <View style={styles.container}>
                <ScrollView style={styles.wrap}>

                    <Text h4>Delete Reagent</Text>
                    <Text>Are you sure you want to delete the following reagent?</Text>
                    <Divider style={{ margin: 10 }} />

                    <Text h4>Name</Text>
                    <Text>{this.state.Name}</Text>

                    <Text h4>Quantity</Text>
                    <Text>{this.state.Quantity}</Text>
                    
                    <Text h4>Expiry Date</Text>
                    <Text>{this.state.ExpiryDate}</Text>
                    
                    <Text h4>Manufacturer Code</Text>
                    <Text>{this.state.ManufacturerCode}</Text>

                    <View style={{ marginTop: 15 }}>
                        <Button title='Delete'
                            buttonStyle={{ backgroundColor: '#a33' }}
                            onPress={() => this._delete()} />
                    </View>
                </ScrollView>
            </View>
        );
    }

    async _delete() {
        try {
            await remove(this.reagentId);
            this.navigate('ReagentsList');
        } catch(e) {
            // TODO: report error
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
});
