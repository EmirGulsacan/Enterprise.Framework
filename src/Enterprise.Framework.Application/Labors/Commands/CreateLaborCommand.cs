namespace Enterprise.Framework.Application.Labors.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public record CreateLaborCommand : IRequest<long>
{
    public int MaintenanceId { get; init; }
    public int EmployeeId { get; init; }
    public decimal HoursWorked { get; init; }
    public decimal HourlyRate { get; init; }
}

public class CreateLaborCommandHandler : IRequestHandler<CreateLaborCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateLaborCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = new Labor
        {
            MaintenanceId = request.MaintenanceId,
            EmployeeId = request.EmployeeId,
            HoursWorked = request.HoursWorked,
            HourlyRate = request.HourlyRate
        };

        _context.Labors.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
