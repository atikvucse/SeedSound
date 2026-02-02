namespace SeedSound.Core.Models;

public class Song
{
    public int Index { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Likes { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public List<string> Lyrics { get; set; } = new();
    public CoverImage Cover { get; set; } = new();
    public MusicData Music { get; set; } = new();
}

public class CoverImage
{
    public string BackgroundColor { get; set; } = string.Empty;
    public string PatternType { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
    public int PatternSeed { get; set; }
}

public class MusicData
{
    public int Tempo { get; set; }
    public string Key { get; set; } = string.Empty;
    public List<NoteEvent> Notes { get; set; } = new();
    public List<DrumEvent> Drums { get; set; } = new();
    public List<BassNote> Bass { get; set; } = new();
    public int DurationMs { get; set; }
}

public class NoteEvent
{
    public double Time { get; set; }
    public int Note { get; set; }
    public double Duration { get; set; }
    public double Velocity { get; set; }
    public string Instrument { get; set; } = "synth";
}

public class DrumEvent
{
    public double Time { get; set; }
    public string Type { get; set; } = string.Empty;
    public double Velocity { get; set; }
}

public class BassNote
{
    public double Time { get; set; }
    public int Note { get; set; }
    public double Duration { get; set; }
}
