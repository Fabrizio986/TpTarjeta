using System;
using System.Collections.Generic;
using ManejoDeTiempos;

namespace TransporteUrbano
{
    public class TarjetaMedioBoleto : Tarjeta
    {
        private DateTime? ultimoViaje;
        private int viajesRealizados;
    
        public TarjetaMedioBoleto(decimal saldoInicial, Tiempo tiempo) : base(saldoInicial, tiempo) { }
    
        public bool PuedeViajar(Tiempo tiempo)
        {
            if (!EsHorarioPermitido(tiempo))
            {
                Console.WriteLine("Viaje no permitido fuera de la franja horaria de 6 a 22, de lunes a viernes.");
                return false;
            }
    
            if (ultimoViaje == null)
            {
                return true;
            }
    
            if ((tiempo.Now() - ultimoViaje.Value).TotalMinutes < 5)
            {
                return false;
            }
    
            return viajesRealizados < 2;
        }
    
        public void RegistrarViaje(Tiempo tiempo)
        {
            if (ultimoViaje == null || (tiempo.Now() - ultimoViaje.Value).TotalMinutes >= 5)
            {
                ultimoViaje = tiempo.Now();
                viajesRealizados++;
            }
        }
    
        public override decimal ObtenerTarifa(Tiempo tiempo)
        {
            return TarifaBase / 2;
        }
    }
}