using BookingOrchestratorService.Database;
using BookingOrchestratorService.Database.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Contracts;
using static UI.Contracts.GetAllOrdersInfoStateResponse;

namespace BookingOrchestratorService.Consumers
{
    public class GetAllOrdersInfoConsumer : IConsumer<GetAllOrdersInfoState>
    {
        private readonly StateMachinesDBContext _dbContext;

        public GetAllOrdersInfoConsumer(StateMachinesDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<GetAllOrdersInfoState> context)
        {
            var sagas = await _dbContext.BookingStates
                .OrderByDescending(x => x.CreateDate)
                .AsNoTracking().ToListAsync();

            List<BookingStateDTO> bookingStateDTOs = sagas.Select(ToBookingStateDTO).ToList();

            
            await context.RespondAsync<GetAllOrdersInfoStateResponse>(new OrdersInfo { BookingStates= bookingStateDTOs });
        }
       
        private class OrdersInfo : GetAllOrdersInfoStateResponse
        {
            public List<BookingStateDTO> BookingStates { get; set; } = new List<BookingStateDTO>();
        }
        private BookingStateDTO ToBookingStateDTO( BookingState saga)
        {

            BookingStateDTO bookingStateDTO = new BookingStateDTO();
            bookingStateDTO.BookingId = saga.BookingId;
            bookingStateDTO.BookingDate = saga.BookingDate;
            bookingStateDTO.CreateDate = saga.CreateDate;
            bookingStateDTO.CurrentState = saga.CurrentState;
            bookingStateDTO.BookingItemStates = saga.BookingItemStates
                ==null? 
                new List<BookingItemStateDTO>() : saga.BookingItemStates.Select(ToBookingItemStateDTO).ToList();

            return bookingStateDTO;
        }

        private  BookingItemStateDTO ToBookingItemStateDTO(BookingItemState bookingItemState)
        {
            BookingItemStateDTO bookingItemStateDTO = new BookingItemStateDTO();
            bookingItemStateDTO.Id = bookingItemState.Id;
            bookingItemStateDTO.BookingId = bookingItemState.BookingId;
            bookingItemStateDTO.RefusalReason = bookingItemState.RefusalReason;
            bookingItemStateDTO.State = (BookingItemStateEnum)bookingItemState.State;
            bookingItemStateDTO.Type = (BookingItemTypeEnum)bookingItemState.Type;

            return bookingItemStateDTO;
        }  
    }
}
