using FluentAssertions;
using LazyEntityGraph.Core;
using LazyEntityGraph.Core.Constraints;
using LazyEntityGraph.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Xunit;

namespace LazyEntityGraph.EntityFrameworkCore.Tests
{
    public class ModelMetadataGeneratorTests
    {
        public static ModelMetadata GetMetadata()
        {
            return ModelMetadataGenerator.LoadFromContext<BlogContext>(options => new BlogContext(options));
        }

        [Fact]
        public void EntityTypesShouldBeDetected()
        {
            // arrange
            var expected = new[]
            {
                typeof (Post), typeof (User), typeof (ContactDetails), typeof (Category), typeof(Story)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.EntityTypes.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void ConstraintsShouldBeGenerated()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                new OneToManyPropertyConstraint<User, Post>(u => u.Posts, p => p.Poster),
                new ManyToOnePropertyConstraint<Post, User>(p => p.Poster, u => u.Posts),
                new OneToOnePropertyConstraint<User,ContactDetails>(u => u.ContactDetails, c => c.User),
                new OneToOnePropertyConstraint<ContactDetails, User>(c => c.User, u => u.ContactDetails),
                new ForeignKeyConstraint<Post, User, int>(p => p.Poster, p => p.PosterId, u => u.Id),
                new ForeignKeyConstraint<ContactDetails, User, int>(c => c.User, c => c.UserId, u => u.Id)
            };

            // act
            var metadata = GetMetadata();

            // assert
            metadata.Constraints.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void CanHandleMultiColumnForeignKey()
        {
            // arrange
            var expected = new IPropertyConstraint[]
            {
                new OneToManyPropertyConstraint<MultiColumnContext.Parent, MultiColumnContext.Child>(u => u.Children, p => p.Parent),
                new ManyToOnePropertyConstraint<MultiColumnContext.Child, MultiColumnContext.Parent>(p => p.Parent, u => u.Children),
            };

            var metadata = ModelMetadataGenerator.LoadFromContext<MultiColumnContext>(options => new MultiColumnContext(options));

            // assert
            metadata.Constraints.Should().BeEquivalentTo(expected);
        }
    }
}
