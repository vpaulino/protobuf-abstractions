using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Xunit;
using Protobuff.Schemas;
using Protobuf.Schemas.Tests.Models;

namespace Protobuf.Schemas.Tests
{
    
    public class ProtoSchemaBuilderTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void BuildSchema_FromType()
        {

            string expected = @"syntax = ""proto3"";package Protobuf.Schemas.Tests.Models;message Root {    int32 Id = 1;    string Name = 2;}".Replace(" ", string.Empty);
            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

            string actual = builder.BuildSchema(typeof(Root));

            Assert.NotNull(actual);
            var actualResult = actual.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
            Assert.Equal(expected, actualResult);
             

        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void BuildSchema_FromAssembly()
        {

            string expected = @"syntax = ""proto3"";package Protobuf.Schemas.Tests.Models;import ""protobuf-net/bcl.proto""; // schema for protobuf-net's handling of core .NET types message Root {   int32 Id = 1;   string Name = 2;}message NotGeneric {   Root Data = 1;}message Data {   string Id = 1;   .bcl.DateTime Created = 2;  // the following represent sub-types; at most 1 should have a value  ExtendedData ExtendedData = 1000;}message ExtendedData {   map<string,Object> Extended = 3;   Processes WorkToDo = 4; }message Object { }message Processes {   .bcl.DateTime StartDate = 3;}".Replace(" ", string.Empty);
            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var assemblyFiles = Directory.GetFiles(Environment.CurrentDirectory);
            var modelAssembly = assemblyFiles.Where((file) => file.Contains("Protobuf")).ToList();


            var assemblyName = modelAssembly.FirstOrDefault((assm) => assm.Contains("Protobuf.Schemas.Tests.Models"));

            var assembly = Assembly.LoadFrom(assemblyName);

            string actual = builder.BuildSchema(assembly);

            Assert.NotNull(actual);
            var actualResult = actual.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
            Assert.Equal(expected, actualResult);


        }

        [Fact]
        public void BuildSchema_FromArrayOfTypes()
        {

            string expected = @"syntax = ""proto3"";package Protobuf.Schemas.Tests.Models;import ""protobuf-net/bcl.proto""; // schema for protobuf-net's handling of core .NET typesmessage Root {   int32 Id = 1;   string Name = 2;}message NotGeneric {   Root Data = 1;}message Data {   string Id = 1;   .bcl.DateTime Created = 2;  // the following represent sub-types; at most 1 should have a value  ExtendedData ExtendedData = 1000;}message ExtendedData {   map<string,Object> Extended = 3;   Processes WorkToDo = 4; }message Object { }message Processes {   .bcl.DateTime StartDate = 3;}".Replace(" ", string.Empty);
            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);
          
            var assemblyFiles = Directory.GetFiles(Environment.CurrentDirectory);
            var modelAssembly = assemblyFiles.Where((file) => file.Contains("Protobuf")).ToList();


            var assemblyName = modelAssembly.FirstOrDefault((assm) => assm.Contains("Protobuf.Schemas.Tests.Models"));

            var assembly = Assembly.LoadFrom(assemblyName);

            string actual = builder.BuildSchema(assembly.GetTypes());

            Assert.NotNull(actual);
            var actualResult = actual.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
            Assert.Equal(expected, actualResult);


        }
    }
}
