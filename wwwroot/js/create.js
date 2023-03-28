var connectionC = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/create")
    .build();

connectionC.on("ReceiveAddDocumentMessage", function (maxRoom, roomId, docName, userId, userName) {
    addMessage(`${userName} has created a document  ${docName}`);
    fillDocumentDropDown();
});

connectionC.on("ReceiveDeleteDocumentMessage", function (deleted, selected, docName, userName) {
    addMessage(`${userName} has deleted room  ${docName}`);
});

function addnewDocument(maxDocs) {

    let createDocName = document.getElementById('createDocName');

    var docName = createDocName.value;

    if (docName == null && docName == '') {
        return;
    }

    /*POST*/
    $.ajax({
        url: '/Documents/CreateDocument',
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ id: 0, name: docName }),
        async: true,
        processData: false,
        cache: false,
        success: function (json) {

            /*ADD ROOM COMPLETED SUCCESSFULLY*/
            connectionC.send("SendAddDocumentMessage", maxDocs, json.id, json.name);
            createDocName.value = '';
        },
        error: function (xhr) {
            alert('error');
        }
    })
}

function deleteDocument() {

    let ddlDelDoc = document.getElementById('ddlDelDoc');

    var docName = ddlDelDoc.options[ddlDelDoc.selectedIndex].text;

    let text = `Do you want to delete Document ${docName}?`;
    if (confirm(text) == false) {
        return;
    }
    if (docName == null && docName == '') {
        return;
    }
    let docId = ddlDelDoc.value;

    $.ajax({
        url: `/Documents/DeleteDocument/${docId}`,
        dataType: "json",
        type: "DELETE",
        contentType: 'application/json;',
        async: true,
        processData: false,
        cache: false,
        success: function (json) {

            connectionC.send("SendDeleteDocumentMessage", json.deleted, json.selected, docName);
            fillDocumentDropDown();
        },
        error: function (xhr) {
            alert('error');
        }
    })
}

document.addEventListener('DOMContentLoaded', (event) => {
    fillDocumentDropDown();
})


function fillDocumentDropDown() {

    $.getJSON('/Documents/GetDocuments')
        .done(function (json) {
            var ddlDelDoc = document.getElementById("ddlDelDoc");
            var ddlSelDoc = document.getElementById("ddlSelDoc");

            ddlDelDoc.innerText = null;
            ddlSelDoc.innerText = null;

            json.forEach(function (item) {
                var newOption = document.createElement("option");

                newOption.text = item.name;
                newOption.value = item.id;
                ddlDelDoc.add(newOption);

                var newOption1 = document.createElement("option");

                newOption1.text = item.name;
                newOption1.value = item.id;
                ddlSelDoc.add(newOption1);
            });
        })
        .fail(function (jqxhr, textStatus, error) {

            var err = textStatus + ", " + error;
            console.log("Request Failed: " + jqxhr.detail);
        });
}

function addMessage(msg) {
    if (msg == null && msg == '') {
        return;
    }
    let ui = document.getElementById('messagesList');
    let li = document.createElement("li");
    li.innerHTML = msg;
    li.style.listStyleType = 'none';
    ui.appendChild(li);
}

connectionC.start();