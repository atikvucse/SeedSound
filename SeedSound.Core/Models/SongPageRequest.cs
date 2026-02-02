namespace SeedSound.Core.Models;

public class SongPageRequest
{
    public string Locale { get; set; } = "en_US";
    public long Seed { get; set; }
    public double AverageLikes { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SongPageResponse
{
    public List<Song> Songs { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; } = true;
}
