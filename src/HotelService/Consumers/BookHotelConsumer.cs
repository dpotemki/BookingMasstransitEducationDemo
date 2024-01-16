using HotelService.Contracts;
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
    public class BookHotelConsumer : IConsumer<IBookHotel>
    {
        private readonly ILogger<BookHotelConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookHotelConsumer(ILogger<BookHotelConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IBookHotel> context)
        {
            _logger.LogInformation($"Received 'Book Hotel' message for BookingId: {context.Message.BookingId}");


            // imitate Some work with external repository
            Random rnd = new Random();
            await Task.Delay(rnd.Next(3000, 5000));

            bool isConfirmed = context.Message.BookingDate.Day != 10;


            if (isConfirmed)
            {
                await _publishEndpoint.Publish<HotelBookingConfirmed>(new BookingConfirmedEvent
                {
                    BookingId = context.Message.BookingId,
                });
                _logger.LogInformation($"Sended Hotel confirmation for BookingId: {context.Message.BookingId}");
            }
            else
            {
                var rejectReason = "Hotel is full";
                await _publishEndpoint.Publish<HotelBookingRejected>(new
                {
                    BookingId = context.Message.BookingId,
                    Reason = rejectReason
                });

                _logger.LogWarning($"Hotel Booking rejected for BookingId: {context.Message.BookingId}, Reason: {rejectReason}");
            }
        }
    }
}
