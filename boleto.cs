using System;

namespace TransporteUrbano
{
    public class Boleto
    {
        public decimal Monto { get; private set; }
        public DateTime Fecha { get; private set; }
        public string TipoTarjeta { get; private set; }
        public string LineaColectivo { get; private set; }
        public decimal SaldoRestante { get; private set; }
        public int IdTarjeta { get; private set; }
        public decimal SaldoAbonado { get; private set; }

        public Boleto(decimal monto, string tipoTarjeta, string lineaColectivo, decimal saldoRestante, int idTarjeta)
        {
            Monto = monto;
            Fecha = DateTime.Now.AddHours(-3);
            TipoTarjeta = tipoTarjeta;
            LineaColectivo = lineaColectivo;
            SaldoRestante = saldoRestante;
            IdTarjeta = idTarjeta;
            SaldoAbonado = SaldoRestante < 0 ? Math.Abs(SaldoRestante) : 0;
        }

        public void MostrarDetalles()
        {
            Console.WriteLine($"Monto: ${Monto}");
            Console.WriteLine($"Fecha: {Fecha}");
            Console.WriteLine($"Tipo de Tarjeta: {TipoTarjeta}");
            Console.WriteLine($"Línea de Colectivo: {LineaColectivo}");
            Console.WriteLine($"Saldo Restante: ${SaldoRestante}");
            Console.WriteLine($"Saldo Abonado: ${SaldoAbonado}");
        }
    }
}
