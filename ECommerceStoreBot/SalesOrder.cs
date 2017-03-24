using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceStoreBot
{

        public class Salesorder
        {
            public string processid { get; set; }

        [Prompt("Please enter the order number {&}")]
        public string ordernumber { get; set; }
            public string new_shippingstatus { get; set; }
        }

        public class RootObject
        {
            public List<Salesorder> salesorders { get; set; }
        }
 }

