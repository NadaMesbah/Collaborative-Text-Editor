Quill.register('modules/cursors', QuillCursors);

var quill = new Quill("#documentEditor", {
    modules: {
        toolbar: [
            ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
            ['blockquote', 'code-block'],

            [{ 'header': 1 }, { 'header': 2 }],               // custom button values
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
            [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
            [{ 'direction': 'rtl' }],                         // text direction

            [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
            ['link', 'image', 'video', 'formula'],          // add's image support
            [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
            [{ 'font': [] }],
            [{ 'align': [] }],

            ['clean']                                         // remove formatting button
        ],
        cursors: {
            transformOnTextChange: true,

        },

    },
    theme: "snow"
});
const cursors = quill.getModule('cursors');
let colors = ['blue', 'red', 'green', 'orange', 'BlueViolet', 'Aqua', 'BurlyWood', 'Coral', 'Crimson']
let myCursor;


let userName = document.getElementById("hdUserName").value;
let userId = document.getElementById("hdUserId").value;

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/texteditor")
    .build();

connection
    .start()
    .then(res => {
        connection.invoke('JoinGroup', "global"); //${groupName}

        connection.on("newMemberJoined", function (userId, userName, groupName) {
            toastr.success(`${userName} has joined the document ${groupName}`);
        });

        connection.on("newMemberLeft", function (userId, userName) {
            toastr.info(`${userName} has left the document global`);
        });
        
        console.log(connection.connectionId)
        myCursor = cursors.createCursor(userId, userName, colors[Math.floor(Math.random() * colors.length)]);
     
        quill.on('selection-change', function (range, oldRange, source) {
            if (source != "user") return
            connection.invoke('CursorSend', JSON.stringify(range), JSON.stringify(myCursor))
            console.log(range);
        });

        connection.on('cursorReceive', (range, cursor) => {
            console.log('cursor invoked')
            range = JSON.parse(range)
            cursor = JSON.parse(cursor)
            console.log(cursor)
            if (cursor.id == userId)
                return;
            cursors.createCursor(cursor.id, cursor.name, cursor.color);
            cursors.moveCursor(cursor.id, range)
        })

        // Send updates to the server when the document content changes
        quill.on("text-change", function (delta, oldDelta, source) {
            if (source == "user") {
                connection.invoke("SendMessage", JSON.stringify(delta));
            }
        });
        // Subscribe to updates from the server
        connection.on("updateDocument", function (content) {
            // Update the document content with the received update
            console.log(content);
            quill.updateContents(JSON.parse(content));
        });
    })
    .catch(function (error) {
        console.error("Error connecting to SignalR hub:", error);
    });