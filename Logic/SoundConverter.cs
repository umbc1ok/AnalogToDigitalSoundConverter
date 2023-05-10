
using NAudio.Wave;

using System.Net.Sockets;
using System.Net;
using System.IO;
using Microsoft.VisualBasic;

namespace Logic
{

    public class SoundConverter
    {
        WaveInEvent waveIn;
        WaveInEvent waveInStreaming;
        WaveFileWriter writer;

        public int sampleRate = 44100; // Częstotliwość próbkowania (przykładowo 44100 Hz)
        public int bitDepth = 16; // Ilość bitów kwantyzacji (przykładowo 16 bitów)
        public int channels = 2; // Liczba kanałów (przykładowo 1 dla mono)

        private string outputPath;
        public SoundConverter()
        {
            sampleRate = 44100; // Częstotliwość próbkowania (przykładowo 44100 Hz)
            bitDepth = 16; // Ilość bitów kwantyzacji (przykładowo 16 bitów)
            channels = 2; // Liczba kanałów (przykładowo 1 dla mono)

    }
        public void SetSavePath(string path)
        {
            outputPath = path;
        }
        public void StartRecording()
        {
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
            writer = new WaveFileWriter(outputPath, waveIn.WaveFormat);
            // Obsługa zdarzenia zakończenia nagrywania
            waveIn.DataAvailable += (sender, e) =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            };

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

                        WaveFormat waveF = new WaveFormat(sampleRate, bitDepth, channels);


                        waveOut.OutputWaveFormat = waveF;
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
            TcpListener listener = new TcpListener(IPAddress.Parse(IP), 12000);
            listener.Start();

            using (TcpClient client = listener.AcceptTcpClient())
            {
                Task.Run(() => { waveInStreaming.StartRecording(); });

                using (NetworkStream stream = client.GetStream())
                {
                    waveInStreaming.DataAvailable += (sender, args) =>
                    {
                        // Send the audio data to the receiving PC
                        stream.Write(args.Buffer, 0, args.BytesRecorded);
                    };
                    Task.Delay(10000).Wait(); //THIS LINE DECIDES HOW LONG THE CONNECTION WILL LAST
                    waveInStreaming.StopRecording();
                }
                client.Close();
            }
            listener.Stop();
        }

        public void ReceiveAudio(string IP)
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(IPAddress.Parse(IP), 12000);

                WaveOutEvent waveOut = new WaveOutEvent();
                WaveFormat waveFormat = new WaveFormat(44100, 16, 1); // Assuming the audio is in stereo and 44100 Hz sample rate

                using (NetworkStream stream = client.GetStream())
                {
                    while (true)
                    {
                        byte[] buffer = new byte[131064];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        using (MemoryStream memoryStream = new MemoryStream(buffer, 0, bytesRead))
                        {
                            using (WaveStream waveStream = new RawSourceWaveStream(memoryStream, waveFormat))
                            {
                                waveOut.Init(waveStream);
                                waveOut.Play();
                                while (waveOut.PlaybackState == PlaybackState.Playing)
                                {
                                    Thread.Sleep(10);
                                }
                            }
                        }
                    }
                }
                client.Close();
            }
        }
    }
}


