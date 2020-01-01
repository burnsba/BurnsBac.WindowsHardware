using System;
using System.Collections.Generic;
using System.Text;

namespace BurnsBac.WindowsHardware.Bluetooth.Sensors
{
    /// <summary>
    /// <para>
    /// Bluetooth low energy heart rate sensor.
    /// </para>
    /// <para>
    /// Service <see cref="Services.ServiceUuids.AssignedNumbers.Heart_Rate"/>.
    /// </para>
    /// <para>
    /// Characteristic <see cref="Characteristics.CharacteristicUuids.AssignedNumbers.Heart_Rate_Measurement"/>.
    /// </para>
    /// </summary>
    public class LowEnergyHeartrateSensor : BluetoothSensorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LowEnergyHeartrateSensor"/> class.
        /// </summary>
        /// <param name="deviceAddress">Bluetooth device address.</param>
        public LowEnergyHeartrateSensor(ulong deviceAddress)
            : base(
                  deviceAddress,
                  (ushort)Services.ServiceUuids.AssignedNumbers.Heart_Rate,
                  (ushort)Characteristics.CharacteristicUuids.AssignedNumbers.Heart_Rate_Measurement)
        {
            this.DataReceivedEvent += DataReceivedEventHandler;
        }

        /// <summary>
        /// Event notification for data received from device.
        /// </summary>
        public event EventHandler<Characteristics.HeartRateMeasurement> HeartRateReceivedEvent;

        /// <summary>
        /// Converts raw byte data to <see cref="Characteristics.HeartRateMeasurement"/> and forwards event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Raw data.</param>
        private void DataReceivedEventHandler(object sender, SesnsorReadEventArgs e)
        {
            var hrm = Characteristics.HeartRateMeasurement.FromBytes(e.Data, 0);

            HeartRateReceivedEvent?.Invoke(sender, hrm);
        }
    }
}
