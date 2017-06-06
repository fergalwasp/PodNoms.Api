using System;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using PodNoms.Api.Controllers.Resources;
using PodNoms.Api.Models;
using PodNoms.Api.Persistence;
using PodNoms.Api.Services.Processor.Hangfire;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class EntryController : Controller {
        private readonly IEntryRepository repostory;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUrlProcessService processor;
        public EntryController(IEntryRepository repostory, IUnitOfWork unitOfWork, IMapper mapper,
            IUrlProcessService processor) {
            this.processor = processor;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.repostory = repostory;
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry([FromBody] EntryResource item) {
            var entry = mapper.Map<EntryResource, PodcastEntry>(item);
            await repostory.AddAsync(entry);
            await unitOfWork.CompleteAsync();

            var infoJobId = BackgroundJob.Enqueue<IUrlProcessService>(service => service.GetInformation(entry.Id));
            var extractJobId = BackgroundJob.ContinueWith<IUrlProcessService>(infoJobId, service => service.DownloadAudio(entry.Id));
            // await processor.GetInformation(entry.Id);
            // await processor.DownloadAudio(entry.Id);

            var result = mapper.Map<PodcastEntry, EntryResource>(entry);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id) {
            await this.repostory.DeleteAsync(id);
            await unitOfWork.CompleteAsync();

            return Ok();
        }
    }
}