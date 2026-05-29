namespace SurveyBasket.Errors
{
    public class VoteErrors 
    {
      public static readonly Error DuplicatedVote = new("Vote.DulicatedVote", "the user is already voted to question", StatusCodes.Status409Conflict);
        public static readonly Error InvaidQuestions = new("Vote.Invalid Questions","the questions is invalid ", StatusCodes.Status400BadRequest);


    }
}
