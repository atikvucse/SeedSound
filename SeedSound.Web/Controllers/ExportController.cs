using Microsoft.AspNetCore.Mvc;
using SeedSound.Core.Models;
using SeedSound.Core.Services;

namespace SeedSound.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly ISongGeneratorService _songGenerator;
    private readonly IExportService _exportService;

    public ExportController(ISongGeneratorService songGenerator, IExportService exportService)
    {
        _songGenerator = songGenerator;
        _exportService = exportService;
    }

    [HttpGet]
    public IActionResult ExportSongs(
        [FromQuery] string locale = "en_US",
        [FromQuery] long seed = 12345,
        [FromQuery] double likes = 5.0,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        likes = Math.Clamp(likes, 0, 10);
        if (pageSize > 50) pageSize = 50;

        var request = new SongPageRequest
        {
            Locale = locale,
            Seed = seed,
            AverageLikes = likes,
            Page = page,
            PageSize = pageSize
        };

        var response = _songGenerator.GenerateSongs(request);
        var zipData = _exportService.ExportToZip(response.Songs);

        return File(zipData, "application/zip", $"seedsound_export_{seed}_{page}.zip");
    }
}
