using System;

namespace TransporteUrbano
{
    public class Boleto
    {
        public decimal Monto { get; private set; }

        public Boleto(decimal monto)
        {
            Monto = monto;
        }
    }
}

