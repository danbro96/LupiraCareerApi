using Xunit;

namespace LupiraCareerApi.Server.Tests;

/// <summary>One ephemeral Postgres container shared across the whole run; tests in this collection run serially
/// (they share DB state and reset it per test), which avoids cross-test interference.</summary>
[CollectionDefinition("integration")]
public sealed class CareerApiCollection : ICollectionFixture<CareerApiTestFactory>;
