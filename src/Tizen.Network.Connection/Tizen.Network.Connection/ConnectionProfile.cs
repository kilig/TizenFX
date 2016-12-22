﻿/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Tizen.Network.Connection
{
    /// <summary>
    /// This Class is ConnectionProfile
    /// </summary>
    public class ConnectionProfile : IDisposable
    {
        internal IntPtr ProfileHandle = IntPtr.Zero;
        private IAddressInformation Ipv4;
        private IAddressInformation Ipv6;
        private bool disposed = false;
        private EventHandler _ProfileStateChanged;

        /// <summary>
        /// The event that is called when the state of profile is changed.
        /// </summary>
        public event EventHandler ProfileStateChanged
        {
            add
            {
                if (_ProfileStateChanged == null)
                {
                    ProfileStateChangedStart();
                }
                _ProfileStateChanged += value;
            }
            remove
            {
                _ProfileStateChanged -= value;
                if (_ProfileStateChanged == null)
                {
                    ProfileStateChangedStop();
                }
            }
        }

        private void TypeChangedCallback(ProfileState state, IntPtr userData)
        {
            if (_ProfileStateChanged != null)
            {
                _ProfileStateChanged(null, new ProfileStateEventArgs(state));
            }
        }

        private void ProfileStateChangedStart()
        {
            int ret = Interop.ConnectionProfile.SetStateChangeCallback(ProfileHandle, TypeChangedCallback, IntPtr.Zero);
            if ((ConnectionError)ret != ConnectionError.None)
            {
                Log.Error(Globals.LogTag, "It failed to register callback for changing profile state, " + (ConnectionError)ret);
            }
        }

        private void ProfileStateChangedStop()
        {
            int ret = Interop.ConnectionProfile.UnsetStateChangeCallback(ProfileHandle);
            if ((ConnectionError)ret != ConnectionError.None)
            {
                Log.Error(Globals.LogTag, "It failed to unregister callback for changing profile state, " + (ConnectionError)ret);
            }
        }

        public ConnectionProfile(IntPtr handle)
        {
            ProfileHandle = handle;
            Ipv4 = AddressFactory.CreateAddressInformation(handle, AddressFamily.Ipv4, AddressInformationType.Connection);
            Ipv6 = AddressFactory.CreateAddressInformation(handle, AddressFamily.Ipv6, AddressInformationType.Connection);
        }

        ~ConnectionProfile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free managed objects.
            }
            Interop.ConnectionProfile.Destroy(ProfileHandle);
            ProfileStateChangedStop();
            disposed = true;
        }

        /// <summary>
        /// Gets the profile ID.
        /// </summary>
        public string Id
        {
            get
            {
                IntPtr Value;
                int ret = Interop.ConnectionProfile.GetId(ProfileHandle, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get id of connection profile, " + (ConnectionError)ret);
                }
                string result = Marshal.PtrToStringAnsi(Value);
                Interop.Libc.Free(Value);
                return result;
            }
        }

        /// <summary>
        /// Gets the profile name.
        /// </summary>
        /// <privilege>http://tizen.org/privilege/network.get</privilege>
        public string Name
        {
            get
            {
                IntPtr Value;
                int ret = Interop.ConnectionProfile.GetName(ProfileHandle, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get name of connection profile, " + (ConnectionError)ret);
                }
                string result = Marshal.PtrToStringAnsi(Value);
                Interop.Libc.Free(Value);
                return result;
            }
        }

        /// <summary>
        /// Gets the network type.
        /// </summary>
        public ConnectionProfileType Type
        {
            get
            {
                int Value;
                int ret = Interop.ConnectionProfile.GetType(ProfileHandle, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get type of connection profile, " + (ConnectionError)ret);
                }
                return (ConnectionProfileType)Value;
            }
        }

        /// <summary>
        /// Gets the name of the network interface, e.g. eth0 and pdp0.
        /// </summary>
        public string InterfaceName
        {
            get
            {
                IntPtr Value;
                int ret = Interop.ConnectionProfile.GetNetworkInterfaceName(ProfileHandle, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get network interface name, " + (ConnectionError)ret);
                }
                string result = Marshal.PtrToStringAnsi(Value);
                Interop.Libc.Free(Value);
                return result;
            }
        }

        /// <summary>
        /// Gets the profile state.
        /// </summary>
        public ProfileState State
        {
            get
            {
                int Value;
                int ret = Interop.ConnectionProfile.GetState(ProfileHandle, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get profile state, " + (ConnectionError)ret);
                }
                return (ProfileState)Value;
            }
        }

        /// <summary>
        /// Gets the Proxy type.
        /// </summary>
        public ProxyType ProxyType
        {
            get
            {
                int Value;
                int ret = Interop.ConnectionProfile.GetProxyType(ProfileHandle, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get proxy type, " + (ConnectionError)ret);
                }
                return (ProxyType)Value;

            }
            set
            {
                int ret = Interop.ConnectionProfile.SetProxyType(ProfileHandle, (int)value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to set proxy type, " + (ConnectionError)ret);
                    ConnectionErrorFactory.ThrowConnectionException(ret);
                }
            }
        }

        /// <summary>
        /// The proxy address.
        /// </summary>
        public String ProxyAddress
        {
            get
            {
                IntPtr Value;
                int ret = Interop.ConnectionProfile.GetProxyAddress(ProfileHandle, (int)AddressFamily.Ipv4, out Value);
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to get proxy address, " + (ConnectionError)ret);
                }
                string result = Marshal.PtrToStringAnsi(Value);
                Interop.Libc.Free(Value);
                return result;

            }
            set
            {
                int ret = Interop.ConnectionProfile.SetProxyAddress(ProfileHandle, (int)AddressFamily.Ipv4, value.ToString());
                if ((ConnectionError)ret != ConnectionError.None)
                {
                    Log.Error(Globals.LogTag, "It failed to set proxy address, " + (ConnectionError)ret);
                    ConnectionErrorFactory.ThrowConnectionException(ret);
                }
            }
        }

        /// <summary>
        /// The subnet mask address(Ipv4).
        /// </summary>
        public IAddressInformation Ipv4Settings
        {
            get
            {
                return Ipv4;

            }
        }

        /// <summary>
        /// The subnet mask address(Ipv4).
        /// </summary>
        public IAddressInformation Ipv6Settings
        {
            get
            {
                return Ipv6;
            }
        }
    }

    /// <summary>
    /// An extended EventArgs class which contains changed profile state.
    /// </summary>
    public class ProfileStateEventArgs : EventArgs
    {
        private ProfileState _State = ProfileState.Disconnected;

        internal ProfileStateEventArgs(ProfileState state)
        {
            _State = state;
        }

        /// <summary>
        /// The profile state.
        /// </summary>
        public ProfileState State
        {
            get
            {
                return _State;
            }
        }
    }
}
