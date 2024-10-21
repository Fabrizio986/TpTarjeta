using System;
using System.Collections.Generic;

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

        public Tarjeta(decimal saldoInicial)
        {
            if (!EsSaldoValido(saldoInicial))
            {
                throw new ArgumentException("Saldo inicial no válido.");
            }

            Saldo = saldoInicial;
            SaldoPendiente = 0m;
            viajesEsteMes = 0;
            fechaPrimerViajeDelMes = DateTime.Now; 
        }

        public virtual decimal ObtenerTarifa()
        {
            if (DateTime.Now.Month != fechaPrimerViajeDelMes.Month)
            {
                viajesEsteMes = 0;
                fechaPrimerViajeDelMes = DateTime.Now;
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

        protected bool EsHorarioPermitido()
        {
            DateTime now = DateTime.Now;
            if (now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday)
            {
                if (now.Hour >= 6 && now.Hour < 22)
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

        public TarjetaMedioBoleto(decimal saldoInicial) : base(saldoInicial) { }

        public bool PuedeViajar()
        {
            if (!EsHorarioPermitido())
            {
                Console.WriteLine("Viaje no permitido fuera de la franja horaria de 6 a 22, de lunes a viernes.");
                return false;
            }

            if (ultimoViaje == null)
            {
                return true;
            }

            if ((DateTime.Now - ultimoViaje.Value).TotalMinutes < 5)
            {
                return false;
            }

            return viajesRealizados < 2;
        }


        public void RegistrarViaje()
        {
            if (ultimoViaje == null || (DateTime.Now - ultimoViaje.Value).TotalMinutes >= 5)
            {
                ultimoViaje = DateTime.Now;
                viajesRealizados++;
            }
        }

        public override decimal ObtenerTarifa()
        {
            return TarifaBase / 2;
        }
    }

    public class TarjetaBoletoEducativo : Tarjeta
    {
        private int viajesGratisRealizados;
        private DateTime? ultimoViaje; 

        public TarjetaBoletoEducativo(decimal saldoInicial) : base(saldoInicial)
        {
            viajesGratisRealizados = 0;
            ultimoViaje = null;
        }

        public bool PuedeViajar()
        {
            if (!EsHorarioPermitido())
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
                var tiempoTranscurrido = DateTime.Now - ultimoViaje.Value;
                if (tiempoTranscurrido.TotalMinutes < 5)
                {
                    Console.WriteLine("Debe esperar 5 minutos entre los viajes gratuitos.");
                    return false;
                }
            }

            return true;
        }

        public void RegistrarViaje()
        {
            if (viajesGratisRealizados < 2)
            {
                viajesGratisRealizados++;
                ultimoViaje = DateTime.Now;
            }
            else
            {
                Console.WriteLine("Ya ha alcanzado el límite de viajes gratuitos por hoy.");
            }
        }

        public override decimal ObtenerTarifa()
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
        public TarjetaJubilado(decimal saldoInicial) : base(saldoInicial) { }

        public override decimal ObtenerTarifa()
        {
            return 0m;
        }

        public bool PuedeViajar()
        {
            if (!EsHorarioPermitido())
            {
                Console.WriteLine("Viaje no permitido fuera de la franja horaria de 6 a 22, de lunes a viernes.");
                return false;
            }

            return true;
        }
    }
}

