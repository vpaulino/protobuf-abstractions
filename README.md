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


![UML](https://go.gliffy.com/go/share/image/srk6lentbt23cfw41y9e.png?utm_medium=live-embed&utm_source=custom) 


### Usage

````
// renders the proto schema in version 2

ISchemaRender protoSchemaRender = new ProtoSchemaRender()

ISchemaBuilder protoSchemaBuilder = new ProtoSchemaBuilder(protoSchemaRender);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
var types = assemblies.FirstOrDefault().GetTypes().Where(t => t.Namespace.Contains());

String fullSchema = protoSchemaBuilder.BuildSchema(types);

```
