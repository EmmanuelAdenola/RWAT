//ko represents usage of knockout namespace
function VoteViewModel(voteModel) {
    var self = this;
    self.voteViewModel = voteModel;
    self.upVotePath = ko.observable((voteModel["SelectedUpVotePath"] != null && voteModel["SelectedUpVotePath"] != undefined  && voteModel["SelectedUpVotePath"] != ""  ) ? voteModel["SelectedUpVotePath"] : voteModel["NoUpVotePath"]);
    self.downVotePath = ko.observable((voteModel["SelectedDownVotePath"] != null && voteModel["SelectedDownVotePath"] != undefined && voteModel["SelectedDownVotePath"] != "") ? voteModel["SelectedDownVotePath"] : voteModel["NoDownVotePath"]);
    self.currentVote = ko.observable(voteModel["CurrentVote"]);
    self.imagepath = ko.observable(voteModel["ImagePath"]);
}
    
function AnswerViewModel(answer) {
    this.answer = answer.Answer;
    this.answerer = answer.Answerer;
    this.gravatar = answer.gravatar;
    this.voteViewModel = new VoteViewModel(answer["VoteViewModel"]);
}
  

function Question(question) {
    var self = this;
    self.question = question["Question"];
    self.user = question["User"];
    self.voteViewModel =new VoteViewModel(question["VoteViewModel"]);
    self.gravatar = "http://www.gravatar.com/avatar/" + question["User"]["HashedEmail"] + "?d=identicon&f=y&s=50";
        
    var answerArray = $.map(question["Answers"], function (answer) {
        answer.gravatar = "http://www.gravatar.com/avatar/" + answer["Answerer"]["HashedEmail"] + "?d=identicon&f=y&s=50";
        return new AnswerViewModel(answer);
    });

    self.answers = ko.observableArray(answerArray);
}
    
  
function initQuestionHub(initialQuestion)
{
    var question = new Question(initialQuestion);
    var questionhub = $.connection.questionhub;
    var answerhub = $.connection.answerhub;

    answerhub.client.showAnswer = function (answer) {
        answer.gravatar = "http://www.gravatar.com/avatar/" + answer["Answerer"]["HashedEmail"] + "?d=identicon&f=y&s=50";
        question.answers.push(new AnswerViewModel(answer));
    };

    questionhub.client.updateQuestionVote = function (currentVote,errorMessage) {
        question.voteViewModel.currentVote(currentVote);
        if(errorMessage!=null && errorMessage!=undefined&& errorMessage!="") {
            alert(errorMessage);
        }
    };

    questionhub.client.updateQuestionUpVoteImage = function (img) {
        question.voteViewModel.downVotePath(question.voteViewModel.voteViewModel.NoDownVotePath);
        question.voteViewModel.upVotePath(img);
    };
    questionhub.client.updateQuestionDownVoteImage = function (img) {
        question.voteViewModel.upVotePath(question.voteViewModel.voteViewModel.NoUpVotePath);
        question.voteViewModel.downVotePath(img);  
    };
        
    $("#up").click(function () {
        question.voteViewModel.upVotePath( question.voteViewModel.voteViewModel["UpVotePath"]);
        questionhub.server.upVote(question.question["_id"]);
    });
        
    $("#down").click(function () {
        question.voteViewModel.downVotePath(question.voteViewModel.voteViewModel["DownVotePath"]);
        questionhub.server.downVote(question.question["_id"]);
    });

    answerhub.client.updateAnswerVote = function (answerid, currentVote, errorMessage) {
        var answers = question.answers();
        for (var idx in answers) {
            var currAnswer = answers[idx];
            if (answerid == currAnswer["answer"]["_id"]) {
                currAnswer.voteViewModel.currentVote(currentVote);
                if (errorMessage != null && errorMessage != undefined && errorMessage != "") {
                    alert(errorMessage);
                }
            }
        }
    };

    answerhub.client.updateAnswerUpVoteImage = function (answerid, img) {
        var answers = question.answers();

        for (var idx in answers) {
            var currAnswer = answers[idx];
            if (answerid == currAnswer["answer"]["_id"]) {
                currAnswer.voteViewModel.downVotePath(currAnswer.voteViewModel.voteViewModel.NoDownVotePath);
                currAnswer.voteViewModel.upVotePath(img);
            }
        }
    };
        
    answerhub.client.updateAnswerDownVoteImage = function (answerid, img) {
        var answers = question.answers();
        for (var idx in answers) {
            var currAnswer = answers[idx];
            if (answerid == currAnswer["answer"]["_id"]) {
                currAnswer.voteViewModel.upVotePath(currAnswer.voteViewModel.voteViewModel.NoUpVotePath);
                currAnswer.voteViewModel.downVotePath(img);
            }
        }
          
    };
        
    $(document).on("click", ".voteanswerup", function () {
        var idSearch = $(this).attr('data-id');
        var answers = question.answers();
        for (var idx in answers) {
            var currAnswer = answers[idx];
            if (idSearch == currAnswer["answer"]["_id"]) {
                currAnswer.voteViewModel.upVotePath(currAnswer.voteViewModel.voteViewModel["UpVotePath"]);
                answerhub.server.upVote(currAnswer["answer"]["_id"]);
            }
        }
    });
        
    $(document).on("click", ".voteanswerdown", function () {
        var idSearch = $(this).attr('data-id');
        var answers = question.answers();
        for (var idx in answers) {
            var currAnswer = answers[idx];
            if (idSearch == currAnswer["answer"]["_id"]) {
                currAnswer.voteViewModel.downVotePath(question.voteViewModel.voteViewModel["DownVotePath"]);
                answerhub.server.downVote(currAnswer.answer["_id"]);
            }
        }
    });
        
    $.connection.hub.start();
    ko.applyBindings(question);
}
