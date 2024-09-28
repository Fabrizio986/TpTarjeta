using System;

namespace TransporteUrbano
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese el saldo inicial de la tarjeta:");
            decimal saldoInicial = Convert.ToDecimal(Console.ReadLine());

            Tarjeta tarjeta = new Tarjeta(saldoInicial);

            Colectivo colectivo = new Colectivo();

            bool continuar = true;

            while (continuar)
            {
                Console.WriteLine("\nOpciones:");
                Console.WriteLine("1. Ver saldo");
                Console.WriteLine("2. Cargar saldo");
                Console.WriteLine("3. Pagar boleto");
                Console.WriteLine("4. Ver historial de boletos");
                Console.WriteLine("5. Salir");
                Console.Write("Elige una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        MostrarSaldo(tarjeta);
                        break;

                    case "2":
                        CargarSaldo(tarjeta);
                        break;

                    case "3":
                        PagarBoleto(tarjeta, colectivo);
                        break;

                    case "4":
                        tarjeta.VerHistorialBoletos();
                        break;

                    case "5":
                        continuar = false;
                        Console.WriteLine("Saliendo del programa...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intenta de nuevo.");
                        break;
                }
            }
        }

        static void MostrarSaldo(Tarjeta tarjeta)
        {
            if (tarjeta.Saldo < 0)
            {
                Console.WriteLine($"Saldo actual de la tarjeta: ${tarjeta.Saldo} (deuda por viaje plus)");
            }
            else
            {
                Console.WriteLine($"Saldo actual de la tarjeta: ${tarjeta.Saldo}");
            }
        }

        static void CargarSaldo(Tarjeta tarjeta)
        {
            Console.Write("Ingresa el monto a cargar (opciones: 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000): ");
            decimal montoCarga = Convert.ToDecimal(Console.ReadLine());

            try
            {
                tarjeta.CargarSaldo(montoCarga);
                Console.WriteLine($"Saldo después de cargar: ${tarjeta.Saldo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar saldo: {ex.Message}");
            }
        }

        static void PagarBoleto(Tarjeta tarjeta, Colectivo colectivo)
        {
            try
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Console.WriteLine($"Pago realizado. Monto del boleto: ${boleto.Monto}");
                Console.WriteLine($"Saldo restante en la tarjeta: ${tarjeta.Saldo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al pagar el boleto: {ex.Message}");
            }
        }
    }
}

