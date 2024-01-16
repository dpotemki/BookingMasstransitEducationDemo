using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Contracts
{
    public interface BookingRequested
    {
        public Guid BookingId { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
