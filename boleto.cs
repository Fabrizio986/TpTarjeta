using System;

namespace TransporteUrbano
{
    public class Boleto
    {
        public decimal Monto { get; }
        public string Tipo { get; }
        public string Linea { get; }
        public decimal SaldoRestante { get; }
        public int ViajeId { get; }

        public Boleto(decimal monto, string tipo, string linea, decimal saldoRestante, int viajeId)
        {
            Monto = monto;
            Tipo = tipo;
            Linea = linea;
            SaldoRestante = saldoRestante;
            ViajeId = viajeId;
        }

        public void MostrarDetalles()
        {
            Console.WriteLine($"Boleto {ViajeId} - Tipo: {Tipo} - Línea: {Linea} - Monto: ${Monto} - Saldo restante: ${SaldoRestante}");
        }
    }
}

