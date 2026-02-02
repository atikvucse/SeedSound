using SeedSound.Core.Models;

namespace SeedSound.Core.Services;

public interface IMusicGeneratorService
{
    MusicData GenerateMusic(long seed, int songIndex, string genre);
}
