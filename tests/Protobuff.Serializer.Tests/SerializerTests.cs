using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Protobuf.Schemas.Tests.Models;
using Serialization.Proto.Schemas;
using VBAnalysisTests;
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

        /// <summary>
         // descerializado com sucesso em nodejs
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task Serializer_ObjectType_WithCollectionElements_proto3_Success()
        {
           
            Serializer serializer = new Serializer();

            Root root = new Root();
            root.Id = 9999;
            root.Name = "Root";

            root.Collection.Add(new Data() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });
            root.Collection.Add(new ExtendedData() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });

            FileManager jsonFile = new FileManager(new Uri(Environment.CurrentDirectory + "//RootWCollection.json"));

            await jsonFile.WriteAsync(JsonConvert.SerializeObject(root));
            var serialized = serializer.Serialize<Root>(root);

            Assert.NotNull(serialized);
            Assert.Equal(118, serialized.Length);

            var deserialized = serializer.Deserialize<Root>(serialized);

            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto2);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

            string schema = builder.BuildSchema(typeof(Root));

            Assert.NotNull(schema);

            FileManager payloadFile = new FileManager(new Uri(Environment.CurrentDirectory + "//RootWCollection_proto2.dat"));

            await payloadFile.WriteAsync(serialized);

            FileManager protoFile = new FileManager(new Uri(Environment.CurrentDirectory + "//RootWCollection_proto2.proto"));

            await protoFile.WriteAsync(schema);
             
            Assert.NotNull(serialized);

            Assert.Equal(118, serialized.Length);

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task Serializer_ObjectType_WithCollectionElements_proto2_Success()
        {

            Serializer serializer = new Serializer();

            Root root = new Root();
            root.Id = 9999;
            root.Name = "Root";

            root.Collection.Add(new Data() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });
            root.Collection.Add(new ExtendedData() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });

            FileManager jsonFile = new FileManager(new Uri(Environment.CurrentDirectory + "//RootWCollection.json"));

            await jsonFile.WriteAsync(JsonConvert.SerializeObject(root));
            var serialized = serializer.Serialize<Root>(root);

            Assert.NotNull(serialized);
            Assert.Equal(118, serialized.Length);

            var deserialized = serializer.Deserialize<Root>(serialized);

            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto2);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

            string schema = builder.BuildSchema(typeof(Root));

            Assert.NotNull(schema);

            //FileManager payloadFile = new FileManager(new Uri(Environment.CurrentDirectory + "//RootWCollection_proto2.dat"));

            //await payloadFile.WriteAsync(serialized);

            //FileManager protoFile = new FileManager(new Uri(Environment.CurrentDirectory + "//RootWCollection_proto2.proto"));

            //await protoFile.WriteAsync(schema);


            Assert.NotNull(serialized);

            //Assert.Equal(8, serialized.Length);

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
        public async Task Serializer_GenericType_WithInheritComposedChild_Success()
        {
            Serializer serializer = new Serializer();

            NotGeneric notGeneric = new NotGeneric();
            notGeneric.DataObj = new ExtendedData()
            {
                Extended = new System.Collections.Generic.Dictionary<string, object>(),
                 Created = DateTime.UtcNow.ToString(),
                  ExtendedPRop = "ExtendedPRop_NotGeneric",
            
            };
            notGeneric.Data = new Root()
            {
                 Id =11111,
                  EnumType =  new ClassWithEnumProperty() { ValueEnum = EnumValues.Face },
                   Name = "FaceRoot"
            };

            var serialized = serializer.Serialize<NotGeneric>(notGeneric);

            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

            string schema = builder.BuildSchema(typeof(NotGeneric));

            Assert.NotNull(schema);

            //FileManager payloadFile = new FileManager(new Uri(Environment.CurrentDirectory + "//NotGeneric.dat"));

            //await payloadFile.WriteAsync(serialized);

            //FileManager protoFile = new FileManager(new Uri(Environment.CurrentDirectory + "//NotGeneric.proto"));

            //await protoFile.WriteAsync(schema);


            Assert.NotNull(serialized);

         //   Assert.Equal(8, serialized.Length);


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
        public async Task Serializer_CollectionWithInheritanceElements_Success()
        {
            try
            {
                Serializer serializer = new Serializer();

                 
                var WorkToDo = new Processes();
                WorkToDo.Name = "Processes";
                WorkToDo.Priority = 1;
                WorkToDo.StartDate = DateTime.Now.ToString();
                WorkToDo.Collection.Add(new Data() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });
                WorkToDo.Collection.Add(new ExtendedData() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });

                FileManager jsonFile = new FileManager(new Uri(Environment.CurrentDirectory + "//Processes.json"));

                await jsonFile.WriteAsync(JsonConvert.SerializeObject(WorkToDo));
                var serialized = serializer.Serialize<Processes>(WorkToDo);

                //Assert.NotNull(serialized);
                //Assert.Equal(185, serialized.Length);

               // var deserialized = serializer.Deserialize<Processes>(serialized);

                ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);

                ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

                string schema = builder.BuildSchema(typeof(Processes));

                Assert.NotNull(schema);

                //FileManager payloadFile = new FileManager(new Uri(Environment.CurrentDirectory + "//Processes.dat"));

                //await payloadFile.WriteAsync(serialized);

                //FileManager protoFile = new FileManager(new Uri(Environment.CurrentDirectory + "//Processes.proto"));

                //await protoFile.WriteAsync(schema);
            }
            catch (Exception ex)
            {

                throw;
            }



        }


        //[Fact]
        //[Trait("TestCategory", "Unit")]
        //public async Task Serializer_InheritType_Success()
        //{
        //    try
        //    {
        //        Serializer serializer = new Serializer();

        //        ExtendedData extended = new ExtendedData();
        //        extended.Created = DateTime.UtcNow.ToString();
        //        extended.Id = "1";
        //        extended.Extended = new System.Collections.Generic.Dictionary<string, object>();
        //        extended.Extended.Add("key1", new object());
        //        extended.WorkToDo = new Processes();
        //        extended.WorkToDo.Name = "Processes";
        //        extended.WorkToDo.Priority = 1;
        //        extended.WorkToDo.StartDate = DateTime.Now.ToString();
        //        extended.WorkToDo.Collection.Add(new Data() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });
        //        extended.WorkToDo.Collection.Add(new ExtendedData() { Id = "dataCreatedInsideCOllection", Created = DateTime.UtcNow.ToString() });

        //        FileManager jsonFile = new FileManager(new Uri(Environment.CurrentDirectory + "//ExtendedData.json"));

        //        await jsonFile.WriteAsync(JsonConvert.SerializeObject(extended));
        //        var serialized = serializer.Serialize<ExtendedData>(extended);

        //        Assert.NotNull(serialized);
        //        Assert.Equal(186, serialized.Length);

        //        var deserialized = serializer.Deserialize<ExtendedData>(serialized);

        //        ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);

        //        ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

        //        string schema = builder.BuildSchema(typeof(ExtendedData));

        //        Assert.NotNull(schema);

        //        //FileManager payloadFile = new FileManager(new Uri(Environment.CurrentDirectory + "//ExtendedData.dat"));

        //        //await payloadFile.WriteAsync(serialized);

        //        //FileManager protoFile = new FileManager(new Uri(Environment.CurrentDirectory + "//ExtendedData.proto"));

        //        //await protoFile.WriteAsync(schema);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
          


        //}

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

        //[Fact]
        //[Trait("TestCategory", "Unit")]
        //public void DeSerializer_InheritType_Success()
        //{
        //    Serializer serializer = new Serializer();

        //    ExtendedData extended = new ExtendedData();

        //    byte[] array = new byte[] { 194, 62, 85, 58, 8, 10, 4, 107, 101, 121, 49, 18, 0, 66, 73, 194, 62, 13, 90, 11, 8, 134, 214, 153, 165, 169, 162, 151, 54, 16, 5, 74, 9, 80, 114, 111, 99, 101, 115, 115, 101, 115, 80, 1, 90, 42, 42, 27, 100, 97, 116, 97, 67, 114, 101, 97, 116, 101, 100, 73, 110, 115, 105, 100, 101, 67, 79, 108, 108, 101, 99, 116, 105, 111, 110, 50, 11, 8, 252, 212, 246, 136, 157, 160, 151, 54, 16, 5, 42, 1, 49, 50, 11, 8, 168, 232, 243, 136, 157, 160, 151, 54, 16, 5 };

        //    var instance = serializer.Deserialize<ExtendedData>(array);

        //    Assert.NotNull(instance);

        //    Assert.Equal("1", instance.Id);
        //    Assert.NotNull(instance.WorkToDo);
        //    Assert.NotNull(instance.Extended);

        //}

        //[Fact]
        //[Trait("TestCategory", "Unit")]
        //public void Serialize_DeSerializer_CollectionWithInHeritanceElements_Success()
        //{
        //    Serializer serializer = new Serializer();

        //    ClassWithCollection instance = new ClassWithCollection();
        //    instance.Root = 

        //}


    }
}
