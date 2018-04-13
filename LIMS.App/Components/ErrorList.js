import React from 'react';
import { StyleSheet, FlatList } from 'react-native';
import { FormValidationMessage } from 'react-native-elements';

export default class ErrorList extends React.Component {
    render() {
        let errors = this.props.errors;

        // wrap in an array for FlatList
        if (errors && typeof errors === "string") {
            errors = [ errors ];
        }

        // empty error list
        if (errors.length == 0) {
            return null;
        }

        function renderItem({ item }) {
            return (
                <FormValidationMessage>{item}</FormValidationMessage>
            );
        }

        return (
            <FlatList data={errors} renderItem={renderItem} keyExtractor={item => item} />
        );
    }
}

const styles = StyleSheet.create({
});
