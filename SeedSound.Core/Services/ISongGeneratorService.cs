using SeedSound.Core.Models;

namespace SeedSound.Core.Services;

public interface ISongGeneratorService
{
    SongPageResponse GenerateSongs(SongPageRequest request);
    Song GenerateSingleSong(string locale, long seed, double averageLikes, int index);
}
