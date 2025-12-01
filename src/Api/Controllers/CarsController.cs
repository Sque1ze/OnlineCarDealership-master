using Microsoft.AspNetCore.Authorization;
﻿using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Cars.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/cars")]
public class CarsController(
    ISender sender,
    ICarQueries carQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarDto>>> GetAll(CancellationToken cancellationToken)
    {
        var cars = await carQueries.GetAllAsync(cancellationToken);
        return cars.Select(CarDto.FromDomainModel).ToList();
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<ActionResult<IReadOnlyList<CarDto>>> GetByCategory(
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        var cars = await carQueries.GetByCategoryIdAsync(categoryId, cancellationToken);
        return cars.Select(CarDto.FromDomainModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<CarDto>> Create(
        [FromBody] CreateCarDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCarCommand
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Categories = request.Categories
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<CarDto>>(
            f => CarDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<CarDto>> Update(
        [FromBody] UpdateCarDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCarCommand
        {
            CarId = request.Id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Categories = request.Categories
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<CarDto>>(
            f => CarDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpDelete("{carId:guid}")]
    public async Task<ActionResult<CarDto>> Delete(
        [FromRoute] Guid carId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCarCommand { CarId = carId };
        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<CarDto>>(
            f => CarDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpPost("{carId:guid}/images")]
    public async Task<ActionResult<CarDto>> UploadImages(
        [FromRoute] Guid carId,
        [FromForm] IFormFileCollection? files,
        CancellationToken cancellationToken)
    {
        var imageDtos = files?.Select(file => new ImageFileDto
        {
            OriginalName = file.FileName,
            FileStream = file.OpenReadStream()
        }).ToList() ?? new List<ImageFileDto>(); 

        var command = new UploadCarImagesCommand
        {
            CarId = carId,
            Images = imageDtos
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<CarDto>>(
            f => CarDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpDelete("{carId:guid}/images/{imageId:guid}")]
    public async Task<ActionResult<CarDto>> DeleteImage(
        [FromRoute] Guid carId,
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCarImageCommand
        {
            CarId = carId,
            ImageId = imageId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<CarDto>>(
            f => CarDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}