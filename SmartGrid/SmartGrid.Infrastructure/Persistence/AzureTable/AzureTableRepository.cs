using Azure;
using Azure.Data.Tables;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable
{
    internal class AzureTableRepository<TDomain, TEntity>(
        TableClient tableClient,
        ITableKeyProvider<TDomain> keyProvider,
        ITableMapper<TDomain, TEntity> mapper
    )
        where TEntity : class, ITableEntity, new()
    {
        protected readonly TableClient _tableClient = tableClient;
        protected readonly ITableKeyProvider<TDomain> _keyProvider = keyProvider;
        protected readonly ITableMapper<TDomain, TEntity> _mapper = mapper;

        public async Task<TDomain?> GetByIdAsync(string partitionKey, string rowKey, CancellationToken ct)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey, cancellationToken: ct);
                return _mapper.ToDomain(response.Value);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return default;
            }
        }

        public async Task AddAsync(TDomain domain, CancellationToken ct = default)
        {
            var entity = _mapper.ToEntity(domain);

            entity.PartitionKey = _keyProvider.GetPartitionKey(domain);
            entity.RowKey = _keyProvider.GetRowKey(domain);

            await _tableClient.AddEntityAsync(entity, ct);
        }

        public async Task UpdateAsync(TDomain domain, CancellationToken ct)
        {
            var entity = _mapper.ToEntity(domain);
            entity.PartitionKey = _keyProvider.GetPartitionKey(domain);
            entity.RowKey = _keyProvider.GetRowKey(domain);

            await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace, ct);
        }

        public async Task DeleteAsync(TDomain domain, CancellationToken ct)
        {
            var partitionKey = _keyProvider.GetPartitionKey(domain);
            var rowKey = _keyProvider.GetRowKey(domain);

            await _tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: ct);
        }

        public async Task<List<TDomain>> QueryAsync(string filter, CancellationToken ct)
        {
            var query = _tableClient.QueryAsync<TEntity>(filter, cancellationToken: ct);
            var results = new List<TDomain>();

            await foreach (var entity in query)
            {
                var domainModel = _mapper.ToDomain(entity);
                if (domainModel is not null)
                {
                    results.Add(domainModel);
                }
            }

            return results;
        }
        public async Task<List<TDomain>> QueryByPartitionKeyAsync(string partitionKey, CancellationToken ct = default)
        {
            string filter = $"PartitionKey eq '{partitionKey}'";

            var results = new List<TDomain>();
            await foreach (var entity in _tableClient.QueryAsync<TEntity>(filter, cancellationToken: ct))
            {
                var domainModel = _mapper.ToDomain(entity);
                if (domainModel is not null)
                {
                    results.Add(domainModel);
                }
            }

            return results;
        }
        public async Task UpsertAsync(TDomain domain, CancellationToken ct)
        {
            var entity = _mapper.ToEntity(domain);
            entity.PartitionKey = _keyProvider.GetPartitionKey(domain);
            entity.RowKey = _keyProvider.GetRowKey(domain);

            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, ct);
        }
    }
}