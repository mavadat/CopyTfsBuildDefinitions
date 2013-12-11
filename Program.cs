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
                Console.WriteLine(@"Usage: CopyTfsBuildDefinitions <SourceTfsUri> <SourceProjectName> <TargetTfsUri> <TargetProjectName>
e.g. CopyTfsBuildDefinitions http://tfs1:8080/tfs/default projectA http://tfs2:8080/tfs/default projectB");
            }
        }

        static IBuildServer GetTfsBuildServer(string tfsUri)
        {
            var collection = new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(new Uri(tfsUri));
            collection.EnsureAuthenticated();
            return collection.GetService<IBuildServer>();
        }
    }
}
