using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModel.Model
{
    public class DataBlockHeader:MessageBase
    {
        public short DB { get; set; }
        public short BufferPointer { get; set; }
        public DataBlockHeader(short db, short bufferPointer , byte priority) 
            : base(priority, MessageType.DataBlockHeader)
        {
            DB = db;
            BufferPointer = bufferPointer;
        }

        public override string ToString()
        {
            return $"{DB}, {BufferPointer}";
        }

    }
}
