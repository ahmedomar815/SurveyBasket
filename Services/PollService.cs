
namespace SurveyBasket.Services
{
    public class PollService : IPollService
    {
        private readonly List<Poll> _polls = [
            new Poll
            {Id=1,Title="poll 1",Description="my first poll"    }
            ];

         public Poll Add(Poll poll)
        {
           _polls.Add( poll );
            return poll;
        }

        public bool Update(int Id, Poll poll)
        {
            var current = Get(Id);
            if (current is null)
            {
                return false;
            }
            current.Description= poll.Description;  
            current.Title= poll.Title;
            return true;
        }

        public Poll ?Get(int Id)
        {
            return _polls.SingleOrDefault(p => p.Id == Id);
        }

        IEnumerable<Poll> IPollService.GetAll()
        {
            return _polls;
        }

        public bool Delete(int Id)
        {
            var poll = Get(Id);
            var IsDelete = _polls.Remove(poll!);
            if (IsDelete)
                return true;
            return false;
        }
    }
}
