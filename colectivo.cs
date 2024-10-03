using System;

namespace TransporteUrbano
{
    public class Colectivo
    {
        public Boleto PagarCon(Tarjeta tarjeta)
        {
            TarjetaMedioBoleto tarjetaMedioBoleto = tarjeta as TarjetaMedioBoleto;
            TarjetaBoletoEducativo tarjetaBoletoEducativo = tarjeta as TarjetaBoletoEducativo;

            if (tarjetaMedioBoleto != null)
            {
                if (!tarjetaMedioBoleto.PuedeViajar())
                {
                    throw new InvalidOperationException("No se puede realizar otro viaje antes de 5 minutos o se ha alcanzado el límite de viajes para el día.");
                }
            }

            decimal tarifa = tarjeta.ObtenerTarifa();

            if (tarjetaBoletoEducativo != null)
            {
                
                if (tarjetaBoletoEducativo.PuedeViajar())
                {
                    Console.WriteLine("Viaje gratuito con boleto educativo.");
                    tarifa = 0;
                }
                else
                {
                    Console.WriteLine("Se ha alcanzado el límite de viajes gratuitos por hoy. Ahora se cobrará tarifa normal.");
                }
            }

            if (tarifa > 0)
            {
                
                if (tarjeta.Saldo >= tarifa)
                {
                    tarjeta.DescontarSaldo(tarifa);
                }
                else
                {
                    throw new InvalidOperationException("Saldo insuficiente para realizar el pago.");
                }
            }

            Boleto boleto = new Boleto(tarifa, tarjeta.GetType().Name, "Línea 1", tarjeta.Saldo, 1);
            tarjeta.AgregarBoletoAlHistorial(boleto);

            if (tarjetaMedioBoleto != null)
            {
                tarjetaMedioBoleto.RegistrarViaje();
            }

            if (tarjetaBoletoEducativo != null)
            {
                tarjetaBoletoEducativo.RegistrarViaje();
            }

            return boleto;
        }
    }
}

