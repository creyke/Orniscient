
(function (orniscient, $, undefined) {

    var orniscientConnection,
        nodes = new vis.DataSet([]),
        edges = new vis.DataSet([]),
        container,
        summaryView = false;

    var options = {
        autoResize: true,
        height: '100%',
        nodes: {
            borderWidth: 3,
            shape: 'dot',
            scaling: {
                label: {
                    min: 8,
                    max: 20,
                    drawThreshold: 12,
                    maxVisible: 20
                }
            }
        },
        interaction: {
            hover: true,
            navigationButtons: true
        },
        layout: {
            randomSeed: 2
        }
    };

    orniscient.data = {
        nodes: nodes,
        edges: edges
    };

    orniscient.summaryView = function () { return summaryView; };

    orniscient.init = function () {
        console.log('orniscient.init was called');
        container = document.getElementById('mynetwork');
        var network = new vis.Network(container, orniscient.data, options);
        network.on("hoverNode", onHover);
        network.on('selectNode',
            function (params) {
                //this is where we will set id for grain details menu.
                window.dispatchEvent(new CustomEvent('nodeSelected', { detail: nodes.get(params.nodes)[0] }));
            });
        network.on('deselectNode',
            function () {
                window.dispatchEvent(new Event('nodeDeselected'));
            });

        orniscientConnection = new signalR.HubConnection("/orniscientHub");

        orniscientConnection.addMethod("grainActivationChanged", grainActivationChanged);
        
        orniscientConnection.start()
            .then(() => init())
            .catch(err => {
                console.log('connection error');
            });
    };

    orniscient.getServerData = function getServerData(filter) {
        summaryView = false; //need to reset the summary view here.
        orniscient.data.nodes.clear();
        orniscient.data.edges.clear();

        console.log('getting server data');
        if (filter === null)
            filter = {};

        return orniscientConnection.invoke("GetCurrentSnapshot", filter)
            .then((data) => {
                $.each(data.newGrains,
                    function (index, grainData) {
                        addToNodes(grainData, data.summaryView);
                    });

                if (data.summaryView === true) {
                    addSummaryViewEdges(data.summaryViewLinks);
                }

            })
            .catch(function (data) {
                alert('Oops, we cannot connect to the server...');
            });
    };

    function init() {
        return orniscient.getServerData();
    }

    function addToNodes(grainData, isSummaryView) {
        var nodeLabel = isSummaryView ? grainData.typeShortName + '(' + grainData.count + ')' : grainData.grainName;
        if (summaryView !== isSummaryView) {
            orniscient.data.nodes.clear();
            orniscient.data.edges.clear();
            summaryView = isSummaryView;
        }

        //find and update 
        var updateNode = orniscient.data.nodes.get(grainData.id);
        if (updateNode !== null && updateNode !== undefined) {
            updateNode.label = nodeLabel;
            updateNode.value = grainData.count;
            orniscient.data.nodes.update(updateNode);
            return;
        }


        var node = {
            id: grainData.id,
            label: nodeLabel,
            color: {
                border: grainData.colour
            },
            silo: grainData.silo,
            linkToId: grainData.linkToId,
            graintype: grainData.type,
            grainId: grainData.grainId,
            group: grainData.silo
        };

        if (grainData.count > 1 && isSummaryView === true) {
            node.value = grainData.count;
        }

        //add the node
        orniscient.data.nodes.add(node);

        if (isSummaryView === false) {
            //add the edge (link)
            if (grainData.linkToId && grainData.linkToId !== '') {
                orniscient.data.edges.add({
                    id: grainData.typeShortName + '_' + grainData.grainId + 'temp',
                    from: grainData.typeShortName + '_' + grainData.grainId,
                    to: grainData.linkToId,
                    label: ""
                });
            }
        }
    }


    function deleteNodes(grainData, isSummaryView) {
        var nodeToDelete = orniscient.data.nodes.get(grainData);
        if (nodeToDelete != undefined) {
            orniscient.data.nodes.remove(nodeToDelete);
        }
    }

    function addSummaryViewEdges(links) {
        orniscient.data.edges.clear();
        $.each(links, function (index, link) {
            var linkId = (link.fromId + '_' + link.toId).replace(/[^\w]/gi, '.');
            var updateEdge = orniscient.data.edges.get(linkId);
            if (updateEdge !== null && updateEdge !== undefined) {
                updateEdge.value = link.count;
                updateEdge.label = link.count;
                orniscient.data.edges.update(updateEdge);
            } else {
                orniscient.data.edges.add({
                    id: linkId,
                    from: link.fromId,
                    to: link.toId,
                    value: link.count,
                    label: link.count
                });
            }
        });
    }

    function onHover(params) {
        //get the node's information from the server.
        var node = nodes.get(params.node);
        if (node.serverCalled !== true) {

            var requestData = {
                GrainType: node.graintype,
                GrainId: node.grainId
            };

            var xhr = new XMLHttpRequest();
            xhr.open('post', orniscienturls.getGrainInfo, true);
            xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            xhr.onload = function () {
                var grainInfo = [];
                var infoRows = "";

                infoRows = infoRows + "<tr><td><strong>Grain Id</strong></td><td>" + node.grainId + "</td></tr>";
                infoRows = infoRows + "<tr><td><strong>Silo</strong></td><td>" + node.silo + "</td></tr>";
                infoRows = infoRows + "<tr><td><strong>Grain Type</strong></td><td>" + node.graintype + "</td></tr>";

                if (xhr.responseText && xhr.responseText !== "" && xhr.responseText !== "null") {
                    grainInfo = JSON.parse(xhr.responseText);
                    for (var i = 0; i < grainInfo.length; i++) {
                        infoRows = infoRows + "<tr><td><strong>" + grainInfo[i].filterName + "<strong></td><td>" + grainInfo[i].value + "</td></tr>";
                    }
                }
                node.title = "<h5>" + node.label + "</h5><table class='table'>" + infoRows + "</table>";
                node.serverCalled = false; //TODO : Change this back
                orniscient.data.nodes.update(node);
            };
            xhr.send(JSON.stringify(requestData));
        }
    }

    function grainActivationChanged(diffModel) {
        window.dispatchEvent(new CustomEvent('orniscientUpdated', { detail: diffModel.typeCounts }));
        $.each(diffModel.removedGrains, function (index, grainData) {
            deleteNodes(grainData, diffModel.summaryView);
        });

        $.each(diffModel.newGrains, function (index, grainData) {
            addToNodes(grainData, diffModel.summaryView);
        });

        if (diffModel.summaryView === true) {
            addSummaryViewEdges(diffModel.summaryViewLinks);
        }
    }

}(window.orniscient = window.orniscient || {}, jQuery));
