using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Xunit;
using Protobuf.Schemas.Tests.Models;
using Serialization.Proto.Schemas;

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
        public void BuildSchema_FromTypeWithEnumProperties()
        {

            string expected = @"syntax=""proto2"";packageProtobuf.Schemas.Tests.Models;import""protobuf - net / bcl.proto"";//schemaforprotobuf-net'shandlingofcore.NETtypesmessageEnumValuesExtensions{}enumEnumValues{//thisisacomposite/flagsenumerationNone=0;Multiple=1;Scene=2;Scan=4;Face=8;Voice=16;Iris=32;}messageClassWithEnumProperty{optionalint32Id=1[default=0];optionalstringName=2;optionalEnumValuesValueEnum=12[default=None];}messageRoot{optionalint32Id=1[default=0];optionalstringName=2;}messageNotGeneric{optionalRootData=3;}messageData{optionalstringId=5;optional.bcl.DateTimeCreated=6;//thefollowingrepresentsub-types;atmost1shouldhaveavalueoptionalExtendedDataExtendedData=1000;}messageExtendedData{map<string,Object>Extended=7;requiredStack_DataWorkToDo=8;}messageObject{}messageProcesses{optional.bcl.DateTimeStartDate=11;}messageStack_Data{optionalstringName=9;optionalint32Priority=10[default=0];repeatedDataCollection=11;//thefollowingrepresentsub-types;atmost1shouldhaveavalueoptionalProcessesProcesses=1000;}".Replace(" ", string.Empty);
            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto2);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

            var assemblyFile = Directory.GetFiles(Environment.CurrentDirectory).Where((fileName)=> fileName.Contains("Protobuf.Schemas.Tests.Models")).FirstOrDefault();

            var assembly = Assembly.LoadFrom(assemblyFile);


            string actual = builder.BuildSchema(assembly);

            Assert.NotNull(actual);
            var actualResult = actual.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
            Assert.Equal(expected, actualResult);


        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void BuildSchema_FromAssembly_FilteredByName()
        {

            string expected = @"syntax=""proto2"";packageProtobuf.Schemas.Tests.Models;import""protobuf - net / bcl.proto"";//schemaforprotobuf-net'shandlingofcore.NETtypesmessageEnumValuesExtensions{}enumEnumValues{//thisisacomposite/flagsenumerationNone=0;Multiple=1;Scene=2;Scan=4;Face=8;Voice=16;Iris=32;}messageClassWithEnumProperty{optionalint32Id=1[default=0];optionalstringName=2;optionalEnumValuesValueEnum=12[default=None];}messageRoot{optionalint32Id=1[default=0];optionalstringName=2;}messageNotGeneric{optionalRootData=3;}messageData{optionalstringId=5;optional.bcl.DateTimeCreated=6;//thefollowingrepresentsub-types;atmost1shouldhaveavalueoptionalExtendedDataExtendedData=1000;}messageExtendedData{map<string,Object>Extended=7;requiredStack_DataWorkToDo=8;}messageObject{}messageProcesses{optional.bcl.DateTimeStartDate=11;}messageStack_Data{optionalstringName=9;optionalint32Priority=10[default=0];repeatedDataCollection=11;//thefollowingrepresentsub-types;atmost1shouldhaveavalueoptionalProcessesProcesses=1000;}".Replace(" ", string.Empty);
            ProtobuffSchemaRender schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto2);

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(schemaRender);

            var assemblyFile = Directory.GetFiles(Environment.CurrentDirectory).Where((fileName) => fileName.Contains("Protobuf.Schemas.Tests.Models")).FirstOrDefault();

            var assembly = Assembly.LoadFrom(assemblyFile);


            string actual = builder.BuildSchema(assembly);

            Assert.NotNull(actual);
            var actualResult = actual.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
            Assert.Equal(expected, actualResult);


        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void BuildSchema_FromAssembly()
        {

            string expected = @"syntax=""proto3"";packageProtobuf.Schemas.Tests.Models;import""protobuf - net / bcl.proto"";//schemaforprotobuf-net'shandlingofcore.NETtypesmessageEnumValuesExtensions{}enumEnumValues{//thisisacomposite/flagsenumerationNone=0;Multiple=1;Scene=2;Scan=4;Face=8;Voice=16;Iris=32;}messageClassWithEnumProperty{int32Id=1;stringName=2;EnumValuesValueEnum=12;}messageRoot{int32Id=1;stringName=2;}messageNotGeneric{RootData=3;}messageData{stringId=5;.bcl.DateTimeCreated=6;//thefollowingrepresentsub-types;atmost1shouldhaveavalueExtendedDataExtendedData=1000;}messageExtendedData{map<string,Object>Extended=7;Stack_DataWorkToDo=8;}messageObject{}messageProcesses{.bcl.DateTimeStartDate=11;}messageStack_Data{stringName=9;int32Priority=10;repeatedDataCollection=11;//thefollowingrepresentsub-types;atmost1shouldhaveavalueProcessesProcesses=1000;}".Replace(" ", string.Empty);
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

            string expected = @"syntax=""proto3"";packageProtobuf.Schemas.Tests.Models;import""protobuf - net / bcl.proto"";//schemaforprotobuf-net'shandlingofcore.NETtypesmessageEnumValuesExtensions{}enumEnumValues{//thisisacomposite/flagsenumerationNone=0;Multiple=1;Scene=2;Scan=4;Face=8;Voice=16;Iris=32;}messageClassWithEnumProperty{int32Id=1;stringName=2;EnumValuesValueEnum=12;}messageRoot{int32Id=1;stringName=2;}messageNotGeneric{RootData=3;}messageData{stringId=5;.bcl.DateTimeCreated=6;//thefollowingrepresentsub-types;atmost1shouldhaveavalueExtendedDataExtendedData=1000;}messageExtendedData{map<string,Object>Extended=7;Stack_DataWorkToDo=8;}messageObject{}messageProcesses{.bcl.DateTimeStartDate=11;}messageStack_Data{stringName=9;int32Priority=10;repeatedDataCollection=11;//thefollowingrepresentsub-types;atmost1shouldhaveavalueProcessesProcesses=1000;}".Replace(" ", string.Empty);
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
