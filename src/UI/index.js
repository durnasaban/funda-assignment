const topAgentsUrl = "http://localhost:5000/api/top-agents";

function loadTopAgentsInAmsterdam() {
    loadTopAgents("TopAgentsInAmsterdam", "topAgentsInAmsterdamBody");
}

function loadTopAgentsInAmsterdamWithGarden() {
    loadTopAgents("TopAgentsInAmsterdamWithGarden", "topAgentsInAmsterdamWithGardenBody");
}

function loadTopAgents(key, bodyId) {
    fetch(`${topAgentsUrl}/${key}`)
        .then(res => res.json())
        .then(ta => loadTopAgentsToTable(bodyId, ta));
}

function loadTopAgentsToTable(bodyId, items) {

    const table = document.getElementById(bodyId);

    items.forEach(function (item, index) {
        let row = table.insertRow();

        setValueToCell(row, 0, index + 1);
        setValueToCell(row, 1, item.agentName);
        setValueToCell(row, 2, item.objectCount);
    });

    function setValueToCell(row, cellNum, value) {
        let agentName = row.insertCell(cellNum);
        agentName.innerHTML = value;
    }
}