﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public interface IBookingCancellation
    {
        Guid CorrelationId { get; }
    }
}
