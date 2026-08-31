using Api.Modules.Customer.Domain.Entities;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Persistence.Models;
using Api.Shared.Database;
using Dapper;

namespace Api.Modules.Customer.Infrastructure.Persistence
{
    public sealed class AddressRepository
        : IAddressRepository
    {
        private readonly IDbSession _dbSession;

        public AddressRepository(
            IDbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<IReadOnlyList<Address>>
            GetActiveByUserIdAsync(
                ulong userId,
                CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    user_id AS UserId,
                    name AS Name,
                    recipient_name AS RecipientName,
                    postal_code AS PostalCode,
                    street AS Street,
                    number AS Number,
                    complement AS Complement,
                    district AS District,
                    city AS City,
                    state AS State,
                    country AS Country,
                    active AS Active,
                    is_default AS IsDefault,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM customer.addresses
                WHERE user_id = @UserId
                  AND active = TRUE
                ORDER BY
                    is_default DESC,
                    created_at DESC;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { UserId = userId },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var rows =
                await _dbSession.Connection
                    .QueryAsync<AddressRow>(command);

            return rows
                .Select(Map)
                .ToList();
        }

        public async Task<Address?>
            GetByExternalIdAndUserIdAsync(
                Guid externalId,
                ulong userId,
                CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    user_id AS UserId,
                    name AS Name,
                    recipient_name AS RecipientName,
                    postal_code AS PostalCode,
                    street AS Street,
                    number AS Number,
                    complement AS Complement,
                    district AS District,
                    city AS City,
                    state AS State,
                    country AS Country,
                    active AS Active,
                    is_default AS IsDefault,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM customer.addresses
                WHERE external_id = @ExternalId
                  AND user_id = @UserId
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    ExternalId = externalId,
                    UserId = userId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row =
                await _dbSession.Connection
                    .QuerySingleOrDefaultAsync<AddressRow>(
                        command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<Address?> GetFirstActiveByUserIdAsync(
            ulong userId,
            ulong excludeAddressId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    id AS Id,
                    external_id AS ExternalId,
                    user_id AS UserId,
                    name AS Name,
                    recipient_name AS RecipientName,
                    postal_code AS PostalCode,
                    street AS Street,
                    number AS Number,
                    complement AS Complement,
                    district AS District,
                    city AS City,
                    state AS State,
                    country AS Country,
                    active AS Active,
                    is_default AS IsDefault,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM customer.addresses
                WHERE user_id = @UserId
                  AND active = TRUE
                  AND id <> @ExcludeAddressId
                ORDER BY created_at ASC
                LIMIT 1;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    ExcludeAddressId = excludeAddressId
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            var row =
                await _dbSession.Connection
                    .QuerySingleOrDefaultAsync<AddressRow>(
                        command);

            return row is null
                ? null
                : Map(row);
        }

        public async Task<bool> HasActiveAddressesAsync(
            ulong userId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT EXISTS(
                    SELECT 1
                    FROM customer.addresses
                    WHERE user_id = @UserId
                      AND active = TRUE
                );
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { UserId = userId },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection
                .ExecuteScalarAsync<bool>(command);
        }

        public async Task<ulong> CreateAsync(
            Address address,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO customer.addresses
                (
                    external_id,
                    user_id,
                    name,
                    recipient_name,
                    postal_code,
                    street,
                    number,
                    complement,
                    district,
                    city,
                    state,
                    country,
                    active,
                    is_default,
                    created_at,
                    updated_at
                )
                VALUES
                (
                    @ExternalId,
                    @UserId,
                    @Name,
                    @RecipientName,
                    @PostalCode,
                    @Street,
                    @Number,
                    @Complement,
                    @District,
                    @City,
                    @State,
                    @Country,
                    @Active,
                    @IsDefault,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT LAST_INSERT_ID();
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    address.ExternalId,
                    address.UserId,
                    address.Name,
                    address.RecipientName,
                    address.PostalCode,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.District,
                    address.City,
                    address.State,
                    address.Country,
                    address.Active,
                    address.IsDefault,
                    address.CreatedAt,
                    address.UpdatedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            return await _dbSession.Connection
                .ExecuteScalarAsync<ulong>(command);
        }

        public async Task UpdateAsync(
            Address address,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.addresses
                SET
                    name = @Name,
                    recipient_name = @RecipientName,
                    postal_code = @PostalCode,
                    street = @Street,
                    number = @Number,
                    complement = @Complement,
                    district = @District,
                    city = @City,
                    state = @State,
                    country = @Country,
                    active = @Active,
                    is_default = @IsDefault,
                    updated_at = @UpdatedAt
                WHERE id = @Id
                  AND user_id = @UserId;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    address.Id,
                    address.UserId,
                    address.Name,
                    address.RecipientName,
                    address.PostalCode,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.District,
                    address.City,
                    address.State,
                    address.Country,
                    address.Active,
                    address.IsDefault,
                    address.UpdatedAt
                },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        public async Task RemoveDefaultByUserIdAsync(
            ulong userId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE customer.addresses
                SET
                    is_default = FALSE,
                    updated_at = UTC_TIMESTAMP()
                WHERE user_id = @UserId
                  AND active = TRUE
                  AND is_default = TRUE;
                """;

            await _dbSession.OpenConnectionAsync(
                cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { UserId = userId },
                transaction: _dbSession.Transaction,
                cancellationToken: cancellationToken);

            await _dbSession.Connection
                .ExecuteAsync(command);
        }

        private static Address Map(
            AddressRow row)
        {
            return Address.Restore(
                row.Id,
                row.ExternalId,
                row.UserId,
                row.Name,
                row.RecipientName,
                row.PostalCode,
                row.Street,
                row.Number,
                row.Complement,
                row.District,
                row.City,
                row.State,
                row.Country,
                row.Active,
                row.IsDefault,
                row.CreatedAt,
                row.UpdatedAt);
        }
    }
}