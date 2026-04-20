namespace Enterprise.Framework.Application.Features.Labors.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record UpdateLaborCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public long MaintenanceId { get; init; }
    public long EmployeeId { get; init; }
    public decimal HoursWorked { get; init; }
    public decimal HourlyRate { get; init; }
}

public class UpdateLaborCommandHandler : IRequestHandler<UpdateLaborCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateLaborCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Labor>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Labor), request.Id);
        }

        entity.MaintenanceId = request.MaintenanceId;
        entity.EmployeeId = request.EmployeeId;
        entity.HoursWorked = request.HoursWorked;
        entity.HourlyRate = request.HourlyRate;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
