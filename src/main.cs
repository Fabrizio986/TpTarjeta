using System;
using ManejoDeTiempos;

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

            Tiempo tiempoActual = new Tiempo();

            Tarjeta tarjeta;

            switch (tipoTarjeta)
            {
                case "2":
                    tarjeta = new TarjetaMedioBoleto(saldoInicial, tiempoActual);
                    break;
                case "3":
                    tarjeta = new TarjetaJubilado(saldoInicial, tiempoActual);
                    break;
                case "4":
                    tarjeta = new TarjetaBoletoEducativo(saldoInicial, tiempoActual);
                    break;
                default:
                    tarjeta = new Tarjeta(saldoInicial, tiempoActual);
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
                Console.WriteLine("4. Pagar boleto interurbano");
                Console.WriteLine("5. Ver historial de boletos");
                Console.WriteLine("6. Salir");
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
                        PagarBoleto(tarjeta, colectivo, tiempoActual); // Pasar tiempoActual
                        break;

                    case "4":
                        PagarBoletoInterurbano(tarjeta, colectivo, tiempoActual); // Pasar tiempoActual
                        break;

                    case "5":
                        tarjeta.VerHistorialBoletos();
                        break;

                    case "6":
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

        static void PagarBoleto(Tarjeta tarjeta, Colectivo colectivo, Tiempo tiempoActual) // Recibir tiempoActual
        {
            try
            {
                if (tarjeta is TarjetaBoletoEducativo tarjetaEducativa)
                {
                    if (tarjetaEducativa.PuedeViajar(tiempoActual))
                    {
                        Boleto boleto = colectivo.PagarCon(tarjeta, tiempoActual);
                        tarjetaEducativa.RegistrarViaje(tiempoActual);  
                        Console.WriteLine("Pago realizado:");
                        boleto.MostrarDetalles();
                    }
                    else
                    {
                        Console.WriteLine("No se puede realizar el viaje gratuito en este momento. Han pasado menos de 5 minutos desde el último viaje.");
                    }
                }
                else
                {
                    Boleto boleto = colectivo.PagarCon(tarjeta, tiempoActual);
                    Console.WriteLine("Pago realizado:");
                    boleto.MostrarDetalles();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al pagar el boleto: {ex.Message}");
            }
        }

        static void PagarBoletoInterurbano(Tarjeta tarjeta, Colectivo colectivo, Tiempo tiempoActual) // Recibir tiempoActual
        {
            try
            {
                Boleto boleto = colectivo.PagarCon(tarjeta, tiempoActual, true);
                Console.WriteLine("Pago realizado para boleto interurbano:");
                boleto.MostrarDetalles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al pagar el boleto interurbano: {ex.Message}");
            }
        }
    }
}
