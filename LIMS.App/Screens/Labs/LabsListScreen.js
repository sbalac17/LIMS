import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { SearchBar, ListItem, Button } from 'react-native-elements';
import { list } from '../../DataAccess/LabsDao';
import { debounce } from 'lodash';
import AutoRefreshable from '../../Components/AutoRefreshable';

export default class LabsListScreen extends AutoRefreshable {
    static navigationOptions = {
        title: 'Labs',
        drawerLabel: 'Work Setup'
    };

    constructor(props) {
        super(props);

        this.state = {
            loaded: false,
            permissions: {},
            query: '',
            labs: {},
        };

        this.search = debounce(query => this.refresh(query), 300);
    }
    
    render() {
        const { navigate } = this.props.navigation;
        let labs = this.state.labs;
        let permissions = this.state.permissions || labs.$permissions;
        let loaded = this.state.loaded;

        function renderItem({ item }) {
            return (
                <ListItem key={item.LabId}
                    title={`${item.CourseCode} (week ${item.WeekNumber})`}
                    disabled={!item.IsMember}
                    onPress={() => navigate('LabsDetails', { labId: item.LabId })} />
            );
        }

        return (
            <View style={styles.container}>
                {permissions.CanCreate &&
                    <View style={{ flexDirection: 'row', marginTop: 15, marginBottom: 15 }}>
                        <View style={{ flex: 1 }}>
                            <Button title='Add'
                                buttonStyle={{ backgroundColor: '#3a3' }}
                                onPress={() => navigate('LabsCreate')} />
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
                        <FlatList data={labs.Results}
                            keyExtractor={item => `lab-${item.LabId}`}
                            renderItem={renderItem}
                            refreshing={!loaded}
                            onRefresh={() => this.refresh()} />
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
                query: searchQuery || '',
                labs: this.state.labs
            });
        } else {
            this.state.query = searchQuery;
        }

        let query = this.state.query;

        try {
            let labs = await list(query);
            this.setState({ loaded: true, permissions: labs.$permissions, query, labs });
        } catch(e) {
            // TODO: display error
            this.setState({ loaded: true, permissions: this.state.permissions, query, labs: {} });
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
