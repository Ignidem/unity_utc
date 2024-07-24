using System;
using System.Collections.Generic;
using Mirror;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using UnityEngine;

namespace Utp
{
	public class RelayNetworkManager : NetworkManager
	{
		protected UtpTransport UtpTransport => transport as UtpTransport;

		/// <summary>
		/// Get whether Relay is enabled or not.
		/// </summary>
		/// <returns>True if enabled, false otherwise.</returns>
		public bool IsRelayEnabled()
		{
			return UtpTransport.useRelay;
		}

		/// <summary>
		/// Ensures Relay is disabled. Starts the server, listening for incoming connections.
		/// </summary>
		public void StartStandardServer()
		{
			UtpTransport.useRelay = false;
			StartServer();
		}

		/// <summary>
		/// Ensures Relay is disabled. Starts a network "host" - a server and client in the same application
		/// </summary>
		public void StartStandardHost()
		{
			UtpTransport.useRelay = false;
			StartHost();
		}

		/// <summary>
		/// Gets available Relay regions.
		/// </summary>
		/// 
		public void GetRelayRegions(Action<List<Region>> onSuccess, Action onFailure)
		{
			UtpTransport.GetRelayRegions(onSuccess, onFailure);
		}

		/// <summary>
		/// Ensures Relay is enabled. Starts a network "host" - a server and client in the same application
		/// </summary>
		public virtual async Task<string> StartRelayHost(int maxPlayers, string regionId = null)
		{
			
			UtpTransport utp = UtpTransport;
			utp.useRelay = true;
			string code = await utp.AllocateRelayServer(maxPlayers, regionId);
			StartHost();
			return code;
		}

		/// <summary>
		/// Ensures Relay is disabled. Starts the client, connects it to the server with networkAddress.
		/// </summary>
		public void JoinStandardServer()
		{
			UtpTransport.useRelay = false;
			StartClient();
		}

		/// <summary>
		/// Ensures Relay is enabled. Starts the client, connects to the server with the relayJoinCode.
		/// </summary>
		public async void JoinRelayServer(string code)
		{
			try 
			{ 
				UtpTransport utp = UtpTransport;
				await utp.ConfigureClientWithJoinCode(code);
				StartClient();
			}
			catch (Exception e)
			{
				UtpLog.Error(e.Message);
			}
		}
	}
}