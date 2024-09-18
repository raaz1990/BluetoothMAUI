#if ANDROID
using Android;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
#endif
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo_bluetooth
{
    public partial class MainPage : ContentPage
    {
#if ANDROID
        private BluetoothAdapter _bluetoothAdapter;
        private readonly List<BluetoothDeviceItem> _devices = new List<BluetoothDeviceItem>();
        private BluetoothReceiver _receiver;

        private class GattCallback : BluetoothGattCallback
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
#endif

        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            RequestPermissions();
            var bluetoothManager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
            _bluetoothAdapter = bluetoothManager.Adapter;

            if (_bluetoothAdapter == null)
            {
                DisplayAlert("Error", "Bluetooth is not supported on this device.", "OK");
                return;
            }

            _receiver = new BluetoothReceiver(_devices, DevicesListView);
#endif
        }

#if ANDROID
        private void RequestPermissions()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                string[] permissions = new string[]
                {
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect,
                    Manifest.Permission.AccessFineLocation
                };

                var activity = Platform.CurrentActivity;
                activity.RequestPermissions(permissions, 0);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            Android.App.Application.Context.RegisterReceiver(_receiver, filter);
            Android.App.Application.Context.RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Android.App.Application.Context.UnregisterReceiver(_receiver);
        }
#endif

        private void OnConnectionClicked(object sender, EventArgs e)
        {
#if ANDROID
            var bluetoothManager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
            var bluetoothAdapter = bluetoothManager.Adapter;

            if (bluetoothAdapter == null)
            {
                DisplayAlert("Error", "Bluetooth is not supported on this device.", "OK");
                return;
            }

            if (bluetoothAdapter.IsEnabled)
            {
                if (!bluetoothAdapter.IsDiscovering)
                {
                    bluetoothAdapter.StartDiscovery();
                }
            }
            else
            {
                var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                enableIntent.SetFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(enableIntent);
            }
#endif
        }

#if ANDROID
        private void OnDeviceTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is BluetoothDeviceItem device)
            {
                PairDevice(device);
            }
        }

        private void PairDevice(BluetoothDeviceItem device)
{
    try
    {
        var bluetoothDevice = _bluetoothAdapter.GetRemoteDevice(device.Address);
        if (bluetoothDevice.BondState == Bond.None)
        {
            // Device is not paired; initiate pairing
            bluetoothDevice.CreateBond();
            DisplayAlert("Pairing", $"Pairing with {device.Name}", "OK");
        }
        else if (bluetoothDevice.BondState == Bond.Bonded)
        {
            // Device is already paired
            DisplayAlert("Already Paired", $"{device.Name} is already paired.", "OK");
            ConnectToDevice(device);
        }
    }
    catch (Exception ex)
    {
        //Console.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
        //DisplayAlert("Error", $"Failed to pair with {device.Name}: {ex.Message}", "OK");
    }
}


        private async void ConnectToDevice(BluetoothDeviceItem device)
        {
            try
            {
                var bluetoothDevice = _bluetoothAdapter.GetRemoteDevice(device.Address);
                var uuid = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"); // Standard Serial Port Profile UUID
                var socket = bluetoothDevice.CreateRfcommSocketToServiceRecord(uuid);

                _bluetoothAdapter.CancelDiscovery();

                await Task.Run(() =>
                {
                    try
                    {
                        socket.Connect();
                    }
                    catch (Java.IO.IOException connectEx)
                    {
                        socket.Close();
                        throw;
                    }
                });

                DisplayAlert("Connected", $"Connected to {device.Name}", "OK");
            }
            catch (Java.IO.IOException ioEx)
            {
                Console.WriteLine($"IO Exception: {ioEx.Message}\n{ioEx.StackTrace}");
                //DisplayAlert("Error", $"Failed to connect to {device.Name} due to IO error: {ioEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
                //DisplayAlert("Error", $"Failed to connect to {device.Name}: {ex.Message}\nStackTrace: {ex.StackTrace}", "OK");
            }
        }
#endif
    }
}

//#if ANDROID
//using Android;
//using Android.Bluetooth;
//using Android.Content;
//using Android.Content.PM;
//using Android.OS;
//#endif
//using Microsoft.Maui.Controls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Demo_bluetooth
//{
//    public partial class MainPage : ContentPage
//    {
//#if ANDROID
//        private BluetoothAdapter _bluetoothAdapter;
//        private readonly List<BluetoothDeviceItem> _devices = new List<BluetoothDeviceItem>();
//        private BluetoothReceiver _receiver;

//        private class GattCallback : BluetoothGattCallback
//        {
//            public event EventHandler<List<string>> ServicesDiscovered;

//            public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
//            {
//                base.OnServicesDiscovered(gatt, status);

//                if (status == GattStatus.Success)
//                {
//                    var serviceUuids = new List<string>();
//                    foreach (var service in gatt.Services)
//                    {
//                        serviceUuids.Add(service.Uuid.ToString());
//                        Console.WriteLine($"Service UUID: {service.Uuid}");
//                    }
//                    ServicesDiscovered?.Invoke(this, serviceUuids);
//                }
//                else
//                {
//                    Console.WriteLine($"Service discovery failed with status: {status}");
//                }
//            }
//        }
//#endif

//        public MainPage()
//        {
//            InitializeComponent();

