using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Contracts
{
    public interface GetAllOrdersInfoStateResponse
    {
      List<BookingStateDTO> BookingStates { get; set; }
       
    }
    public class BookingStateDTO
    {
        public string CurrentState { get; set; }
        public Guid BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CreateDate { get; set; }
        public List<BookingItemStateDTO> BookingItemStates { get; set; }
    }

    public class BookingItemStateDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        public BookingItemTypeEnum Type { get; set; }
        public BookingItemStateEnum State { get; set; }
        public string RefusalReason { get; set; }
        // Other specific properties...
    }


    public enum BookingItemStateEnum
    {
        Pending = 0,
        Success = 1,
        Failed = 2
        // Other conditions...
    }
}
