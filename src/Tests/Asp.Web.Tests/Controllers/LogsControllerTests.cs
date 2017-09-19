using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Data;
using Asp.Core.Domains;
using Asp.Repositories.Logging;
using Asp.Web.Areas.Administration.Controllers;
using Asp.Web.Common.Mapper;
using Asp.Web.Common.Models.LogViewModels;
using AutoMapper;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Asp.Web.Tests.Controllers
{
    public class LogsControllerTests
    {
        private readonly Log _log1;
        private readonly Log _log2;
        private readonly Log _log3;
        private readonly Log _log4;
        private readonly IPagedList<Log> _logs;
        private readonly Mapper _mapper;

        public LogsControllerTests()
        {
            _log1 = new Log
            {
                Id = 1,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 01),
                Level = "Info",
                Message = "Info message",
            };
            _log2 = new Log
            {
                Id = 2,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 02),
                Level = "Debug",
                Message = "Debug message"
            };
            _log3 = new Log
            {
                Id = 3,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 03),
                Level = "Error",
                Message = "Error message"
            };
            _log4 = new Log
            {
                Id = 4,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 04),
                Level = "Error",
                Message = "Error message"
            };
            _logs = new PagedList<Log>
            {
                _log1,
                _log2,
                _log3,
                _log4,
            };

            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
        }

        [Fact]
        public async Task List_ReturnViewResult()
        {
            // Arrange
            var mockLogRepository = new Mock<ILogRepository>();
            mockLogRepository.Setup(x => x.GetLevels()).ReturnsAsync(_logs.Select(x => x.Level).Distinct().ToList());

            var sut = new LogsController(mockLogRepository.Object, null);

            // Act
            IActionResult result = await sut.List();

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("List", viewResult.ViewName);

            var model = Assert.IsType<LogSearchViewModel>(viewResult.Model);
            Assert.Equal(4, model.AvailableLevels.Count);
            Assert.Equal("All", model.AvailableLevels[0].Text);
            Assert.Equal("", model.AvailableLevels[0].Value);
        }

        [Fact]
        public async Task List_PostDataSourceRequestAndLogSearchModel_ReturnJsonResult()
        {
            // Arrange
            var mockLogRepository = new Mock<ILogRepository>();
            mockLogRepository.Setup(x => x.GetLogs(It.IsAny<LogPagedDataRequest>())).ReturnsAsync(_logs);
            var sut = new LogsController(mockLogRepository.Object, _mapper);

            var request = new DataSourceRequest {Sorts = new List<SortDescriptor>()};
            var model = new LogSearchViewModel();

            // Act
            IActionResult result = await sut.List(request, model);

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            DataSourceResult dataSourceResult = Assert.IsType<DataSourceResult>(jsonResult.Value);
            var models = Assert.IsType<List<LogViewModel>>(dataSourceResult.Data);
            Assert.Equal(4, models.Count);
        }
    }
}