using SeedSound.Core.Models;
using SeedSound.Core.Services;

namespace SeedSound.Infrastructure.Services;

public class MusicGeneratorService : IMusicGeneratorService
{
    private static readonly int[] MajorScale = { 0, 2, 4, 5, 7, 9, 11 };
    private static readonly int[] MinorScale = { 0, 2, 3, 5, 7, 8, 10 };
    private static readonly int[] PentatonicScale = { 0, 2, 4, 7, 9 };
    private static readonly int[] BluesScale = { 0, 3, 5, 6, 7, 10 };

    private static readonly int[][] ChordProgressions = new[]
    {
        new[] { 0, 3, 4, 4 },
        new[] { 0, 4, 5, 3 },
        new[] { 0, 0, 3, 4 },
        new[] { 0, 5, 3, 4 },
        new[] { 0, 3, 0, 4 },
        new[] { 0, 4, 0, 3 },
        new[] { 5, 3, 0, 4 },
        new[] { 0, 0, 4, 3 }
    };

    private static readonly Dictionary<string, (int minTempo, int maxTempo, bool useMinor)> GenreSettings = new()
    {
        ["Rock"] = (110, 140, false),
        ["Pop"] = (100, 130, false),
        ["Electronic"] = (120, 150, false),
        ["Hip-Hop"] = (85, 100, true),
        ["R&B"] = (70, 95, true),
        ["Jazz"] = (90, 140, true),
        ["Blues"] = (70, 100, true),
        ["Country"] = (100, 130, false),
        ["Folk"] = (90, 120, false),
        ["Classical"] = (60, 120, false),
        ["Metal"] = (140, 180, true),
        ["Punk"] = (150, 190, false),
        ["Indie"] = (100, 130, false),
        ["Alternative"] = (110, 140, true),
        ["Soul"] = (75, 100, true),
        ["Funk"] = (100, 130, true),
        ["Disco"] = (115, 130, false),
        ["House"] = (120, 130, false),
        ["Techno"] = (125, 145, true),
        ["Trance"] = (130, 150, false),
        ["Ambient"] = (60, 90, true),
        ["Reggae"] = (80, 100, true),
        ["Latin"] = (100, 130, false),
        ["World"] = (90, 120, false),
        ["Heavy Metal"] = (140, 180, true),
        ["Rock'n'Roll"] = (130, 160, false),
        ["Schlager"] = (100, 130, false),
        ["Volksmusik"] = (90, 120, false),
        ["Рок"] = (110, 140, false),
        ["Поп"] = (100, 130, false),
        ["Електроніка"] = (120, 150, false),
        ["Метал"] = (140, 180, true),
        ["Класика"] = (60, 120, false),
        ["Elektronisch"] = (120, 150, false),
        ["Klassik"] = (60, 120, false)
    };

    public MusicData GenerateMusic(long seed, int songIndex, string genre)
    {
        long musicSeed = seed * 41 + songIndex * 59;
        var random = new Random((int)(musicSeed & 0x7FFFFFFF));

        var settings = GenreSettings.GetValueOrDefault(genre, (100, 130, false));
        int tempo = random.Next(settings.Item1, settings.Item2 + 1);
        double beatDuration = 60.0 / tempo;

        var scale = settings.Item3
            ? (random.Next(3) == 0 ? BluesScale : MinorScale)
            : (random.Next(3) == 0 ? PentatonicScale : MajorScale);

        int rootNote = 48 + random.Next(12);
        var progression = ChordProgressions[random.Next(ChordProgressions.Length)];

        int totalBars = 16;
        int beatsPerBar = 4;
        double totalDuration = totalBars * beatsPerBar * beatDuration;

        var notes = GenerateMelody(random, scale, rootNote, tempo, totalBars, beatsPerBar, progression);
        var drums = GenerateDrums(random, tempo, totalBars, beatsPerBar, genre);
        var bass = GenerateBassLine(random, scale, rootNote, tempo, totalBars, beatsPerBar, progression);

        string key = GetKeyName(rootNote, settings.Item3);

        return new MusicData
        {
            Tempo = tempo,
            Key = key,
            Notes = notes,
            Drums = drums,
            Bass = bass,
            DurationMs = (int)(totalDuration * 1000)
        };
    }

    private List<NoteEvent> GenerateMelody(Random random, int[] scale, int rootNote, int tempo,
        int totalBars, int beatsPerBar, int[] progression)
    {
        var notes = new List<NoteEvent>();
        double beatDuration = 60.0 / tempo;

        var rhythmPatterns = new[]
        {
            new[] { 1.0, 1.0, 1.0, 1.0 },
            new[] { 2.0, 1.0, 1.0 },
            new[] { 1.0, 0.5, 0.5, 1.0, 1.0 },
            new[] { 0.5, 0.5, 1.0, 0.5, 0.5, 1.0 },
            new[] { 1.5, 0.5, 1.0, 1.0 },
            new[] { 1.0, 1.0, 2.0 }
        };

        int lastScaleIndex = random.Next(scale.Length);
        string[] instruments = { "synth", "piano", "strings" };
        string instrument = instruments[random.Next(instruments.Length)];

        for (int bar = 0; bar < totalBars; bar++)
        {
            int chordIndex = progression[bar % progression.Length];
            int chordRoot = rootNote + scale[chordIndex % scale.Length];

            var pattern = rhythmPatterns[random.Next(rhythmPatterns.Length)];
            double barStartTime = bar * beatsPerBar * beatDuration;
            double timeInBar = 0;

            foreach (double noteLength in pattern)
            {
                if (timeInBar >= beatsPerBar) break;

                bool playNote = random.Next(10) > 1;
                if (playNote)
                {
                    int jump = random.Next(-2, 3);
                    lastScaleIndex = Math.Clamp(lastScaleIndex + jump, 0, scale.Length - 1);

                    int octaveShift = random.Next(3) == 0 ? (random.Next(2) == 0 ? 12 : -12) : 0;
                    int noteValue = chordRoot + scale[lastScaleIndex] + octaveShift;

                    double duration = noteLength * beatDuration * (0.7 + random.NextDouble() * 0.3);
                    double velocity = 0.5 + random.NextDouble() * 0.4;

                    notes.Add(new NoteEvent
                    {
                        Time = barStartTime + timeInBar * beatDuration,
                        Note = noteValue,
                        Duration = duration,
                        Velocity = velocity,
                        Instrument = instrument
                    });
                }

                timeInBar += noteLength;
            }
        }

        return notes;
    }

