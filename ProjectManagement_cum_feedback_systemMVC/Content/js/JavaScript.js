
    function like(id) {

        $.ajax('/Post/like/' + id, {
            type: 'POST',  // http method
            data: { myData: 'This is my data.' },  // data to submit
            success: function (data, status, xhr) {
                if (data == "already voted") {
                    alert(data);
                }
                else {
                    if (parseInt(data) >= 1000) {
                        $("#" + id).html(Math.Floor(parseInt(data) / 1000) + "K");
                    }
                    else {
                        $("#" + id).html(data);
                    }                 
                }
            },
            error: function (jqXhr, textStatus, errorMessage) {
                alert(errorMessage);
            }
        });
}
function loadissues(id) {    
    
    $.ajax('/Post/getallissue/' + id, {
        type: 'GET',  // http method
        data: { myData: 'This is my data.' },  // data to submit
        success: function (data, status, xhr) {            
            if (data != "error") {                                
                var temp="";
                for (i = 0; i < data.length; i++) {
                    temp += "<option value='"+data[i].issue_Id + "'>" + data[i].issue_title + "</option>"; 
                }
                console.log(temp);
                $("#issus_list").html(temp);
                $('#myModal').modal('show');
            }
            else
                alert("error");
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}
/*function getstatus(id)
{
    $.ajax('/Post/getstatus/' + id, {
        type: 'GET',  // http method
        data: { myData: 'This is my data.' },  // data to submit
        success: function (data, status, xhr) {
            console.log(data);
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}*/