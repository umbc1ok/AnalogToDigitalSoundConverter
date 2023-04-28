using NAudio;
using NAudio.Wave;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Microsoft.VisualBasic;
using System;

namespace Logic
{

    public class SoundConverter
    {
        WaveInEvent waveIn;
        WaveInEvent waveInStreaming;
        string outputPath;
        WaveFileWriter writer;


        public SoundConverter()
        {
            int sampleRate = 44100; // Częstotliwość próbkowania (przykładowo 44100 Hz)
            int bitDepth = 16; // Ilość bitów kwantyzacji (przykładowo 16 bitów)
            int channels = 1; // Liczba kanałów (przykładowo 1 dla mono)

            // Utworzenie obiektu nagrywania
            waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(sampleRate, bitDepth, channels),
                BufferMilliseconds = 100 // Czas buforowania próbek
            };
            waveInStreaming = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(sampleRate, bitDepth, channels),
                BufferMilliseconds = 100 // Czas buforowania próbek
            };


        }
        public void SetSavePath(string path)
        {
            outputPath = path;
            writer = new WaveFileWriter(outputPath, waveIn.WaveFormat);
            // Obsługa zdarzenia zakończenia nagrywania
            waveIn.DataAvailable += (sender, e) =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            };
        }
        public void StartRecording()
        {
            waveIn.StartRecording();
        }
        public void StopRecording()
        {
            waveIn.StopRecording();

            // Zakończenie zapisu do pliku WAV
            writer.Dispose();
        }

        public void Play(string filePath)
        {
            Task.Run(() =>
            {
                // Utworzenie obiektu odtwarzacza
                using (var waveOut = new WaveOutEvent())
                {
                    // Utworzenie obiektu czytającego z pliku WAV
                    using (var audioFile = new AudioFileReader(filePath))
                    {
                        // Dodanie czytacza do odtwarzacza
                        waveOut.Init(audioFile);

                        // Odtworzenie pliku WAV
                        waveOut.Play();

                        // Oczekiwanie na zakończenie odtwarzania
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        waveOut.Dispose();
                    }
                }
            });

        }
        public void StreamAudio(string IP)
        {
            UdpClient udpClient = new UdpClient(12000);
            Task.Run(() => { waveInStreaming.StartRecording(); });
            waveInStreaming.DataAvailable += (sender, args) =>
            {
                // Send the audio data to the receiving PC
                udpClient.Send(args.Buffer, args.BytesRecorded, new IPEndPoint(IPAddress.Parse(IP), 12000));
            };
            Task.Delay(10000).Wait();
            waveInStreaming.StopRecording();
            udpClient.Close();
        }
        public void ReceiveAudio(string IP)
        {
            UdpClient udpClient = new UdpClient(12000);

            WaveOutEvent waveOut = new WaveOutEvent();
            WaveFormat waveFormat = new WaveFormat(44100, 16, 1);

            byte[] buffer = new byte[4096]; // Adjust the buffer size as per your requirements
            MemoryStream memoryStream = new MemoryStream();

            Task.Run(() =>
            {
                while (true)
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(IP), 12000);
                    byte[] audioData = udpClient.Receive(ref RemoteIpEndPoint);
                    memoryStream.Write(audioData, 0, audioData.Length);

                    while (memoryStream.Length >= buffer.Length)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.Read(buffer, 0, buffer.Length);

                        using (WaveStream waveStream = new RawSourceWaveStream(new MemoryStream(buffer), waveFormat))
                        {
                            waveOut.Init(waveStream);
                            waveOut.Play();
                        }

                        memoryStream.SetLength(memoryStream.Length - buffer.Length);
                        memoryStream.Position = memoryStream.Length;
                    }
                }
            });
            Task.Delay(10000).Wait();
            
            udpClient.Close();
        }
    }
}


