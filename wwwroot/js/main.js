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
let colors = ['blue', 'red', 'yellow', 'green', 'orange', 'BlueViolet', 'Aqua', 'BurlyWood', 'Coral', 'Crimson']
let myCursor;

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/texteditor")
    .build();

connection
    .start()
    .then(res => {
        connection.invoke('JoinGroup', "global");
        console.log(connection.connectionId)
        myCursor = cursors.createCursor(connection.connectionId, "userX", colors[Math.floor(Math.random() * colors.length)]);
        connection.on('cursorReceive', (range, cursor) => {
            console.log('cursor invoked')
            range = JSON.parse(range)
            cursor = JSON.parse(cursor)
            console.log(cursor)
            cursors.createCursor(cursor.id, cursor.name, cursor.color);
            cursors.moveCursor(cursor.id, range)
        })
        quill.on('selection-change', function (range, oldRange, source) {
            if (source != "user") return
            connection.invoke('CursorSend', JSON.stringify(range), JSON.stringify(myCursor))
        });

        // Subscribe to updates from the server
        connection.on("updateDocument", function (content) {
            // Update the document content with the received update
            console.log(content);
            quill.updateContents(JSON.parse(content));

        });
        // Send updates to the server when the document content changes
        quill.on("text-change", function (delta, oldDelta, source) {
            if (source == "user") {
                connection.invoke("SendMessage", JSON.stringify(delta));
            }

            //// Extract the content of the Quill editor
            //var content = quill.root.innerHTML;

            //// Insert the content into the output element
            //document.getElementById("output").innerHTML = content;
        });
    })
    .catch(function (error) {
        console.error("Error connecting to SignalR hub:", error);
    });