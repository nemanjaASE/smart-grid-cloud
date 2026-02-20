using SmartGrid.Application.Features.Devices.Queries;
using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Devices.Mappers
{
    internal sealed class DeviceMapper : IMapper<Device, DeviceDto>
    {
        public DeviceDto Map(Device source)
        {
            return new DeviceDto(
                source.Id,
                source.Name,
                source.Type,
                source.Location,
                source.NominalPower,
                source.RegisteredAt);
        }
    }
}
