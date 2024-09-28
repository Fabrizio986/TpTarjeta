using System;

namespace TransporteUrbano
{
    public class Colectivo
    {
        public Boleto PagarCon(Tarjeta tarjeta)
        {
            decimal tarifa = tarjeta.ObtenerTarifa();
            tarjeta.DescontarSaldo(tarifa);
            Boleto boleto = new Boleto(tarifa);
            tarjeta.AgregarBoletoAlHistorial(boleto);
            return boleto;
        }
    }
}
