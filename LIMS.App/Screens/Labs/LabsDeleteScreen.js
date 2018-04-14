import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button, Divider } from 'react-native-elements';
import { remove } from '../../DataAccess/LabsDao';

export default class LabsDeleteScreen extends React.Component {
    static navigationOptions = {
        title: 'Delete Lab',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate, goBack } = this.props.navigation;
        this.navigate = navigate;
        this.goBack = goBack;

        let lab = this.props.navigation.state.params;
        this.labId = lab.LabId;
        this.state = lab;
    }
    
    // TODO: need to refresh when coming back
    render() {
        return (
            <View style={styles.container}>
                <ScrollView style={styles.wrap}>

                    <Text h4>Delete Lab</Text>
                    <Text>Are you sure you want to delete the following lab?</Text>
                    <Divider style={{ margin: 10 }} />

                    <Text h4>College Name</Text>
                    <Text>{this.state.CollegeName}</Text>

                    <Text h4>Course Code</Text>
                    <Text>{this.state.CourseCode}</Text>
                    
                    <Text h4>Week Number</Text>
                    <Text>{this.state.WeekNumber}</Text>
                    
                    <Text h4>Test Code</Text>
                    <Text>{this.state.TestId}</Text>
                    
                    <Text h4>Location</Text>
                    <Text>{this.state.Location}</Text>

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
            await remove(this.labId);
            this.navigate('LabsList');
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
