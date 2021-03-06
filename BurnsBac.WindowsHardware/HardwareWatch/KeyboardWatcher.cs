﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BurnsBac.WinApi.User32;
using BurnsBac.WinApi.Windows;
using BurnsBac.WindowsHardware.HardwareWatch.Enums;
using BurnsBac.WindowsHardware.Windows;

namespace BurnsBac.WindowsHardware.HardwareWatch
{
    /// <summary>
    /// Hook keyboard events and notify changes.
    /// </summary>
    public class KeyboardWatcher : LowLevelWatcher, IDisposable
    {
        // this needs to be an instance member, otherwise it gets garbage collected away.
        private LowLevelKeyboardProc _llkp = null;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, WindowsMessages wParam, IntPtr lParam);

        /// <summary>
        /// Change event delegate.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Args.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "WindowsHardwareWatch")]
        public delegate void KeyboardChangeEventHandler(object sender, KeyboardChangeEventArgs args);

        /// <summary>
        /// Change event.
        /// </summary>
        public event KeyboardChangeEventHandler KeyboardChangeEvent;

        /// <summary>
        /// Gets time last message was received.
        /// </summary>
        public DateTime LastMessageReceived { get; private set; } = DateTime.MinValue;

        /// <inheritdoc />
        protected override IntPtr SetWindowsHook()
        {
            _llkp = new LowLevelKeyboardProc(LowLevelKeyboardHook);

            var handle = IntPtr.Zero;

            using (var currentProcess = Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                var module = WinApi.Kernel32.Api.GetModuleHandle(currentModule.ModuleName);
                handle = WinApi.User32.Api.SetWindowsHookEx(SetWindowsHookExType.WH_KEYBOARD_LL, _llkp, module, 0);
            }

            return handle;
        }

        /// <summary>
        /// Handler to run on keychange.
        /// </summary>
        /// <param name="nCode">Event code.</param>
        /// <param name="wParam">Type of windows message.</param>
        /// <param name="lParam">Keyboard status struct.</param>
        /// <returns>Pointer to next callback.</returns>
        private IntPtr LowLevelKeyboardHook(int nCode, WindowsMessages wParam, IntPtr lParam)
        {
            bool isKeyDown = wParam == WindowsMessages.KEYDOWN || wParam == WindowsMessages.SYSKEYDOWN;
            bool isKeyUp = wParam == WindowsMessages.KEYUP || wParam == WindowsMessages.SYSKEYUP;
            bool isAlt = false;

            KeyboardLowLevelHookStruct kbd = (KeyboardLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHookStruct));

            if (kbd.flags.HasFlag(KeyboardLowLevelHookStructFlags.LLKHF_ALTDOWN))
            {
                isAlt = true;
            }

            if (nCode >= 0 && FromAllowedWindow())
            {
                if (isKeyDown)
                {
                    LastMessageReceived = DateTime.Now;

                    RaiseEvent(new KeyboardChangeEventArgs()
                    {
                        Direction = KeyChangeDirection.KeyDown,
                        Key = (Keys)kbd.vkCode,
                        Alt = isAlt,
                    });
                }
                else if (isKeyUp)
                {
                    LastMessageReceived = DateTime.Now;

                    RaiseEvent(new KeyboardChangeEventArgs()
                    {
                        Direction = KeyChangeDirection.KeyUp,
                        Key = (Keys)kbd.vkCode,
                        Alt = isAlt,
                    });
                }
            }

            return WinApi.User32.Api.CallNextHookEx(IntPtr.Zero, nCode, (IntPtr)wParam, lParam);
        }

        /// <summary>
        /// Fires the event to any listeners.
        /// </summary>
        /// <param name="args">Arguments to send.</param>
        private void RaiseEvent(KeyboardChangeEventArgs args)
        {
            if (!object.ReferenceEquals(null, KeyboardChangeEvent))
            {
                KeyboardChangeEvent.Invoke(this, args);
            }
        }
    }
}
