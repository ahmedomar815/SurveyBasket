namespace SurveyBasket.Errors
{
    public class PollErrors
    {
        public static readonly Error PollNotFound = new("poll.NotFound", "No poll was found with the given Id",StatusCodes.Status404NotFound);
        public static readonly Error DuplicatedPollTitle = new("poll.DulicatedTitle", "The title is dulicated",StatusCodes.Status409Conflict);
        
    }
}
