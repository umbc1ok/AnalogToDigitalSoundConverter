using NAudio;
using NAudio.Wave;

namespace Logic
{

    public class SoundConverter
    {
        WaveInEvent waveIn;
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
    }
}