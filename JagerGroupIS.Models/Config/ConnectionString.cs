using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.Models.Config
{
    public class ConnectionString
    {
        public string Value { get; set; }

        public override string ToString() => Value;
    }
}
