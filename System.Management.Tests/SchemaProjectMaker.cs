using System;

namespace System.Management.Tests
{
	internal class SchemaProjectMaker : CodeWriterBase
	{
		private string schemaPath;

		public SchemaProjectMaker (string schemaPath)
			: base(null)
		{
			this.schemaPath = schemaPath;
		}

		public override void Write()
		{
			WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			WriteLine ("<Project DefaultTargets=\"Build\" ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
			WriteLine (" <PropertyGroup>");
			WriteLine ("    <Configuration Condition=\" '$(Configuration)' == '' \">Default</Configuration>");
			WriteLine ("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
			WriteLine ("    <ItemType>GenericProject</ItemType>");
			WriteLine ("    <ProjectGuid>{8CA05B26-9F53-42A8-B8D8-2D88083601ED}</ProjectGuid>");
			WriteLine (" </PropertyGroup>");
			WriteLine("   <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Default|AnyCPU' \">");
			WriteLine ("      <OutputPath>.</OutputPath>");
			WriteLine ("   </PropertyGroup>");
			WriteLine (" <ItemGroup>");
			foreach (var manifest in GeneratorFactory.Classes) {
				if (!manifest.HaveChildren) {
					WriteLine ("  <None Include=\"{0}20.mof\" />", CodeWriterBase.GetClassName (manifest));
					WriteLine ("  <None Include=\"{0}20R.mof\" />");
				}
			}
			WriteLine (" </ItemGroup>");
			WriteLine ("</Project>");
			Save (System.IO.Path.Combine (schemaPath, "Schemas.mdproj"));
		}

	}
}

