using System;
using ManejoDeTiempos;

namespace TransporteUrbano
{
    public class Colectivo
    {
        private const decimal TarifaInterurbana = 2500m;

        public Boleto PagarCon(Tarjeta tarjeta, Tiempo tiempo, bool esInterurbano = false)
        {
            TarjetaMedioBoleto tarjetaMedioBoleto = tarjeta as TarjetaMedioBoleto;
            TarjetaBoletoEducativo tarjetaBoletoEducativo = tarjeta as TarjetaBoletoEducativo;

            if (tarjetaMedioBoleto != null)
            {
                if (!tarjetaMedioBoleto.PuedeViajar(tiempo))
                {
                    throw new InvalidOperationException("No se puede realizar otro viaje antes de 5 minutos o se ha alcanzado el límite de viajes para el día.");
                }
            }

            decimal tarifa = esInterurbano ? TarifaInterurbana : tarjeta.ObtenerTarifa(tiempo);

            if (tarjetaBoletoEducativo != null)
            {
                if (tarjetaBoletoEducativo.PuedeViajar(tiempo))
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
                tarjetaMedioBoleto.RegistrarViaje(tiempo);
            }

            if (tarjetaBoletoEducativo != null)
            {
                tarjetaBoletoEducativo.RegistrarViaje(tiempo);
            }

            return boleto;
        }
    }
}

