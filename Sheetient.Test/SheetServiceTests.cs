using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Sheetient.App.Dtos.Sheet;
using Sheetient.App.Services;
using Sheetient.App.Services.Interfaces;
using Sheetient.Domain.Entities;
using Sheetient.Domain.Interfaces;
using System.Linq.Expressions;

namespace Sheetient.Test
{
    public class SheetServiceTests
    {
        private readonly SheetDto _sheetDto;
        private readonly SheetSummaryDto _sheetSummaryDto;
        private readonly Sheet _sheet;
        private readonly int _userId = 1;

        private readonly Mock<IRepository<Sheet>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly SheetService _sheetService;
        public SheetServiceTests()
        {
            _sheetDto = new SheetDto
            {
                Id = 1,
                Name = "Test Sheet",
                Description = "This is a test"
            };
            _sheetSummaryDto = new SheetSummaryDto
            {
                Id = 1,
                Name = "Test Sheet",
                Description = "This is a test"
            };
            _sheet = new Sheet
            {
                Id = 1,
                Name = "Test Sheet",
                Description = "This is a test"
            };

            _mockRepository = new Mock<IRepository<Sheet>>();
            _mockRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Sheet, bool>>>(), It.IsAny<Func<IQueryable<Sheet>, IIncludableQueryable<Sheet, object>>>()))
                .ReturnsAsync(_sheet);
            _mockRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<Sheet, bool>>>()))
                .ReturnsAsync([_sheet]);

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(x => x.GetRepository<Sheet>())
                .Returns(_mockRepository.Object);

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.UserId)
                .Returns(_userId);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<Sheet>(It.IsAny<SheetDto>()))
                .Returns(_sheet);
            mockMapper.Setup(x => x.Map<SheetDto>(It.IsAny<Sheet>()))
                .Returns(_sheetDto);
            mockMapper.Setup(x => x.Map<List<SheetSummaryDto>>(It.IsAny<List<Sheet>>()))
                .Returns([_sheetSummaryDto]);
            mockMapper.Setup(x => x.Map<JsonPatchDocument<Sheet>>(It.IsAny<JsonPatchDocument<SheetDto>>()))
                .Returns(new JsonPatchDocument<Sheet>());

            _sheetService = new SheetService(_mockUnitOfWork.Object, mockUserService.Object, mockMapper.Object);
        }

        [Fact]
        public async Task CreateSheet_AddsNewSheet()
        {
            var result = await _sheetService.CreateSheet(_sheetDto);
            _mockRepository.Verify(x => x.Add(_sheet), Times.Once());
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once());
            Assert.Equal(_sheet.Id, result);
            Assert.Equal(_userId, _sheet.UserId);
        }

        [Fact]
        public async Task GetSheet_ReturnsSheetDto()
        {
            var result = await _sheetService.GetSheet(_sheetDto.Id!.Value);
            _mockRepository.Verify(x => x.Get(It.IsAny<Expression<Func<Sheet, bool>>>(), It.IsAny<Func<IQueryable<Sheet>, IIncludableQueryable<Sheet, object>>>()), Times.Once());
            Assert.Equal(_sheetDto, result);
        }

        [Fact]
        public async Task GetSheets_ReturnsSheetSummaryDtoList()
        {
            var result = await _sheetService.GetSheets();
            _mockRepository.Verify(x => x.GetMany(It.IsAny<Expression<Func<Sheet, bool>>>()), Times.Once());
            Assert.Equal([_sheetSummaryDto], result);
        }

        [Fact]
        public async Task PatchSheet_UpdatesSheet()
        {
            await _sheetService.PatchSheet(_sheetDto.Id!.Value, new JsonPatchDocument<SheetDto>());
            _mockRepository.Verify(x => x.Get(It.IsAny<Expression<Func<Sheet, bool>>>(), It.IsAny<Func<IQueryable<Sheet>, IIncludableQueryable<Sheet, object>>>()), Times.Once());
            _mockRepository.Verify(x => x.Update(It.IsAny<Sheet>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once());
        }

        [Fact]
        public async Task DeleteSheet_DeletesSheet()
        {
            await _sheetService.DeleteSheet(_sheetDto.Id!.Value);
            _mockRepository.Verify(x => x.Get(It.IsAny<Expression<Func<Sheet, bool>>>(), It.IsAny<Func<IQueryable<Sheet>, IIncludableQueryable<Sheet, object>>>()), Times.Once());
            _mockRepository.Verify(x => x.Delete(It.IsAny<Sheet>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once());

        }
    }
}
