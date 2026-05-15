namespace SurveyBasket.Errors
{
    public class PollErrors
    {
        public static readonly Error PollNotFound = new("poll.NotFound", "No poll was found with the given Id");
        public static readonly Error CreationFailed = new("poll.CreationFailed", "An error occurred while creating the poll");
        
    }
}
