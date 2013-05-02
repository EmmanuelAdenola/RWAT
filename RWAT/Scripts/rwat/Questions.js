//ko represents usage of knockout namespace
function QuestionWrapper(questionWrapper) {
    var self = this;
    self.question = questionWrapper["Question"];
    self.answers = questionWrapper["Answers"];
    self.user = questionWrapper["User"];
    console.log(self.question);
    self.Link = "/Questions/Question?id="+self.question["_id"];
}

function QuestionsViewModel(initValue) {
    var self = this;
    self.questions = ko.observableArray(initValue);
}
  
    
function initQuestionsHub(initQuestions)
{
    var questionhub = $.connection.questionhub;
        
    var questionsArray = $.map(initQuestions, function (questionWrapper) {
        return new QuestionWrapper(questionWrapper);
    });
        
    var questionsViewModel = new QuestionsViewModel(questionsArray);
    ko.applyBindings(questionsViewModel);


    questionhub.client.showQuestion = function (questionWrapper) {
        console.log(questionWrapper);
        var qw = $.parseJSON(questionWrapper);
        questionsViewModel.questions.push(new QuestionWrapper(qw));
    };

    $.connection.hub.start();
}