using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Sheetient.App.Dtos.Sheet;
using Sheetient.App.Exceptions;
using Sheetient.App.Services.Interfaces;
using Sheetient.Domain.Entities;
using Sheetient.Domain.Interfaces;

namespace Sheetient.App.Services
{
    public class SheetService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper) : ISheetService
    {
        private readonly IRepository<Sheet> _repository = unitOfWork.GetRepository<Sheet>();

        public async Task<int> CreateSheet(SheetDto sheetDto)
        {
            var sheet = mapper.Map<Sheet>(sheetDto);
            sheet.UserId = userService.UserId;
            await _repository.Add(sheet);
            unitOfWork.Commit();
            return sheet.Id;
        }

        public async Task<SheetDto> GetSheet(int sheetId)
        {
            var sheet = await GetSheetEntity(sheetId);
            return mapper.Map<SheetDto>(sheet);
        }

        public async Task<List<SheetDto>> GetSheets()
        {
            var sheets = await _repository.GetMany(x => x.UserId == userService.UserId);
            return mapper.Map<List<SheetDto>>(sheets);
        }

        public async Task PatchSheet(int sheetId, JsonPatchDocument<SheetDto> patch)
        {
            var sheet = await GetSheetEntity(sheetId);
            var mappedPatch = mapper.Map<JsonPatchDocument<Sheet>>(patch);
            mappedPatch.ApplyTo(sheet);
            _repository.Update(sheet);
            unitOfWork.Commit();
        }

        public async Task DeleteSheet(int sheetId)
        {
            var sheet = await GetSheetEntity(sheetId);
            _repository.Delete(sheet);
            unitOfWork.Commit();
        }

        private async Task<Sheet> GetSheetEntity(int sheetId)
        {
            var sheet = await _repository.Get(
                x => x.Id == sheetId && x.UserId == userService.UserId,
                x => x.Include(x => x.Pages).ThenInclude(y => y.Fields))
                ?? throw new NotFoundException($"Sheet {sheetId} does not exist or you do not have access.");

            return sheet;
        }
    }
}
