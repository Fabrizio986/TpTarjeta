using System;
using System.Collections.Generic;
using ManejoDeTiempos;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal LimiteSaldo = 36000m; 
        private const decimal SaldoNegativoMaximo = -480m; 
        protected const decimal TarifaBase = 1200m;
        private List<Boleto> historialBoletos = new List<Boleto>();
        private DateTime fechaPrimerViajeDelMes;
        private int viajesEsteMes;

        public decimal Saldo { get; protected set; }
        public decimal SaldoPendiente { get; private set; }

        public Tarjeta(decimal saldoInicial, Tiempo tiempo)
        {
            if (!EsSaldoValido(saldoInicial))
            {
                throw new ArgumentException("Saldo inicial no válido.");
            }

            Saldo = saldoInicial;
            SaldoPendiente = 0m;
            viajesEsteMes = 0;
            fechaPrimerViajeDelMes = tiempo.Now(); 
        }

        public virtual decimal ObtenerTarifa(Tiempo tiempo)
        {
            if (tiempo.Now().Month != fechaPrimerViajeDelMes.Month)
            {
                viajesEsteMes = 0;
                fechaPrimerViajeDelMes = tiempo.Now();
            }

            viajesEsteMes++;

            if (viajesEsteMes >= 30 && viajesEsteMes <= 79)
            {
                return TarifaBase * 0.80m; 
            }
            else if (viajesEsteMes == 80)
            {
                return TarifaBase * 0.75m; 
            }
            else
            {
                return TarifaBase; 
            }
        }

        public void CargarSaldo(decimal monto)
        {
            if (!EsCargaValida(monto))
            {
                throw new ArgumentException("Monto de carga no válido.");
            }

            decimal totalSaldo = Saldo + monto;

            if (totalSaldo > LimiteSaldo)
            {
                SaldoPendiente = totalSaldo - LimiteSaldo;
                Saldo = LimiteSaldo;
            }
            else
            {
                Saldo += monto;
            }
        }

        public void DescontarSaldo(decimal monto)
        {
            if (Saldo - monto < SaldoNegativoMaximo)
            {
                throw new InvalidOperationException($"No se puede realizar la transacción. Saldo mínimo permitido: ${SaldoNegativoMaximo}");
            }

            Saldo -= monto;
            AcreditarSaldoPendiente(); 
        }

        private void AcreditarSaldoPendiente()
        {
            if (SaldoPendiente > 0)
            {
                decimal espacioDisponible = LimiteSaldo - Saldo;
                decimal montoAcreditado = Math.Min(espacioDisponible, SaldoPendiente);

                Saldo += montoAcreditado;
                SaldoPendiente -= montoAcreditado;
            }
        }

        private bool EsCargaValida(decimal monto)
        {
            decimal[] cargasAceptadas = { 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };
            foreach (var carga in cargasAceptadas)
            {
                if (monto == carga)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EsSaldoValido(decimal saldo)
        {
            return saldo <= LimiteSaldo;
        }

        public void AgregarBoletoAlHistorial(Boleto boleto)
        {
            historialBoletos.Add(boleto);
        }

        public void VerHistorialBoletos()
        {
            if (historialBoletos.Count == 0)
            {
                Console.WriteLine("No hay boletos en el historial.");
                return;
            }

            Console.WriteLine("Historial de boletos:");
            foreach (var boleto in historialBoletos)
            {
                Console.WriteLine($"Monto: ${boleto.Monto}");
            }
        }

        protected bool EsHorarioPermitido(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();
            
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday)
            {
                if (ahora.Hour >= 6 && ahora.Hour < 22)
                {
                    return true;
                }
            }
            return false;
        }
    }

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
