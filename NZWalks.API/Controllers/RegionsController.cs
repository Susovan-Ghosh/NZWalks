using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using NZWalks.API.WebSocket;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly IHubContext<NotificationHub> hubContext;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper, IHubContext<NotificationHub> hubContext)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.hubContext = hubContext;
        }

        // Get all regions
        // GET: https://localhost:portnumber/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var regions = new List<Region>
            //{
            //    new Region
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Auckland Region",
            //        Code = "AKL",
            //        RegionImageUrl = "https://unsplash.com/photos/aerial-view-of-cityscape-YsWUSaf8pZA"
            //    },
            //    new Region
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Wellington Region",
            //        Code = "WLG",
            //        RegionImageUrl = "https://www.pexels.com/photo/city-skyline-under-white-clouds-3709415"
            //    }
            //};

            // Get the response in Domain Model form
            var regionsDomain = await regionRepository.GetAllAsync();
            // Map the Domain Model response to DTO
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
            //var regionsDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = regionDomain.Id,
            //        Code = regionDomain.Code,
            //        Name = regionDomain.Name,
            //        RegionImageUrl = regionDomain.RegionImageUrl
            //    });
            //}
            // returnn the DTO
            await hubContext.Clients.All.SendAsync("ReceiveMessage", $"Item(s) fetched: '{JsonSerializer.Serialize(regionsDto)}'.");
            return Ok(regionsDto);
        }

        // Get a single region
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var regionDomain = dbContext.Regions.Find(id);
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain == null)
            {
                return NotFound();
            }
            var regionDto = mapper.Map<RegionDto>(regionDomain);
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            return Ok(regionDto);
        }

        // Creates a new Region resource
        // POST: https://localhost:portnumber/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var regionDomain = mapper.Map<Region>(addRegionRequestDto);
            //var regionDomain = new Region()
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};
            regionDomain = await regionRepository.CreateAsync(regionDomain);
            var regionDto = mapper.Map<RegionDto>(regionDomain);
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            await hubContext.Clients.All.SendAsync("ReceiveMessage", $"Item created: '{JsonSerializer.Serialize(regionDto)}'");
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        // Updates a region
        // PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);
            //var regionDomainModel = new Region()
            //{
            //    Name = updateRegionRequestDto.Name,
            //    Code = updateRegionRequestDto.Code,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            return Ok(regionDto);
        }

        // Deletes a region
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            return Ok(regionDto);
        }
    }
}
