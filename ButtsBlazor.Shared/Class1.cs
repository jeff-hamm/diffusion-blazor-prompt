// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace ButtsBlazor.Printer;

/// <summary>
/// Provides an implementation of <see cref="IMqttClientAccessor" /> based on the current execution context.
/// </summary>
public class MqttClientAccessor
{
    private static readonly AsyncLocal<MqttClientHolder> _MqttClientCurrent = new AsyncLocal<MqttClientHolder>();

    /// <inheritdoc/>
    public IMqttClient? MqttClient
    {
        get
        {
            return _MqttClientCurrent.Value?.Context;
        }
        set
        {
            var holder = _MqttClientCurrent.Value;
            if (holder != null)
            {
                // Clear current MqttClient trapped in the AsyncLocals, as its done.
                holder.Context = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the MqttClient in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _MqttClientCurrent.Value = new MqttClientHolder { Context = value };
            }
        }
    }

    private sealed class MqttClientHolder
    {
        public IMqttClient? Context;
    }
}

public class MqttManagedClientAccessor
{
    private static readonly AsyncLocal<MqttClientHolder> _MqttClientCurrent = new AsyncLocal<MqttClientHolder>();

    /// <inheritdoc/>
    public IManagedMqttClient? MqttClient
    {
        get
        {
            return _MqttClientCurrent.Value?.Context;
        }
        set
        {
            var holder = _MqttClientCurrent.Value;
            if (holder != null)
            {
                // Clear current MqttClient trapped in the AsyncLocals, as its done.
                holder.Context = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the MqttClient in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _MqttClientCurrent.Value = new MqttClientHolder { Context = value };
            }
        }
    }

    private sealed class MqttClientHolder
    {
        public IManagedMqttClient? Context;
    }
}