import React from 'react';
import { StyleSheet } from 'react-native';
import { Badge } from 'react-native-elements';

export default class StatusBadge extends React.Component {
    render() {
        let status = this.props.status;
        let text = 'Unknown';
        let textColor = styles.text;
        let color = styles.gray;

        switch (status) {
            case 0:
                text = 'In Progress';
                textColor = styles.textInvert;
                color = styles.yellow;
                break;

            case 1:
                text = 'Approved';
                color = styles.green;
                break;

            case 2:
                text = 'Rejected';
                color = styles.red;
                break;
        }

        return (
            <Badge value={text} wrapperStyle={styles.wrapper} containerStyle={color} textStyle={textColor} />
        )
    }
}

const styles = StyleSheet.create({
    wrapper: {
        flex: 0,
        alignSelf: 'flex-start',
    },
    text: {
        color: '#fff',
        fontSize: 12,
    },
    textInvert: {
        color: '#000',
        fontSize: 12,
    },
    yellow: {
        backgroundColor: '#aa3',
    },
    green: {
        backgroundColor: '#3a3',
    },
    red: {
        backgroundColor: '#c33',
    },
    gray: {
        backgroundColor: '#444',
    },
});
