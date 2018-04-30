using System;
using Protobuf.Schemas.Tests.Models;
using Xunit;

namespace Serialization.Proto.Serializer.Tests
{
    public class SerializerTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_SimpleObjectType_Success()
        {
            Serializer serializer = new Serializer();

            Root root = new Root();
            root.Id = 1;
            root.Name = "Root";

            var serialized = serializer.Serialize<Root>(root);

            Assert.NotNull(serialized);

            Assert.Equal(8, serialized.Length);

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_GenericType_WithPrimitive_Success()
        {
            Serializer serializer = new Serializer();

            Generic<int> generic = new Generic<int>();
            generic.Data = 1;
            
            var serialized = serializer.Serialize<Generic<int>>(generic);

            Assert.NotNull(serialized);

            Assert.Equal(2, serialized.Length);
             

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_GenericType_WithReferenceType_Success()
        {
            Serializer serializer = new Serializer();

            Generic<Root> generic = new Generic<Root>();
            generic.Data = new Root();
            generic.Data.Id = 1;
            generic.Data.Name = "Generic<Root>";

            var serialized = serializer.Serialize<Generic<Root>>(generic);

            Assert.NotNull(serialized);

            var deserializedResult = serializer.Deserialize<Generic<Root>>(serialized);

            Assert.Equal(19, serialized.Length);

            Assert.NotNull(deserializedResult);

            Assert.Equal(deserializedResult.Data.Id, generic.Data.Id);
            Assert.Equal(deserializedResult.Data.Name, generic.Data.Name);


        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_InheritType_Success()
        {
            Serializer serializer = new Serializer();

            ExtendedData extended = new ExtendedData();
            extended.Created = DateTime.UtcNow;
            extended.Id = "1";
            extended.Extended = new System.Collections.Generic.Dictionary<string, object>();
            extended.Extended.Add("key1", new object());
            extended.WorkToDo = new Processes();
            extended.WorkToDo.Name = "Processes";
            extended.WorkToDo.Priority = 1;
            extended.WorkToDo.StartDate = DateTime.Now;
            extended.WorkToDo.Collection.Add(new Data() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow });
            
            var serialized = serializer.Serialize<ExtendedData>(extended);

            Assert.NotNull(serialized);
            Assert.Equal(104, serialized.Length);

            var deserialized = serializer.Deserialize<ExtendedData>(serialized);

          

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void DeSerializer_SimpleObjectType_Success()
        {
            Serializer serializer = new Serializer();

            byte[] array = new byte[] { 8
    , 1
    , 18
    , 4
    , 82
    , 111
    , 111
    , 116
                };

            var instance = serializer.Deserialize<Root>(array);

            Assert.NotNull(instance);
            Assert.Equal(1, instance.Id);
            Assert.Equal("Root", instance.Name);

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void DeSerializer_GenericType_Success()
        {
            Serializer serializer = new Serializer();

            byte[] array = new byte[] { 32, 1  };


            var instance = serializer.Deserialize<Generic<int>>(array);

            Assert.NotNull(instance);

            Assert.Equal(1, instance.Data);


        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void DeSerializer_InheritType_Success()
        {
            Serializer serializer = new Serializer();

            ExtendedData extended = new ExtendedData();

            byte[] array = new byte[] { 194, 62, 85, 58, 8, 10, 4, 107, 101, 121, 49, 18, 0, 66, 73, 194, 62, 13, 90, 11, 8, 134, 214, 153, 165, 169, 162, 151, 54, 16, 5, 74, 9, 80, 114, 111, 99, 101, 115, 115, 101, 115, 80, 1, 90, 42, 42, 27, 100, 97, 116, 97, 67, 114, 101, 97, 116, 101, 100, 73, 110, 115, 105, 100, 101, 67, 79, 108, 108, 101, 99, 116, 105, 111, 110, 50, 11, 8, 252, 212, 246, 136, 157, 160, 151, 54, 16, 5, 42, 1, 49, 50, 11, 8, 168, 232, 243, 136, 157, 160, 151, 54, 16, 5 };

            var instance = serializer.Deserialize<ExtendedData>(array);

            Assert.NotNull(instance);

            Assert.Equal("1", instance.Id);
            Assert.NotNull(instance.WorkToDo);
            Assert.NotNull(instance.Extended);

        }

    }
}
