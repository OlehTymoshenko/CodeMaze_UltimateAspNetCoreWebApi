using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.LinkModels
{
    public class Link
    {
        public string Href { get; init; }
        public string Rel { get; init; }
        public string Method { get; init; }

        // for xml serialization
        public Link() { }
    }
}
