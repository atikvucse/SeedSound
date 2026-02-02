using SeedSound.Core.Models;

namespace SeedSound.Core.Services;

public interface IExportService
{
    byte[] ExportToZip(IEnumerable<Song> songs);
}
