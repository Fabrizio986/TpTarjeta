using System;
using System.Collections.Generic;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal LimiteSaldo = 9900m; 
        private const decimal SaldoNegativoMaximo = -480m; 
        private List<Boleto> historialBoletos = new List<Boleto>();

        public decimal Saldo { get; protected set; }

        public Tarjeta(decimal saldoInicial)
        {
            if (!EsSaldoValido(saldoInicial))
            {
                throw new ArgumentException("Saldo inicial no válido.");
            }

            Saldo = saldoInicial;
        }

        public virtual decimal ObtenerTarifa()
        {
            return 940m; 
        }

        public void CargarSaldo(decimal monto)
        {
            if (!EsCargaValida(monto))
            {
                throw new ArgumentException("Monto de carga no válido.");
            }

            if (Saldo + monto > LimiteSaldo)
            {
                throw new InvalidOperationException("Carga excede el límite de saldo.");
            }

            Saldo += monto;
        }

        public void DescontarSaldo(decimal monto)
        {
            if (Saldo - monto < SaldoNegativoMaximo)
            {
                throw new InvalidOperationException($"No se puede realizar la transacción. Saldo mínimo permitido: ${SaldoNegativoMaximo}");
            }

            Saldo -= monto;
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
    }

    public class TarjetaMedioBoleto : Tarjeta
    {
        public TarjetaMedioBoleto(decimal saldoInicial) : base(saldoInicial) { }

        public override decimal ObtenerTarifa()
        {
            return base.ObtenerTarifa() / 2;
        }
    }

    public class TarjetaJubilado : Tarjeta
    {
        public TarjetaJubilado(decimal saldoInicial) : base(saldoInicial) { }

        public override decimal ObtenerTarifa()
        {
            return 0m;
        }
    }
}
