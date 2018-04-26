
[![Build Status](https://travis-ci.org/vpaulino/protobuf-abstractions.png)](https://travis-ci.org/vpaulino/protobuf-abstractions)
<!--[![Coverage Status](https://coveralls.io/repos/github/vpaulino/protobuf-abstractions/badge.svg?branch=master)](https://coveralls.io/github/vpaulino/protobuf-abstractions?branch=master)-->

# protobuf-abstractions
a set of funcionalities to deal with proto serialization

# Protobuf-Abstractions

This is a set of classes that abstract the usage of protobuf funcionalities. This library makes available those funcionalities in two flavors:
1. Object Oriented
2. Fluent API

## Object Oriented

The set of entities that make this flavour available is the one presented in picture 1 that belong to namespace Protobuf.Schemas. 

### ProtobuffSchemaRender

This class is responsable to generate the proto schema string from a know type or parse from a type just the headers or the messages generated from that type.

### ProtobufSchemaBuilder

This class is responsable to generate an entire proto schema representation  from an assembly or set of known types and return it in a string.


![UML_OO](https://go.gliffy.com/go/share/image/srk6lentbt23cfw41y9e.png?utm_medium=live-embed&utm_source=custom) 


### Usage

````
// renders the proto schema in version 2

ISchemaRender protoSchemaRender = new ProtoSchemaRender()

ISchemaBuilder protoSchemaBuilder = new ProtoSchemaBuilder(protoSchemaRender);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
var types = assemblies.FirstOrDefault().GetTypes().Where(t => t.Namespace.Contains("MyModelNamespace"));

String fullSchema = protoSchemaBuilder.BuildSchema(types);

````

## FluentApi 

This sdk makes available a Set of extensions methods in the PRotobuf.Schemas.Fluent namespace that enable a fluent utilization of the available funcionality. This fluent API is suported by a scope type that maintains the context of the fluent calls until the end. 
To start the chain of any fluent calls the class ProtobufSchemaBuilder makes available a static method called Start that returns an instance of SchemaBuilderScope and from there is possible to start use the extensions methods available in the static class SchemaBuilderScopeExtensions. 

````

var myAssemblies =  AppDomain.CurrentDomain.GetAssemblies();
var otherASsemblies = LoadedFromDirectory();
 await ProtobufSchemaBuilder.Start()
                     .FetchTypes(myAssemblies.FirstOrDefault(),t => t.Namespace.Contains("MyModelNamespace"))
                     .AddTypes(otherASsemblies.Where(/*some criteria */).FirstOrDefault(),t => t.Namespace.Contains("namespace") )
                     .BuildSchema(ProtoBuf.Meta.ProtoSyntax.3)
                     .WriteSchemaAsync(filePath);
````                     
                     
                     
The methods  available to suport the fluent API are showned in the following diagram. 

![UML_Fluent](https://go.gliffy.com/go/share/image/s33x9h6el0erowtm4y5u.png?utm_medium=live-embed&utm_source=custom) 










