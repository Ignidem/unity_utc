using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;

namespace Utp
{
	public interface IRelayManager
	{
		/// <summary>
		/// The allocation managed by a host who is running as a client and server.
		/// </summary>
		public Allocation ServerAllocation { get; set; }

		/// <summary>
		/// The allocation managed by a client who is connecting to a server.
		/// </summary>
		public JoinAllocation JoinAllocation { get; set; }

		public string JoinCode { get; }

		/// <summary>
		/// Get a Relay Service JoinAllocation from a given joinCode.
		/// </summary>
		/// <param name="joinCode">The code to look up the joinAllocation for.</param>
		/// <param name="onSuccess">A callback to invoke when the Relay allocation is successfully retrieved from the join code.</param>
		/// <param name="onFailure">A callback to invoke when the Relay allocation is unsuccessfully retrieved from the join code.</param>
		public Task GetAllocationFromJoinCode(string joinCode);

		/// <summary>
		/// Get a list of Regions from the Relay Service.
		/// </summary>
		/// <param name="onSuccess">A callback to invoke when the list of regions is successfully retrieved.</param>
		/// <param name="onFailure">A callback to invoke when the list of regions is unsuccessfully retrieved.</param>
		public Task<List<Region>> GetRelayRegions();

		/// <summary>
		/// Allocate a Relay Server.
		/// </summary>
		/// <param name="maxPlayers">The max number of players that may connect to this server.</param>
		/// <param name="regionId">The region to allocate the server in. May be null.</param>
		/// <returns>Join Code</returns>
		public Task<string> AllocateRelayServer(int maxPlayers, string regionId);
	}
}