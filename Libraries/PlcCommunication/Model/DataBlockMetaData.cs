using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcCommunication.Model
{
    public class DataBlockMetaData
    {
        public ushort ChangeCounter { get; private set; }
        public ushort BufferPointer { get; private set; }
        public ushort AuxiliaryCounter { get; private set; }
        public ushort DB { get; private set; }
        
        public DataBlockMetaData()
        {
            ChangeCounter = 0;
            AuxiliaryCounter = 0;
            BufferPointer = 0;
            DB = 0;
        }
        public DataBlockMetaData(ushort changeCounter, ushort auxiliaryCounter, ushort bufferPointer, ushort dB)
        {
            ChangeCounter = changeCounter;
            AuxiliaryCounter = auxiliaryCounter;
            BufferPointer = bufferPointer;
            DB = dB;
        }
    }
}
