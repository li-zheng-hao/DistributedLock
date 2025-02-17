﻿using Medallion.Threading.MySql;
using NUnit.Framework;
using System.Data;

namespace Medallion.Threading.Tests.MySql;

public class MySqlDistributedSynchronizationProviderTest
{
    [Test]
    public void TestArgumentValidation()
    {
        Assert.Throws<ArgumentNullException>(() => new MySqlDistributedSynchronizationProvider(default(string)!));
        Assert.Throws<ArgumentNullException>(() => new MySqlDistributedSynchronizationProvider(default(IDbConnection)!));
        Assert.Throws<ArgumentNullException>(() => new MySqlDistributedSynchronizationProvider(default(IDbTransaction)!));
    }

    [Test]
    public async Task BasicTest()
    {
        foreach (var db in new[] { new TestingMySqlDb(), new TestingMariaDbDb() })
        {
            var provider = new MySqlDistributedSynchronizationProvider(db.ConnectionString);

            const string LockName = TargetFramework.Current + "ProviderBasicTest";
            await using (await provider.AcquireLockAsync(LockName))
            {
                await using var handle = await provider.TryAcquireLockAsync(LockName);
                Assert.IsNull(handle, db.GetType().Name);
            }
        }
    }
}
