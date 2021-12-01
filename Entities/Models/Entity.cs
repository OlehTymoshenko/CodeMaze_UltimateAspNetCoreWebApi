﻿using Entities.Models.LinkModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models
{
    public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
    {
        private readonly string _root = "Entity";
        private readonly IDictionary<string, object> _expando;

        public Entity()
        {
            _expando = new ExpandoObject();
        }

        public object this[string key] 
        { 
            get => _expando[key]; 
            set => _expando[key] = value; 
        }

        public ICollection<string> Keys => _expando.Keys;

        public ICollection<object> Values => _expando.Values;

        public int Count => _expando.Count;

        public bool IsReadOnly => _expando.IsReadOnly;


        public XmlSchema GetSchema() => null;

        // This is a strange method implementation taken from book. 
        // I suppose it won't work correct
        public void ReadXml(XmlReader reader)
        {
            /* var wasEmpty = reader.IsEmptyElement;
             reader.ReadStartElement(_root);
             if (wasEmpty)
                 return;*/

            reader.ReadStartElement(_root);

            while(reader.Name.Equals(_root))
            {
                string typeContent;
                Type underlyingType;
                var name = reader.Name;

                reader.MoveToAttribute("type");
                typeContent = reader.ReadContentAsString();
                underlyingType = Type.GetType(typeContent);
                reader.MoveToContent();
                _expando[name] = reader.ReadElementContentAs(underlyingType, null);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in _expando.Keys)
            {
                WriteLinksToXml(key, _expando[key], writer);
            }
        }

        private void WriteLinksToXml(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);

            if(value.GetType() == typeof(List<Link>))
            {
                foreach (var link in value as List<Link>)
                {
                    writer.WriteStartElement(nameof(Link));
                    WriteLinksToXml(nameof(link.Href), link.Href, writer);
                    WriteLinksToXml(nameof(link.Rel), link.Rel, writer);
                    WriteLinksToXml(nameof(link.Method), link.Method, writer);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value.ToString());
            }

            writer.WriteEndElement();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_expando.TryGetValue(binder.Name, out object resultValue))
            {
                result = resultValue;
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _expando[binder.Name] = value;

            return true;    
        }

        public void Add(string key, object value)
        {
            _expando.Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _expando.Add(item);
        }

        public void Clear()
        {
            _expando.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _expando.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _expando.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _expando.CopyTo(array, arrayIndex);
        }

        public bool Remove(string key)
        {
            return _expando.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _expando.Remove(item);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return _expando.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _expando.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _expando.GetEnumerator();
        }
    }
}
