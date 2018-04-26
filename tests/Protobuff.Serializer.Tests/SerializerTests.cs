using System;
using Protobuf.Schemas.Tests.Models;
using Xunit;

namespace Protobuff.Serializer.Tests
{
    public class SerializerTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_SimpleObjectType_Success()
        {
            Protobuf.Serializer.Serializer serializer = new Protobuf.Serializer.Serializer();

            Root root = new Root();
            root.Id = 1;
            root.Name = "Root";

            var serialized = serializer.Serialize<Root>(root);

            Assert.NotNull(serialized);

            Assert.Equal(8, serialized.Length);

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_GenericType_Success()
        {
            Protobuf.Serializer.Serializer serializer = new Protobuf.Serializer.Serializer();

            Generic<int> generic = new Generic<int>();
            generic.Data = 1;
            
            var serialized = serializer.Serialize<Generic<int>>(generic);

            Assert.NotNull(serialized);

            Assert.Equal(2, serialized.Length);
             

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void Serializer_InheritType_Success()
        {
            Protobuf.Serializer.Serializer serializer = new Protobuf.Serializer.Serializer();

            ExtendedData extended = new ExtendedData();
            extended.Created = DateTime.UtcNow;
            extended.Id = "1";
            extended.Extended = new System.Collections.Generic.Dictionary<string, object>();
            extended.Extended.Add("key1", new object());
            extended.WorkToDo = new Processes();
            extended.WorkToDo.Name = "Processes";
            extended.WorkToDo.Priority = 1;
            extended.WorkToDo.StartDate = DateTime.Now;
            
           var serialized = serializer.Serialize<ExtendedData>(extended);

            Assert.NotNull(serialized);

            Assert.Equal(44, serialized.Length);

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void DeSerializer_SimpleObjectType_Success()
        {
            Protobuf.Serializer.Serializer serializer = new Protobuf.Serializer.Serializer();

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
            Protobuf.Serializer.Serializer serializer = new Protobuf.Serializer.Serializer();

            byte[] array = new byte[] { 8, 1  };


            var instance = serializer.Deserialize<Generic<int>>(array);

            Assert.NotNull(instance);

            Assert.Equal(1, instance.Data);


        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void DeSerializer_InheritType_Success()
        {
            Protobuf.Serializer.Serializer serializer = new Protobuf.Serializer.Serializer();

            ExtendedData extended = new ExtendedData();

            byte[] array = new byte[] { 194, 62, 15, 34, 13, 26, 11, 8, 182, 204, 212, 249, 181, 234, 149, 54, 16, 5, 10, 1, 49, 18, 11, 8, 130, 230, 175, 221, 169, 232, 149, 54, 16, 5, };

            var instance = serializer.Deserialize<ExtendedData>(array);

            Assert.NotNull(instance);

            Assert.Equal("1", instance.Id);
            Assert.NotNull(instance.WorkToDo);
            Assert.NotNull(instance.Extended);

        }

    }
}
