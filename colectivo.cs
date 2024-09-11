using System;

namespace TransporteUrbano
{
    public class Colectivo
    {
        private const decimal TarifaBasica = 940m;

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta.Saldo < TarifaBasica)
            {
                throw new InvalidOperationException("Saldo insuficiente en la tarjeta.");
            }

            tarjeta.DescontarSaldo(TarifaBasica);
            return new Boleto(TarifaBasica);
        }
    }
}
