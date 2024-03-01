using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcCommunication.Model
{
    public class DataBlockMetaData
    {
        public short ChangeCounter { get; private set; }
        public short BufferPointer { get; private set; }
        public short AuxiliaryCounter { get; private set; }
        public short DB { get; private set; }

        public DataBlockMetaData()
        {
            ChangeCounter = 0;
            AuxiliaryCounter = 0;
            BufferPointer = 0;
            DB = 0;
        }
        public DataBlockMetaData(short changeCounter, short auxiliaryCounter, short bufferPointer, short dB)
        {
            ChangeCounter = changeCounter;
            AuxiliaryCounter = auxiliaryCounter;
            BufferPointer = bufferPointer;
            DB = dB;
        }
    }
}
