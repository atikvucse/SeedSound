using System.IO.Compression;
using NAudio.Lame;
using NAudio.Wave;
using SeedSound.Core.Models;
using SeedSound.Core.Services;

namespace SeedSound.Infrastructure.Services;

public class ExportService : IExportService
{
    public byte[] ExportToZip(IEnumerable<Song> songs)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var song in songs)
            {
                string safeName = SanitizeFileName($"{song.Title} - {song.Album} - {song.Artist}");
                string fileName = $"{safeName}.mp3";

                var entry = archive.CreateEntry(fileName);
                using var entryStream = entry.Open();

                var mp3Data = GenerateMp3FromMusic(song.Music, song.Title, song.Artist, song.Album);
                entryStream.Write(mp3Data, 0, mp3Data.Length);
            }
        }

        return memoryStream.ToArray();
    }

    private string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
    }

    private byte[] GenerateMp3FromMusic(MusicData music, string title, string artist, string album)
    {
        int sampleRate = 44100;
        int durationSamples = (int)(music.DurationMs / 1000.0 * sampleRate);
        var samples = new float[durationSamples];

        foreach (var note in music.Notes)
        {
            AddNoteToSamples(samples, sampleRate, note.Time, note.Duration, MidiToFreq(note.Note), (float)note.Velocity * 0.3f);
        }

        foreach (var bass in music.Bass)
        {
            AddNoteToSamples(samples, sampleRate, bass.Time, bass.Duration, MidiToFreq(bass.Note), 0.35f);
        }

        foreach (var drum in music.Drums)
        {
            AddDrumToSamples(samples, sampleRate, drum);
        }

        NormalizeSamples(samples);

        // Convert to 16-bit PCM bytes
        var pcmBytes = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * 32767);
            pcmBytes[i * 2] = (byte)(sample & 0xFF);
            pcmBytes[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        // Encode to MP3
        using var mp3Stream = new MemoryStream();
        var waveFormat = new WaveFormat(sampleRate, 16, 1);

        var id3Tag = new ID3TagData
        {
            Title = title,
            Artist = artist,
            Album = album,
            Year = DateTime.Now.Year.ToString()
        };

        using (var rawStream = new RawSourceWaveStream(new MemoryStream(pcmBytes), waveFormat))
        using (var mp3Writer = new LameMP3FileWriter(mp3Stream, waveFormat, 128, id3Tag))
        {
            rawStream.CopyTo(mp3Writer);
        }

        return mp3Stream.ToArray();
    }

    private void AddNoteToSamples(float[] samples, int sampleRate, double startTime, double duration, float frequency, float amplitude)
    {
        int startSample = (int)(startTime * sampleRate);
        int numSamples = (int)(duration * sampleRate);

        for (int i = 0; i < numSamples && startSample + i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f;

            if (t < 0.02f) envelope = t / 0.02f;
            else if (t > duration - 0.1f) envelope = (float)(duration - t) / 0.1f;

            envelope = Math.Clamp(envelope, 0f, 1f);

            float sample = (float)Math.Sin(2 * Math.PI * frequency * t) * amplitude * envelope;
            samples[startSample + i] += sample;
        }
    }

    private void AddDrumToSamples(float[] samples, int sampleRate, DrumEvent drum)
    {
        int startSample = (int)(drum.Time * sampleRate);
        var random = new Random((int)(drum.Time * 1000));

        if (drum.Type == "kick")
        {
            int numSamples = (int)(0.2 * sampleRate);
            for (int i = 0; i < numSamples && startSample + i < samples.Length; i++)
            {
                float t = (float)i / sampleRate;
                float freq = 150 * (float)Math.Exp(-t * 30);
                float envelope = (float)Math.Exp(-t * 10);
                samples[startSample + i] += (float)Math.Sin(2 * Math.PI * freq * t) * (float)drum.Velocity * 0.6f * envelope;
            }
        }
        else if (drum.Type == "snare")
        {
            int numSamples = (int)(0.1 * sampleRate);
            for (int i = 0; i < numSamples && startSample + i < samples.Length; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = (float)Math.Exp(-t * 20);
                samples[startSample + i] += ((float)random.NextDouble() * 2 - 1) * (float)drum.Velocity * 0.4f * envelope;
            }
        }
        else if (drum.Type == "hihat")
        {
            int numSamples = (int)(0.05 * sampleRate);
            for (int i = 0; i < numSamples && startSample + i < samples.Length; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = (float)Math.Exp(-t * 50);
                samples[startSample + i] += ((float)random.NextDouble() * 2 - 1) * (float)drum.Velocity * 0.15f * envelope;
            }
        }
    }

    private void NormalizeSamples(float[] samples)
    {
        float max = 0;
        foreach (var sample in samples)
        {
            if (Math.Abs(sample) > max) max = Math.Abs(sample);
        }

        if (max > 0)
        {
            float scale = 0.9f / max;
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= scale;
            }
        }
    }

    private float MidiToFreq(int midi)
    {
        return 440f * (float)Math.Pow(2, (midi - 69) / 12.0);
    }
}
