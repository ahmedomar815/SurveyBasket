

    public class LoginRequestValidator:AbstractValidator<LoginRequest>
    {
        
        public LoginRequestValidator() 
        {
           RuleFor(x => x.Eamil).NotEmpty()
            .EmailAddress();
           RuleFor(x=>x.Password).NotEmpty();

        }
      
    }

 