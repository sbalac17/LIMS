import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button } from 'react-native-elements';
import { read } from '../../DataAccess/SamplesDao';
import AutoRefreshable from '../../Components/AutoRefreshable';

export default class SamplesDetailsScreen extends AutoRefreshable {
    static navigationOptions = {
        title: 'Sample Details',
        drawerLabel: 'Samples'
    };

    constructor(props) {
        super(props);

        this.sampleId = this.props.navigation.state.params.sampleId;
        this.state = {
            loaded: false,
            sample: {},
        };
    }

    render() {
        const { navigate } = this.props.navigation;
        let permissions = this.state.sample && this.state.sample.$permissions;

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
                                            onPress={() => navigate('SamplesEdit', this.state.sample)} />
                                    </View>
                                }

                                {permissions.CanDelete && 
                                    <View style={{ flex: 1 }}>
                                        <Button title='Delete'
                                            buttonStyle={{ backgroundColor: '#c33' }}
                                            onPress={() => navigate('SamplesDelete', this.state.sample)} />
                                    </View>
                                }
                            </View>
                        }

                        <Text h4>Test Code</Text>
                        <Text>{this.state.sample.TestId}</Text>
    
                        <Text h4>Description</Text>
                        <Text>{this.state.sample.Description}</Text>
                        
                        <Text h4>Taken</Text>
                        <Text>{this.state.sample.AddedDate}</Text>
                    </ScrollView>
                }
            </View>
        );
    }

    async refreshImpl() {
        if (this.state.loaded) {
            this.setState({ loaded: false, sample: null });
        }

        try {
            let sample = await read(this.sampleId);
            this.setState({ loaded: true, sample });
        } catch(e) {
            // TODO: report error
            this.setState({ loaded: true, sample: null });
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
