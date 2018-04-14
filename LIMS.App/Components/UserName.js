import React from 'react';
import { StyleSheet, View } from 'react-native';
import { Text, Badge } from 'react-native-elements';

export default class UserName extends React.Component {
    render() {
        let user = this.props.user;

        return (
            <View style={styles.wrapper}>
                <Text style={styles.text}>{user.UserName}</Text>

                {user.IsLabManager &&
                    <Badge value='Lab Manager' wrapperStyle={styles.badge} containerStyle={styles.badgeBg} textStyle={styles.badgeText} />
                }
            </View>
        )
    }
}

const styles = StyleSheet.create({
    wrapper: {
        flexDirection: 'row',
        justifyContent: 'flex-start',
        alignItems: 'baseline',
    },
    text: {
        flex: 0,
        color: '#000',
    },
    badge: {
        flex: 0,
        marginLeft: 5,
    },
    badgeBg: {
        backgroundColor: '#34f',
    },
    badgeText: {
        color: '#fff',
        fontSize: 12,
    },
});
