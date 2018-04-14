import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button } from 'react-native-elements';
import { read } from '../../DataAccess/ReagentsDao';

export default class ReagentsDetailsScreen extends React.Component {
    static navigationOptions = {
        title: 'Reagent Details',
        drawerLabel: 'Reagents'
    };

    constructor(props) {
        super(props);

        this.reagentId = this.props.navigation.state.params.reagentId;
        this.state = {
            loaded: false,
            reagent: {},
        };

        this._refresh();
    }

    // TODO: need to refresh when coming backs
    render() {
        const { navigate } = this.props.navigation;
        let permissions = this.state.reagent.$permissions;

        return (
            <View style={styles.container}>
                {!this.state.loaded && 
                    <ActivityIndicator style={{ margin: 20 }} size="large" />
                }

                {this.state.loaded &&
                    <ScrollView style={styles.wrap}>

                        {(permissions.CanUpdate || permissions.CanDelete) &&
                            <View style={{ flex: 1, flexDirection: 'row', marginBottom: 15 }}>
                                {permissions.CanUpdate &&
                                    <View style={{ flex: 1 }}>
                                        <Button title='Edit'
                                            buttonStyle={{ backgroundColor: '#34f' }}
                                            onPress={() => navigate('ReagentsEdit', this.state.reagent)} />
                                    </View>
                                }

                                {permissions.CanDelete && 
                                    <View style={{ flex: 1 }}>
                                        <Button title='Delete'
                                            buttonStyle={{ backgroundColor: '#c33' }}
                                            onPress={() => navigate('ReagentsDelete', this.state.reagent)} />
                                    </View>
                                }
                            </View>
                        }

                        <Text h4>Name</Text>
                        <Text>{this.state.reagent.Name}</Text>
    
                        <Text h4>Quantity</Text>
                        <Text>{this.state.reagent.Quantity}</Text>
                        
                        <Text h4>Expiry Date</Text>
                        <Text>{this.state.reagent.ExpiryDate}</Text>
                        
                        <Text h4>Manufacturer Code</Text>
                        <Text>{this.state.reagent.ManufacturerCode}</Text>
                    </ScrollView>
                }
            </View>
        );
    }

    async _refresh() {
        try {
            let reagent = await read(this.reagentId);
            this.setState({ loaded: true, reagent });
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
