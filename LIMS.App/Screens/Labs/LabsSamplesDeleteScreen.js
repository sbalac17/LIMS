import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import { removeSample } from '../../DataAccess/LabsDao';

export default class LabsSamplesDeleteScreen extends React.Component {
    static navigationOptions = {
        title: 'Delete Lab Sample',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let { labSample, sample } = this.props.navigation.state.params;
        this.labId = labSample.LabId;
        this.sampleId = labSample.SampleId;
        
        this.state = {
            labSample,
            sample
        };
    }
    
    // TODO: need to refresh when coming back
    render() {
        let { labSample, sample } = this.state;

        return (
            <View style={styles.container}>
                <ScrollView style={styles.wrap}>

                    <Text h4>Delete Lab Sample</Text>
                    <Text>Are you sure you want to delete the following lab sample?</Text>
                    <Divider style={{ margin: 10 }} />

                    <Text h4>Description</Text>
                    <Text>{sample.Description}</Text>

                    <Text h4>Taken</Text>
                    <Text>{sample.AddedDate}</Text>
                    
                    <Text h4>Assigned</Text>
                    <Text>{labSample.AssignedDate}</Text>
                    
                    <Text h4>Notes</Text>
                    <Text>{labSample.Notes}</Text>

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
            await removeSample(this.labId, this.sampleId);
            this.navigate('LabsSamplesList');
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
