//ko represents usage of knockout namespace

function WordCountViewModel(initialWordCount) {
    var self = this;
    self.wordCount = ko.observable(initialWordCount);

}


$(function () {
    var maxWordCount = 500;
    var wordCountViewModel = new WordCountViewModel(maxWordCount);

    $(document).on("keyup", "#Description", function () {
        var value = maxWordCount - $(this).val().length;
        if (value > 0) {
            wordCountViewModel.wordCount(value);
        }
        else {
            wordCountViewModel.wordCount(0);
            $(this).val($(this).val().substring(0, maxWordCount + 1));
        }
    });
    ko.applyBindings(wordCountViewModel);
});