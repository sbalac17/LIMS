import React from 'react';
import { StyleSheet, View, KeyboardAvoidingView, ScrollView } from 'react-native';

export default class LabsEditScreen extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <KeyboardAvoidingView style={styles.grow} contentContainerStyle={styles.container} behavior="padding" keyboardVerticalOffset={60}>
                <View style={styles.grow}>
                    <ScrollView style={[styles.grow, styles.wrap]}>
                        {this.props.children}
                    </ScrollView>
                </View>
            </KeyboardAvoidingView>
        );
    }
};

const styles = StyleSheet.create({
    grow: {
        flex: 1,
    },
    wrap: {
        margin: 15,
    }
});
