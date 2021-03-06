import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button } from 'react-native-elements';
import { read } from '../../DataAccess/TestsDao';
import AutoRefreshable from '../../Components/AutoRefreshable';

export default class TestsDetailsScreen extends AutoRefreshable {
    static navigationOptions = {
        title: 'Test Details',
        drawerLabel: 'Tests'
    };

    constructor(props) {
        super(props);

        this.testId = this.props.navigation.state.params.testId;
        this.state = {
            loaded: false,
            test: {},
        };
    }

    render() {
        const { navigate } = this.props.navigation;
        let permissions = this.state.test && this.state.test.$permissions;

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
                                            onPress={() => navigate('TestsEdit', this.state.test)} />
                                    </View>
                                }

                                {permissions.CanDelete && 
                                    <View style={{ flex: 1 }}>
                                        <Button title='Delete'
                                            buttonStyle={{ backgroundColor: '#c33' }}
                                            onPress={() => navigate('TestsDelete', this.state.test)} />
                                    </View>
                                }
                            </View>
                        }

                        <Text h4>Test Code</Text>
                        <Text>{this.state.test.TestId}</Text>
    
                        <Text h4>Name</Text>
                        <Text>{this.state.test.Name}</Text>
                        
                        <Text h4>Description</Text>
                        <Text>{this.state.test.Description}</Text>
                    </ScrollView>
                }
            </View>
        );
    }

    async refreshImpl() {
        if (this.state.loaded) {
            this.setState({ loaded: false, test: null });
        }

        try {
            let test = await read(this.testId);
            this.setState({ loaded: true, test });
        } catch(e) {
            // TODO: report error
            this.setState({ loaded: true, test: null });
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
