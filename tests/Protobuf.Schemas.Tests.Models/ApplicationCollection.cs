using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ProtoBuf;

namespace Protobuf.Schemas.Tests.Models
{
    [ProtoContract]
    public class ApplicationCollection : Collection<Data>
    {

    }
}
