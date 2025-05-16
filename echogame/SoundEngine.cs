using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

public static class SoundEngine
{
    private static readonly WaveOutEvent outputDevice;
    private static readonly MixingSampleProvider mixer;
    private static readonly ConcurrentDictionary<string, AudioFileReader> cachedSounds;
    private static readonly object lockObj = new object();
    private static float masterVolume = 1.0f;

    // Реализация Clamp для float
    private static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    static SoundEngine()
    {
        outputDevice = new WaveOutEvent();
        mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
        {
            ReadFully = true
        };
        outputDevice.Init(mixer);
        outputDevice.Play();
        cachedSounds = new ConcurrentDictionary<string, AudioFileReader>();
    }

    public static float MasterVolume
    {
        get => masterVolume;
        set => masterVolume = Clamp(value, 0f, 1f);
    }

    public static void PreloadSound(string key, string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Sound file not found: {filePath}");

        lock (lockObj)
        {
            if (!cachedSounds.ContainsKey(key))
            {
                var reader = new AudioFileReader(filePath);
                cachedSounds.TryAdd(key, reader);
            }
        }
    }

    public static void Play(string key, float volume = 1.0f)
    {
        if (!cachedSounds.TryGetValue(key, out var cachedSound))
            return;

        lock (lockObj)
        {
            var sound = new AudioFileReader(cachedSound.FileName)
            {
                Volume = Clamp(volume, 0f, 1f) * masterVolume
            };

            var volumeProvider = new VolumeSampleProvider(sound.ToSampleProvider());
            mixer.AddMixerInput(volumeProvider);
        }
    }

    public static void UnloadSound(string key)
    {
        lock (lockObj)
        {
            if (cachedSounds.TryRemove(key, out var sound))
            {
                sound.Dispose();
            }
        }
    }

    public static void Dispose()
    {
        lock (lockObj)
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();

            foreach (var sound in cachedSounds.Values)
            {
                sound.Dispose();
            }
            cachedSounds.Clear();
        }
    }
}