﻿using System;
using Chargify;
using System.Linq;
using RestSharp.Serializers;
using System.Xml.Linq;
#if NUNIT
using NUnit.Framework;
#else
using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using SetUp = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
#endif

namespace ChargifyTests
{
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public void Product_Serialize()
        {
            // Arrange
            Guid randomName = Guid.NewGuid();

            Product newProduct = new Product()
            {
                name = randomName.ToString(),
                price_in_cents = 100,
                interval_unit = IntervalUnit.month,
                interval = 1
            };

            // Act
            string msg = string.Empty;
            var serializer = new DotNetXmlSerializer();
            msg = serializer.Serialize(newProduct);

            // Assert
            XDocument doc = XDocument.Parse(msg);
            XElement nameElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "name");
            XElement priceElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "price_in_cents");
            XElement intervalUnitElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "interval_unit");
            XElement intervalElement = doc.Root.Elements().FirstOrDefault(e => e.Name == "interval");

            Assert.IsTrue(doc.Root.Elements().Count() == 4);
            Assert.IsFalse(string.IsNullOrEmpty(nameElement.Value));
            Assert.IsTrue(randomName.ToString() == nameElement.Value);
            Assert.IsNotNull(priceElement);
            Assert.IsTrue(int.Parse(priceElement.Value) > int.MinValue);
            Assert.AreEqual((int)newProduct.price_in_cents, int.Parse(priceElement.Value));
            Assert.IsFalse(string.IsNullOrEmpty(intervalUnitElement.Value));
            Assert.IsTrue((int)newProduct.interval_unit == (int)Enum.Parse(typeof(IntervalUnit), intervalUnitElement.Value));
        }

        [Test]
        public void Product_All()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.All();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public async Task Product_All_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = await chargify.Products.AllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public void Product_All_WithFamilyId()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.All(490966);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IEnumerable<Product>));
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public async Task Product_All_WithFamilyId_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = await chargify.Products.AllAsync(490966);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(IEnumerable<Product>));
            Assert.IsTrue(result.Count() > 0);
        }

        [Test]
        public void Product_Single_WithId()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.Single(3691421);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Product));
        }

        [Test]
        public async Task Product_Single_WithId_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = await chargify.Products.SingleAsync(3691421);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Product));
        }

        [Test]
        public void Product_Single_WithHandle()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = chargify.Products.Single("basic");

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Product));
        }

        [Test]
        public async Task Product_Single_WithHandle_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();

            // Act
            var result = await chargify.Products.SingleAsync("basic");

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Product));
        }

        [Test]
        public void Product_Create()
        {
            // Arrange
            var chargify = new ChargifyClient();
            var newProduct = GetNewProduct();

            // Act
            var result = chargify.Products.Create(490966, newProduct);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Product));
            Assert.IsTrue(newProduct.name == result.name);
            Assert.IsTrue(newProduct.price_in_cents == result.price_in_cents);
            Assert.IsTrue(newProduct.interval_unit == result.interval_unit);
            Assert.IsTrue(newProduct.interval == result.interval);
        }

        [Test]
        public async Task Product_Create_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();
            var newProduct = GetNewProduct();

            // Act
            var result = await chargify.Products.CreateAsync(490966, newProduct);

            // Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(Product));
            Assert.IsTrue(newProduct.name == result.name);
            Assert.IsTrue(newProduct.price_in_cents == result.price_in_cents);
            Assert.IsTrue(newProduct.interval_unit == result.interval_unit);
            Assert.IsTrue(newProduct.interval == result.interval);
        }

        [Test, ExpectedException(typeof(NotImplementedException))]
        public void Product_Archive()
        {
            // Arrange
            var chargify = new ChargifyClient();
            var newProduct = GetNewProduct();
            var createdProduct = chargify.Products.Create(490966, newProduct);

            // Act
            chargify.Products.Archive(createdProduct);
            var foundProduct = chargify.Products.Single(createdProduct.id);

            // Assert
            Assert.IsNotNull(foundProduct);
            Assert.IsTrue(foundProduct.archived_at.HasValue);
            Assert.IsTrue(foundProduct.archived_at.Value != DateTime.MinValue);
        }

        [Test, ExpectedException(typeof(NotImplementedException))]
        public async Task Product_Archive_Async()
        {
            // Arrange
            var chargify = new ChargifyClient();
            var newProduct = GetNewProduct();
            var createdProduct = await chargify.Products.CreateAsync(490966, newProduct);

            // Act
            await chargify.Products.ArchiveAsync(createdProduct);
            var foundProduct = await chargify.Products.SingleAsync(createdProduct.id);

            // Assert
            Assert.IsNotNull(foundProduct);
            Assert.IsTrue(foundProduct.archived_at.HasValue);
            Assert.IsTrue(foundProduct.archived_at.Value != DateTime.MinValue);
        }

        public Product GetNewProduct()
        {
            Guid randomName = Guid.NewGuid();
            Product newProduct = new Product()
            {
                name = randomName.ToString(),
                price_in_cents = 100,
                interval_unit = IntervalUnit.month,
                interval = 1
            };
            return newProduct;
        }
    }
}
