
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Mapping
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest,Question>()
                .Map(dest => dest.Answers, src => src.Answers.Select(a => new Answer { Content = a }));

        }
    }
}
