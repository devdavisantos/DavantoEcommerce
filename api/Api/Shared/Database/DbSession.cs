using System.Data;
using System.Data.Common;

namespace Api.Shared.Database
{
    public sealed class DbSession : IDbSession
    {
        public DbConnection Connection { get; }
        public DbTransaction? Transaction { get; private set; }

        public DbSession(IDbConnectionFactory connectionFactory)
        {
            Connection = connectionFactory.CreateConnection();
        }

        public async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (Connection.State == ConnectionState.Open)
                return;

            await Connection.OpenAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await OpenConnectionAsync(cancellationToken);

            if (Transaction is not null)
                return;

            Transaction = await Connection.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (Transaction is null)
                return;

            await Transaction.CommitAsync(cancellationToken);

            await Transaction.DisposeAsync();

            Transaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (Transaction is null)
                return;

            await Transaction.RollbackAsync(cancellationToken);

            await Transaction.DisposeAsync();

            Transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            if (Transaction is not null)
            {
                await Transaction.DisposeAsync();
                Transaction = null;
            }

            await Connection.DisposeAsync();
        }
    }
}
