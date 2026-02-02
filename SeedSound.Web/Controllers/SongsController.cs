using Microsoft.AspNetCore.Mvc;
using SeedSound.Core.Localization;
using SeedSound.Core.Models;
using SeedSound.Core.Services;

namespace SeedSound.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongGeneratorService _songGenerator;

    public SongsController(ISongGeneratorService songGenerator)
    {
        _songGenerator = songGenerator;
    }

    [HttpGet]
    public ActionResult<SongPageResponse> GetSongs(
        [FromQuery] string locale = "en_US",
        [FromQuery] long seed = 12345,
        [FromQuery] double likes = 5.0,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;
        likes = Math.Clamp(likes, 0, 10);

        var request = new SongPageRequest
        {
            Locale = locale,
            Seed = seed,
            AverageLikes = likes,
            Page = page,
            PageSize = pageSize
        };

        var response = _songGenerator.GenerateSongs(request);
        return Ok(response);
    }

    [HttpGet("{index}")]
    public ActionResult<Song> GetSong(
        int index,
        [FromQuery] string locale = "en_US",
        [FromQuery] long seed = 12345,
        [FromQuery] double likes = 5.0)
    {
        if (index < 1) return BadRequest("Index must be at least 1");
        likes = Math.Clamp(likes, 0, 10);

        var song = _songGenerator.GenerateSingleSong(locale, seed, likes, index);
        return Ok(song);
    }

    [HttpGet("locales")]
    public ActionResult<IEnumerable<object>> GetLocales()
    {
        var locales = LocaleDataStore.GetAvailableLocales()
            .Select(l => new { code = l.Code, name = l.Name });
        return Ok(locales);
    }
}
