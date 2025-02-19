using Microsoft.AspNetCore.JsonPatch;
using Sheetient.App.Dtos.Sheet;

namespace Sheetient.App.Services.Interfaces
{
    public interface ISheetService
    {
        public Task<int> CreateSheet(SheetDto sheetDto);
        public Task<SheetDto> GetSheet(int sheetId);
        public Task<List<SheetSummaryDto>> GetSheets();
        public Task PatchSheet(int sheetId, JsonPatchDocument<SheetDto> patch);
        public Task DeleteSheet(int sheetId);
    }
}
