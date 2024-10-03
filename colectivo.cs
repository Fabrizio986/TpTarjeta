using System;

namespace TransporteUrbano
{
    public class Colectivo
    {
        public Boleto PagarCon(Tarjeta tarjeta)
        {
            TarjetaMedioBoleto tarjetaMedioBoleto = tarjeta as TarjetaMedioBoleto;

            if (tarjetaMedioBoleto != null)
            {
                if (!tarjetaMedioBoleto.PuedeViajar())
                {
                    throw new InvalidOperationException("No se puede realizar otro viaje antes de 5 minutos o se ha alcanzado el límite de viajes para el día.");
                }
            }

            decimal tarifa = tarjeta.ObtenerTarifa();
            tarjeta.DescontarSaldo(tarifa);
            Boleto boleto = new Boleto(tarifa, tarjeta.GetType().Name, "Línea 1", tarjeta.Saldo, 1);
            tarjeta.AgregarBoletoAlHistorial(boleto);

            if (tarjetaMedioBoleto != null)
            {
                tarjetaMedioBoleto.RegistrarViaje(); 
            }

            return boleto;
        }
    }

}
