using SICER.MIGRACION.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.TEST
{
    class Program
    {
        static void Main(string[] args)
        {
            var rs = new SQLConnection().DoQuery("Select * from Clientes");
        }
    }
}
