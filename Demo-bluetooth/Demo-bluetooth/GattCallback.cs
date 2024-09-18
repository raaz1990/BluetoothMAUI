#if ANDROID
using Android;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Bluetooth.LE;
#endif
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo_bluetooth;

public class GattCallback : BluetoothGattCallback
{
    public event EventHandler<List<string>> ServicesDiscovered;

    public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
    {
        base.OnServicesDiscovered(gatt, status);

        if (status == GattStatus.Success)
        {
            var serviceUuids = new List<string>();
            foreach (var service in gatt.Services)
            {
                serviceUuids.Add(service.Uuid.ToString());
                Console.WriteLine($"Service UUID: {service.Uuid}");
            }
            ServicesDiscovered?.Invoke(this, serviceUuids);
        }
        else
        {
            Console.WriteLine($"Service discovery failed with status: {status}");
        }
    }
}

