using System;
using System.Collections.Generic;
using ManejoDeTiempos;

namespace TransporteUrbano
{    
    public class TarjetaBoletoEducativo : Tarjeta
    {
        private int viajesGratisRealizados;
        private DateTime? ultimoViaje; 

        public TarjetaBoletoEducativo(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo)
        {
            viajesGratisRealizados = 0;
            ultimoViaje = null;
        }

        public bool PuedeViajar(Tiempo tiempo)
        {
            if (!EsHorarioPermitido(tiempo))
            {
                Console.WriteLine("Viaje no permitido fuera de la franja horaria de 6 a 22, de lunes a viernes.");
                return false;
            }

            if (viajesGratisRealizados >= 2)
            {
                return false;
            }

            if (ultimoViaje.HasValue)
            {
                var tiempoTranscurrido = tiempo.Now() - ultimoViaje.Value;
                if (tiempoTranscurrido.TotalMinutes < 5)
                {
                    Console.WriteLine("Debe esperar 5 minutos entre los viajes gratuitos.");
                    return false;
                }
            }

            return true;
        }

        public void RegistrarViaje(Tiempo tiempo)
        {
            if (viajesGratisRealizados < 2)
            {
                viajesGratisRealizados++;
                ultimoViaje = tiempo.Now();
            }
            else
            {
                Console.WriteLine("Ya ha alcanzado el límite de viajes gratuitos por hoy.");
            }
        }

        public override decimal ObtenerTarifa(Tiempo tiempo)
        {
            if (viajesGratisRealizados < 2)
            {
                return 0;
            }
            else
            {
                return TarifaBase;
            }
        }
    }
}