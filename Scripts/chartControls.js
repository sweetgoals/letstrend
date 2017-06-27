function turnDarkGreen(idButton) {
    var customButton = document.getElementById('MainContent_' + idButton);
    if (customButton.style.backgroundColor == "") {
        customButton.style.backgroundColor = "green";
        customButton.style.color = "white";
    }
    else {
        customButton.style.backgroundColor = "";
        customButton.style.color = "black";
    }
};

function onHide(hide, graph, comboBoxControl) {
    var bControl = document.getElementById('MainContent_' + comboBoxControl);
    var hid = document.getElementById('MainContent_' + hide);
    $("#" + graph).show();
    selectChart(graph);
    hid.value = "on";

}

function offHide(hide, graph, comboBoxControl) {
    graph.hide();
    hide.value = "off";
}

function setHide(hide, graph, comboBoxControl) {
    //Turns the graph on and off. 

    if (hide.val() == "on") {
        $("#" + graph).hide();
        hide.val = "off";

    }
    else if (hide.val() == "off") {
        graph.show();
        selectChart(graph);
        hide.value = "on";
    }
}

function iniGraphVis(blah, graph, comboBoxControl) {
    //Sets the postback state of the graph.
    //If it was on before it will be on after and vice versa.
    var hideValue;
    hideValue = 1;

    if (hideValue = "on") {
        graph.show();
        selectChart(graph);
    }
    else if (hideValue = "off") {
        //graph.hide();
    }
}

function selectChart(cName) {
    //Selects which graph to make
    if (cName == "volumeGraph")
        renderVolChart();
    else if (cName == "stochGraph")
        renderStochChart();
    else if (cName == "macdGraph")
        renderMacdChart();
    else if (cName == "rsiGraph")
        renderRsiChart();
    else if (cName == "rocGraph")
        renderRocChart();
    else if (cName == "stockGraph")
        renderStockChart();

}

