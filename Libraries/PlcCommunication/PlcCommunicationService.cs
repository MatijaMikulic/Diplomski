using Microsoft.Extensions.Options;
using S7.Net;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlcCommunication
{
    /// <summary>
    /// Provides functionality related to communication with a PLC (Programmable Logic Controller).
    /// </summary>
    public class PlcCommunicationService:IDisposable,INotifyPropertyChanged
    {
        private readonly PlcCommunicationManager _connectionManager;
        private readonly PlcDataAccess _dataAccess;
        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set 
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the PlcCommunicationService class.
        /// </summary>
        /// <param name="options">The configuration options for the PLC.</param>
        public PlcCommunicationService(IOptions<PlcConfiguration> options)
        {
            var plcConfiguration = options.Value;
            _connectionManager = new PlcCommunicationManager(plcConfiguration);
            _connectionManager.InitializeCommunication();
            //_connectionManager.OpenCommunication();
            _dataAccess = new PlcDataAccess(_connectionManager.Plc);
            _isConnected = false;
        }

        /// <summary>
        /// Opens communication with the PLC.
        /// </summary>
        public void OpenCommunication()
        {
            _connectionManager.OpenCommunication();
        }

        /// <summary>
        /// Checks whether the communicator is connected to the PLC.
        /// </summary>
        /// <returns>True if connected; otherwise, false.</returns>
        public bool IsConnectionActive()
        {
            PingConnection();
            // Attempting to read data will automatically set plc.IsConnected to true or false
            IsConnected = _connectionManager.IsCommunicationReady();
            return IsConnected;
        }

        /// <summary>
        /// Pings connection by attempting to read some data
        /// </summary>
        /// <returns>True if data was read successfully; otherwise, false.</returns>
        private void PingConnection()
        {
            try
            {
                var data = DataAccess.Read(DataType.DataBlock, 4, 0, VarType.Int, 1);
                //return data is not null;
            }
            catch (PlcException ex)
            {
                Console.WriteLine($"Ping failed: {ex.ErrorCode}");
                //return false;
            }
        }

        /// <summary>
        /// Gets the PlcDataAccess component for accessing PLC data.
        /// </summary>
        public PlcDataAccess DataAccess => _dataAccess;

        public void Dispose()
        {
            _connectionManager.CloseCommunication();
        }

    }
}