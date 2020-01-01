using System;
using System.Collections.Generic;
using System.Text;
using BurnsBac.WinApi.Windows;
using BurnsBac.WindowsHardware.HardwareWatch.Enums;
using BurnsBac.WindowsHardware.Windows;

namespace BurnsBac.WindowsHardware.HardwareWatch
{
    /// <summary>
    /// Arguments for keyboard change event.
    /// </summary>
    public class KeyboardChangeEventArgs
    {
        /// <summary>
        /// Gets type of key change, either key up or key down.
        /// </summary>
        public KeyChangeDirection Direction { get; internal set; }

        /// <summary>
        /// Gets which key changed.
        /// </summary>
        public Keys Key { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether or not the alt key is currently being held.
        /// </summary>
        public bool Alt { get; internal set; }
    }
}
