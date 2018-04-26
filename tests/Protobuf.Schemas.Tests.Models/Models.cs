using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Protobuf.Schemas.Tests.Models
{

    [DataContract]
    public class Root
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

    }

    [DataContract]
    public class NotGeneric
    {
        [DataMember(Order = 1)]
        public Root Data { get; set; }
    }

    [DataContract]
    public class Generic<T>
    {
        [DataMember(Order = 1)]
        public T Data { get; set; }
    }


    [ProtoBuf.ProtoInclude(1000, typeof(ExtendedData))]
    [DataContract]
    public class Data
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public DateTime Created { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class ExtendedData : Data
    {
        [DataMember(Order = 3)]
        public Dictionary<string, object> Extended { get; set; }

        [ProtoBuf.ProtoMember(tag: 4, Options = ProtoBuf.MemberSerializationOptions.Required)]
        public Processes WorkToDo { get; set; }
    }

    [KnownType(typeof(Processes))]
    [DataContract]
    public class Stack<T>
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public int Priority { get; set; }

    }


    [DataContract]
    public class Processes : Stack<Data>
    {
        [DataMember(Order = 3)]
        public DateTime StartDate { get; set; }
    }
}
