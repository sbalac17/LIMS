import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button } from 'react-native-elements';
import { read } from '../../DataAccess/LabsDao';

export default class LabsDetailsScreen extends React.Component {
    static navigationOptions = {
        title: 'Lab Details',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        this.labId = this.props.navigation.state.params.labId;
        this.state = {
            loaded: false,
            lab: {},
        };

        this._refresh();
    }

    // TODO: need to refresh when coming backs
    render() {
        const { navigate } = this.props.navigation;
        let permissions = this.state.lab.$permissions;

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
                                            onPress={() => navigate('LabsEdit', this.state.lab)} />
                                    </View>
                                }

                                {permissions.CanDelete && 
                                    <View style={{ flex: 1 }}>
                                        <Button title='Delete'
                                            buttonStyle={{ backgroundColor: '#c33' }}
                                            onPress={() => navigate('LabsDelete', this.state.lab)} />
                                    </View>
                                }
                            </View>
                        }

                        <Text h4>College Name</Text>
                        <Text>{this.state.lab.CollegeName}</Text>
    
                        <Text h4>Course Code</Text>
                        <Text>{this.state.lab.CourseCode}</Text>
                        
                        <Text h4>Week Number</Text>
                        <Text>{this.state.lab.WeekNumber}</Text>
                        
                        <Text h4>Test Code</Text>
                        <Text>{this.state.lab.TestId}</Text>
                        
                        <Text h4>Location</Text>
                        <Text>{this.state.lab.Location}</Text>

                        <Button title='Samples'
                            containerViewStyle={styles.spacing}
                            buttonStyle={{ backgroundColor: '#34f' }}
                            onPress={() => navigate('LabsSamplesList', this.state.lab)} />

                        <Button title='Reagents'
                            containerViewStyle={styles.spacing}
                            buttonStyle={{ backgroundColor: '#34f' }}
                            onPress={() => navigate('LabsReagentsList', this.state.lab)} />
                    </ScrollView>
                }
            </View>
        );
    }

    async _refresh() {
        try {
            let lab = await read(this.labId);
            this.setState({ loaded: true, lab });
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
    spacing: {
        marginTop: 15,
    },
});
