﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dbqf.Configuration;
using dbqf.Criterion;
using dbqf.Display.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Standalone.Core.Serialization.Assemblers;
using Standalone.Core.Serialization.Assemblers.Builders;
using Standalone.Core.Serialization.Assemblers.Criterion;
using Standalone.Core.Serialization.DTO;
using Standalone.Core.Serialization.DTO.Builders;
using Standalone.Core.Serialization.DTO.Criterion;

namespace Standalone.Core.tests.Serialization
{
    [TestFixture]
    public class BuilderAssembler_Fixture
    {
        [Test]
        public void SimpleBuilder_Restore()
        {
            // Arrange
            var assemblerUnderTest = new SimpleBuilderAssembler(null);
            var dto = new SimpleBuilderDTO()
            {
                Label = "test",
                Operator = "="
            };

            // Act
            var restored = assemblerUnderTest.Restore(dto);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<SimpleBuilder>(restored);
            var builder = (SimpleBuilder)restored;
            Assert.AreEqual(dto.Label, builder.Label);
            Assert.AreEqual(dto.Operator, builder.Operator);
        }

        [Test]
        public void SimpleBuilder_Create()
        {
            // Arrange
            var assemblerUnderTest = new SimpleBuilderAssembler(null);
            var builder = new SimpleBuilder()
            {
                Label = "test",
                Operator = "="
            };

            // Act
            var restored = assemblerUnderTest.Create(builder);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<SimpleBuilderDTO>(restored);
            var dto = (SimpleBuilderDTO)restored;
            Assert.AreEqual(builder.Label, dto.Label);
            Assert.AreEqual(builder.Operator, dto.Operator);
        }

        [Test]
        public void NullBuilder_Restore()
        {
            // Arrange
            var assemblerUnderTest = new NullBuilderAssembler(null);
            var dto = new NullBuilderDTO()
            {
                Label = "test"
            };

            // Act
            var restored = assemblerUnderTest.Restore(dto);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<NullBuilder>(restored);
            var builder = (NullBuilder)restored;
            Assert.AreEqual(dto.Label, builder.Label);
        }

        [Test]
        public void NullBuilder_Create()
        {
            // Arrange
            var assemblerUnderTest = new NullBuilderAssembler(null);
            var builder = new NullBuilder()
            {
                Label = "test"
            };

            // Act
            var restored = assemblerUnderTest.Create(builder);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<NullBuilderDTO>(restored);
            var dto = (NullBuilderDTO)restored;
            Assert.AreEqual(builder.Label, dto.Label);
        }

        [Test]
        public void NotBuilder_Restore()
        {
            // Arrange
            var mockOtherDTO = MockRepository.GenerateMock<ParameterBuilderDTO>();
            var mockOther = MockRepository.GenerateMock<ParameterBuilder>();
            var mockChain = MockRepository.GenerateMock<BuilderAssembler>((BuilderAssembler)null);
            mockChain.Expect(x => x.Restore(mockOtherDTO)).Return(mockOther);

            var assemblerUnderTest = new NotBuilderAssembler(null) { Chain = mockChain };
            var dto = new NotBuilderDTO()
            {
                Label = "test",
                Other = mockOtherDTO
            };

            // Act
            var restored = assemblerUnderTest.Restore(dto);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<NotBuilder>(restored);
            var builder = (NotBuilder)restored;
            Assert.AreEqual(dto.Label, builder.Label);
            Assert.AreSame(mockOther, builder.Other);
            mockChain.VerifyAllExpectations();
        }

        [Test]
        public void NotBuilder_Create()
        {
            // Arrange
            var mockOtherDTO = MockRepository.GenerateMock<ParameterBuilderDTO>();
            var mockOther = MockRepository.GenerateMock<ParameterBuilder>();
            var mockChain = MockRepository.GenerateMock<BuilderAssembler>((BuilderAssembler)null);
            mockChain.Expect(x => x.Create(mockOther)).Return(mockOtherDTO);

            var assemblerUnderTest = new NotBuilderAssembler(null) { Chain = mockChain };
            var builder = new NotBuilder()
            {
                Label = "test",
                Other = mockOther
            };

            // Act
            var restored = assemblerUnderTest.Create(builder);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<NotBuilderDTO>(restored);
            var dto = (NotBuilderDTO)restored;
            Assert.AreEqual(builder.Label, dto.Label);
            Assert.AreSame(mockOtherDTO, dto.Other);
            mockChain.VerifyAllExpectations();
        }

        [Test]
        public void LikeBuilder_Restore()
        {
            // Arrange
            var assemblerUnderTest = new LikeBuilderAssembler(null);
            var dto = new LikeBuilderDTO()
            {
                Label = "test",
                Mode = "Exact"
            };

            // Act
            var restored = assemblerUnderTest.Restore(dto);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<LikeBuilder>(restored);
            var builder = (LikeBuilder)restored;
            Assert.AreEqual(dto.Label, builder.Label);
            Assert.AreSame(MatchMode.Exact, builder.Mode);
        }

        [Test]
        public void LikeBuilder_Create()
        {
            // Arrange
            var assemblerUnderTest = new LikeBuilderAssembler(null);
            var builder = new LikeBuilder()
            {
                Label = "test",
                Mode = MatchMode.Exact
            };

            // Act
            var restored = assemblerUnderTest.Create(builder);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<LikeBuilderDTO>(restored);
            var dto = (LikeBuilderDTO)restored;
            Assert.AreEqual(builder.Label, dto.Label);
            Assert.AreEqual("Exact", dto.Mode);
        }

        [Test]
        public void BooleanBuilder_Restore()
        {
            // Arrange
            var assemblerUnderTest = new BooleanBuilderAssembler(null);
            var dto = new BooleanBuilderDTO()
            {
                Label = "test",
                Value = true
            };

            // Act
            var restored = assemblerUnderTest.Restore(dto);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<BooleanBuilder>(restored);
            var builder = (BooleanBuilder)restored;
            Assert.AreEqual(dto.Label, builder.Label);
            Assert.AreEqual(true, builder.Value);
        }

        [Test]
        public void BooleanBuilder_Create()
        {
            // Arrange
            var assemblerUnderTest = new BooleanBuilderAssembler(null);
            var builder = new BooleanBuilder()
            {
                Label = "test",
                Value = true
            };

            // Act
            var restored = assemblerUnderTest.Create(builder);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<BooleanBuilderDTO>(restored);
            var dto = (BooleanBuilderDTO)restored;
            Assert.AreEqual(builder.Label, dto.Label);
            Assert.AreEqual(true, dto.Value);
        }

        [Test]
        public void BetweenBuilder_Restore()
        {
            // Arrange
            var assemblerUnderTest = new BetweenBuilderAssembler(null);
            var dto = new BetweenBuilderDTO()
            {
                Label = "test"
            };

            // Act
            var restored = assemblerUnderTest.Restore(dto);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<BetweenBuilder>(restored);
            var builder = (BetweenBuilder)restored;
            Assert.AreEqual(dto.Label, builder.Label);
        }

        [Test]
        public void BetweenBuilder_Create()
        {
            // Arrange
            var assemblerUnderTest = new BetweenBuilderAssembler(null);
            var builder = new BetweenBuilder()
            {
                Label = "test"
            };

            // Act
            var restored = assemblerUnderTest.Create(builder);

            // Assert
            Assert.IsNotNull(restored);
            Assert.IsInstanceOf<BetweenBuilderDTO>(restored);
            var dto = (BetweenBuilderDTO)restored;
            Assert.AreEqual(builder.Label, dto.Label);
        }
    }
}
