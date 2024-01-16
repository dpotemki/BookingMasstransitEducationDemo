using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingOrchestratorService.Database.Models
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        public Guid BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CreateDate { get; set; }

        public List<BookingItemState> BookingItemStates { get; set; } = new List<BookingItemState>();
    }
}
