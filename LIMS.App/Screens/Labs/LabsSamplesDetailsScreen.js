import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView, ActivityIndicator, TextInput } from 'react-native';
import { Text, Button, Card, Divider } from 'react-native-elements';
import StatusBadge from '../../Components/StatusBadge';
import UserName from '../../Components/UserName';
import ErrorList from '../../Components/ErrorList';
import { sampleDetails, postComment } from '../../DataAccess/LabsDao';
import { extractErrorMessages } from '../../DataAccess/HttpClient';

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
            comment: '',
            posting: false,
            commentErrors: null,
        };

        this._refresh();
    }

    // TODO: need to refresh when coming backs
    render() {
        const { navigate } = this.props.navigation;
        let { loaded, obj, comment, posting, commentErrors } = this.state;
        let permissions = obj.$permissions;

        // TODO: the view needs to expand back when keyboard closes
        return (
            <KeyboardAvoidingView style={styles.container} behavior="height" keyboardVerticalOffset={60}>
                <View>
                <ScrollView>
                    {!loaded && 
                        <ActivityIndicator style={{ margin: 20 }} size="large" />
                    }

                    {loaded && obj &&
                        <View style={styles.wrap}>
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
                                    <Card key={`comment-${comment.UserId}-${comment.Date}`}>
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

                            <View style={styles.spacing}>
                                <TextInput placeholder='Comment'
                                    placeholderTextColor='#666'
                                    value={comment}
                                    multiline={true}
                                    style={styles.input}
                                    onChangeText={text => this.setState({ loaded, obj, comment: text, commentErrors })} />

                                <ErrorList errors={commentErrors} />

                                <Button title='Post'
                                    buttonStyle={[styles.spacing, { backgroundColor: '#34f' }]}
                                    loading={posting}
                                    onPress={() => this._postComment()} />

                                {obj.IsLabManager &&
                                    <View>
                                        {obj.LabSample.Status != 0 &&
                                            <Button title='In Progress'
                                                containerViewStyle={styles.spacing}
                                                buttonStyle={{ backgroundColor: '#cc3' }}
                                                loading={posting}
                                                textStyle={{ color: '#000' }}
                                            onPress={() => this._postComment(0)} />
                                        }

                                        {obj.LabSample.Status != 1 &&
                                            <Button title='Approve'
                                                containerViewStyle={styles.spacing}
                                                buttonStyle={{ backgroundColor: '#3a3' }}
                                                loading={posting}
                                                onPress={() => this._postComment(1)} />
                                        }

                                        {obj.LabSample.Status != 2 &&
                                            <Button title='Reject'
                                                containerViewStyle={styles.spacing}
                                                buttonStyle={{ backgroundColor: '#a33' }}
                                                loading={posting}
                                                onPress={() => this._postComment(2)} />
                                        }
                                    </View>
                                }
                            </View>
                        </View>
                    }
                </ScrollView>
                </View>
            </KeyboardAvoidingView>
        );
    }

    async _refresh() {
        if (this.state.loaded) {
            this.setState({ loaded: false });
        }

        try {
            let obj = await sampleDetails(this.labId, this.sampleId);
            this.setState({ loaded: true, obj });
        } catch(e) {
            // TODO: report error
            this.setState({ loaded: true, obj: null });
        }
    }

    async _postComment(status) {
        if (this.state.posting) return;
        if (typeof status !== 'number') status = null;

        let comment = this.state.comment;
        this.setState({ posting: true });

        try {
            await postComment(this.labId, this.sampleId, {
                Message: comment,
                RequestedStatus: status
            });

            this.setState({ comment: '', posting: false });
            this._refresh();
        } catch(e) {
            this.setState({ commentErrors: extractErrorMessages(e), posting: false });
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
    spacing: {
        marginTop: 15,
    },
    input: {
        backgroundColor: '#ccc',
        margin: 0,
        marginBottom: 15,
        padding: 10,
    },
});
