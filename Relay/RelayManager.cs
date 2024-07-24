using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace Utp
{
	public class RelayManager : MonoBehaviour, IRelayManager
	{
		/// <summary>
		/// The allocation managed by a host who is running as a client and server.
		/// </summary>
		public Allocation ServerAllocation { get; set; }

		/// <summary>
		/// The allocation managed by a client who is connecting to a server.
		/// </summary>
		public JoinAllocation JoinAllocation { get; set; }

		/// <summary>
		/// A callback for when a Relay server is allocated and a join code is fetched.
		/// </summary>
		public Action<string, string> OnRelayServerAllocated { get; set; }

		/// <summary>
		/// The interface to the Relay services API.
		/// </summary>
		public IRelayServiceSDK RelayServiceSDK { get; set; } = new WrappedRelayServiceSDK();

		/// <summary>
		/// Server's join code if using Relay.
		/// </summary>
		public string RelayJoinCode { get; private set; }

		/// <summary>
		/// Retrieve the <seealso cref="Unity.Services.Relay.Models.JoinAllocation"/> corresponding to the specified join code.
		/// </summary>
		/// <param name="joinCode">The join code that will be used to retrieve the JoinAllocation.</param>
		/// <param name="onSuccess">A callback to invoke when the Relay allocation is successfully retrieved from the join code.</param>
		/// <param name="onFailure">A callback to invoke when the Relay allocation is unsuccessfully retrieved from the join code.</param>
		public async void GetAllocationFromJoinCode(string joinCode, Action onSuccess, Action onFailure)
		{
			try
			{
				await JoinFromCode(joinCode);
				onSuccess();
			}
			catch (Exception e)
			{
				UtpLog.Error($"Unable to get Relay allocation from join code, encountered an error: {e.Message}.");
				onFailure();
			}
		}

		public async Task JoinFromCode(string joinCode)
		{
			JoinAllocation = await RelayServiceSDK.JoinAllocationAsync(joinCode);
		}

		/// <summary>
		/// Get a list of Regions from the Relay Service.
		/// </summary>
		/// <param name="onSuccess">A callback to invoke when the list of regions is successfully retrieved.</param>
		/// <param name="onFailure">A callback to invoke when the list of regions is unsuccessfully retrieved.</param>
		public void GetRelayRegions(Action<List<Region>> onSuccess, Action onFailure)
		{
			StartCoroutine(GetRelayRegionsTask(onSuccess, onFailure));
		}

		private IEnumerator GetRelayRegionsTask(Action<List<Region>> onSuccess, Action onFailure)
		{
			Task<List<Region>> listRegions = RelayServiceSDK.ListRegionsAsync();

			while (!listRegions.IsCompleted)
			{
				yield return null;
			}

			if (listRegions.IsFaulted)
			{
				listRegions.Exception.Flatten().Handle((Exception err) =>
				{
					UtpLog.Error($"Unable to retrieve the list of Relay regions, encountered an error: {err.Message}.");
					return true;
				});

				onFailure?.Invoke();

				yield break;
			}

			onSuccess?.Invoke(listRegions.Result);
		}

		/// <summary>
		/// Allocate a Relay Server.
		/// </summary>
		/// <param name="maxPlayers">The max number of players that may connect to this server.</param>
		/// <param name="regionId">The region to allocate the server in. May be null.</param>
		/// <param name="onSuccess">A callback to invoke when the Relay server is successfully allocated.</param>
		/// <param name="onFailure">A callback to invoke when the Relay server is unsuccessfully allocated.</param>
		public async void AllocateRelayServer(int maxPlayers, string regionId, Action<string> onSuccess, Action onFailure)
		{
			try
			{
				string code = await AllocateRelayServer(maxPlayers, regionId);
				GUIUtility.systemCopyBuffer = RelayJoinCode = code;
				onSuccess(code);
			}
			catch (Exception e)
			{
				UtpLog.Error($"Unable to allocate Relay server, encountered an error creating a Relay allocation: {e.Message}.");
				onFailure();
			}
		}

		private async Task<string> AllocateRelayServer(int maxPlayers, string regionId)
		{
			ServerAllocation = await RelayServiceSDK.CreateAllocationAsync(maxPlayers, regionId);
			return await GetJoinCodeTask();
		}

		private Task<string> GetJoinCodeTask()
		{
			return RelayServiceSDK.GetJoinCodeAsync(ServerAllocation.AllocationId);
		}
	}
}