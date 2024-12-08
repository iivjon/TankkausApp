using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankkausApp.Models
{
    class Tankkaus
    {
        public int Tankkausid { get; set; }

        public string RekisteriNum { get; set; } = null!;

        public decimal? TankatutLitrat { get; set; }

        public decimal? TankatutEurot { get; set; }

        public decimal? AjoKilometrit { get; set; }

        public DateTime? PaivaMaara { get; set; }

        public decimal? Litrahinta { get; set; }
    }
}
