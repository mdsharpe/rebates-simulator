﻿namespace RebatesSimulator.Client.SignalR
{
    public interface ISignalRClient
    {
        bool IsConnected { get; }
        Task Start();
    }
}
