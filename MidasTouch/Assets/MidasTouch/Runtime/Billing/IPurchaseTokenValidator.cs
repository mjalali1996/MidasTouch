using System.Threading.Tasks;

namespace MidasTouch.Billing
{
    internal interface IPurchaseTokenValidator
    {
        public enum State
        {
            Invalid = 0,
            Valid = 1,
            Consumed = 2,
        }

        Task<State> ValidateToken(string token);

        Task<bool> Consume(string token);
    }
}