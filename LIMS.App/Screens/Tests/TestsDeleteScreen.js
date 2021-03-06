import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import { remove } from '../../DataAccess/TestsDao';

export default class TestsDeleteScreen extends React.Component {
    static navigationOptions = {
        title: 'Delete Test',
        drawerLabel: 'Tests'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let test = this.props.navigation.state.params;
        this.testId = test.TestId;
        this.state = test;
    }
    
    render() {
        return (
            <View style={styles.container}>
                <ScrollView style={styles.wrap}>

                    <Text h4>Delete Test</Text>
                    <Text>Are you sure you want to delete the following test?</Text>
                    <Divider style={{ margin: 10 }} />

                    <Text h4>Test Code</Text>
                    <Text>{this.state.TestId}</Text>

                    <Text h4>Name</Text>
                    <Text>{this.state.Name}</Text>
                    
                    <Text h4>Description</Text>
                    <Text>{this.state.Description}</Text>

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
            await remove(this.testId);
            this.navigate('TestsList');
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
