using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Persistence;
using PodNoms.Api.Services.Processor.Hangfire;

namespace PodNoms.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class EntryController : Controller
    {
        private readonly IEntryRepository _repository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IUrlProcessService _processor;
        private readonly ILogger _logger;
        public EntryController(IEntryRepository repostory, IUnitOfWork unitOfWork, IMapper mapper,
            IUrlProcessService processor, ILoggerFactory logger)
        {
            this._logger = logger.CreateLogger<EntryController>();
            this._repository = repostory;
            this._uow = unitOfWork;
            this._mapper = mapper;
            this._processor = processor;
        }

        [HttpGet("all/{podcastId}")]
        public async Task<IEnumerable<EntryViewModel>> Get(int podcastId)
        {
            var entries = await _repository.GetAllAsync(podcastId);
            return _mapper.Map<List<PodcastEntry>, List<EntryViewModel>>(entries.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry([FromBody] EntryViewModel item)
        {
            var entry = _mapper.Map<EntryViewModel, PodcastEntry>(item);
            await _repository.AddAsync(entry);
            await _uow.CompleteAsync();

            try
            {
                var infoJobId = BackgroundJob.Enqueue<IUrlProcessService>(service => service.GetInformation(entry.Id));
                var extractJobId = BackgroundJob.ContinueWith<IUrlProcessService>(infoJobId, service => service.DownloadAudio(entry.Id));
                entry.ProcessingStatus = ProcessingStatus.Downloading;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Failed submitting job to processor\n{ex.Message}");
                entry.ProcessingStatus = ProcessingStatus.Failed;
            }
            finally
            {
                await _uow.CompleteAsync();
            }
            var result = _mapper.Map<PodcastEntry, EntryViewModel>(entry);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            await this._repository.DeleteAsync(id);
            await _uow.CompleteAsync();

            return Ok();
        }
    }
}