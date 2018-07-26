﻿function populateQns(response) {
    var tablediv = document.getElementById("tbldiv");
    for (var i = 0; i < response.result.length; i++) {
        //Create two Inner divs to divide each block of result from the for loop
        //The first one serves the purpose of having an horizontal rule divide each block
        var innerdiv = document.createElement("div");
        var rowdiv = document.createElement("div");
        var summdiv = document.createElement("div");
        var statDateDiv = document.createElement("div");
        //define the class of the inner div
        // rowdiv.setAttribute("class", "problem-summary");
        //tablediv.setAttribute("class", "container");
        rowdiv.setAttribute("class", "row");
        summdiv.setAttribute("class", "col-lg-9");
        statDateDiv.setAttribute("class", "col-lg-3");
        //append the inner div to the outer div
        tablediv.appendChild(innerdiv);
        innerdiv.appendChild(rowdiv);
        rowdiv.appendChild(summdiv);

        //Summary Text is the Summary of each problem and it will be a link
        var summarytxt = document.createElement("a");
        //summarytxt.setAttribute("class", "summary");

        //define where the link routes to when it is clicked
        summarytxt.setAttribute("href", "/Answers/Details/" + response.result[i].Id);
        //below variables will hold elements that will store values from our problem result set
        //var status = document.createElement("div");
        //status.setAttribute("class", "timeStatus");
        var post_ModDate = document.createElement("div");
        //post_ModDate.setAttribute("class", "timeStatus");
        //post_ModDate.setAttribute("class", "col");
        var statusby = document.createElement("div");
        //statusby.setAttribute("class", "timeStatus");
        //statusby.setAttribute("class", "col");

        //assign values to the elements created above
        summarytxt.innerHTML = response.result[i].Summary;
        if (response.result[i].Status == 'answered') {
            statusby.innerHTML = response.result[i].Status + ' ' + response.result[i].latestAnswerBy;
        }
        else {
            statusby.innerHTML = response.result[i].Status + ' ' + response.result[i].UserId;
        }

        // var d =  Date(response.result[i].ModifiedDate);
        //if Modified date is not null, return modified date rather than the posted date
        //else return posted date

        //if (response.result[i].ModifiedDate != null) {
        //    var modified = new Date(response.result[i].ModifiedDate.match(/\d+/)[0] * 1);
        //    post_ModDate.innerHTML = new Date(modified.getFullYear(), modified.getDate() - 1, modified.getMonth() + 1, modified.getHours(),
        //        modified.getMinutes(), modified.getSeconds());
        //}
        //else {
        //    var posted = new Date(response.result[i].PostDate.match(/\d+/)[0] * 1);
        //    post_ModDate.innerHTML = new Date(posted.getFullYear(), posted.getDate() - 1, posted.getMonth() + 1, posted.getHours(),
        //        posted.getMinutes(), posted.getSeconds());
        //}
        if (response.result[i].ModifiedDate != '01/01/0001 00:00') {
            post_ModDate.innerHTML = response.result[i].ModifiedDate;
        }
        else {
            post_ModDate.innerHTML = response.result[i].PostDate;
        }

        summdiv.appendChild(summarytxt);
        // summdiv.appendChild(status);
        // summdiv.appendChild(statusby);
        //summdiv.appendChild(post_ModDate);
        rowdiv.appendChild(statDateDiv);
        statDateDiv.appendChild(statusby);
        statDateDiv.appendChild(post_ModDate);

        var hr = document.createElement("hr");
        innerdiv.appendChild(hr);
    }
}


function createCommentBox() {
    //create a button to add comment
    var addcomment = document.createElement("button");
    addcomment.innerHTML = "Add comment";
    addcomment.setAttribute("class", "btn btn-light");
    //append the button to the comment div
    var pComment = document.getElementById("comment");
    pComment.appendChild(addcomment);
    pComment.appendChild(document.createElement("br"));
    pComment.appendChild(document.createElement("br"));
    // once comment button is clicked, unhide text area
    var txtComment = document.createElement("textarea")
    txtComment.setAttribute("rows", "5");
    txtComment.setAttribute("cols", "100");

    addcomment.addEventListener("click", function () {
        pComment.appendChild(txtComment);

    });

}