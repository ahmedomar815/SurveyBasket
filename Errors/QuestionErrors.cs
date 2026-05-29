namespace SurveyBasket.Errors
{
    public class QuestionErrors
    {
        public static readonly Error QuestionNotFound = new("question.NotFound", "No question was found with the given Id", StatusCodes.Status404NotFound);
        public static readonly Error DuplicatedQuestonContent = new("Question.DuplicatedContent"
            , "Another question with the same content is already exists", StatusCodes.Status409Conflict);

    }
}
