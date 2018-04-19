import React from 'react';

export default class AutoRefreshable extends React.Component {
    constructor(props) {
        super(props);

        this.listener = null;
        this.refreshing = false;
    }

    componentDidMount() {
        super.componentDidMount();

        if (!this.props.navigation) {
            console.error('no navigation?');
            return;
        }

        this.listener = this.props.navigation.addListener('didFocus', event => {
            const componentKey = this.props.navigation.state.key;
            const focusKey = event.state.key;

            if (componentKey === focusKey) {
                component._refresh();
            }
        });
    }

    componentWillUnmount() {
        super.componentWillUnmount();

        if (this.listener) {
            this.listener.remove();
            this.listener = null;
        }
    }

    async refresh() {
        // descendents will override
        console.error('refresh() is not implemented');
    }

    async _doRefresh() {
        if (this.refreshing) {
            return;
        }

        this.refreshing = true;

        try {
            await this.refresh();
        } finally {
            this.refreshing = false;
        }
    }
};
