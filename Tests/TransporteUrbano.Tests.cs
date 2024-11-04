using System;
using NUnit.Framework;
using ManejoDeTiempos;


namespace TransporteUrbano.Tests
{
    [TestFixture]
    public class ColectivoTests
    {
        private TiempoFalso tiempoFalso;

        [SetUp]
        public void SetUp()
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
            decimal tarifa = tarjeta.ObtenerTarifa(tiempoFalso);

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            Assert.That(saldoInicial - tarifa, Is.EqualTo(tarjeta.Saldo));
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
            Assert.That(exception.Message, Is.EqualTo("Saldo insuficiente para realizar el pago."));
            Assert.That(saldoInicial, Is.EqualTo(tarjeta.Saldo)); // Saldo no debería cambiar
        }

        [Test]
        public void DescontarSaldo_NoPermiteSaldoNegativo()
        {
            // Arrange
            decimal saldoInicial = 0m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(1000m));
            Assert.That(exception.Message, Is.EqualTo("No se puede realizar la transacción. Saldo mínimo permitido: $-480"));
        }

        [Test]
        public void PagarBoleto_MedioBoleto_MontoEsMitad()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();

            // Act
            tiempoFalso.AgregarDias(1); // Mover al siguiente día
            tiempoFalso.AgregarMinutos(60 * 15); // Avanzar a las 3 PM

            // Pagar primer boleto
            Boleto primerBoleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Avanzar el tiempo para el segundo viaje
            tiempoFalso.AgregarMinutos(15); // Intentar pagar después de 15 minutos

            // Pagar el segundo boleto
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            decimal tarifaEsperada = 1200m / 2; // Tarifa esperada para medio boleto
            Assert.That(tarifaEsperada, Is.EqualTo(boleto.Monto));
            Assert.That(saldoInicial - (tarifaEsperada*2), Is.EqualTo(tarjeta.Saldo));
        }


        [Test]
        public void PagarBoleto_ConTarjetaNormal_DescuentaTarifaCompleta()
        {
            // Arrange
            decimal saldoInicial = 1300m;
            Tarjeta tarjeta = new Tarjeta(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();
            decimal tarifa = tarjeta.ObtenerTarifa(tiempoFalso);

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempoFalso);

            // Assert
            decimal saldoEsperado = saldoInicial - tarifa;
            Assert.That(saldoEsperado, Is.EqualTo(tarjeta.Saldo));
        }

        [Test]
        public void PagarBoleto_MedioBoleto_ViajeConTiempoLimite()
        {
            // Arrange
            decimal saldoInicial = 2000m;
            Tarjeta tarjeta = new TarjetaMedioBoleto(saldoInicial, tiempoFalso);
            Colectivo colectivo = new Colectivo();

            // Act
            tiempoFalso.AgregarMinutos(10 * 60); // Lunes 10 am
            colectivo.PagarCon(tarjeta, tiempoFalso); // Primer viaje

            // Esperar más de 5 minutos
            tiempoFalso.AgregarMinutos(6); // Esperar 6 minutos
            colectivo.PagarCon(tarjeta, tiempoFalso); // Segundo viaje

            // Esperar menos de 5 minutos para el tercer viaje
            tiempoFalso.AgregarMinutos(3); // Esperar 3 minutos

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta, tiempoFalso));
            Assert.That(exception.Message, Is.EqualTo("No se puede realizar otro viaje antes de 5 minutos o se ha alcanzado el límite de viajes para el día."));
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
            Assert.That(tarjeta.Saldo, Is.EqualTo(36000m));
            Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2000m));
        }

        [Test]
        public void ObtenerTarifa_Viaje31_DebeAplicarDescuento20Porciento()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m, tiempoFalso);

            for (int i = 0; i < 30; i++)
            {
                tarjeta.ObtenerTarifa(tiempoFalso);
            }

            // Act
            decimal tarifaViaje31 = tarjeta.ObtenerTarifa(tiempoFalso);

            // Assert
            Assert.That(tarifaViaje31, Is.EqualTo(1200m * 0.80m)); // 20% de descuento
        }

        [Test]
        public void ObtenerTarifa_Viaje80_DebeAplicarDescuento25Porciento()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m, tiempoFalso);

            for (int i = 0; i < 79; i++)
            {
                tarjeta.ObtenerTarifa(tiempoFalso);
            }

            // Act
            decimal tarifaViaje80 = tarjeta.ObtenerTarifa(tiempoFalso);

            // Assert
            Assert.That(tarifaViaje80, Is.EqualTo(1200m * 0.75m));
        }

        [Test]
        public void ObtenerTarifa_NuevoMes_DebeReiniciarConteoDeViajes()
        {
            // Arrange
            var tarjeta = new Tarjeta(30000m, tiempoFalso);

            for (int i = 0; i < 30; i++)
            {
                tarjeta.ObtenerTarifa(tiempoFalso);
            }

            // Simular cambio de mes
            tarjeta = new Tarjeta(10000m, tiempoFalso);

            // Act
            decimal tarifaPrimerViajeNuevoMes = tarjeta.ObtenerTarifa(tiempoFalso);

            // Assert
            Assert.That(tarifaPrimerViajeNuevoMes, Is.EqualTo(1200m));
        }
    }

    [TestFixture]
    public class TarjetaFranquiciaTests
    {
        private TiempoFalso tiempoFalso;

        [SetUp]
        public void SetUp()
        {
            tiempoFalso = new TiempoFalso();
        }

        [Test]
        public void TarjetaMedioBoleto_NoPuedeViajarFueraDeHorario()
        {
            // Arrange
            tiempoFalso.AgregarMinutos(60 * 2); // 2:00 AM
            var tarjeta = new TarjetaMedioBoleto(5000m, tiempoFalso);

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.False(puedeViajar);
        }

        [Test]
        public void TarjetaMedioBoleto_PuedeViajarEnHorarioPermitido()
        {
            tiempoFalso.AgregarMinutos(60 * 7); // 7:00 AM

            // Arrange
            var tarjeta = new TarjetaMedioBoleto(5000m, tiempoFalso);

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
        private TiempoFalso tiempoFalso;

        [SetUp]
        public void SetUp()
        {
            tiempoFalso = new TiempoFalso();
        }

        [Test]
        public void Tarjeta_CargaSaldo_ActualizaSaldo()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta(1000m, tiempoFalso);
        
            // Act
            tarjeta.CargarSaldo(2000m);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(3000m));
        }
    }
}

