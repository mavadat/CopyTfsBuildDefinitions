using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.Client;

namespace CopyTfsBuildDefinitions
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine(@"Usage: CopyTfsBuildDefinitions <TfsCollectionUri> <SourceProjectName> <TargetProjectName>
e.g. CopyTfsBuildDefinitions http://tfs1:8080/tfs/myCollection projectA projectB");
            }

            var tfsCollectionUri = args[0];
            var sourceProjectName = args[1];
            var targetProjectName = args[3];

            var server = GetTfsBuildServer(tfsCollectionUri);

            var sourceBuildDefinitions = server.QueryBuildDefinitions(sourceProjectName);
            foreach (var sourceBuildDef in sourceBuildDefinitions)
            {
                var targetBuildDef = server.CreateBuildDefinition(targetProjectName);
                Copy(sourceBuildDef, targetBuildDef);
                targetBuildDef.Save();
            }
        }

        static IBuildServer GetTfsBuildServer(string tfsCollectionUri)
        {
            var collection = new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(new Uri(tfsCollectionUri));
            collection.EnsureAuthenticated();
            return collection.GetService<IBuildServer>();
        }

        static void Copy(IBuildDefinition source, IBuildDefinition target)
        {
            target.BatchSize = source.BatchSize;
            target.BuildController = source.BuildController;
            target.ContinuousIntegrationType = source.ContinuousIntegrationType;
            target.ContinuousIntegrationQuietPeriod = source.ContinuousIntegrationQuietPeriod;
            target.DefaultDropLocation = source.DefaultDropLocation;
            target.Description = source.Description;
            target.Process = source.Process;
            target.ProcessParameters = source.ProcessParameters;
            target.QueueStatus = source.QueueStatus;
            target.TriggerType = source.TriggerType;

            CopySchedules(source, target);
            CopyMappings(source, target);
            CopyRetentionPolicies(source, target);
        }

        private static void CopyRetentionPolicies(IBuildDefinition source, IBuildDefinition target)
        {
            target.RetentionPolicyList.Clear();

            foreach (var policy in source.RetentionPolicyList)
            {
                target.AddRetentionPolicy(
                    policy.BuildReason,
                    policy.BuildStatus,
                    policy.NumberToKeep,
                    policy.DeleteOptions
                    );
            }
        }

        private static void CopyMappings(IBuildDefinition source, IBuildDefinition target)
        {
            foreach (var mapping in source.Workspace.Mappings)
            {
                target.Workspace.AddMapping(
                    mapping.ServerItem,
                    mapping.LocalItem,
                    mapping.MappingType,
                    mapping.Depth
                    );
            }
        }

        private static void CopySchedules(IBuildDefinition source, IBuildDefinition target)
        {
            foreach (var schedule in source.Schedules)
            {
                var newSchedule = target.AddSchedule();
                newSchedule.DaysToBuild = schedule.DaysToBuild;
                newSchedule.StartTime = schedule.StartTime;
                newSchedule.TimeZone = schedule.TimeZone;
            }
        }
    }
}
