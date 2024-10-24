using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //GET ALL REGIONS
        [HttpGet]
        public IActionResult GetAll()
            {
                //GET DATA FROM DATABASE - DOMAIN MODELS
                var regionsDomain = dbContext.Regions.ToList();

                //Map domain models to DTOs
                var regionsDto = new List<RegionDto>();
                foreach (var regions in regionsDomain)
                {
                    regionsDto.Add(new RegionDto()
                    {
                        Id = regions.Id,
                        Name = regions.Name,
                        Code = regions.Code,
                        RegionImageUrl = regions.RegionImageUrl
                    });
                }
                //return DTOs
                return Ok(regionsDto);
            }
        //GET REGION BY ID
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute]Guid id)
            {
                //var region = dbContext.Regions.Find(id);
                //Get region domai model from dataBase
                var region = dbContext.Regions.FirstOrDefault(x=> x.Id == id);
                if(region == null)
                {
                    return NotFound();
                }
                //Map region domain model to region DTO
                var regionDto = new RegionDto{
                    Id = region.Id,
                    Name = region.Name,
                    Code = region.Code,
                    RegionImageUrl = region.RegionImageUrl
                };


                return Ok(regionDto);
            }

        [HttpPost]
        public IActionResult Create([FromBody]AddRegionRequestDto addRegionRequestDto)
            {
                //Map or convert DTo to domain model

                var createdRegions = new List<RegionDto>();
                foreach (var regionrequest in addRegionRequestDto.Regions)
                {
                    var regionDomainModel = new Region
                    {
                        Code = regionrequest.Code,
                        Name = regionrequest.Name,
                        RegionImageUrl = regionrequest.RegionImageUrl
                    };
                    //Use domain model to create db context

                    dbContext.Regions.Add(regionDomainModel);
                    dbContext.SaveChanges();

                    //Map domain model back to dto

                    var regionDto = new RegionDto
                    {
                        Id = regionDomainModel.Id,
                        Name = regionDomainModel.Name,
                        Code = regionDomainModel.Code,
                        RegionImageUrl = regionDomainModel.RegionImageUrl
                    };
                    createdRegions.Add(regionDto);
                }
                return CreatedAtAction(nameof(GetById), new { id = createdRegions.First().Id }, createdRegions.First());

            }


    }
}
