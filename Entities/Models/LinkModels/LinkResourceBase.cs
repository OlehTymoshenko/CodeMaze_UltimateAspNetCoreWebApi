﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.LinkModels
{
    public class LinkResourceBase
    {
        public List<Link> Links { get; set; } = new();

        public LinkResourceBase() { }
    }
}
