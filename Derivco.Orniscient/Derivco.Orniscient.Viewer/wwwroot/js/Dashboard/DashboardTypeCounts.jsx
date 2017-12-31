﻿var DashboardTypeCounts = React.createClass({
    render: function() {
        var items = this.props.data.map(function(typeCounter) {
            return (<li className="" key={typeCounter.typeName }>{typeCounter.typeName} : <strong>{typeCounter.total} </strong></li>);
        });

        return (
            items.length > 0 &&
            (<div>
                <h4>Grain Type Counts</h4>
                <ul className="">
                    {items}
                </ul>
            </div>)
        );
    }
});