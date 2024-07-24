using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

namespace Utp
{
    /// <summary>
    /// A server or client inside Utp.
    /// </summary>
    public abstract class UtpEntity
    {
        /// <summary>
        /// Temporary storage for connection events that occur on job threads so they may be dequeued on the main thread.
        /// </summary>
        protected NativeQueue<UtpConnectionEvent> connectionsEventsQueue;

        /// <summary>
        /// Used alongside a connection to connect, send, and receive data from a listen server.
        /// </summary>
        protected NetworkDriver Driver { get; private set; }

        /// <summary>
		/// A pipeline on the driver that is sequenced, and ensures messages are delivered.
		/// </summary>
		protected NetworkPipeline reliablePipeline;

        /// <summary>
        /// A pipeline on the driver that is sequenced, but does not ensure messages are delivered.
        /// </summary>
        protected NetworkPipeline unreliablePipeline;

        /// <summary>
		/// Job handle to schedule jobs.
		/// </summary>
		protected JobHandle jobHandle;

        /// <summary>
        /// Timeout(ms) to be set on drivers.
        /// </summary>
        protected int timeoutInMilliseconds;

		public int PayloadCapacity => 10000;

		protected void CreateDriver(NetworkSettings settings)
		{
			settings.WithFragmentationStageParameters(PayloadCapacity);
			Driver = NetworkDriver.Create(settings);
			reliablePipeline = Driver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(ReliableSequencedPipelineStage));
			unreliablePipeline = Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));
		}

		protected void DisposeDriver()
		{
			Driver.Dispose();
			Driver = default;
		}

        /// <summary>
        /// Returns whether a connection is a valid one. Checks against default connection object.
        /// </summary>
        /// <param name="connection">The connection to validate.</param>
        /// <returns>True or false, whether the connection is valid.</returns>
        public bool IsValidConnection(NetworkConnection connection)
        {
            return connection.IsCreated;
        }

        /// <summary>
		/// Determine whether the NetworkDriver has been initialized.
		/// </summary>
		/// <returns>True if initialized, false otherwise.</returns>
		public bool IsNetworkDriverInitialized()
        {
            return Driver.IsCreated;
        }
    }
}