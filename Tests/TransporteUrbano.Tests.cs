using System;
using NUnit.Framework;
using ManejoDeTiempos;


namespace TransporteUrbano.Tests
{
    [TestFixture]
    public class ColectivoTests
    {
        public TiempoFalso tiempoFalso;

        public ColectivoTests()
        {
            tiempoFalso = new TiempoFalso();
        }

        [Test]
        public void PagarBoleto_ConSaldo_DescuentaSaldo()
        {
            // Arrange
            decimal saldoInicial = 1300m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa();

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            Assert.AreEqual(saldoInicial - tarifa, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_SinSaldo_NoDescuentaSaldo()
        {
            // Arrange
            decimal saldoInicial = 600m; // menos que la tarifa
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta, tiempoFalso));
            Assert.AreEqual("Saldo insuficiente para realizar el pago.", exception.Message);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo); // Saldo no debería cambiar
        }

        [Test]
        public void DescontarSaldo_NoPermiteSaldoNegativo()
        {
            // Arrange
            decimal saldoInicial = 0m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(1000m));
            Assert.AreEqual("No se puede realizar la transacción. Saldo mínimo permitido: $-480", exception.Message);
        }

        [Test]
        public void PagarBoleto_MedioBoleto_MontoEsMitad()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();

            // Act
            tiempoFalso.AgregarMinutos(60 * 15);
            Boleto primerBoleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            tiempoFalso.AgregarMinutos(15);
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            decimal tarifaEsperada = 1200 / 2;
            Assert.AreEqual(tarifaEsperada, boleto.Monto);
            Assert.AreEqual(saldoInicial - tarifaEsperada, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_ConTarjetaNormal_DescuentaTarifaCompleta()
        {
            // Arrange
            decimal saldoInicial = 1300m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa();

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            decimal saldoEsperado = saldoInicial - tarifa;
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
        }

        [Test]
        public void PagarBoleto_MedioBoleto_ViajeConTiempoLimite()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();

            // Act
            tiempoFalso.AgregarMinutos(15 * 60);
            colectivo.PagarCon(tarjeta, tiempoFalso); // Primer viaje
            tiempoFalso.AgregarMinutos(3); // Esperar 4 minutos
            colectivo.PagarCon(tarjeta, tiempoFalso); // Segundo viaje

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta, tiempoFalso));
            Assert.AreEqual("No se puede realizar otro viaje antes de 5 minutos o se ha alcanzado el límite de viajes para el día.", exception.Message);
        }

        [Test]
        public void CargarSaldo_CuandoExcedeLimite_CreditaHastaLimiteYGuardaExcedente()
        {
            // Arrange
            decimal saldoInicial = 30000m; // Saldo inicial
            decimal cargaExcesiva = 8000m; // Carga que excede el límite
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);

            // Act
            tarjeta.CargarSaldo(cargaExcesiva);

            // Assert
            Assert.AreEqual(36000m, tarjeta.Saldo);
            Assert.AreEqual(2000m, tarjeta.SaldoPendiente);
        }

        [Test]
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
            Assert.AreEqual(1200m * 0.80m, tarifaViaje31); // 20% de descuento
        }

        [Test]
        public void ObtenerTarifa_Viaje80_DebeAplicarDescuento25Porciento()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m);

            for (int i = 0; i < 79; i++)
            {
                tarjeta.ObtenerTarifa();
            }

            // Act
            decimal tarifaViaje80 = tarjeta.ObtenerTarifa();

            // Assert
            Assert.AreEqual(1200m * 0.75m, tarifaViaje80);
        }

        [Test]
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
            Assert.AreEqual(1200m, tarifaPrimerViajeNuevoMes);
        }
    }

    [TestFixture]
    public class TarjetaFranquiciaTests
    {
        public TiempoFalso tiempoFalso;

        public TarjetaFranquiciaTests()
        {
            tiempoFalso = new TiempoFalso();
        }

        [Test]
        public void TarjetaMedioBoleto_NoPuedeViajarFueraDeHorario()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m, tiempoFalso);

            tiempoFalso.AgregarMinutos(60 * 3); // 3:00 AM no se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.False(puedeViajar);
        }

        [Test]
        public void TarjetaMedioBoleto_PuedeViajarEnHorarioPermitido()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m, tiempoFalso);

            tiempoFalso.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.True(puedeViajar);
        }

        [Test]
        public void TarjetaBoletoEducativo_NoPuedeViajarEnFinde()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m, tiempoFalso);

            tiempoFalso.AgregarDias(6); // Sábado
            tiempoFalso.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.False(puedeViajar);
        }

        [Test]
        public void TarjetaBoletoEducativo_PuedeViajarEntreSemana()
        {
            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m, tiempoFalso);

            tiempoFalso.AgregarDias(2); // Miércoles
            tiempoFalso.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.True(puedeViajar);
        }
    }

    [TestFixture]
    public class TarjetaTests
    {
        public TiempoFalso tiempoFalso;

        public TarjetaTests()
        {
            tiempoFalso = new TiempoFalso();
        }

        [Test]
        public void Tarjeta_CargaSaldo_ActualizaSaldo()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta(1000m, tiempoFalso);
        
            // Act
            tarjeta.CargarSaldo(500m);

            // Assert
            Assert.AreEqual(1500m, tarjeta.Saldo);
        }
    }
}
