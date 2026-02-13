using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RequestsService.Application.Common.Interfaces;
using RequestsService.Application.DTOs;
using RequestsService.Domain.Enums;

namespace RequestsService.Tests.Integration;

public class SolicitudesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public SolicitudesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IRequestCreatedPublisher>();
                services.AddSingleton<IRequestCreatedPublisher>(new Mock<IRequestCreatedPublisher>().Object);
            });
        });
        
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PostSolicitud_ValidData_Returns201()
    {
        var request = new CrearSolicitudRequest("Integration Test", "Payload");
        var response = await _client.PostAsJsonAsync("/api/v1/solicitudes", request);
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<SolicitudResponse>();
        Assert.NotNull(content);
        Assert.Equal(request.Name, content.Name);
        Assert.Equal(SolicitudStatus.Pending, content.Status);
        
        // Check Location header for CreatedAtAction
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/v1/solicitudes/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsList()
    {
        // Add one first to ensure list is not empty or at least test response structure
        var request = new CrearSolicitudRequest("List Test", "Payload");
        await _client.PostAsJsonAsync("/api/v1/solicitudes", request);

        var response = await _client.GetAsync("/api/v1/solicitudes");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<List<SolicitudResponse>>();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task PostSolicitud_MultipleConcurrentRequests_AllShouldSucceed()
    {
        // Arrange
        int numberOfRequests = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < numberOfRequests; i++)
        {
            var request = new CrearSolicitudRequest($"Concurrent {i}", "Payload");
            tasks.Add(_client.PostAsJsonAsync("/api/v1/solicitudes", request));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }

}
