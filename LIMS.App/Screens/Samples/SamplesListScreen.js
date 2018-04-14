import React from 'react';
import { StyleSheet, Text, View, FlatList, ActivityIndicator } from 'react-native';
import { SearchBar, ListItem, Button } from 'react-native-elements';
import { list } from '../../DataAccess/SamplesDao';
import { debounce } from 'lodash';

export default class SamplesListScreen extends React.Component {
    static navigationOptions = {
        title: 'Samples',
        drawerLabel: 'Samples'
    };

    constructor(props) {
        super(props);

        this.state = {
            loaded: false,
            permissions: {},
            query: '',
            samples: {},
        };

        this.search = debounce(query => this._refresh(query), 300);
        this._refresh();
    }
    
    render() {
        const { navigate } = this.props.navigation;
        let samples = this.state.samples;
        let permissions = this.state.permissions || samples.$permissions;
        let loaded = this.state.loaded;

        function renderItem({ item }) {
            return (
                <ListItem key={item.Sample.SampleId}
                    title={item.Sample.Description}
                    onPress={() => navigate('SamplesDetails', { sampleId: item.Sample.SampleId })} />
            );
        }

        return (
            <View style={styles.container}>
                {permissions.CanCreate &&
                    <View style={{ flexDirection: 'row', marginTop: 15, marginBottom: 15 }}>
                        <View style={{ flex: 1 }}>
                            <Button title='Add'
                                buttonStyle={{ backgroundColor: '#3a3' }}
                                onPress={() => navigate('SamplesCreate')} />
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
                        <FlatList data={samples.Results}
                            keyExtractor={item => `sample-${item.Sample.SampleId}`}
                            renderItem={renderItem}
                            refreshing={!loaded}
                            onRefresh={() => this._refresh()} />
                    </View>
                }
            </View>
        );
    }

    async _refresh(searchQuery) {
        searchQuery = searchQuery || '';
        
        if (this.state.loaded) {
            this.setState({
                loaded: false,
                permissions: this.state.permissions,
                query: searchQuery,
                samples: this.state.samples
            });
        } else {
            this.state.query = searchQuery;
        }

        let query = this.state.query;

        try {
            let samples = await list(query);
            this.setState({ loaded: true, permissions: samples.$permissions, query, samples });
        } catch(e) {
            // TODO: display error
            this.setState({ loaded: true, permissions: this.state.permissions, query, samples: {} });
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
