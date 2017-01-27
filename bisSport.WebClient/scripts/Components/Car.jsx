var Car = new React.createClass({
    getInitialState: function () {
        return {
            data: this.props.initialData
        };
    },

    render: function () {
        return <h2>{this.state.data.Email}</h2>;
    }
});