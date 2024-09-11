using System;

namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal LimiteSaldo = 9900m;

        public decimal Saldo { get; private set; }

        public Tarjeta(decimal saldoInicial)
        {
            if (!EsSaldoValido(saldoInicial))
            {
                throw new ArgumentException("Saldo inicial no válido.");
            }

            Saldo = saldoInicial;
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
            if (Saldo < monto)
            {
                throw new InvalidOperationException("Saldo insuficiente.");
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
            return saldo >= 0 && saldo <= LimiteSaldo;
        }
    }
}
