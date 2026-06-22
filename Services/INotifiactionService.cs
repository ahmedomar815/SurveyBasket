using Org.BouncyCastle.Asn1.X509;

namespace SurveyBasket.Services;

public interface INotifiactionService
{
    Task SendNewPollsNoification(int? pollId);
}
