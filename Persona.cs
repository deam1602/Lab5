using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    public class Persona : IComparable<Persona>
    {

        public string name { get; set; }
        public long DPI { get; set; }
        public string datebirth { get; set; }
        public string address { get; set; }
        public string[] companies { get; set; }
        public string recluiter { get; set; }
        public int CompareTo(Persona other)
        {
            if (other == null)
            {
                // Si el objeto other es nulo, la instancia actual es mayor.
                return 1;
            }

            // Comparar por el nombre.
            return this.DPI.CompareTo(other.DPI);
        }
    }
}
