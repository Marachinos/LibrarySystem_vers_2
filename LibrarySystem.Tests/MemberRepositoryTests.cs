using LibrarySystem.Core.Models;
using LibrarySystem.Data;
using LibrarySystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests
{
    public class MemberRepositoryTests
    {
        private static LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LibraryContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Member_When_MemberId_Is_Unique()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new MemberRepository(context);

            var member = new Member
            {
                MemberId = "M001",
                Name = "Sandra Svensson",
                Email = "sandra@test.se",
                MemberSince = DateTime.UtcNow
            };

            // Act
            await repository.AddAsync(member);

            // Assert
            var savedMember = await context.Members.FirstOrDefaultAsync(m => m.MemberId == "M001");

            Assert.NotNull(savedMember);
            Assert.Equal("Sandra Svensson", savedMember.Name);
            Assert.Equal("sandra@test.se", savedMember.Email);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_MemberId_Already_Exists()
        {
            // Arrange
            using var context = CreateContext();

            context.Members.Add(new Member
            {
                MemberId = "M001",
                Name = "Första medlemmen",
                Email = "first@test.se",
                MemberSince = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var repository = new MemberRepository(context);

            var duplicateMember = new Member
            {
                MemberId = "M001",
                Name = "Andra medlemmen",
                Email = "second@test.se",
                MemberSince = DateTime.UtcNow
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => repository.AddAsync(duplicateMember));

            Assert.Equal("MemberId måste vara unik.", exception.Message);
        }
    }
}

