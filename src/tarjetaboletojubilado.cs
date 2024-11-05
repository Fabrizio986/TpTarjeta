using System;
using System.Collections.Generic;
using ManejoDeTiempos;

namespace TransporteUrbano
{
    public class TarjetaJubilado : Tarjeta
    {
        public TarjetaJubilado(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo) { }

        public override decimal ObtenerTarifa(Tiempo tiempo)
        {
            return 0m;
        }

        public bool PuedeViajar(Tiempo tiempo)
        {
            if (!EsHorarioPermitido(tiempo))
            {
                Console.WriteLine("Viaje no permitido fuera de la franja horaria de 6 a 22, de lunes a viernes.");
                return false;
            }

            return true;
        }
    }
}