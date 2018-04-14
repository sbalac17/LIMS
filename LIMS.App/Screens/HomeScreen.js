import React from 'react';
import { StyleSheet, View, FlatList, ActivityIndicator } from 'react-native';
import { Divider, Button, Text, Card } from 'react-native-elements';
import { getRecentLabs } from '../DataAccess/HomeDao';

export default class HomeScreen extends React.Component {
    static navigationOptions = {
        title: 'Home',
        drawerLabel: 'Home'
    };

    constructor(props) {
        super(props);

        this.state = {
            loaded: false,
            recents: [],
        };

        this._refresh();
    }
    
    render() {
        const { navigate } = this.props.navigation;

        function renderLab({ item }) {
            return (
                <Card>
                    <Text h4>{`${item.CourseCode} (week ${item.WeekNumber})`}</Text>
                    <Text>
                        Test code:
                        <Text onPress={() => navigate('TestsDetails', { testId: item.TestId })} style={styles.link}> {item.TestId}</Text>
                    </Text>
                    <Button title="Open »"
                        containerViewStyle={{ marginTop: 20 }}
                        backgroundColor="#34f"
                        onPress={() => navigate('LabsDetails', { labId: item.LabId })} />
                </Card>
            );
        }

        return (
            <View style={styles.container}>
                <View style={{ margin: 20 }}>
                    <Text h1 style={{ textAlign: 'center' }}>LIMS</Text>
                    <Text style={{ textAlign: 'center', fontSize: 16 }}>
                        The <Text style={{ fontStyle: 'italic' }}>Centennial College</Text> laboratory information management system.
                    </Text>
                </View>

                {!this.state.loaded &&
                    <ActivityIndicator style={{ margin: 20 }} size="large" />
                }

                {this.state.loaded &&
                    <FlatList data={this.state.recents}
                        renderItem={renderLab}
                        keyExtractor={lab => `lab-${lab.LabId}`} />
                }
            </View>
        );
    }

    async _refresh() {
        this.setState({ loaded: false, recents: [] });

        try {
            let recents = await getRecentLabs();
            console.log('recents', recents);
            this.setState({ loaded: true, recents });
        } catch (e) {
            // TODO: display error
            this.setState({ loaded: true, recents: [] });
        }
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
        alignItems: 'stretch',
    },
    link: {
        color: '#11f',
        fontWeight: 'bold',
    }
});
