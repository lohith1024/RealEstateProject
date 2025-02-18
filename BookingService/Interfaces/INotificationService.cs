using System.Threading.Tasks;

namespace BookingService.Interfaces
{
    public interface INotificationService
    {
        Task SendBookingConfirmationAsync(int bookingId);
        Task SendBookingCancellationAsync(int bookingId);
        Task SendPaymentConfirmationAsync(int bookingId);
    }
}