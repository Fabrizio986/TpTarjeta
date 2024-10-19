using System;
using Xunit;

namespace TransporteUrbano.Tests
{
    public class ColectivoTests
    {
        [Fact]
        public void PagarBoleto_ConSaldo_DescuentaSaldo()
        {
            // Arrange
            decimal saldoInicial = 1000m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa();

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.Equal(saldoInicial - tarifa, tarjeta.Saldo);
        }

        [Fact]
        public void PagarBoleto_SinSaldo_NoDescuentaSaldo()
        {
            // Arrange
            decimal saldoInicial = 500m; // menos que la tarifa
            Tarjeta tarjeta = new Tarjeta(saldoInicial);
            Colectivo colectivo = new Colectivo();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta));
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
            Boleto boleto = colectivo.PagarCon(tarjeta);
            decimal tarifaEsperada = 940 / 2;

            // Assert
            Assert.Equal(tarifaEsperada, boleto.Monto);
            Assert.Equal(saldoInicial - tarifaEsperada, tarjeta.Saldo);
        }


        [Fact]
        public void PagarBoleto_ConTarjetaNormal_DescuentaTarifaCompleta()
        {
            // Arrange
            decimal saldoInicial = 1000m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa(); 

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta); // Asegúrate de que este método funcione correctamente

            // Assert
            decimal saldoEsperado = saldoInicial - tarifa; // Calcula el saldo esperado
            Assert.Equal(saldoEsperado, tarjeta.Saldo); // Verifica que el saldo sea el esperado
        }

        [Fact]
        public void PagarBoleto_MedioBoleto_ViajeConTiempoLimite()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial);
            Colectivo colectivo = new Colectivo();

            // Act
            colectivo.PagarCon(tarjeta); // Primer viaje
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta));

            // Assert
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
            Assert.Equal(36000m, tarjeta.Saldo); // Verifica que el saldo no supere el límite
            Assert.Equal(2000m, tarjeta.SaldoPendiente); // Verifica que el excedente se haya almacenado como saldo pendiente
        }


    }
}

