using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitoringService.Model
{
    public class CountersState
    {
        public ushort PreviousChangeCounter { get; set; }
        public ushort PreviousAuxiliaryCounter { get; set; }
        public ushort PreviousBufferPointer { get; set; }
        public CountersState()
        {
            PreviousChangeCounter = 0;
            PreviousAuxiliaryCounter = 0;
            PreviousBufferPointer = 0;
        }

    }
}
