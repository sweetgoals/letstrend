function turnDarkGreen(idButton) 
{
    var customButton = document.getElementById('MainContent_' + idButton);
    if (customButton.style.backgroundColor == "") 
    {
        customButton.style.backgroundColor = "green";
        customButton.style.color = "white";       
    }
    else 
    {
        customButton.style.backgroundColor = "";
        customButton.style.color = "black";
    }
};

function onHide(hide, graph, buttonControl) 
{
    var bControl = document.getElementById('MainContent_' + buttonControl);
    var hid = document.getElementById('MainContent_' + hide);    
    $("#" + graph).show();
    bControl.style.backgroundColor = "green";
    bControl.style.color = "white";
    //renderChart(cName);
    selectChart(graph);
    hid.value = "on";

}

function offHide(hide, graph, buttonControl) 
{
    var bControl = document.getElementById('MainContent_' + buttonControl);
    var hid = document.getElementById('MainContent_' + hide);

    $("#" + graph).hide();
    hid.value = "off";
    bControl.style.backgroundColor = "";
    bControl.style.color = "black";
}

function setHide(hide,graph,buttonControl)
{
    //Turns the graph on and off. 
    var bControl = document.getElementById('MainContent_' + buttonControl);
    var hid = document.getElementById('MainContent_' + hide);

    if (hid.value == "on") 
    {
        $("#" + graph).hide();
        hid.value = "off";
        bControl.style.backgroundColor = "";
        bControl.style.color = "black";

    }
    else if (hid.value == "off") 
    {
        $("#" + graph).show();
        bControl.style.backgroundColor = "green";
        bControl.style.color = "white";
        //renderChart(cName);
        selectChart(graph);
        hid.value = "on";
    }
}

function iniGraphVis(hide,graph,buttonControl) 
{
    //Sets the postback state of the graph.
    //If it was on before it will be on after and vice versa.
    var bControl = document.getElementById('MainContent_' + buttonControl);
    var hid = document.getElementById('MainContent_' + hide);

    if (hid.value == "on") {
        $("#" + graph).show();
        selectChart(graph);
        bControl.style.backgroundColor = "green";
        bControl.style.color = "white";
    }
    else if (hid.value = "off") {
        $("#" + graph).hide();
        bControl.style.backgroundColor = "";
        bControl.style.color = "black";
    }
}

function selectChart(cName) 
{
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

