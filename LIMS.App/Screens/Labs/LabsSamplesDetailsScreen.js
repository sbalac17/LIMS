import React from 'react';
import { StyleSheet, View, ScrollView, ActivityIndicator } from 'react-native';
import { Text, Button, Card, Divider } from 'react-native-elements';
import StatusBadge from '../../Components/StatusBadge';
import UserName from '../../Components/UserName';
import { sampleDetails } from '../../DataAccess/LabsDao';

export default class LabsSamplesDetailsScreen extends React.Component {
    static navigationOptions = {
        title: 'Lab Sample Details',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        let params = this.props.navigation.state.params;
        this.labId = params.labId;
        this.sampleId = params.sampleId;

        this.state = {
            loaded: false,
            obj: {},
        };

        this._refresh();
    }

    // TODO: need to refresh when coming backs
    render() {
        const { navigate } = this.props.navigation;
        let obj = this.state.obj;
        let permissions = obj.$permissions;

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
                                            onPress={() => navigate('LabsSamplesEdit', { labSample: obj.LabSample, sample: obj.Sample })} />
                                    </View>
                                }

                                {permissions.CanDelete &&
                                    <View style={{ flex: 1 }}>
                                        <Button title='Delete'
                                            buttonStyle={{ backgroundColor: '#c33' }}
                                            onPress={() => navigate('LabsSamplesDelete', { labSample: obj.LabSample, sample: obj.Sample })} />
                                    </View>
                                }
                            </View>
                        }

                        <Text h4>Description</Text>
                        <Text>{obj.Sample.Description}</Text>
    
                        <Text h4>Status</Text>
                        <StatusBadge status={obj.LabSample.Status} />
                        
                        <Text h4>Taken</Text>
                        <Text>{obj.Sample.AddedDate}</Text>
                        
                        <Text h4>Assigned</Text>
                        <Text>{obj.LabSample.AssignedDate}</Text>
                        
                        <Text h4>Notes</Text>
                        <Text>{obj.LabSample.Notes}</Text>

                        <Text h4>Comments</Text>
                        {obj.Comments.map(comment => {
                            return (
                                <Card>
                                    <View>
                                        <UserName user={comment} />
                                        <Text>{comment.Date}</Text>
                                    </View>

                                    <Divider style={styles.commentDivider} />

                                    {comment.Message &&
                                        <Text>{comment.Message}</Text>
                                    }

                                    {comment.NewStatus != null &&
                                        <View style={styles.commentStatusWrap}>
                                            <Text style={styles.commentStatusText}>Changed status to </Text>
                                            <StatusBadge status={comment.NewStatus} />
                                        </View>
                                    }
                                </Card>
                            );
                        })} 
                    </ScrollView>
                }
            </View>
        );
    }

    async _refresh() {
        try {
            let obj = await sampleDetails(this.labId, this.sampleId);
            this.setState({ loaded: true, obj });
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
    commentDivider: {
        marginVertical: 10,
    },
    commentStatusWrap: {
        flexDirection: 'row',
        justifyContent: 'flex-start',
    },
    commentStatusText: {
        fontStyle: 'italic',
    },
});