    private List<DrumEvent> GenerateDrums(Random random, int tempo, int totalBars, int beatsPerBar, string genre)
    {
        var drums = new List<DrumEvent>();
        double beatDuration = 60.0 / tempo;

        bool isElectronic = genre.Contains("Electro") || genre.Contains("House") ||
                           genre.Contains("Techno") || genre.Contains("Trance") ||
                           genre.Contains("Disco") || genre.Contains("Електро");

        bool isHeavy = genre.Contains("Metal") || genre.Contains("Punk") ||
                      genre.Contains("Rock") || genre.Contains("Метал") ||
                      genre.Contains("Рок");

        for (int bar = 0; bar < totalBars; bar++)
        {
            double barStart = bar * beatsPerBar * beatDuration;

            for (int beat = 0; beat < beatsPerBar; beat++)
            {
                double beatTime = barStart + beat * beatDuration;

                if (beat == 0 || beat == 2)
                {
                    drums.Add(new DrumEvent
                    {
                        Time = beatTime,
                        Type = "kick",
                        Velocity = 0.8 + random.NextDouble() * 0.2
                    });
                }

                if (beat == 1 || beat == 3)
                {
                    drums.Add(new DrumEvent
                    {
                        Time = beatTime,
                        Type = "snare",
                        Velocity = 0.7 + random.NextDouble() * 0.3
                    });
                }

                if (isElectronic)
                {
                    drums.Add(new DrumEvent
                    {
                        Time = beatTime,
                        Type = "hihat",
                        Velocity = 0.3 + random.NextDouble() * 0.3
                    });
                    drums.Add(new DrumEvent
                    {
                        Time = beatTime + beatDuration / 2,
                        Type = "hihat",
                        Velocity = 0.2 + random.NextDouble() * 0.2
                    });
                }
                else
                {
                    if (random.Next(2) == 0)
                    {
                        drums.Add(new DrumEvent
                        {
                            Time = beatTime,
                            Type = "hihat",
                            Velocity = 0.3 + random.NextDouble() * 0.3
                        });
                    }
                }

                if (isHeavy && random.Next(4) == 0)
                {
                    drums.Add(new DrumEvent
                    {
                        Time = beatTime + beatDuration * 0.75,
                        Type = "kick",
                        Velocity = 0.6 + random.NextDouble() * 0.2
                    });
                }
            }
        }

        return drums;
    }

    private List<BassNote> GenerateBassLine(Random random, int[] scale, int rootNote, int tempo,
        int totalBars, int beatsPerBar, int[] progression)
    {
        var bass = new List<BassNote>();
        double beatDuration = 60.0 / tempo;
        int bassRoot = rootNote - 12;

        var patterns = new[]
        {
            new[] { 0, -1, 0, -1 },
            new[] { 0, 0, 2, 0 },
            new[] { 0, 2, 3, 2 },
            new[] { 0, -1, 2, -1 }
        };

        var rhythms = new[]
        {
            new[] { 1.0, 1.0, 1.0, 1.0 },
            new[] { 2.0, 2.0 },
            new[] { 1.5, 0.5, 1.0, 1.0 },
            new[] { 1.0, 0.5, 0.5, 1.0, 1.0 }
        };

        var pattern = patterns[random.Next(patterns.Length)];
        var rhythm = rhythms[random.Next(rhythms.Length)];

        for (int bar = 0; bar < totalBars; bar++)
        {
            int chordIndex = progression[bar % progression.Length];
            int chordRoot = bassRoot + scale[chordIndex % scale.Length];

            double barStart = bar * beatsPerBar * beatDuration;
            double timeInBar = 0;
            int noteIndex = 0;

            foreach (double noteLength in rhythm)
            {
                if (timeInBar >= beatsPerBar) break;

                int patternNote = pattern[noteIndex % pattern.Length];
                if (patternNote >= 0)
                {
                    int noteValue = chordRoot + (patternNote < scale.Length ? scale[patternNote] : 0);

                    bass.Add(new BassNote
                    {
                        Time = barStart + timeInBar * beatDuration,
                        Note = noteValue,
                        Duration = noteLength * beatDuration * 0.8
                    });
                }

                timeInBar += noteLength;
                noteIndex++;
            }
        }

        return bass;
    }

    private string GetKeyName(int midiNote, bool isMinor)
    {
        var noteNames = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        string noteName = noteNames[midiNote % 12];
        return isMinor ? $"{noteName} minor" : $"{noteName} major";
    }
}
