
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class AuthControllerTests
        : IClassFixture<RaceDayWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(
            RaceDayWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        // TEST 1: Register a participant successfully
        [Fact]
        public async Task Register_ValidParticipant_ReturnsSuccess()
        {
            var email = $"participant_{Guid.NewGuid():N}@example.com";

            var request = new
            {
                Name = "Test Participant",
                Email = email,
                Password = "RunTest2026!",
                Role = "Participant",
                DateOfBirth = "2002-06-15",
                Gender = "Female"
            };

            var response = await _client.PostAsJsonAsync(
                "/api/Auth/register", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // TEST 2: Login with valid participant credentials
        [Fact]
        public async Task Login_ValidParticipant_ReturnsSuccess()
        {
            var email = $"login_{Guid.NewGuid():N}@example.com";

            var registerRequest = new
            {
                Name = "Login Test Participant",
                Email = email,
                Password = "RunTest2026!",
                Role = "Participant",
                DateOfBirth = "2002-06-15",
                Gender = "Female"
            };

            var registerResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", registerRequest);

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginRequest = new
            {
                Email = email,
                Password = "RunTest2026!",
                Role = "Participant"
            };

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }

        // TEST 3: Reject an incorrect password
        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var email = $"invalid_{Guid.NewGuid():N}@example.com";

            var registerRequest = new
            {
                Name = "Invalid Login Test",
                Email = email,
                Password = "RunTest2026!",
                Role = "Participant",
                DateOfBirth = "2002-06-15",
                Gender = "Female"
            };

            var registerResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", registerRequest);

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginRequest = new
            {
                Email = email,
                Password = "WrongPassword2026!",
                Role = "Participant"
            };

            var response = await _client.PostAsJsonAsync(
                "/api/Auth/login", loginRequest);

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        // TEST 4: Reject access without a JWT token
        [Fact]
        public async Task GetParticipants_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync(
                "/api/RaceDay/participants");

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        // TEST 5: Forbid an organiser from accessing participants
        [Fact]
        public async Task GetParticipants_AsOrganiser_ReturnsForbidden()
        {
            var email = $"organiser_{Guid.NewGuid():N}@example.com";
            var password = "RunTest2026!";

            var registerRequest = new
            {
                Name = "Test Organiser",
                Email = email,
                Password = password,
                Role = "Organiser",
                Phone = "0712345678"
            };

            var registerResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", registerRequest);

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginRequest = new
            {
                Email = email,
                Password = password,
                Role = "Organiser"
            };

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            using var loginJson = JsonDocument.Parse(
                await loginResponse.Content.ReadAsStringAsync());

            var token = loginJson.RootElement
                .GetProperty("token")
                .GetString();

            Assert.False(string.IsNullOrWhiteSpace(token));

            using var request = new HttpRequestMessage(
                HttpMethod.Get, "/api/RaceDay/participants");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        // TEST 6: Allow a participant with a valid JWT token
        [Fact]
        public async Task GetParticipants_AsParticipant_ReturnsSuccess()
        {
            var email = $"participant_{Guid.NewGuid():N}@example.com";
            var password = "RunTest2026!";

            var registerRequest = new
            {
                Name = "Test Participant",
                Email = email,
                Password = password,
                Role = "Participant",
                DateOfBirth = "2002-06-15",
                Gender = "Female"
            };

            var registerResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", registerRequest);

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginRequest = new
            {
                Email = email,
                Password = password,
                Role = "Participant"
            };

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            using var loginJson = JsonDocument.Parse(
                await loginResponse.Content.ReadAsStringAsync());

            var token = loginJson.RootElement
                .GetProperty("token")
                .GetString();

            Assert.False(string.IsNullOrWhiteSpace(token));

            using var request = new HttpRequestMessage(
                HttpMethod.Get, "/api/RaceDay/participants");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // TEST 7: An organiser can create an event
        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
            var email = $"organiser_{Guid.NewGuid():N}@example.com";
            var password = "RunTest2026!";

            var registerRequest = new
            {
                Name = "Event Test Organiser",
                Email = email,
                Password = password,
                Role = "Organiser",
                Phone = "0712345678"
            };

            var registerResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", registerRequest);

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginRequest = new
            {
                Email = email,
                Password = password,
                Role = "Organiser"
            };

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            using var loginJson = JsonDocument.Parse(
                await loginResponse.Content.ReadAsStringAsync());

            var token = loginJson.RootElement
                .GetProperty("token")
                .GetString();

            Assert.False(string.IsNullOrWhiteSpace(token));

            using var request = new HttpRequestMessage(
                HttpMethod.Post, "/api/RaceDay/events");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(new
            {
                EventName = "Automated Test Run",
                EventDate = new DateTime(2026, 11, 15, 8, 0, 0),
                Location = "Johannesburg",
                Description = "Event created by an automated test",
                EventType = "Run",
                Distance = 5
            });

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // TEST 8: A participant cannot create an event
        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsForbidden()
        {
            var email = $"participant_{Guid.NewGuid():N}@example.com";
            var password = "RunTest2026!";

            var registerRequest = new
            {
                Name = "Event Test Participant",
                Email = email,
                Password = password,
                Role = "Participant",
                Phone = "0712345678"
            };

            var registerResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", registerRequest);

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginRequest = new
            {
                Email = email,
                Password = password,
                Role = "Participant"
            };

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            using var loginJson = JsonDocument.Parse(
                await loginResponse.Content.ReadAsStringAsync());

            var token = loginJson.RootElement
                .GetProperty("token")
                .GetString();

            Assert.False(string.IsNullOrWhiteSpace(token));

            using var request = new HttpRequestMessage(
                HttpMethod.Post, "/api/RaceDay/events");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Content = JsonContent.Create(new
            {
                EventName = "Participant Test Run",
                EventDate = new DateTime(2026, 11, 15, 8, 0, 0),
                Location = "Johannesburg",
                Description = "This event should not be created",
                EventType = "Run",
                Distance = 5
            });

            var response = await _client.SendAsync(request);

            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        // TEST 9: An unauthenticated user cannot create an event
        [Fact]
        public async Task CreateEvent_WithoutToken_ReturnsUnauthorized()
        {
            var eventRequest = new
            {
                EventName = "Unauthorised Test Run",
                EventDate = new DateTime(2026, 11, 15, 8, 0, 0),
                Location = "Johannesburg",
                Description = "This event should not be created",
                EventType = "Run",
                Distance = 5
            };

            var response = await _client.PostAsJsonAsync(
                "/api/RaceDay/events", eventRequest);

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        // TEST 10: An organiser cannot update another organiser's event
        [Fact]
        public async Task UpdateEvent_AsDifferentOrganiser_ReturnsForbidden()
        {
            var ownerEmail = $"owner_{Guid.NewGuid():N}@example.com";
            var password = "RunTest2026!";

            // Register the event owner
            var ownerRegister = new
            {
                Name = "Event Owner",
                Email = ownerEmail,
                Password = password,
                Role = "Organiser",
                Phone = "0712345678"
            };

            var ownerRegisterResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", ownerRegister);

            Assert.Equal(
                HttpStatusCode.OK,
                ownerRegisterResponse.StatusCode);

            // Log in as the event owner
            var ownerLogin = new
            {
                Email = ownerEmail,
                Password = password,
                Role = "Organiser"
            };

            var ownerLoginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", ownerLogin);

            Assert.Equal(
                HttpStatusCode.OK,
                ownerLoginResponse.StatusCode);

            using var ownerJson = JsonDocument.Parse(
                await ownerLoginResponse.Content.ReadAsStringAsync());

            var ownerToken = ownerJson.RootElement
                .GetProperty("token")
                .GetString();

            Assert.False(string.IsNullOrWhiteSpace(ownerToken));

            // Create an event using the owner's token
            using var createRequest = new HttpRequestMessage(
                HttpMethod.Post, "/api/RaceDay/events");

            createRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", ownerToken);

            createRequest.Content = JsonContent.Create(new
            {
                EventName = "Owner's Test Run",
                EventDate = new DateTime(2026, 11, 15, 8, 0, 0),
                Location = "Johannesburg",
                Description = "Event owned by the first organiser",
                EventType = "Run",
                Distance = 5
            });

            var createResponse = await _client.SendAsync(createRequest);

            Assert.Equal(
                HttpStatusCode.Created,
                createResponse.StatusCode);

            using var eventJson = JsonDocument.Parse(
                await createResponse.Content.ReadAsStringAsync());

            var eventId = eventJson.RootElement
                .GetProperty("eventID")
                .GetInt32();

            // Register a different organiser
            var otherEmail = $"other_{Guid.NewGuid():N}@example.com";

            var otherRegister = new
            {
                Name = "Different Organiser",
                Email = otherEmail,
                Password = password,
                Role = "Organiser",
                Phone = "0712345678"
            };

            var otherRegisterResponse = await _client.PostAsJsonAsync(
                "/api/Auth/register", otherRegister);

            Assert.Equal(
                HttpStatusCode.OK,
                otherRegisterResponse.StatusCode);

            // Log in as the different organiser
            var otherLogin = new
            {
                Email = otherEmail,
                Password = password,
                Role = "Organiser"
            };

            var otherLoginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login", otherLogin);

            Assert.Equal(
                HttpStatusCode.OK,
                otherLoginResponse.StatusCode);

            using var otherJson = JsonDocument.Parse(
                await otherLoginResponse.Content.ReadAsStringAsync());

            var otherToken = otherJson.RootElement
                .GetProperty("token")
                .GetString();

            Assert.False(string.IsNullOrWhiteSpace(otherToken));

            // The second organiser attempts to update the owner's event
            using var updateRequest = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/RaceDay/events/{eventId}");

            updateRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", otherToken);

            updateRequest.Content = JsonContent.Create(new
            {
                EventName = "Unauthorised Update",
                EventDate = new DateTime(2026, 11, 16, 8, 0, 0),
                Location = "Pretoria",
                Description = "This update should be rejected",
                EventType = "Run",
                Distance = 5
            });

            var updateResponse = await _client.SendAsync(updateRequest);

            Assert.Equal(
                HttpStatusCode.Forbidden,
                updateResponse.StatusCode);
        }
    }
}
