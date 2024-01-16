using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferService.Consumers
{
    internal class BookTransferCompensateConsumer : IConsumer<IBookingCancellation>
    {
        private readonly ILogger<BookTransferCompensateConsumer> _logger;

        public BookTransferCompensateConsumer(ILogger<BookTransferCompensateConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IBookingCancellation> context)
        {

            _logger.LogInformation($"{nameof(BookTransferCompensateConsumer)} 'Book Transfer' message for BookingId: {context.Message.CorrelationId}");

            return Task.CompletedTask;
        }
    }
}
