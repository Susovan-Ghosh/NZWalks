using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories;
using System.Diagnostics;

namespace NZWalks.API.Tests.RepositoryTests
{
    [TestFixture]
    public class RegionRepositoryTests
    {
        private NZWalksDbContext _context;
        private IRegionRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<NZWalksDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            _context = new NZWalksDbContext(options);
            _repository = new SQLRegionRepository(_context);
            // Seed initial data
            _context.Regions.AddRange(
                new Models.Domain.Region()
                {
                    Id = Guid.Parse("205897a9-ae0e-4f77-970e-8716e1633934"),
                    Code = "TR1",
                    Name = "Test Region 1",
                    RegionImageUrl = "/test-region-1-img-url"
                },
                new Models.Domain.Region()
                {
                    Id = Guid.Parse("2dda8994-50d9-4b7e-a28f-371d9b4d4310"),
                    Code = "TR2",
                    Name = "Test Region 2",
                    RegionImageUrl = "/test-region-2-img-url"
                }
            );
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllRegions()
        {
            // Act
            var regions = await _repository.GetAllAsync();
            // Assert
            Assert.That(regions.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsCorrectRegion()
        {
            // Act
            var region = await _repository.GetByIdAsync(Guid.Parse("205897a9-ae0e-4f77-970e-8716e1633934"));
            // Assert
            Assert.That(region, Is.Not.Null);
            Assert.That(region.Code, Is.EqualTo("TR1"));
        }

        [Test]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var region = await _repository.GetByIdAsync(Guid.Parse("c2bb3274-f631-4876-9ae9-217af14e2000"));
            // Assert
            Assert.That(region, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsRegionToDatabase()
        {
            // Arrange
            var region = new Region()
            {
                Id = Guid.Parse("c2bb3274-f631-4876-9ae9-217af14e2000"),
                Code = "TR3",
                Name = "Test Region 3",
                RegionImageUrl = "/test-region-3-img-url"
            };
            // Act
            await _repository.CreateAsync(region);
            var regions = await _repository.GetAllAsync();
            // Assert
            Assert.That(regions.Count, Is.EqualTo(3));
            Assert.That(regions.Any(r => r.Code == "TR3"), Is.True);
        }

        [Test]
        public async Task UpdateAsync_UpdatesExistingRegion()
        {
            // Arrange
            var regions = await _repository.GetAllAsync();
            Debug.WriteLine(regions);
            var region = await _repository.GetByIdAsync(Guid.Parse("c2bb3274-f631-4876-9ae9-217af14e2000"));
            region.Code = "TR-3";
            // Act
            await _repository.UpdateAsync(Guid.Parse("c2bb3274-f631-4876-9ae9-217af14e2000"), region);
            region = await _repository.GetByIdAsync(Guid.Parse("c2bb3274-f631-4876-9ae9-217af14e2000"));
            // Assert
            Assert.That(region.Code, Is.EqualTo("TR-3"));
        }
    }
}
