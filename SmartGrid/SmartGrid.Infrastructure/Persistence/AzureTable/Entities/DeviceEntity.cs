namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class DeviceEntity : BaseTableEntity
    {
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public double NominalPower { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
