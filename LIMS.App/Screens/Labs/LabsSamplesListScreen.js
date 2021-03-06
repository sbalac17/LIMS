import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { SearchBar, ListItem, Button } from 'react-native-elements';
import StatusBadge from '../../Components/StatusBadge';
import { samplesList } from '../../DataAccess/LabsDao';
import { debounce } from 'lodash';
import AutoRefreshable from '../../Components/AutoRefreshable';

export default class LabsSamplesListScreen extends AutoRefreshable {
    static navigationOptions = {
        title: 'Lab Samples',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate } = this.props.navigation;
        this.navigate = navigate;

        this.state = {
            loaded: false,
            permissions: {},
            query: '',
            obj: {},
        };

        this.lab = this.props.navigation.state.params;

        this.search = debounce(query => this.refresh(query), 300);
    }
    
    render() {
        let obj = this.state.obj;
        let permissions = this.state.permissions || obj.$permissions;
        let loaded = this.state.loaded;

        return (
            <View style={styles.container}>
                <SearchBar placeholder="Search" lightTheme
                    onChangeText={this.search}
                    onClearText={this.search}
                    containerStyle={styles.searchContainer}
                    inputStyle={styles.searchInput} />

                {!loaded &&
                    <ActivityIndicator style={{ margin: 20 }} size="large" />
                }

                {loaded &&
                    <View style={{ flex: 1 }}>
                        <FlatList data={obj.Results}
                            keyExtractor={item => `labsample-${item.SampleId}`}
                            renderItem={item => this.renderItem(item)}
                            refreshing={!loaded}
                            onRefresh={() => this.refresh()} />
                    </View>
                }
            </View>
        );
    }

    renderItem({ item }) {
        return (
            <ListItem key={item.SampleId}
                leftIcon={
                    <View style={styles.item}>
                        <Text>{item.SampleDescription} </Text>
                        <StatusBadge status={item.Status} />
                    </View>
                }
                onPress={() => this.navigate('LabsSamplesDetails', { labId: this.lab.LabId, sampleId: item.SampleId })} />
        );
    }

    async refresh(searchQuery) {
        searchQuery = searchQuery || '';

        if (this.state.loaded) {
            this.setState({
                loaded: false,
                permissions: this.state.permissions,
                query: searchQuery,
                obj: this.state.obj
            });
        } else {
            this.state.query = searchQuery;
        }

        let query = this.state.query;

        try {
            let obj = await samplesList(this.lab.LabId, query);
            this.setState({ loaded: true, permissions: obj.$permissions, query, obj });
        } catch(e) {
            // TODO: display error
            this.setState({ loaded: true, permissions: this.state.permissions, query, obj: {} });
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
    },
    searchContainer: {
        backgroundColor: '#fff',
    },
    searchInput: {
        backgroundColor: '#ddd',
        color: '#000',
    }
});
