import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { SearchBar, ListItem, Button } from 'react-native-elements';
import { list } from '../../DataAccess/TestsDao';
import { debounce } from 'lodash';

export default class TestsListScreen extends React.Component {
    static navigationOptions = {
        title: 'Tests',
        drawerLabel: 'Tests'
    };

    constructor(props) {
        super(props);

        this.state = {
            loaded: false,
            permissions: {},
            query: '',
            tests: {},
        };

        this.search = debounce(query => this._refresh(query), 500);
        this._refresh();
    }
    
    render() {
        const { navigate } = this.props.navigation;
        let tests = this.state.tests;
        let permissions = this.state.permissions || tests.$permissions;
        let loaded = this.state.loaded;

        function renderItem({ item }) { 
            return (
                <ListItem key={item.TestId}
                    title={item.Name}
                    onPress={() => {
                        console.log('navigate to test details'); 
                        navigate('TestsDetails', { testId: item.TestId })}} />
            );
        }

        return (
            <View style={styles.container}>
                {permissions.CanCreate &&
                    <View style={{ flexDirection: 'row', marginTop: 15, marginBottom: 15 }}>
                        <View style={{ flex: 1 }}>
                            <Button title='Create'
                                buttonStyle={{ backgroundColor: '#3a3' }}
                                onPress={() => navigate('TestsCreate')} />
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
                        <FlatList data={tests.Results}
                            keyExtractor={item => item.TestId}
                            renderItem={renderItem}
                            refreshing={!loaded}
                            onRefresh={() => this._refresh()} />
                    </View>
                }
            </View>
        );
    }

    async _refresh(searchQuery) {
        let query = searchQuery || this.state.query;

        this.setState({
            loaded: false,
            permissions: this.state.permissions,
            query,
            tests: this.state.tests
        });

        try {
            let tests = await list(query);
            this.setState({ loaded: true, permissions: tests.$permissions, query, tests });
        } catch(e) {
            this.setState({ loaded: true, permissions: this.state.permissions, query, tests: {} });
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
