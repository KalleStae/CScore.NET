using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using CSCore.Win32;

using System;
using System.Collections.Generic;

namespace PlayInputDeviceSample
{
    class Program
    {
        static List<MMDevice> Devices = new List<MMDevice>();
        static MMDevice DefaultDevice;
        static void Main()
        {
            Console.WriteLine("The following sample will use the default input device");
            Console.WriteLine("and will play its input data on the default output device.");
            Console.WriteLine("In addition, this sample will apply an echo effect.");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            GetDevices();
            Foo();
        }
        static List<MMDevice> GetDevices()
        {
            Devices.Clear();
            using (MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator())
            using (MMDeviceCollection deviceCollection = deviceEnumerator.EnumAudioEndpoints(
                DataFlow.All, DeviceState.Active))
            {
                DefaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                foreach (MMDevice device in deviceCollection)
                {
                    WaveFormat deviceFormat = WaveFormatFromBlob(device.PropertyStore[
                        new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0)].BlobValue);
                    Devices.Add(device);
                    // Debug.WriteLine(device.FriendlyName);
                }
            }
            return Devices;
        }
        /// <summary>
        /// Get Device property from Windows OS blob
        /// </summary>
        /// <param name="blob"></param>
        /// <returns>wave format of the device</returns>
        private static WaveFormat WaveFormatFromBlob(Blob blob)
        {
            if (blob.Length == 40)
            {
                return (WaveFormat)System.Runtime.InteropServices.Marshal.PtrToStructure(blob.Data, typeof(WaveFormatExtensible));
            }

            return (WaveFormat)System.Runtime.InteropServices.Marshal.PtrToStructure(blob.Data, typeof(WaveFormat));

        }
        static MMDevice GetDefaultDevice(DataFlow dataFlow, Role role)
        {
            using (MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator())
            {
                DefaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(dataFlow, role);
            }

            return DefaultDevice;
        }
        static void Foo()
        {

            //create a new soundIn instance for using input devices
            using (WasapiCapture soundIn = new WasapiCapture(true, AudioClientShareMode.Shared, 30))
            {

                soundIn.Device = Devices[2];
                //important: always initialize the soundIn instance before creating the
                //SoundInSource. The SoundInSource needs the WaveFormat of the soundIn,
                //which gets determined by the soundIn.Initialize method.
                soundIn.Initialize();

                //wrap a sound source around the soundIn instance
                //in order to prevent playback interruptions, set FillWithZeros to true
                //otherwise, if the SoundIn does not provide enough data, the playback stops
                IWaveSource source = new SoundInSource(soundIn) { FillWithZeros = true };

                //wrap a EchoEffect around the previously created "source"
                //note: disposing the echoSource will also dispose 
                //the previously created "source"
                using (DmoEchoEffect echoSource = new DmoEchoEffect(source))
                {
                    //start capturing data
                    soundIn.Start();

                    //create a soundOut instance to play the data
                    using (WasapiOut soundOut = new WasapiOut())
                    {
                        //initialize the soundOut with the echoSource
                        //the echoSource provides data from the "source" and applies the echo effect
                        //the "source" provides data from the "soundIn" instance
                        soundOut.Initialize(echoSource);

                        //play
                        soundOut.Play();

                        Console.WriteLine("Press any key to exit the program.");
                        Console.ReadKey();
                    }
                }
            }
        }

    }
}
