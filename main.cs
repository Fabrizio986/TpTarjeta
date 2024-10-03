using System;

namespace TransporteUrbano
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese el saldo inicial de la tarjeta:");
            decimal saldoInicial = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine("Elija el tipo de tarjeta: 1. Regular, 2. Medio Boleto, 3. Jubilado, 4. Boleto Educativo");
            string tipoTarjeta = Console.ReadLine();

            Tarjeta tarjeta;

            switch (tipoTarjeta)
            {
                case "2":
                    tarjeta = new TarjetaMedioBoleto(saldoInicial);
                    break;
                case "3":
                    tarjeta = new TarjetaJubilado(saldoInicial);
                    break;
                case "4":
                    tarjeta = new TarjetaBoletoEducativo(saldoInicial);
                    break;
                default:
                    tarjeta = new Tarjeta(saldoInicial);
                    break;
            }

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
            Console.WriteLine($"Saldo actual de la tarjeta: ${tarjeta.Saldo}");
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
                if (tarjeta is TarjetaBoletoEducativo tarjetaEducativa)
                {
                    // Verificar si han pasado 5 minutos desde el último viaje
                    if (tarjetaEducativa.PuedeViajar())
                    {
                        Boleto boleto = colectivo.PagarCon(tarjeta);
                        tarjetaEducativa.RegistrarViaje();  // Registrar el viaje gratuito
                        Console.WriteLine("Pago realizado:");
                        boleto.MostrarDetalles();
                        Console.WriteLine($"Saldo restante en la tarjeta después del pago: ${tarjeta.Saldo}");
                    }
                    else
                    {
                        Console.WriteLine("No se puede realizar el viaje gratuito en este momento. Han pasado menos de 5 minutos desde el último viaje.");
                    }
                }
                else
                {
                    // Para las tarjetas regulares, medio boleto y jubilado
                    Boleto boleto = colectivo.PagarCon(tarjeta);
                    Console.WriteLine("Pago realizado:");
                    boleto.MostrarDetalles();
                    Console.WriteLine($"Saldo restante en la tarjeta después del pago: ${tarjeta.Saldo}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al pagar el boleto: {ex.Message}");
            }
        }
    }
}

