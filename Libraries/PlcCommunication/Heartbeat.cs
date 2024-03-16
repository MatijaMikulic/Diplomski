using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlcCommunication
{
    ///<summary>
    /// Represents a heartbeat mechanism connectivity checks and automatic reconnection.
    ///</summary>
    public class Heartbeat
    {
        private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(2);
        private System.Threading.Timer? _timer;
        private TimeSpan _interval = DefaultInterval;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly PlcCommunicationService _plcCommunicationService;

        ///<summary>
        /// Initializes a new instance of the Heartbeat class with the provided PLC communication service.
        ///</summary>
        ///<param name="plcCommunicationService">The PLC communication service.</param>
        public Heartbeat(PlcCommunicationService plcCommunicationService)
        {
            _plcCommunicationService = plcCommunicationService;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        ///<summary>
        /// Starts the heartbeat mechanism.
        ///</summary>
        public void Start()
        {
            _timer = new Timer(SendHeartbeat, null, _interval, Timeout.InfiniteTimeSpan);
        }

        ///<summary>
        /// Sends a heartbeat to the PLC.
        ///</summary>
        ///<param name="state">An object containing application-specific information.</param>
        private void SendHeartbeat(object? state)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            bool isConnected = _plcCommunicationService.IsConnectionActive();

            if (!isConnected)
            {
                _plcCommunicationService.AttemptReconnection();
            }

            _timer?.Change(_interval, Timeout.InfiniteTimeSpan);
        }

        ///<summary>
        /// Stops the heartbeat mechanism.
        ///</summary>
        public void Stop() 
        {
            _cancellationTokenSource.Cancel();
            _timer?.Dispose();
        }
    }
}
