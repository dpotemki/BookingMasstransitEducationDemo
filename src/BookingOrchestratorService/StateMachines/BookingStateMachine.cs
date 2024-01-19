using BookingOrchestratorService.Configurations;
using BookingOrchestratorService.Database.Models;
using BookingOrchestratorService.Extensions;
using FlyghtService.Contracts;
using HotelService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Contracts;
using TransferService.Contracts;
using UI.Contracts;
using static MassTransit.Transports.ReceiveEndpoint;
using State = MassTransit.State;

namespace BookingOrchestratorService.BookingStateMachines
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        private readonly EndpointsConfiguration _endpointConfiguration;
        private readonly ILogger<BookingStateMachine> _logger;
        public State AwaitingConfirmation { get; private set; }
        public State CompensateBooking { get; private set; }
        public State Rejected { get; private set; }
        public State Completed { get; private set; }

        public Event<BookingRequested> BookingRequestedEvent { get; private set; }
        public Event<BookingConfirmed> BookingConfirmedEvent { get; private set; }
        public Event<BookingRejected> BookingRejectedEvent { get; private set; }
        public Event<CompensateBooking> CompensateBookingEvent { get; private set; }


        public Event<FlyghtBookingConfirmed> FlyghtBookingConfirmedEvent { get; private set; }
        public Event<FlyghtBookingRejected> FlyghtBookingRejectedEvent { get; private set; }

        public Event<TransferBookingConfirmed> TransferBookingConfirmedEvent { get; private set; }
        public Event<TransferBookingRejected> TransferBookingRejectedEvent { get; private set; }

        public Event<HotelBookingRejected> HotelBookingRejectedEvent { get; private set; }
        public Event<HotelBookingConfirmed> HotelBookingConfirmedEvent { get; private set; }


        public BookingStateMachine(IOptions<EndpointsConfiguration> settings, ILogger<BookingStateMachine> logger)
        {
            _logger = logger;
            _endpointConfiguration = settings.Value;

            ConfigureEvents();
            ConfigureStates();


            //Uncomment Finalize for production use, it`s only for study
            Initially(
                When(BookingRequestedEvent)
                .Then(InitializeBookingData)
                .Then(async context => await SendBookingMessageForAllServices(context))
                .TransitionTo(AwaitingConfirmation));

            During(AwaitingConfirmation,
                When(FlyghtBookingConfirmedEvent)
                    .Then(UpdateFlyghtServiceConfirmation())
                    .If(x => x.Saga.AreAllServicesConfirmed(), x => x.TransitionTo(Completed)/*.Finalize()*/),
               When(FlyghtBookingRejectedEvent)
                    .Then(UpdateFlyghtServiceRejection())
                    .ThenAsync(CompensateOrders)
                    .TransitionTo(Rejected)
                    /*.Finalize()*/,
                When(HotelBookingConfirmedEvent)
                    .Then(UpdateHotelServiceConfirmation())
                    .If(x => x.Saga.AreAllServicesConfirmed(), x => x.TransitionTo(Completed)/*.Finalize()*/),
               When(HotelBookingRejectedEvent)
                    .Then(UpdateHotelServiceRejection())
                    .ThenAsync(CompensateOrders)
                    .TransitionTo(Rejected)/*.Finalize()*/,

                When(TransferBookingConfirmedEvent)
                    .Then(UpdateTransferServiceConfirmation())
                    .If(x => x.Saga.AreAllServicesConfirmed(), x => x.TransitionTo(Completed)/*.Finalize()*/),
               When(TransferBookingRejectedEvent)
                    .Then(UpdateTransferServiceRejection())
                    .ThenAsync(CompensateOrders)
                    .TransitionTo(Rejected)/*.Finalize()*/
        );
        }

        private Action<BehaviorContext<BookingState, HotelBookingRejected>> UpdateHotelServiceRejection()
        {
            return context =>
            {
                LogRejectEvent(BookingItemTypeEnum.Hotel, context.Saga.BookingId, context.Message.Reason);
                context.Saga.UpdateServiceState(BookingItemTypeEnum.Hotel, BookingItemStatus.Rejected, context.Message.Reason);
            };
        }

        private Action<BehaviorContext<BookingState, HotelBookingConfirmed>> UpdateHotelServiceConfirmation()
        {
            return context =>
            {
                context.Saga.UpdateServiceState(BookingItemTypeEnum.Hotel, BookingItemStatus.Confirmed);
                LogConfirmEvent(BookingItemTypeEnum.Hotel, context.Saga.BookingId);
            };
        }
        private Action<BehaviorContext<BookingState, TransferBookingRejected>> UpdateTransferServiceRejection()
        {
            return context =>
            {
                LogRejectEvent(BookingItemTypeEnum.Transfer, context.Saga.BookingId, context.Message.Reason);
                context.Saga.UpdateServiceState(BookingItemTypeEnum.Transfer, BookingItemStatus.Rejected, context.Message.Reason);
            };
        }

        private Action<BehaviorContext<BookingState, TransferBookingConfirmed>> UpdateTransferServiceConfirmation()
        {
            return context =>
            {
                context.Saga.UpdateServiceState(BookingItemTypeEnum.Transfer, BookingItemStatus.Confirmed);
                LogConfirmEvent(BookingItemTypeEnum.Transfer, context.Saga.BookingId);
            };
        }
        private Action<BehaviorContext<BookingState, FlyghtBookingRejected>> UpdateFlyghtServiceRejection()
        {
            return context =>
            {
                LogRejectEvent(BookingItemTypeEnum.Flight, context.Saga.BookingId, context.Message.Reason);
                context.Saga.UpdateServiceState(BookingItemTypeEnum.Flight, BookingItemStatus.Rejected, context.Message.Reason);
            };
        }

        private Action<BehaviorContext<BookingState, FlyghtBookingConfirmed>> UpdateFlyghtServiceConfirmation()
        {
            return context =>
            {
                context.Saga.UpdateServiceState(BookingItemTypeEnum.Flight, BookingItemStatus.Confirmed);
                LogConfirmEvent(BookingItemTypeEnum.Flight, context.Saga.BookingId);
            };
        }
        private void LogRejectEvent(BookingItemTypeEnum serviceType, Guid bookingId, string reason)
        {
            _logger.LogWarning($"SAGA {serviceType.ToString()} booking rejected: BookingId {bookingId}, Reason: {reason}");
        }
        private void LogConfirmEvent(BookingItemTypeEnum serviceType, Guid bookingId)
        {
            _logger.LogWarning($"SAGA {serviceType.ToString()} booking confirmed: BookingId {bookingId}");
        }
        private void ConfigureEvents()
        {
            Event(() => BookingRequestedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => BookingConfirmedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => BookingRejectedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => CompensateBookingEvent, x => x.CorrelateById(m => m.Message.BookingId));

            Event(() => FlyghtBookingConfirmedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => FlyghtBookingRejectedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => HotelBookingConfirmedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => HotelBookingRejectedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => TransferBookingConfirmedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => TransferBookingRejectedEvent, x => x.CorrelateById(m => m.Message.BookingId));

        }

        private void ConfigureStates()
        {
            InstanceState(x => x.CurrentState);
            OnUnhandledEvent(HandleUnhandledEvent);

            State(() => AwaitingConfirmation);
            State(() => CompensateBooking);
            State(() => Rejected);
            State(() => Completed);
        }



        private static void InitializeBookingData(BehaviorContext<BookingState, BookingRequested> context)
        {
            context.Saga.BookingDate = context.Message.BookingDate;
            context.Saga.CreateDate = DateTime.UtcNow;
            context.Saga.BookingId = context.Message.BookingId;
            context.Saga.BookingItemStates.Add(new BookingItemState
            {
                Type = BokkingItemType.Flight,
                State = BookingItemStatus.Pending,
                BookingId = context.Message.BookingId,
                Id = Guid.NewGuid(),
                RefusalReason = ""

            });
            context.Saga.BookingItemStates.Add(new BookingItemState
            {
                Type = BokkingItemType.Hotel,
                State = BookingItemStatus.Pending,
                BookingId = context.Message.BookingId,
                Id = Guid.NewGuid(),
                RefusalReason = ""
            });
            context.Saga.BookingItemStates.Add(new BookingItemState
            {
                Type = BokkingItemType.Transfer,
                State = BookingItemStatus.Pending,
                BookingId = context.Message.BookingId,
                Id = Guid.NewGuid(),
                RefusalReason = ""
            });
        }

        private async Task SendBookingMessageForAllServices(BehaviorContext<BookingState, BookingRequested> context)
        {

            var endpointFlight = await context.GetSendEndpoint(new Uri(_endpointConfiguration.FlightServiceQueueName));
            await endpointFlight.Send<IBookFlight>(new
            {
                BookingId = context.Saga.BookingId,
                BookingDate = context.Saga.BookingDate,
            });
            _logger.LogInformation($"Sent 'Book Flight' message for order {context.Saga.CorrelationId}");

            var endpointHotel = await context.GetSendEndpoint(new Uri($"{_endpointConfiguration.HotelServiceQueueName}"));
            await endpointHotel.Send<IBookHotel>(new
            {
                BookingId = context.Saga.BookingId,
                BookingDate = context.Saga.BookingDate,
            });
            _logger.LogInformation($"Sent 'Book Hotel' message for order {context.Saga.CorrelationId}");

            var endpointTransfer = await context.GetSendEndpoint(new Uri($"{_endpointConfiguration.TransferServiceQueueName}"));
            await endpointTransfer.Send<IBookTransfer>(new
            {
                BookingId = context.Saga.BookingId,
                BookingDate = context.Saga.BookingDate,
            });
            _logger.LogInformation($"Sent 'Book Transfer' message for order {context.Saga.CorrelationId}");
        }


        private async Task CompensateOrders(BehaviorContext<BookingState> context)
        {
            // Получаем все услуги, за исключением отвергнутых
            var servicesToCompensate = context.Saga.BookingItemStates
                .Where(kvp => kvp.State != BookingItemStatus.Rejected)
                .Select(kvp => kvp.Type);

            foreach (var serviceType in servicesToCompensate)
            {
                var currentItem = context.Saga.BookingItemStates.Where(kvp => kvp.Type == serviceType).FirstOrDefault();
                currentItem!.State = BookingItemStatus.Compensated;
                var endpointUri = GetServiceQueueName(serviceType);
                var endpoint = await context.GetSendEndpoint(new Uri(endpointUri));
                await endpoint.Send<IBookingCancellation>(new
                {
                    CorrelationId = context.Saga.BookingId,

                });
                _logger.LogInformation($"Sent compensation message for {serviceType} booking for {context.Saga.CorrelationId}");
            }
        }

        private string GetServiceQueueName(BokkingItemType serviceType)
        {
            return serviceType switch
            {
                BokkingItemType.Flight => _endpointConfiguration.FlightServiceQueueName,
                BokkingItemType.Hotel => _endpointConfiguration.HotelServiceQueueName,
                BokkingItemType.Transfer => _endpointConfiguration.TransferServiceQueueName,
                _ => throw new ArgumentOutOfRangeException(nameof(serviceType), $"Not expected service type: {serviceType}")
            };
        }
        private Task HandleUnhandledEvent(UnhandledEventContext<BookingState> context)
        {
            if (context.Event.Name.Contains("TimeoutExpired"))
            {
                _logger.LogDebug($"[{DateTime.Now}][SAGA] Ignored unhandled event: {context.Event.Name}");

                context.Ignore();
            }
            else
                _logger.LogDebug($"[{DateTime.Now}][SAGA] Not finished saga: {context.Event.Name}");

            //context.Throw();

            return Task.CompletedTask;
        }

    }
}
