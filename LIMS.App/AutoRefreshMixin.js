export default function autoRefresh(component) {
    if (typeof component._refresh !== 'function') {
        throw new Error('Component cannot be refreshed');
    }

    component.props.navigation.addListener('didFocus', event => {
        component._refresh();
    });

    if (!component.state.loaded) {
        component._refresh();
    }
}
