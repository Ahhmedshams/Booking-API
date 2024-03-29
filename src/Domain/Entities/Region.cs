﻿using Org.BouncyCastle.Asn1.Cmp;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Entities
{

    public class Region : BaseEntity
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
    }
}
