using FlyghtService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferService.Models;

namespace TransferService.Consumers
{
    public class BookFlyghtConsumer : IConsumer<IBookFlight>
    {
        private readonly ILogger<BookFlyghtConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookFlyghtConsumer(ILogger<BookFlyghtConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IBookFlight> context)
        {
            _logger.LogInformation($"Received 'Book Flight' message for BookingId: {context.Message.BookingId}");


            // imitate Some work with external repository
            Random rnd = new Random();
            await Task.Delay(rnd.Next(3000, 5000));

            bool isConfirmed = context.Message.BookingDate.Day != 10;


            if (isConfirmed)
            {
                await _publishEndpoint.Publish<FlyghtBookingConfirmed>(new BookingConfirmedEvent
                {
                    BookingId = context.Message.BookingId,
                });
                _logger.LogInformation($"Sended Flight confirmation for BookingId: {context.Message.BookingId}");
            }
            else
            {
                var rejectReason = "Flight is full"; 
                await _publishEndpoint.Publish<FlyghtBookingRejected>(new
                {
                    BookingId = context.Message.BookingId,
                    Reason = rejectReason
                });

                _logger.LogWarning($"Booking rejected for BookingId: {context.Message.BookingId}, Reason: {rejectReason}");
            }
        }
    }
}
