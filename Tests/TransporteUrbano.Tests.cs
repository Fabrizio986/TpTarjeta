using System;
using Xunit;
using ManejoDeTiempos;

namespace TransporteUrbano.Tests
{
    public class ColectivoTests
    {
        public TiempoFalso tiempoFalso;

        public ColectivoTests()
        {
            tiempoFalso = new TiempoFalso();
        }

        [Fact]
        public void TestAgregarDias()
        {
            // Arrange
            var tiempoFalso = new TiempoFalso();
        
            // Act
            tiempoFalso.AgregarDias(7);
            var fechaActual = tiempoFalso.Now();

            // Assert
            Assert.Equal(new DateTime(2024, 10, 21), fechaActual);
        }

        [Fact]
        public void PagarBoleto_ConSaldo_DescuentaSaldo()
        {
            // Arrange
            decimal saldoInicial = 1300m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa();

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            Assert.Equal(saldoInicial - tarifa, tarjeta.Saldo);
        }

        [Fact]
        public void PagarBoleto_SinSaldo_NoDescuentaSaldo()
        {
            // Arrange
            decimal saldoInicial = 600m; // menos que la tarifa
            Tarjeta tarjeta = new Tarjeta(saldoInicial);
            Colectivo colectivo = new Colectivo();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta, tiempoFalso));
            Assert.Equal("Saldo insuficiente para realizar el pago.", exception.Message);
            Assert.Equal(saldoInicial, tarjeta.Saldo); // Saldo no debería cambiar
        }

        [Fact]
        public void DescontarSaldo_NoPermiteSaldoNegativo()
        {
            // Arrange
            decimal saldoInicial = 0m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(1000m));
            Assert.Equal("No se puede realizar la transacción. Saldo mínimo permitido: $-480", exception.Message);
        }

        [Fact]
        public void PagarBoleto_MedioBoleto_MontoEsMitad()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial);
            Colectivo colectivo = new Colectivo();

            // Act
            tiempoFalso.AgregarMinutos(60 * 15);
            Boleto primerBoleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            tiempoFalso.AgregarMinutos(15);
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            decimal tarifaEsperada = 1200 / 2;
            Assert.Equal(tarifaEsperada, boleto.Monto);
            Assert.Equal(saldoInicial - tarifaEsperada, tarjeta.Saldo);
        }

        [Fact]
        public void PagarBoleto_ConTarjetaNormal_DescuentaTarifaCompleta()
        {
            // Arrange
            decimal saldoInicial = 1300m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa();

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            decimal saldoEsperado = saldoInicial - tarifa;
            Assert.Equal(saldoEsperado, tarjeta.Saldo);
        }

        [Fact]
        public void PagarBoleto_MedioBoleto_ViajeConTiempoLimite()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial);
            Colectivo colectivo = new Colectivo();

            // Act
            tiempoFalso.AgregarMinutos(15 * 60);
            colectivo.PagarCon(tarjeta, tiempoFalso); // Primer viaje
            tiempoFalso.AgregarMinutos(3); // Esperar 4 minutos
            colectivo.PagarCon(tarjeta, tiempoFalso); // Segundo viaje

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta, tiempoFalso));
            Assert.Equal("No se puede realizar otro viaje antes de 5 minutos o se ha alcanzado el límite de viajes para el día.", exception.Message);
        }

        [Fact]
        public void CargarSaldo_CuandoExcedeLimite_CreditaHastaLimiteYGuardaExcedente()
        {
            // Arrange
            decimal saldoInicial = 30000m; // Saldo inicial
            decimal cargaExcesiva = 8000m; // Carga que excede el límite
            Tarjeta tarjeta = new Tarjeta(saldoInicial);

            // Act
            tarjeta.CargarSaldo(cargaExcesiva);

            // Assert
            Assert.Equal(36000m, tarjeta.Saldo);
            Assert.Equal(2000m, tarjeta.SaldoPendiente);
        }

        [Fact]
        public void ObtenerTarifa_Viaje31_DebeAplicarDescuento20Porciento()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m);

            for (int i = 0; i < 30; i++)
            {
                tarjeta.ObtenerTarifa();
            }

            // Act
            decimal tarifaViaje31 = tarjeta.ObtenerTarifa();

            // Assert
            Assert.Equal(1200m * 0.80m, tarifaViaje31); // 20% de descuento
        }

        [Fact]
        public void ObtenerTarifa_Viaje80_DebeAplicarDescuento25Porciento()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m);

            // Simular 79 viajes en el mes actual
            for (int i = 0; i < 79; i++)
            {
                tarjeta.ObtenerTarifa();
            }

            // Act
            decimal tarifaViaje80 = tarjeta.ObtenerTarifa();

            // Assert
            Assert.Equal(1200m * 0.75m, tarifaViaje80);
        }

        [Fact]
        public void ObtenerTarifa_NuevoMes_DebeReiniciarConteoDeViajes()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m);

            for (int i = 0; i < 30; i++)
            {
                tarjeta.ObtenerTarifa();
            }

            // Simular cambio de mes
            tarjeta = new Tarjeta(10000m);

            // Act
            decimal tarifaPrimerViajeNuevoMes = tarjeta.ObtenerTarifa();

            // Assert
            Assert.Equal(1200m, tarifaPrimerViajeNuevoMes);
        }
    }

    public class TarjetaFranquiciaTests
    {
        public TiempoFalso tiempo;

        public TarjetaFranquiciaTests()
        {
            tiempo = new TiempoFalso();
        }

        [Fact]
        public void TarjetaMedioBoleto_NoPuedeViajarFueraDeHorario()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m);

            tiempo.AgregarMinutos(60 * 3); // 3:00 AM no se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempo);
        
            // Assert
            Assert.False(puedeViajar);
        }

        [Fact]
        public void TarjetaMedioBoleto_PuedeViajarEnHorarioPermitido()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m);

            tiempo.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempo);
        
            // Assert
            Assert.True(puedeViajar);
        }

        [Fact]
        public void TarjetaBoletoEducativo_NoPuedeViajarEnFinde()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m);

            tiempo.AgregarDias(6); // Sábado
            tiempo.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempo);
        
            // Assert
            Assert.False(puedeViajar);

        }

        [Fact]
        public void TarjetaBoletoEducativo_PuedeViajarEntreSemana()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m);

            tiempo.AgregarDias(2); // Miércoles
            tiempo.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempo);
        
            // Assert
            Assert.True(puedeViajar);

        }


    }


    public class TarjetaTests
    {
        [Fact]
        public void TestCargaDeSaldo()
        {
            // Arrange
            decimal saldoInicial = 1000m;
            decimal carga = 2000m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial);

            // Act
            tarjeta.CargarSaldo(carga);

            // Assert
            Assert.Equal(saldoInicial + carga, tarjeta.Saldo);
        }
    }
}
