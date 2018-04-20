import React from 'react';

export default class AutoRefreshable extends React.Component {
    constructor(props) {
        super(props);

        this.listener = null;
        this.refreshing = false;
    }

    componentDidMount() {
        if (super.componentDidMount) {
            super.componentDidMount();
        }

        if (!this.props.navigation) {
            console.error('component has no navigation');
            return;
        }

        this.listener = this.props.navigation.addListener('didFocus', event => {
            const componentKey = this.props.navigation.state.key;
            const focusKey = event.state.key;

            if (componentKey === focusKey) {
                this.refresh();
            }
        });

        this.refresh();
    }

    componentWillUnmount() {
        if (super.componentWillUnmount) {
            super.componentWillUnmount();
        }

        if (this.listener) {
            this.listener.remove();
            this.listener = null;
        }
    }

    async refreshImpl() {
        // descendents will override
        console.error('refresh() is not implemented');
    }

    async refresh() {
        if (this.refreshing) {
            return;
        }

        this.refreshing = true;

        try {
            await this.refreshImpl();
        } finally {
            this.refreshing = false;
        }
    }
};
