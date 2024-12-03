using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Models.Response
{
    public record CurrencyDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int DecimalDigits { get; set; }
        public string Symbol { get; set; } = null!;
    }
}
