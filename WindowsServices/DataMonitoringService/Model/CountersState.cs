using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitoringService.Model
{
    public class CountersState
    {
        public short PreviousChangeCounter { get; set; }
        public short PreviousAuxiliaryCounter { get; set; }
        public short PreviousBufferPointer { get; set; }
        public CountersState()
        {
            PreviousChangeCounter = 0;
            PreviousAuxiliaryCounter = 0;
            PreviousBufferPointer = 0;
        }

    }
}