//#if ANDROID
//            RequestPermissions();
//            var bluetoothManager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
//            _bluetoothAdapter = bluetoothManager.Adapter;

//            if (_bluetoothAdapter == null)
//            {
//                DisplayAlert("Error", "Bluetooth is not supported on this device.", "OK");
//                return;
//            }

//            _receiver = new BluetoothReceiver(_devices, DevicesListView);
//#endif
//        }

//#if ANDROID
//        private void RequestPermissions()
//        {
//            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
//            {
//                string[] permissions = new string[]
//                {
//                    Manifest.Permission.BluetoothScan,
//                    Manifest.Permission.BluetoothConnect,
//                    Manifest.Permission.AccessFineLocation
//                };

//                var activity = Platform.CurrentActivity;
//                activity.RequestPermissions(permissions, 0);
//            }
//        }

//        protected override void OnAppearing()
//        {
//            base.OnAppearing();
//            var filter = new IntentFilter(BluetoothDevice.ActionFound);
//            Android.App.Application.Context.RegisterReceiver(_receiver, filter);
//            Android.App.Application.Context.RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
//        }

//        protected override void OnDisappearing()
//        {
//            base.OnDisappearing();
//            Android.App.Application.Context.UnregisterReceiver(_receiver);
//        }
//#endif

//        private void OnConnectionClicked(object sender, EventArgs e)
//        {
//#if ANDROID
//            var bluetoothManager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
//            var bluetoothAdapter = bluetoothManager.Adapter;

//            if (bluetoothAdapter == null)
//            {
//                DisplayAlert("Error", "Bluetooth is not supported on this device.", "OK");
//                return;
//            }

//            if (bluetoothAdapter.IsEnabled)
//            {
//                if (!bluetoothAdapter.IsDiscovering)
//                {
//                    bluetoothAdapter.StartDiscovery();
//                }
//            }
//            else
//            {
//                var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
//                enableIntent.SetFlags(ActivityFlags.NewTask);
//                Android.App.Application.Context.StartActivity(enableIntent);
//            }
//#endif
//        }

//#if ANDROID
//        private void OnDeviceTapped(object sender, ItemTappedEventArgs e)
//        {
//            if (e.Item is BluetoothDeviceItem device)
//            {
//                // Discover services and use the UUID to conn ect
//                DiscoverServices(device);
//            }
//         }

//        private void DiscoverServices(BluetoothDeviceItem device)
//        {
//            try
//            {
//                var bluetoothDevice = _bluetoothAdapter.GetRemoteDevice(device.Address);
//                var gattCallback = new GattCallback();
//                gattCallback.ServicesDiscovered += (sender, serviceUuids) =>
//                {
//                    // Assuming you want to use the first service UUID found for this example
//                    if (serviceUuids.Any())
//                    {
//                        ConnectToDevice(device, serviceUuids.First());
//                    }
//                    else
//                    {
//                        DisplayAlert("Error", "No services found on device.", "OK");
//                    }
//                };

//                BluetoothGatt gatt = bluetoothDevice.ConnectGatt(Android.App.Application.Context, false, gattCallback);
//                gatt.DiscoverServices();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
//                DisplayAlert("Error", $"Failed to discover services for {device.Name}: {ex.Message}\nStackTrace: {ex.StackTrace}", "OK");
//            }
//        }

//        private async void ConnectToDevice(BluetoothDeviceItem device, string serviceUuid)
//        {
//            try
//            {
//                Console.WriteLine($"Attempting to connect to {device.Name} with address {device.Address} and service UUID {serviceUuid}");

//                if (!_bluetoothAdapter.IsEnabled)
//                {
//                    Console.WriteLine("Bluetooth is not enabled.");
//                    DisplayAlert("Error", "Bluetooth is not enabled.", "OK");
//                    return;
//                }

//                var bluetoothDevice = _bluetoothAdapter.GetRemoteDevice(device.Address);
//                var uuid = Java.Util.UUID.FromString(serviceUuid);

//                Console.WriteLine("Creating RFComm socket...");
//                var socket = bluetoothDevice.CreateRfcommSocketToServiceRecord(uuid);

//                Console.WriteLine("Cancelling discovery to speed up the connection...");
//                _bluetoothAdapter.CancelDiscovery();

//                await Task.Run(() =>
//                {
//                    try
//                    {
//                        Console.WriteLine("Connecting to socket...");
//                        socket.Connect();
//                        Console.WriteLine("Socket connected.");
//                    }
//                    catch (Java.IO.IOException connectEx)
//                    {
//                        Console.WriteLine($"Socket connect exception: {connectEx.Message}");
//                        socket.Close();
//                        throw;
//                    }
//                });

//                DisplayAlert("Connected", $"Connected to {device.Name}", "OK");
//            }
//            catch (Java.IO.IOException ioEx)
//            {
//                Console.WriteLine($"IO Exception: {ioEx.Message}\n{ioEx.StackTrace}");
//                DisplayAlert("Error", $"Failed to connect to {device.Name} due to IO error: {ioEx.Message}", "OK");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
//                DisplayAlert("Error", $"Failed to connect to {device.Name}: {ex.Message}\nStackTrace: {ex.StackTrace}", "OK");
//            }
//        }
//#endif
//    }
//}
