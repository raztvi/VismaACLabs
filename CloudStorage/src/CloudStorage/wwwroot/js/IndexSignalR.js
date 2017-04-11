$(function() {
    $.connection.hub.logging = true;

    $.connection.hub.error(function(err) {
        console.log("An error occured: " + err);
    });

    var fileEvents = $.connection.fileOperationsHub;

    fileEvents.client.pongHello = function(data) {
        console.log("Got back: " + data);
    };

    fileEvents.client.fileModified = function(data) {
        console.log(data);

        var element = '<div class="alert alert-info alert-dismissible" role="alert">';
        element +=
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close"> <span aria-hidden="true">&times;</span></button >';
        element += data;
        element += '</div>';

        $("#notifications").append(element);
    };
    $.connection.hub.start()
        .done(function() {
            fileEvents.server.pingHello("yo yo yo, I'm connected!");

        });
});