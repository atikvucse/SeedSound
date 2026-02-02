using Bogus;
using SeedSound.Core.Localization;
using SeedSound.Core.Models;
using SeedSound.Core.Services;

namespace SeedSound.Infrastructure.Services;

public class SongGeneratorService : ISongGeneratorService
{
    private readonly IMusicGeneratorService _musicGenerator;

    public SongGeneratorService(IMusicGeneratorService musicGenerator)
    {
        _musicGenerator = musicGenerator;
    }

    public SongPageResponse GenerateSongs(SongPageRequest request)
    {
        var songs = new List<Song>();
        int startIndex = (request.Page - 1) * request.PageSize + 1;

        for (int i = 0; i < request.PageSize; i++)
        {
            int songIndex = startIndex + i;
            var song = GenerateSingleSong(request.Locale, request.Seed, request.AverageLikes, songIndex);
            songs.Add(song);
        }

        return new SongPageResponse
        {
            Songs = songs,
            Page = request.Page,
            PageSize = request.PageSize,
            HasMore = true
        };
    }

    public Song GenerateSingleSong(string locale, long seed, double averageLikes, int index)
    {
        var localeData = LocaleDataStore.GetLocale(locale);
        long songSeed = CombineSeed(seed, index);
        var random = new Random((int)(songSeed & 0x7FFFFFFF));
        var faker = new Faker(localeData.BogusLocale);
        faker.Random = new Randomizer((int)(songSeed & 0x7FFFFFFF));

        string title = GenerateTitle(random, localeData);
        string artist = GenerateArtist(random, localeData, faker);
        string album = GenerateAlbum(random, localeData, title);
        string genre = localeData.Genres[random.Next(localeData.Genres.Count)];
        int likes = CalculateLikes(seed, index, averageLikes);
        string review = GenerateReview(random, localeData);
        var lyrics = GenerateLyrics(random, localeData);
        var cover = GenerateCover(random, index);
        var music = _musicGenerator.GenerateMusic(seed, index, genre);

        return new Song
        {
            Index = index,
            Title = title,
            Artist = artist,
            Album = album,
            Genre = genre,
            Likes = likes,
            ReviewText = review,
            Lyrics = lyrics,
            Cover = cover,
            Music = music
        };
    }

    private long CombineSeed(long baseSeed, int index)
    {
        return baseSeed * 31 + index * 17;
    }

    private string GenerateTitle(Random random, LocaleData locale)
    {
        int pattern = random.Next(5);
        return pattern switch
        {
            0 => $"{Pick(random, locale.TitlePrefixes)} {Pick(random, locale.TitleNouns)}",
            1 => $"{Pick(random, locale.TitleAdjectives)} {Pick(random, locale.TitleNouns)}",
            2 => $"{Pick(random, locale.TitleNouns)} {Pick(random, locale.TitleSuffixes)}",
            3 => $"{Pick(random, locale.TitlePrefixes)} {Pick(random, locale.TitleAdjectives)} {Pick(random, locale.TitleNouns)}",
            _ => $"{Pick(random, locale.TitleNouns)}"
        };
    }

    private string GenerateArtist(Random random, LocaleData locale, Faker faker)
    {
        bool isBand = random.Next(2) == 0;
        if (isBand)
        {
            int bandPattern = random.Next(4);
            return bandPattern switch
            {
                0 => $"{Pick(random, locale.BandPrefixes)} {Pick(random, locale.BandNouns)}",
                1 => $"{Pick(random, locale.BandNouns)} {Pick(random, locale.BandSuffixes)}",
                2 => $"{Pick(random, locale.BandPrefixes)} {Pick(random, locale.BandNouns)} {Pick(random, locale.BandSuffixes)}",
                _ => Pick(random, locale.BandNouns)
            };
        }
        else
        {
            return $"{faker.Name.FirstName()} {faker.Name.LastName()}";
        }
    }

    private string GenerateAlbum(Random random, LocaleData locale, string songTitle)
    {
        bool isSingle = random.Next(4) == 0;
        if (isSingle)
        {
            return locale.SingleText;
        }

        int pattern = random.Next(3);
        return pattern switch
        {
            0 => $"{Pick(random, locale.TitlePrefixes)} {Pick(random, locale.AlbumWords)}",
            1 => $"{Pick(random, locale.AlbumWords)} {Pick(random, locale.TitleSuffixes)}",
            _ => Pick(random, locale.AlbumWords)
        };
    }

    private int CalculateLikes(long seed, int index, double averageLikes)
    {
        if (averageLikes <= 0) return 0;
        if (averageLikes >= 10) return 10;

        long likesSeed = seed * 37 + index * 53;
        var likesRandom = new Random((int)(likesSeed & 0x7FFFFFFF));

        int baseLikes = (int)Math.Floor(averageLikes);
        double fractionalPart = averageLikes - baseLikes;

        int likes = baseLikes;
        if (likesRandom.NextDouble() < fractionalPart)
        {
            likes++;
        }

        return Math.Min(likes, 10);
    }

    private string GenerateReview(Random random, LocaleData locale)
    {
        var review = new List<string>();
        int sentenceCount = random.Next(2, 4);
        for (int i = 0; i < sentenceCount; i++)
        {
            review.Add(Pick(random, locale.ReviewPhrases));
        }
        return string.Join(" ", review);
    }

    private List<string> GenerateLyrics(Random random, LocaleData locale)
    {
        var lyrics = new List<string>();
        int lineCount = random.Next(8, 16);
        for (int i = 0; i < lineCount; i++)
        {
            lyrics.Add(Pick(random, locale.LyricPhrases));
        }
        return lyrics;
    }

    private CoverImage GenerateCover(Random random, int index)
    {
        var backgroundColors = new[]
        {
            "#1a1a2e", "#16213e", "#0f3460", "#533483", "#e94560",
            "#2c3e50", "#34495e", "#1abc9c", "#2ecc71", "#3498db",
            "#9b59b6", "#e74c3c", "#f39c12", "#27ae60", "#8e44ad",
            "#2980b9", "#c0392b", "#d35400", "#16a085", "#7f8c8d"
        };

        var accentColors = new[]
        {
            "#ffffff", "#f1c40f", "#e74c3c", "#3498db", "#2ecc71",
            "#9b59b6", "#1abc9c", "#e91e63", "#00bcd4", "#ff9800"
        };

        var patterns = new[] { "circles", "lines", "dots", "waves", "triangles", "squares", "gradient", "noise" };

        return new CoverImage
        {
            BackgroundColor = backgroundColors[random.Next(backgroundColors.Length)],
            AccentColor = accentColors[random.Next(accentColors.Length)],
            PatternType = patterns[random.Next(patterns.Length)],
            PatternSeed = random.Next(1000)
        };
    }

    private static T Pick<T>(Random random, IList<T> list)
    {
        return list[random.Next(list.Count)];
    }
}
