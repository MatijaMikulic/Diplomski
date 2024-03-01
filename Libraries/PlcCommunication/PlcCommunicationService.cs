using Microsoft.Extensions.Options;
using S7.Net;

namespace PlcCommunication
{
    /// <summary>
    /// Provides functionality related to communication with a PLC (Programmable Logic Controller).
    /// </summary>
    public class PlcCommunicationService:IDisposable
    {
        private readonly PlcCommunicationManager _connectionManager;
        private readonly PlcDataAccess _dataAccess;

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
        public bool IsConnected()
        {
            return _connectionManager.IsCommunicationReady();
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