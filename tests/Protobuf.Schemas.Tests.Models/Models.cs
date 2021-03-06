﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Xml.Serialization;

namespace Protobuf.Schemas.Tests.Models
{

    [XmlType("EnumValues")]
    [Flags]
    public enum EnumValues
    {
        None = 0x00000000,
        Multiple = 0x00000001,
        Scene = 0x00000002,
        Scan = 0x00000004,
        Face = 0x00000008,
        Voice = 0x00000010,
        Iris = 0x00000020

    }

    [ProtoBuf.ProtoContract]
    public class ClassWithEnumProperty
    {
        [ProtoBuf.ProtoMember(tag: 1)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(tag: 2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(tag: 12)]
        public EnumValues ValueEnum { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class Root
    {
        [ProtoBuf.ProtoMember(tag: 1)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(tag: 2)]
        public string Name { get; set; }

        //[ProtoBuf.ProtoMember(tag: 12)]
        //public EnumValues ValueEnum { get; set; }


    }

    [ProtoBuf.ProtoContract]
    public class NotGeneric
    {
        [ProtoBuf.ProtoMember(tag: 3)]
        public Root Data { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class Generic<T>
    {
        [ProtoBuf.ProtoMember(tag: 4)]
        public T Data { get; set; }
    }


    [ProtoBuf.ProtoInclude(1000, typeof(ExtendedData))]
    [ProtoBuf.ProtoContract]
    public class Data
    {
        [ProtoBuf.ProtoMember(tag: 5)]
        public string Id { get; set; }

        [ProtoBuf.ProtoMember(tag: 6)]
        public DateTime Created { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class ExtendedData : Data
    {
        [ProtoBuf.ProtoMember(tag: 7)]
        public Dictionary<string, object> Extended { get; set; }

        [ProtoBuf.ProtoMember(tag: 8, Options = ProtoBuf.MemberSerializationOptions.Required)]
        public Processes WorkToDo { get; set; }
    }

    //[KnownType("GetKnownTypes")]
    [ProtoBuf.ProtoInclude(1000, typeof(Processes))]
    [ProtoBuf.ProtoContract]
    public class Stack<T>
    {

        public Stack()
        {
            Collection = new List<T>(); //new PrioritizeCollection<T>();
        }

        static Type[] GetKnownTypes()
        {
            return new Type[] { typeof(Stack<T>) };
        }

        [ProtoBuf.ProtoMember(tag: 9)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(tag: 10)]
        public int Priority { get; set; }

        [ProtoBuf.ProtoMember(tag: 11)]
        [XmlArrayItem(typeof(Data))]
        public ICollection<T> Collection { get; set; }

    }

    [KnownType("GetKnownTypes")]
    [ProtoBuf.ProtoContract]
    public class PrioritizeCollection<T> : AbsCollection<T>, IEnumerable<T>
    {

        [KnownType("GetKnownTypes")]
        [ProtoBuf.ProtoContract]

        public class Element<U>
        {
            public Element(int priority, T item)
            {
                this.Priority = priority;
                this.Item = item;
                this.Created = DateTime.UtcNow;
            }

            static Type[] GetKnownTypes()
            {
                return new Type[] { typeof(Element<U>) };
            }

            [ProtoBuf.ProtoMember(tag: 12)]

            public DateTime Created { get; private set; }

            [ProtoBuf.ProtoMember(tag: 13)]
            public int Priority { get; set; }

            [ProtoBuf.ProtoMember(tag: 14)]
            public T Item { get; }
        }


        static Type[] GetKnownTypes()
        {
            return new Type[] { typeof(PrioritizeCollection<T>) };
        }
        public override void Add(T item)
        {
            this.container.Add(item);
        }

        public override void Clear()
        {
            this.container.Clear();
        }

        public override bool Contains(T item)
        {
            return this.container.Contains(item);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            this.container.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return this.container.GetEnumerator();
        }

        public override bool Remove(T item)
        {
            return this.container.Remove(item);
        }

        IEnumerator  IEnumerable.GetEnumerator()
        {
            return this.container.GetEnumerator();
        }

   
    }

    public abstract class AbsCollection<T> : ICollection<T>
    {
      
        protected ICollection<T> container = new List<T>();

        public int Count => container.Count;

        public bool IsReadOnly => container.IsReadOnly;

        public abstract void Add(T item);


        public abstract void Clear();

        public abstract bool Contains(T item);


        public abstract void CopyTo(T[] array, int arrayIndex);


        public abstract IEnumerator<T> GetEnumerator();


        abstract public bool Remove(T item);

        IEnumerator  IEnumerable.GetEnumerator()
        {
            return this.container.GetEnumerator();
        }
         
    }


    [ProtoBuf.ProtoContract]
    public class Processes : Stack<Data>
    {
        [ProtoBuf.ProtoMember(tag: 11)]
        public DateTime StartDate { get; set; }
    }
}
