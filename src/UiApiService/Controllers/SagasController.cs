using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UI.Contracts;
using static UI.Contracts.GetAllOrdersInfoStateResponse;

namespace UiApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagasController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        private readonly IRequestClient<GetAllOrdersInfoState> _allOrderInfoStatesClient;
        private readonly ILogger<SagasController> _logger;

        public SagasController(
            IRequestClient<GetAllOrdersInfoState> allOrderInfoStatesClient,
            ILogger<SagasController> logger,
            ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint
            )
        {
            _allOrderInfoStatesClient = allOrderInfoStatesClient;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }


        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _allOrderInfoStatesClient.GetResponse<GetAllOrdersInfoStateResponse>(new { });

                return Ok(response?.Message?.BookingStates);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> PostCartPosition(
            [FromQuery][Required] DateTime bookingDate)
        {
            try
            {
                var bookingId = Guid.NewGuid();
                await _publishEndpoint.Publish<BookingRequested>( new
                {
                    BookingId = bookingId,
                    BookingDate = bookingDate
                });

                return Ok(new { BookingId = bookingId });

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }

    }
}
