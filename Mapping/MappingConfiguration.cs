
using SurveyBasket.Contracts.Questions;
using SurveyBasket.Contracts.User;

namespace SurveyBasket.Mapping
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest,Question>()
                .Map(dest => dest.Answers, src => src.Answers.Select(a => new Answer { Content = a }));
            config.NewConfig<RegisterRequest, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email);
            config.NewConfig<CreateUsreRequest, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.EmailConfirmed, src => true);
            config.NewConfig<UpdateUserRequest, ApplicationUser>()
               .Map(dest => dest.UserName, src => src.Email)
               .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper());


        }
    }
}
