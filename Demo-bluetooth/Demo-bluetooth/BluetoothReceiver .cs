#if ANDROID
using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

public class BluetoothReceiver : BroadcastReceiver
{
    private readonly List<BluetoothDeviceItem> _devices;
    private readonly ListView _listView;

    public BluetoothReceiver(List<BluetoothDeviceItem> devices, ListView listView)
    {
        _devices = devices;
        _listView = listView;
    }

    public override void OnReceive(Context context, Intent intent)
    {
        string action = intent.Action;
        if (action == BluetoothDevice.ActionFound)
        {
            BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
            if (!_devices.Any(d => d.Address == device.Address))
            {
                _devices.Add(new BluetoothDeviceItem
                {
                    Name = device.Name ?? "Unknown Device",
                    Address = device.Address
                });
                _listView.ItemsSource = null;
                _listView.ItemsSource = _devices;
            }
        }
        else if (action == BluetoothAdapter.ActionDiscoveryFinished)
        {
            // Handle discovery finished
        }
    }
}

public class BluetoothDeviceItem
{
    public string Name { get; set; }
    public string Address { get; set; }
}
#endif
