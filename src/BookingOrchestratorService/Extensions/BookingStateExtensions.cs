using BookingOrchestratorService.Database.Models;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingOrchestratorService.Extensions
{
    public static class BookingStateExtensions
    {
        // Этот метод помогает обновить состояние бронирования для конкретного сервиса
        public static void UpdateServiceState(
            this BookingState state, BookingItemTypeEnum serviceType,
            BookingItemStatus serviceState,
            string reason = null)
        {
            var serviceUpdate = state.BookingItemStates.FirstOrDefault(s => (int)s.Type == (int)serviceType);
            if (serviceUpdate != null)
            {
                serviceUpdate.State = serviceState;
                serviceUpdate.RefusalReason = reason;
            }
            else
            {
                state.BookingItemStates.Add(new BookingItemState
                {
                    BookingId = state.BookingId,
                    Type = (BokkingItemType)serviceType,
                    State = serviceState,
                    RefusalReason = reason
                });
            }
        }

        // Проверяем, все ли сервисы подтвердили бронирование
        public static bool AreAllServicesConfirmed(this BookingState state)
        {
            var services = state.BookingItemStates;

            var transfer = services.FirstOrDefault(x => x.State == BookingItemStatus.Confirmed && x.Type == BokkingItemType.Transfer);
            var hotel = services.FirstOrDefault(x => x.State == BookingItemStatus.Confirmed && x.Type == BokkingItemType.Hotel);
            var flight = services.FirstOrDefault(x => x.State == BookingItemStatus.Confirmed && x.Type == BokkingItemType.Flight);

            return transfer != null && hotel != null && flight != null;
        }
    }
}
