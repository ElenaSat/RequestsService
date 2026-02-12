using FluentValidation;
using FluentValidation.Results;
using Moq;
using RequestsService.Application.Common.Interfaces;
using RequestsService.Application.DTOs;
using RequestsService.Application.Features.Solicitudes.Create;
using RequestsService.Domain.Entities;
using RequestsService.Domain.Repositories;

namespace RequestsService.Tests.Unit.Application;

public class CreateSolicitudHandlerTests
{
    private readonly Mock<ISolicitudRepository> _mockRepo;
    private readonly Mock<IRequestCreatedPublisher> _mockPublisher;
    private readonly Mock<IValidator<CrearSolicitudRequest>> _mockValidator;
    private readonly CreateSolicitudHandler _handler;

    public CreateSolicitudHandlerTests()
    {
        _mockRepo = new Mock<ISolicitudRepository>();
        _mockPublisher = new Mock<IRequestCreatedPublisher>();
        _mockValidator = new Mock<IValidator<CrearSolicitudRequest>>();
        _handler = new CreateSolicitudHandler(_mockRepo.Object, _mockPublisher.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesSolicitudAndPublishesEvent()
    {
        // Arrange
        var request = new CrearSolicitudRequest("Test Name", "Test Payload");
        var command = new CrearSolicitudCommand(request);
        
        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // Valid result

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(request.Name, result.Value.Name);
        
        _mockRepo.Verify(r => r.AddAsync(It.Is<Solicitud>(s => s.Name == request.Name), It.IsAny<CancellationToken>()), Times.Once);
        _mockPublisher.Verify(p => p.PublishAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ReturnsFailure()
    {
        // Arrange
        var request = new CrearSolicitudRequest("", ""); // Invalid
        var command = new CrearSolicitudCommand(request);
        var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Name is required") };
        
        _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Name is required", result.Errors);
        
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Solicitud>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockPublisher.Verify(p => p.PublishAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
