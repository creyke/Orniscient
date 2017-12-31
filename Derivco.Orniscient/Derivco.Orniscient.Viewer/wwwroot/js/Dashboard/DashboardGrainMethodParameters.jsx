var DashboardGrainMethodParameters = React.createClass({
    render: function() {
        var methodData = this.props.data;
        var displayParameters = (<label for="parameters">Select a method.</label>);

        if (methodData != null) {
            if (methodData.parameters.length > 0) {
                displayParameters = methodData.parameters.map(function (parameter) {

                    var inputElement = parameter.IsComplexType === true ?
                                            (<textarea rows="5" className="form-control" id={parameter.name} placeholder={parameter.type}></textarea>) :
                                            (<input type="text" className="form-control" id={parameter.name} placeholder={parameter.type}/>);

                    return (
                            <div className="" key={parameter.name}>
                                <label for="parameter" className="">{parameter.name} : </label>
                                {inputElement}
                            </div>
                            );
                }, this);
            } else {
                displayParameters = (<label for="parameters">{methodData.label} has no parameters.</label>);
            }
        }

        return (
            <div className="">
                <h5>Parameters</h5>
                    {displayParameters}
    </div>);
    }
});

