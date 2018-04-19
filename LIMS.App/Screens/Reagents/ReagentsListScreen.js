import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { SearchBar, ListItem, Button } from 'react-native-elements';
import { list } from '../../DataAccess/ReagentsDao';
import { debounce } from 'lodash';
import AutoRefreshable from '../../Components/AutoRefreshable';

export default class ReagentsListScreen extends AutoRefreshable {
    static navigationOptions = {
        title: 'Reagents',
        drawerLabel: 'Reagents'
    };

    constructor(props) {
        super(props);

        this.state = {
            loaded: false,
            permissions: {},
            query: '',
            reagents: {},
        };

        this.search = debounce(query => this._refresh(query), 300);
        autoRefresh(this);
    }
    
    render() {
        const { navigate } = this.props.navigation;
        let reagents = this.state.reagents;
        let permissions = this.state.permissions || reagents.$permissions;
        let loaded = this.state.loaded;

        function renderItem({ item }) {
            return (
                <ListItem key={item.ReagentId}
                    title={item.Name}
                    onPress={() => navigate('ReagentsDetails', { reagentId: item.ReagentId })} />
            );
        }

        return (
            <View style={styles.container}>
                {permissions.CanCreate &&
                    <View style={{ flexDirection: 'row', marginTop: 15, marginBottom: 15 }}>
                        <View style={{ flex: 1 }}>
                            <Button title='Add'
                                buttonStyle={{ backgroundColor: '#3a3' }}
                                onPress={() => navigate('ReagentsCreate')} />
                        </View>
                    </View>
                }

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
                        <FlatList data={reagents.Results}
                            keyExtractor={item => `reagent-${item.ReagentId}`}
                            renderItem={renderItem}
                            refreshing={!loaded}
                            onRefresh={() => this._refresh()} />
                    </View>
                }
            </View>
        );
    }

    async refresh(searchQuery) {
        searchQuery = searchQuery || '';

        if (this.state.loaded) {
            this.setState({
                loaded: false,
                permissions: this.state.permissions,
                query: searchQuery,
                reagents: this.state.reagents
            });
        } else {
            this.state.query = searchQuery;
        }

        let query = this.state.query;

        try {
            let reagents = await list(query);
            this.setState({ loaded: true, permissions: reagents.$permissions, query, reagents });
        } catch(e) {
            // TODO: display error
            this.setState({ loaded: true, permissions: this.state.permissions, query, reagents: {} });
        }
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    searchContainer: {
        backgroundColor: '#fff',
    },
    searchInput: {
        backgroundColor: '#ddd',
        color: '#000',
    }
});
