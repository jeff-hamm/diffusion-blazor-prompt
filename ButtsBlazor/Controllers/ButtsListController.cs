using ButtsBlazor.Server.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ButtsBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ButtsListController : ControllerBase
    {
        private readonly ButtsListFileService fileService;

        public ButtsListController(ButtsListFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpGet("")]
        public ButtImage? Get([FromQuery] DateTime? known, [FromQuery] int? except) => 
            this.fileService.GetLatestOrRandom(known, except);

        // GET: api/<ValuesController>
        [HttpGet("latest")]
        public ButtImage? GetLatest([FromQuery]DateTime? known) =>
            this.fileService.GetLatest(known);
        [HttpGet("random/{except:int?}")]
        public ButtImage? GetRandom(int? except) =>
            this.fileService.GetRandom(except);

        // GET: api/<ValuesController>
        [HttpGet("{index:int}")]
        public ButtImage? GetIndex(int index) =>
            this.fileService.Index(index);

    }
}
