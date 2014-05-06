using System;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal class SolutionFileCreator : CodeWriterBase
	{
		private IEnumerable<SolutionItem> projects;
		private IEnumerable<SolutionItem> projectTests;

		public SolutionFileCreator (IEnumerable<SolutionItem> projects, IEnumerable<SolutionItem> projectTests)
			: base (null)
		{
			this.projects = projects;
			this.projectTests = projectTests;
		}

		public override void Write ()
		{
			WriteLine ("");
			WriteLine ("Microsoft Visual Studio Solution File, Format Version 12.00");
			WriteLine ("# Visual Studio 2012");
			WriteLine ("Project(\"{9344BDBB-3E7F-41FC-A0DD-8665D75EE146}\") = \"Schemas\", \"Schemas\\Schemas.mdproj\", \"{8CA05B26-9F53-42A8-B8D8-2D88083601ED}\"");
			WriteLine ("EndProject");

			foreach (var prj in projects) {
				WriteLine ("Project(\"{" + prj.ProjectId.ToString().ToUpper() + "}\") = \"" + prj.Name + "\", \"" + prj.Path + "\", \"{" + prj.SchemaId.ToString().ToUpper() + "}\"");
				WriteLine ("EndProject");
			}

			//WriteLine ("Project(\"{2857B73E-F847-4B02-9238-064979017E93}\") = \"UNIXProviders\", \"src\\Providers\\UNIXProviders\\UNIXProviders.cproj\", \"{4DC6B981-CE60-4CD1-9DF5-7AEE2873EBDD}\"");
			//WriteLine ("EndProject");

			WriteLine ("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Tests\", \"Tests\", \"{DA5BD9E9-EA02-4E26-AAA9-33C7BB9E3C95}\"");
			WriteLine ("EndProject");

			/*
			WriteLine ("Project(\"{2857B73E-F847-4B02-9238-064979017E93}\") = \"UNIXProviders.Tests\", \"src\\Providers\\UNIXProviders\\tests\\UNIXProviders.Tests\\UNIXProviders.Tests.cproj\", \"{FB6D38B1-B280-4DC5-BFA8-4709837175EF}\"");
			WriteLine ("EndProject");
			*/

			foreach (var prj in projectTests) {
				WriteLine ("Project(\"{" + prj.ProjectId.ToString().ToUpper() + "}\") = \"" + prj.Name + "\", \"" + prj.Path + "\", \"{" + prj.SchemaId.ToString().ToUpper() + "}\"");
				WriteLine ("EndProject");
			}

			WriteLine ("Global");
			WriteLine ("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			WriteLine ("\t\tDefault|Any CPU = Default|Any CPU");
			WriteLine ("\t\tDebug|Any CPU = Debug|Any CPU");
			WriteLine ("\t\tRelease|Any CPU = Release|Any CPU");
			WriteLine ("\tEndGlobalSection");
			WriteLine ("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			foreach (var prj in projects) {
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Debug|Any CPU.Build.0 = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Default|Any CPU.ActiveCfg = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Default|Any CPU.Build.0 = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Release|Any CPU.ActiveCfg = Release|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Release|Any CPU.Build.0 = Release|Any CPU");
			}
			foreach (var prj in projectTests) {
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Debug|Any CPU.Build.0 = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Default|Any CPU.ActiveCfg = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Default|Any CPU.Build.0 = Debug|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Release|Any CPU.ActiveCfg = Release|Any CPU");
				WriteLine ("\t\t{" + prj.SchemaId.ToString().ToUpper() + "}.Release|Any CPU.Build.0 = Release|Any CPU");
			}

			WriteLine ("\t\t{8CA05B26-9F53-42A8-B8D8-2D88083601ED}.Debug|Any CPU.ActiveCfg = Default|Any CPU");
			WriteLine ("\t\t{8CA05B26-9F53-42A8-B8D8-2D88083601ED}.Debug|Any CPU.Build.0 = Default|Any CPU");
			WriteLine ("\t\t{8CA05B26-9F53-42A8-B8D8-2D88083601ED}.Default|Any CPU.ActiveCfg = Default|Any CPU");
			WriteLine ("\t\t{8CA05B26-9F53-42A8-B8D8-2D88083601ED}.Default|Any CPU.Build.0 = Default|Any CPU");
			WriteLine ("\t\t{8CA05B26-9F53-42A8-B8D8-2D88083601ED}.Release|Any CPU.ActiveCfg = Default|Any CPU");
			WriteLine ("\t\t{8CA05B26-9F53-42A8-B8D8-2D88083601ED}.Release|Any CPU.Build.0 = Default|Any CPU");
			WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
			WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF}.Debug|Any CPU.Build.0 = Debug|Any CPU");
			WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF}.Default|Any CPU.ActiveCfg = Debug|Any CPU");
			WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF}.Default|Any CPU.Build.0 = Debug|Any CPU");
			WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF}.Release|Any CPU.ActiveCfg = Release|Any CPU");
			WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF}.Release|Any CPU.Build.0 = Release|Any CPU");
			WriteLine ("\tEndGlobalSection");
			WriteLine ("\tGlobalSection(NestedProjects) = preSolution");

			//WriteLine ("\t\t{FB6D38B1-B280-4DC5-BFA8-4709837175EF} = {DA5BD9E9-EA02-4E26-AAA9-33C7BB9E3C95}");

			foreach (var prj in projectTests) {
				WriteLine ("\t\t{" + prj.SchemaId.ToString() + "} = {DA5BD9E9-EA02-4E26-AAA9-33C7BB9E3C95}");
			}

			WriteLine ("\tEndGlobalSection");
			WriteLine ("\tGlobalSection(MonoDevelopProperties) = preSolution");
			WriteLine ("\t\tStartupItem = Schemas\\Schemas.mdproj");
			WriteLine ("\tEndGlobalSection");
			WriteLine ("\tEndGlobal");


		}
	}
}

