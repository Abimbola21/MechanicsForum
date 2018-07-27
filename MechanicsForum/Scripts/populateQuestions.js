function populateQns(response) {
    var tablediv = document.getElementById("tbldiv");
    for (var i = 0; i < response.result.length; i++) {
        //Create two Inner divs to divide each block of result from the for loop
        //The first one serves the purpose of having an horizontal rule divide each block
        var innerdiv = document.createElement("div");
        var rowdiv = document.createElement("div");
        var summdiv = document.createElement("div");
        var statDateDiv = document.createElement("div");
        //define the class of the inner div
        rowdiv.setAttribute("class", "row");
        summdiv.setAttribute("class", "col-lg-8");
        statDateDiv.setAttribute("class", "col-lg-4");
        //append the inner div to the outer div
        tablediv.appendChild(innerdiv);
        innerdiv.appendChild(rowdiv);
        rowdiv.appendChild(summdiv);

        //Summary Text is the Summary of each problem and it will be a link
        var summarytxt = document.createElement("a");
        //summarytxt.setAttribute("class", "summary");
        //lblansCount will be assigned the number of answers to each problem
        var lblansCount = document.createElement("label");
        lblansCount.setAttribute("class","col-lg-2")

        //define where the summary text link routes to when it is clicked
        summarytxt.setAttribute("href", "/Answers/Details/" + response.result[i].g[0].Id);
        //below variables will hold elements that will store values from our problem result set
        var post_ModDate = document.createElement("div");
        var statusby = document.createElement("div");

        //assign values to the elements created above
        summarytxt.innerHTML = response.result[i].g[0].Summary;
        //im using the tenary operator to find the number of answers to a question
        //if ProblemId is 0, it means there are no answers to this questions
        lblansCount.innerHTML = ((response.result[i].g[0].ProblemId) != 0) ?
            response.result[i].count + " answers":
            "0 answers";
        if (response.result[i].Status == 'answered') {
            statusby.innerHTML = response.result[i].g[0].Status + ' ' + response.result[i].g[0].latestAnswerBy;
        }
        else {
            statusby.innerHTML = response.result[i].g[0].Status + ' ' + response.result[i].g[0].UserId;
        }

        if (response.result[i].ModifiedDate != '01/01/0001 00:00') {
            post_ModDate.innerHTML = response.result[i].g[0].ModifiedDate;
        }
        else {
            post_ModDate.innerHTML = response.result[i].g[0].PostDate;
        }

        summdiv.appendChild(summarytxt);
        summdiv.appendChild(document.createElement("br"));
        summdiv.appendChild(document.createElement("br"));
        summdiv.appendChild(lblansCount);
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