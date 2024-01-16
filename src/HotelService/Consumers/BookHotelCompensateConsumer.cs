using HotelService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferService.Consumers;

namespace HotelService.Consumers
{
    public class BookHotelCompensateConsumer : IConsumer<IBookingCancellation>
    {
        private readonly ILogger<BookHotelCompensateConsumer> _logger;

        public BookHotelCompensateConsumer(ILogger<BookHotelCompensateConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IBookingCancellation> context)
        {

            _logger.LogInformation($"{nameof(BookHotelCompensateConsumer)} 'Book Hotel' message for BookingId: {context.Message.CorrelationId}");

           return Task.CompletedTask;
        }
    }
}
