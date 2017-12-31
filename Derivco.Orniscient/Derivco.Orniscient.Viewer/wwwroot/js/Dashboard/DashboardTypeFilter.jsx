/*
    This is the filter grouping for the type.
*/
var DashboardTypeFilter = React.createClass({
    propTypes: {
        filterSelected: React.PropTypes.func
    },
   
    filterSelected: function (filterName,val ) {
        console.log('dashboardTypeFilter filterselected');
        this.props.filterSelected(this.props.data.typeName,filterName, val);
    },
    render: function() {
        var filters = this.props.data.filters.map(function (filter) {
            return (<DashboardTypeFilterRow data={filter} filterSelected={this.filterSelected} key={filter.filterName } typeName={this.props.data.typeName}/>);
        },this);

        return (
            <div>
                <h5>{this.props.data.typeNameShort}</h5>
                {filters}
            </div>
        );
    }

});

var DashboardTypeFilterRow = React.createClass({
    contextTypes: {
        selectedFilters: React.PropTypes.object
    },
    propTypes: {
        filterSelected: React.PropTypes.func
    },
    getselectedfilters() {

        var selectedFilters = this.context.selectedFilters;
        if (selectedFilters === undefined) {
            return null;
        }

        if (selectedFilters[this.props.typeName] === undefined) {
            return null;
        }
        var filterid = this.props.data.filterName.replace(/[^\w]/gi, '.'); //remove special characters
        if (selectedFilters[this.props.typeName][filterid] === undefined) {
            return null;
        }

        return selectedFilters[this.props.typeName][filterid];
    },
    filterSelected : function(val) {
        this.props.filterSelected(this.props.data.filterName,val);
    },
    getInitialState: function () {
        return {
            SelectedValue :[]
        }
    },
    render: function() {
        var filterValues = orniscientutils.stringArrToSelectOptions(this.props.data.values);
        return (
            <div className="form-group" key={this.props.data.filterName}>
                    <label>{this.props.data.filterName}</label>
                    <Select name="form-field-name" options={filterValues} multi={true} onChange={this.filterSelected} value={this.getselectedfilters()} />
                </div>
            );
    }
});