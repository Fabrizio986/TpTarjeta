using System;
using NUnit.Framework;
using ManejoDeTiempos;


namespace TransporteUrbano.Tests
{
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
        
        [Test]
        public void TarjetaJubilados_NoPuedeViajarFinDeSemana()
        {
            // Arrange
            var tarjeta = new TarjetaJubilado(5000m, tiempoFalso);

            tiempoFalso.AgregarDias(6); // Sábado
            tiempoFalso.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.False(puedeViajar);
        }


        [Test]
        public void TarjetaJubilados_PuedeViajarEntreSemana()
        {
            // Arrange
            var tarjeta = new TarjetaJubilado(5000m, tiempoFalso);

            tiempoFalso.AgregarDias(3); // Miercoles
            tiempoFalso.AgregarMinutos(60 * 7); // 7:00 AM se puede

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.True(puedeViajar);
        }

        [Test]
        public void TarjetaJubilado_NoPuedeViajarFueraDeHorario()
        {
            // Arrange
            tiempoFalso.AgregarMinutos(60 * 2); // 2:00 AM
            var tarjeta = new TarjetaJubilado(5000m, tiempoFalso);

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.False(puedeViajar);
        }

        [Test]
        public void TarjetaJubilado_PuedeViajarEnHorario()
        {
            // Arrange
            tiempoFalso.AgregarMinutos(60 * 8); // 8:00 AM
            var tarjeta = new TarjetaJubilado(5000m, tiempoFalso);

            // Act
            bool puedeViajar = tarjeta.PuedeViajar(tiempoFalso);
        
            // Assert
            Assert.True(puedeViajar);
        }


    }

}

