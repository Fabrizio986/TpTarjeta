using System;
using NUnit.Framework;
using ManejoDeTiempos;


namespace TransporteUrbano.Tests
{
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
        public void Tarjeta_CargaSaldo_ActualizaSaldo_Normal()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta(1000m, tiempoFalso);
        
            // Act
            tarjeta.CargarSaldo(2000m);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(3000m));
        }

        [Test]
        public void Tarjeta_CargaSaldo_ActualizaSaldo_BoletoEducativo()
        {
            // Arrange
            Tarjeta tarjeta = new TarjetaBoletoEducativo(1000m, tiempoFalso);
        
            // Act
            tarjeta.CargarSaldo(2000m);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(3000m));
        }

        [Test]
        public void Tarjeta_CargaSaldo_ActualizaSaldo_MedioBoleto()
        {
            // Arrange
            Tarjeta tarjeta = new TarjetaMedioBoleto(1000m, tiempoFalso);
        
            // Act
            tarjeta.CargarSaldo(2000m);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(3000m));
        }

    }

}

