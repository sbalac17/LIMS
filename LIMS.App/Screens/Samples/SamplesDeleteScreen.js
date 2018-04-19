import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import { remove } from '../../DataAccess/SamplesDao';

export default class SamplesDeleteScreen extends React.Component {
    static navigationOptions = {
        title: 'Delete Sample',
        drawerLabel: 'Samples'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let sample = this.props.navigation.state.params;
        this.sampleId = sample.SampleId;
        this.state = sample;
    }
    
    render() {
        return (
            <View style={styles.container}>
                <ScrollView style={styles.wrap}>

                    <Text h4>Delete Sample</Text>
                    <Text>Are you sure you want to delete the following sample?</Text>
                    <Divider style={{ margin: 10 }} />

                    <Text h4>Test Code</Text>
                    <Text>{this.state.TestId}</Text>

                    <Text h4>Description</Text>
                    <Text>{this.state.Description}</Text>
                    
                    <Text h4>Taken</Text>
                    <Text>{this.state.AddedDate}</Text>

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
            await remove(this.sampleId);
            this.navigate('SamplesList');
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
