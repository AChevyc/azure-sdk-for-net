﻿
using Microsoft.AzureStack.Management.Compute.Admin;
using Microsoft.AzureStack.Management.Compute.Admin.Models;
using System.Collections.Generic;
using Xunit;

namespace Compute.Tests
{
    public class DiskMigrationJobTests : ComputeTestBase
    {
        private void ValidateDiskMigration(DiskMigrationJob diskMigration)
        {
            Assert.NotNull(diskMigration);
            Assert.NotNull(diskMigration.CreationTime);
            Assert.NotNull(diskMigration.Id);
            Assert.NotNull(diskMigration.Location);
            Assert.NotNull(diskMigration.Name);
            Assert.NotNull(diskMigration.Status);
            Assert.NotNull(diskMigration.TargetShare);
            Assert.NotNull(diskMigration.Type);
            Assert.NotNull(diskMigration.MigrationId);
        }

        [Fact]
        public void TestDiskMigration()
        {
            string targetShare = @"\\SU1FileServer.azurestack.local\SU1_ObjStore\";
            RunTest((client) => {
                var disks = client.Disks.List(Location);
                List<Disk> toMigrationDisks = new List<Disk>();
                foreach(var disk in disks)
                {
                    if (toMigrationDisks.Count < 3)
                    {
                        toMigrationDisks.Add(disk);
                    }
                    else
                    {
                        break;
                    }
                }

                var migrationId = "ba0644a4-c2ed-4e3c-a167-089a32865297"; // System.Guid.NewGuid().ToString(); This guid should be the same as the ones in sessionRecord

                var migration = client.DiskMigrationJobs.Create(Location, migrationId, targetShare, toMigrationDisks);
                ValidateDiskMigration(migration);

                migration = client.DiskMigrationJobs.Cancel(Location, migrationId);
                ValidateDiskMigration(migration);

                var migrationFromGet = client.DiskMigrationJobs.Get(Location, migrationId);
                ValidateDiskMigration(migrationFromGet);

                var migrationList = client.DiskMigrationJobs.List(Location);
                migrationList.ForEach(ValidateDiskMigration);

                var migrationSucceededList = client.DiskMigrationJobs.List(Location, status: "Succeeded");
                migrationSucceededList.ForEach(ValidateDiskMigration);
            });
        }
    }
}
