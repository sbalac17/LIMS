import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { ListItem, Button, Badge } from 'react-native-elements';
import { reagentsList } from '../../DataAccess/LabsDao';

export default class LabsSamplesListScreen extends React.Component {
    static navigationOptions = {
        title: 'Lab Reagents',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate } = this.props.navigation;
        this.navigate = navigate;

        this.state = {
            loaded: false,
            permissions: {},
            obj: {},
        };

        this.lab = this.props.navigation.state.params;

        this._refresh();
    }
    
    render() {
        let { obj, loaded } = this.state;
        let permissions = this.state.permissions || obj.$permissions;

        return (
            <View style={styles.container}>
                {permissions.CanCreate &&
                    <View style={{ flexDirection: 'row', marginTop: 15, marginBottom: 15 }}>
                        <View style={{ flex: 1 }}>
                            <Button title='Add'
                                buttonStyle={{ backgroundColor: '#3a3' }}
                                onPress={() => this.navigate('LabsReagentsSelection', { labId: this.lab.LabId })} />
                        </View>
                    </View>
                }

                {!loaded &&
                    <ActivityIndicator style={{ margin: 20 }} size="large" />
                }

                {loaded &&
                    <View style={{ flex: 1 }}>
                        <FlatList data={obj.Results}
                            keyExtractor={item => `usedreagent-${item.UsedReagentId}`}
                            renderItem={item => this.renderItem(item, permissions)}
                            refreshing={!loaded}
                            onRefresh={() => this._refresh()} />
                    </View>
                }
            </View>
        );
    }

    renderItem({ item }, permissions) {
        return (
            <ListItem key={item.UsedReagentId}
                leftIcon={
                    <View style={styles.item}>
                        <Badge value={item.Quantity} />
                        <Text> {item.ReagentName} </Text>
                    </View>
                }
                rightIcon={{ name: 'delete', color: '#c33' }}
                hideChevron={!permissions.CanDelete}
                onPressRightIcon={() => this.navigate('LabsReagentsDelete', { labId: this.lab.LabId, usedReagent: item })}
                onPress={() => this.navigate('ReagentsDetails', { reagentId: item.ReagentId })} />
        );
    }

    async _refresh() {
        if (this.state.loaded) {
            this.setState({ loaded: false });
        }

        try {
            let obj = await reagentsList(this.lab.LabId);
            this.setState({ loaded: true, permissions: obj.$permissions, obj });
        } catch(e) {
            // TODO: display error
            this.setState({ loaded: true, obj: {} });
        }
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    item: {
        flexDirection: 'row',
        alignItems: 'baseline',
    },
});
