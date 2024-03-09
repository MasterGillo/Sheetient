using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Sheetient.App.Dtos.Sheet;
using Sheetient.App.Services.Interfaces;

namespace Sheetient.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SheetController(ISheetService sheetService) : BaseController
    {
        [HttpPost]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] SheetDto sheetDto)
        {
            var id = await sheetService.CreateSheet(sheetDto);
            return Ok(id);
        }

        [HttpGet("{sheetId:int}")]
        [ProducesResponseType<SheetDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int sheetId)
        {
            var sheet = await sheetService.GetSheet(sheetId);
            return Ok(sheet);
        }

        [HttpGet]
        [ProducesResponseType<SheetDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var sheet = await sheetService.GetSheets();
            return Ok(sheet);
        }

        [HttpPatch("{sheetId:int}")]
        public async Task<IActionResult> Update(int sheetId, [FromBody] JsonPatchDocument<SheetDto> patch)
        {
            await sheetService.PatchSheet(sheetId, patch);
            return Ok();
        }

        [HttpDelete("{sheetId:int}")]
        public async Task<IActionResult> Delete(int sheetId)
        {
            await sheetService.DeleteSheet(sheetId);
            return Ok();
        }
    }
}
