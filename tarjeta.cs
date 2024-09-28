using System;
using System.Collections.Generic;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal LimiteSaldo = 9900m;
        private const decimal MaximoSaldoNegativo = -480m;
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

            if (Saldo < 0)
            {
                decimal deuda = -Saldo;
                if (monto >= deuda)
                {
                    monto -= deuda;
                    Saldo = 0;
                }
                else
                {
                    Saldo += monto;
                    monto = 0; 
                }
            }

            Saldo += monto;
        }

        public void DescontarSaldo(decimal monto)
        {
            if (Saldo - monto < MaximoSaldoNegativo)
            {
                throw new InvalidOperationException("No se puede realizar la transacción. El saldo no puede ser menor a -$480.");
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
            return saldo >= MaximoSaldoNegativo && saldo <= LimiteSaldo;
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
}

