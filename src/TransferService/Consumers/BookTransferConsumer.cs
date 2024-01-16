using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferService.Contracts;
using TransferService.Models;

namespace TransferService.Consumers
{
    public class BookTransferConsumer : IConsumer<IBookTransfer>
    {
        private readonly ILogger<BookTransferConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookTransferConsumer(ILogger<BookTransferConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IBookTransfer> context)
        {
            _logger.LogInformation($"Received 'Book Transfer' message for BookingId: {context.Message.BookingId}");


            // imitate Some work with external repository
            Random rnd = new Random();
            await Task.Delay(rnd.Next(3000, 5000));

            bool isConfirmed = context.Message.BookingDate.Day != 10;


            if (isConfirmed)
            {
                await _publishEndpoint.Publish<TransferBookingConfirmed>(new BookingConfirmedEvent
                {
                    BookingId = context.Message.BookingId,
                });
                _logger.LogInformation($"Sended Transfer confirmation for BookingId: {context.Message.BookingId}");
            }
            else
            {
                var rejectReason = "Transfer is full";
                await _publishEndpoint.Publish<TransferBookingRejected>(new
                {
                    BookingId = context.Message.BookingId,
                    Reason = rejectReason
                });

                _logger.LogWarning($"Transfer Booking rejected for BookingId: {context.Message.BookingId}, Reason: {rejectReason}");
            }
        }
    }
}
