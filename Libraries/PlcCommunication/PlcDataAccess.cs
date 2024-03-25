using MessageModel.Model.DataBlockModel;
using PlcCommunication.Constants;
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

        //temporary lists for testing
        private readonly List<DataItem> _changeCounters =
            new List<DataItem>
        {
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=4,StartByteAdr=0,VarType=VarType.Int},
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=5,StartByteAdr=0,VarType=VarType.Int}
        };
        private readonly List<DataItem> _auxCounters =
            new List<DataItem>
        {
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=4,StartByteAdr=18,VarType=VarType.Int},
            new DataItem{ Count=1,DataType=DataType.DataBlock, DB=5,StartByteAdr=18,VarType=VarType.Int}
        };
        private readonly List<DataItem> _bufferPointers =
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
        public List<DataBlockMetaData> ReadDBMetaData()
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
        public async Task<List<DataBlockMetaData>> ReadDBMetaDataAsync()
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
        /// Reads buffer element from Data block
        /// </summary>
        /// <returns>A PlcData object containing buffer element.</returns>
        public PlcData ReadDBContent(short dataBlock, short bufferPointer)
        {
            int offset;
            switch (dataBlock)
            {
                case 4:
                    PlcData l1L2_Example1 = new L1L2_Process();
                    offset = GetOffset(dataBlock, bufferPointer);
                    _plc.ReadClass(l1L2_Example1, dataBlock, offset);
                    return l1L2_Example1;
                case 5:
                    PlcData l1L2_Request = new L1L2_Request();
                    offset = GetOffset(dataBlock, bufferPointer);
                    _plc.ReadClass(l1L2_Request, dataBlock, offset);
                    return l1L2_Request;
                default:
                    throw new ArgumentException("Unknown data block type.");
            }          
        }

        /// <summary>
        /// Calculates offset for element inside buffer
        /// </summary>
        /// <returns>An offset.</returns>
        private int GetOffset(short dataBlock, short bufferPointer)
        {
            switch (dataBlock)
            {
                case 4:
                    return DataBlockInfo.DataOffset + DataBlockInfo.BufferElementOffsetDB4 * (bufferPointer - 1); // -1 in case bufferPointer starts at 1.
                case 5:
                    return DataBlockInfo.DataOffset + DataBlockInfo.BufferElementOffsetDB5 * (bufferPointer - 1);
                default:
                    throw new ArgumentException("Unknown data block type.");
            }
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
