import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { SearchBar, ListItem, Button } from 'react-native-elements';
import { list } from '../../DataAccess/ReagentsDao';
import { debounce } from 'lodash';
import AutoRefreshable from '../../Components/AutoRefreshable';

export default class LabsReagentsSelectionScreen extends AutoRefreshable {
    static navigationOptions = {
        title: 'Add Reagent to Lab',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        const { navigate } = this.props.navigation;
        this.navigate = navigate;

        let { labId } = this.props.navigation.state.params;
        this.labId = labId;

        this.state = {
            loaded: false,
            query: '',
            reagents: {},
        };

        this.search = debounce(query => this._refresh(query), 300);
    }
    
    render() {
        let { reagents, loaded } = this.state;

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
                        <FlatList data={reagents.Results}
                            keyExtractor={item => `reagent-${item.ReagentId}`}
                            renderItem={item => this.renderItem(item)}
                            refreshing={!loaded}
                            onRefresh={() => this._refresh()} />
                    </View>
                }
            </View>
        );
    }

    renderItem({ item }) {
        return (
            <ListItem key={item.ReagentId}
                title={item.Name}
                onPress={() => this.navigate('LabsReagentsAdd', { labId: this.labId, reagent: item })} />
        );
    }

    async refresh(searchQuery) {
        searchQuery = searchQuery || '';

        if (this.state.loaded) {
            this.setState({
                loaded: false,
                query: searchQuery,
                reagents: this.state.reagents
            });
        } else {
            this.state.query = searchQuery;
        }

        let query = this.state.query;

        try {
            let reagents = await list(query);
            this.setState({ loaded: true, query, reagents });
        } catch(e) {
            // TODO: display error
            this.setState({ loaded: true, query, reagents: {} });
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
