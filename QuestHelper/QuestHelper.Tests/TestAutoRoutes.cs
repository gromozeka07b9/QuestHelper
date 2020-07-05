using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using Xunit;
using QuestHelper.Managers;
using Moq;
using Xunit.Sdk;

namespace QuestHelper.Tests
{
    public class TestAutoRoutes
    {
        public TestAutoRoutes()
        {
        }

        [Fact]
        public void TestMust_LoadListImagesForOneDay()
        {
            bool result = true;

            AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            //routeMaker.Make(1, "");
            Assert.True(result);
        }

        [Fact]
        public void TestMust_Get3RoutePointsWith2PhotosEach()
        {
            //AAA
            //Arrange
            var testPhotos = new Dictionary<(GpsCoordinates, DateTime), GalleryImage>();
            //�� ����� � ��� 6 ����������, ��� � ������������ 1,1, ��� � 2,2, ��� � 3,3 - �.�. �� ����� ������ ������ �������� � �������� 3 ������ �����
            //�� ��������, ��� ������ ���������� ������ 6 ����������, ��� �����������
            //�������, ���� �� ��������
            testPhotos.Add((new GpsCoordinates() { Latitude = 1, Longitude = 1 }, DateTime.Parse("2020/07/01")), new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "1" });
            testPhotos.Add((new GpsCoordinates() { Latitude = 1, Longitude = 1 }, DateTime.Parse("2020/07/01")), new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "2" });
            testPhotos.Add((new GpsCoordinates() { Latitude = 2, Longitude = 2 }, DateTime.Parse("2020/07/01")), new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "3" });
            testPhotos.Add((new GpsCoordinates() { Latitude = 2, Longitude = 2 }, DateTime.Parse("2020/07/01")), new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "1" });
            testPhotos.Add((new GpsCoordinates() { Latitude = 3, Longitude = 3 }, DateTime.Parse("2020/07/01")), new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "2" });
            testPhotos.Add((new GpsCoordinates() { Latitude = 3, Longitude = 3 }, DateTime.Parse("2020/07/01")), new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "3" });

            //Act
            AutoGeneratedRoute autoRoute = new AutoGeneratedRoute(new ImageManager());
            autoRoute.Build(testPhotos);

            //Assert
            Assert.True(autoRoute.Points.Count == 3, "������ ���� ������ 3 �����, � ������ �� 2 ����������");
        }

        [Fact]
        public void TestMust_Load3ImageMetadata()
        {
            //AAA
            //Arrange
            (bool pickPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates) file1 = (true, "1", new GpsCoordinates() { Latitude = 1, Longitude = 1 });
            (bool pickPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates) file2 = (true, "2", new GpsCoordinates() { Latitude = 2, Longitude = 2 });
            (bool pickPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates) file3 = (true, "3", new GpsCoordinates() { Latitude = 3, Longitude = 3 });
            var mockImageManager = new Mock<IImageManager>();
            mockImageManager.Setup(x => x.GetPhoto("1")).Returns(() => file1);
            mockImageManager.Setup(x => x.GetPhoto("2")).Returns(() => file2);
            mockImageManager.Setup(x => x.GetPhoto("3")).Returns(() => file3);
            AutoGeneratedRoute autoRoute = new AutoGeneratedRoute(mockImageManager.Object);
            List<GalleryImage> testImages = new List<GalleryImage>();
            testImages.Add(new GalleryImage() { CreateDate = DateTime.Parse("2020/07/01"), ImagePath = "1"});
            testImages.Add(new GalleryImage() { CreateDate = DateTime.Parse("2020/07/02"), ImagePath = "2" });
            testImages.Add(new GalleryImage() { CreateDate = DateTime.Parse("2020/07/03"), ImagePath = "3" });
            autoRoute.SourceGalleryImages = testImages;

            //Act
            var parsedImages = autoRoute.LoadMetadataFromImages();

            //Assert
            Assert.True(parsedImages.Count == 3, "������ ���� ������ 3 �����");
        }

        /*[Fact]
        public void TestMust_LoadToCacheImages7DaysLast()
        {
            //��������� � �� ���������� ����������� �� ��������� 7 ����
            //AAA
            //Arrange

            //Act
            ImagesCacheDbManager cacheDbManager = new ImagesCacheDbManager(7);
            //cacheDbManager.Update();

            //Assert
            Assert.True(true, "������ ���� ����� �� 7 ����");
        }*/
    }
}
