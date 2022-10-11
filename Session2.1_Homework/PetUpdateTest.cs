using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Session2._1_Homework;

namespace Session2._1_homework
{
    [TestClass]
    public class Pet
    {
        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetsEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestMethod]
        public async Task UpdatePet()
        {

            #region Add new pet to the store
            // Create New Pet

            PetModel petData = new PetModel()
            {
                Id = 1123,
                Category = new Category()
                {
                    Id = 0,
                    Name = "Zkye"
                },
                Name = "CutePet",
                PhotoUrls = new string[]
                {
                    "pic.photo"
                },
                Status = "available",
                Tags = new Tag[]
                {
                    new Tag()
                    {
                       Id = 0,
                       Name = "Hypoallergenic, soft fur"
                    }
                }
            };

            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await httpClient.PostAsync(GetURL(PetsEndpoint), postRequest);
            #endregion

            #region Get name of created pet
            // Get Request
            var getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.Id}"));

            // Deserialize Content
            var listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var createdPetName = listPetData.Name;

            //Assert the new pet has been added
            Assert.AreEqual(petData.Name, createdPetName, "Pet name not matching");
            #endregion

            #region Update Pet Data
            petData = new PetModel()
            {
                Id = 1124,
                Category = new Category()
                {
                    Id = 0,
                    Name = "Zola"
                },
                Name = "PetUpdated",
                PhotoUrls = new string[]
                {
                    "photo.pic"
                },
                Status = "sold",
                Tags = new Tag[]
                {
                    new Tag()
                    {
                       Id = 0,
                       Name = "Shih Tzu"
                    },
                    new Tag()
                    {
                       Id = 0,
                       Name = "Black Fur"
                    }
                }
            };

            // Serialize Content
            request = JsonConvert.SerializeObject(petData);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Put Request
            var httpResponse = await httpClient.PutAsync(GetURL($"{PetsEndpoint}"), postRequest);

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            #endregion

            #region Get Updated Data
            // Get Request
            getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.Id}"));

            // Deserialize Content
            listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var newCreatedPetName = listPetData.Name;

            // Add data to cleanup list
            cleanUpList.Add(listPetData);

            #endregion

            #region Assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 201.");
            Assert.AreEqual(petData.Name, newCreatedPetName, "Pet names are not matching.");

            #endregion
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetsEndpoint}/{data.Id}"));
            }
        }
    }
}
