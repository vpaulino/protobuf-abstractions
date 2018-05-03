
[![Build Status](https://api.travis-ci.org/vpaulino/protobuf-abstractions.svg?branch=master)](https://travis-ci.org/vpaulino/protobuf-abstractions)

 
# Protobuf-Abstractions

This is a set of classes and interfaces that abstract the usage of protobuf funcionalities. 

# Requirements

* .NetStandard >= 2.0
* .Net >= 4.5.1
* protobuf-net >=2.3.7

# Serialization.Proto.Schemas

This library makes available a set of functionalities to generate proto schema from .NET Types. The set of funcionalities comes in two flavors: 

1. Object Oriented
2. Fluent API

## Entities

The set of entities available is the one presented in picture 1 that belong to namespace Serialization.Proto.Schemas. 


![UML_OO](https://go.gliffy.com/go/share/image/srk6lentbt23cfw41y9e.png?utm_medium=live-embed&utm_source=custom) 

### BuilderSettings

This class is responsible to parameterize the Builder execution.

### BuilderRule

This class represents a rule that as a Type to be evaluated using is Predicate during a build of a schema.

### ITransformer

This interface represents a contract to transform proto schemas of a certain type. There are two properties in the BuilderSettings that allows the programmer to register their own Transformers. 

### ProtobuffSchemaRender

This class is responsible to generate the proto schema string from a know type or parse from a type just the headers or the messages generated from that type.

### ProtobufSchemaBuilder

This class is responsible to generate an entire proto schema representation  from an assembly or set of known types and return it in a string.



### Samples
 

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










