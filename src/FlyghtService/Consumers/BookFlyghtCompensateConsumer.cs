using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyghtService.Consumers
{
    internal class BookFlyghtCompensateConsumer : IConsumer<IBookingCancellation>
    {
        private readonly ILogger<BookFlyghtCompensateConsumer> _logger;

        public BookFlyghtCompensateConsumer(ILogger<BookFlyghtCompensateConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IBookingCancellation> context)
        {

            _logger.LogInformation($"{nameof(BookFlyghtCompensateConsumer)} 'Book Flyght' message for BookingId: {context.Message.CorrelationId}");
            await Task.Delay(1000);
            return ;
        }
    }
}
