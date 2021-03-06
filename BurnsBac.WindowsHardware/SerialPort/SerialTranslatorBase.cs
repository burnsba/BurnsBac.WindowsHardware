﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BurnsBac.WindowsHardware.SerialPort
{
    /// <summary>
    /// Base class to receive data from a serial port and translate to high level events.
    /// </summary>
    public abstract class SerialTranslatorBase
    {
        /// <summary>
        /// Gets or sets serial port to monitor.
        /// </summary>
        public SerialPortProxy SerialPort { get; set; }

        /// <summary>
        /// Attaches to serial port to listen for data.
        /// </summary>
        /// <param name="serialPort">Serial port to monitor.</param>
        public void Hook(SerialPortProxy serialPort)
        {
            SerialPort = serialPort;

            SerialPort.DataReceivedEvent += Handler;
        }

        /// <summary>
        /// Translates serial data to high level event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="data">Serial data.</param>
        protected abstract void Handler(object sender, byte[] data);
    }
}
