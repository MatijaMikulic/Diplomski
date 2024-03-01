using PlcCommunication.Model;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlcCommunication
{
    /// <summary>
    /// Provides access and operations for reading and writing data to PLC Data blocks.
    /// </summary>
    public class PlcDataAccess
    {
        private readonly Plc _plc;

        //temporary static lists for testing
        private static readonly List<DataItem> _changeCounters =
            new List<DataItem>
        {
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=4,StartByteAdr=0,VarType=VarType.Int},
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=5,StartByteAdr=0,VarType=VarType.Int}
        };
        private static readonly List<DataItem> _auxCounters =
            new List<DataItem>
        {
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=4,StartByteAdr=18,VarType=VarType.Int},
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=5,StartByteAdr=18,VarType=VarType.Int}
        };
        private static readonly List<DataItem> _bufferPointers =
            new List<DataItem>
        {
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=4,StartByteAdr=2,VarType=VarType.Int},
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=5,StartByteAdr=2,VarType=VarType.Int}
        };

        /// <summary>
        /// Initializes a new instance of the PlcDataAccess class.
        /// </summary>
        /// <param name="plc">The PLC object used for communication.</param>
        public PlcDataAccess(Plc plc)
        {
            _plc = plc;
        }

        /// <summary>
        /// Reads data from the PLC based on specified parameters.
        /// </summary>
        /// <param name="dataType">The data type to be read.</param>
        /// <param name="db">The data block number.</param>
        /// <param name="startByte">The starting byte address.</param>
        /// <param name="type">The variable type.</param>
        /// <param name="varCount">The number of variables to read.</param>
        /// <returns>The read data from the PLC.</returns>
        public object? Read(DataType dataType, int db, int startByte, VarType type, int varCount)
        {
            return _plc.Read(dataType, db, startByte, type, varCount);
        }
        /// <summary>
        /// Reads and processes multiple data items from the PLC to create a list of DataBlockMetaData.
        /// </summary>
        /// <returns>A list of DataBlockMetaData objects containing processed PLC data.</returns>
        public List<DataBlockMetaData> ReadDBContent()
        {
            List<DataBlockMetaData> result = new List<DataBlockMetaData>();

            _plc.ReadMultipleVars(_changeCounters);
            _plc.ReadMultipleVars(_bufferPointers);
            _plc.ReadMultipleVars(_auxCounters);

            result = _changeCounters.Zip(_auxCounters, _bufferPointers)
                       .Select(tuple => new DataBlockMetaData(
                           (short)tuple.First.Value,
                           (short)tuple.Second.Value,
                           (short)tuple.Third.Value,
                           (short)tuple.Item1.DB
                       ))
                       .ToList();

            return result;
        }

        /// <summary>
        /// Reads and processes multiple data items asynchronously from the PLC to create a list of DataBlockMetaData.
        /// </summary>
        /// <returns>A list of DataBlockMetaData objects containing processed PLC data.</returns>
        public async Task<List<DataBlockMetaData>> ReadDBContentAsync()
        {
            List<DataBlockMetaData> result = new List<DataBlockMetaData>();

            Task<List<DataItem>> changeCountersTask = _plc.ReadMultipleVarsAsync(_changeCounters);
            Task<List<DataItem>> bufferPointersTask = _plc.ReadMultipleVarsAsync(_bufferPointers);
            Task<List<DataItem>> auxCountersTask = _plc.ReadMultipleVarsAsync(_auxCounters);

            await Task.WhenAll(changeCountersTask, bufferPointersTask, auxCountersTask);

            List<DataItem> changeCounters = changeCountersTask.Result;
            List<DataItem> bufferPointers = bufferPointersTask.Result;
            List<DataItem> auxCounters    = auxCountersTask.Result;

            result = changeCounters.Zip(auxCounters, bufferPointers)
                .Select(tuple => new DataBlockMetaData(
                    (short)tuple.First.Value,
                    (short)tuple.Second.Value,
                    (short)tuple.Third.Value,
                    (short)tuple.First.DB
                ))
                .ToList();

            return result;
        }

        /// <summary>
        /// Gets the default value for a specified variable type.
        /// </summary>
        public object GetDefaultValueForType(VarType type)
        {

            switch (type)
            {
                case VarType.Bit:
                    return false;
                case VarType.Byte:
                case VarType.Word:
                case VarType.DInt:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported variable type: {type}");
            }
        }
    }
}
