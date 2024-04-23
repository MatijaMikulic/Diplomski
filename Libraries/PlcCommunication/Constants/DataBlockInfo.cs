using S7.Net.Types;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcCommunication.Constants
{
    public static class DataBlockInfo
    {
        public const short DataOffset = 6;

        // 6 + 4*(bufferPointer -1)
        public const short BufferElementOffsetDB4 = 4;
        public const short BufferElementOffsetDB5 = 4;

        public const short DB4 = 4;
        public const short DB5 = 5;

        public const short BufferSize = 3;
    }
}
