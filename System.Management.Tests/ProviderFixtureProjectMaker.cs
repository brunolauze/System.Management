using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Management.Tests
{
	internal class ProviderFixtureProjectMaker : CodeWriterBase
	{
		private Guid id;
		private string basePath;

		public ProviderFixtureProjectMaker (ClassManifest manifest, string basePath)
			: base(manifest)
		{
			id = Guid.NewGuid ();
			this.basePath = basePath;
		}

		public SolutionItem Project
		{
			get { return new SolutionItem (ClassName, id, true); }
		}

		public override void Write ()
		{
			WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			WriteLine("<Project DefaultTargets=\"Build\" ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
			WriteLine("  <PropertyGroup>");
			WriteLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
			WriteLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
			WriteLine("    <ProjectGuid>{0}</ProjectGuid>", id.ToString());
			WriteLine("    <Compiler>");
			WriteLine("      <Compiler ctype=\"ClangppCompiler\" />");
			WriteLine("    </Compiler>");
			WriteLine("    <Language>CPP</Language>");
			WriteLine("    <Target>Bin</Target>");
			WriteLine ("    <Packages>");
			WriteLine("      <Packages>");
			WriteLine ("        <Package file=\"../../{0}.md.pc\" name=\"{0}\" IsProject=\"True\" />", ClassName);
			AddPackages ();
			WriteLine("      </Packages>");
			WriteLine("    </Packages>");
			WriteLine("  </PropertyGroup>");
			WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
			WriteLine("    <DebugSymbols>true</DebugSymbols>");
			WriteLine("    <OutputPath>bin\\Debug</OutputPath>");
			WriteLine("    <OutputName>Test{0}</OutputName>", ClassName);
			WriteLine("    <DefineSymbols>DEBUG MONODEVELOP</DefineSymbols>");
			WriteLine("    <SourceDirectory>.</SourceDirectory>");
			WriteLine("    <Includes>");
			WriteLine("      <Includes>");
			WriteLine("        <Include>/usr/local/include</Include>");
			WriteLine("        <Include>${ProjectDir}</Include>");
			WriteLine("        <Include>${ProjectDir}/*</Include>");
			WriteLine("        <Include>${ProjectDir}/../../..</Include>");
			WriteLine("      </Includes>");
			WriteLine("    </Includes>");
			WriteLine("    <ExtraCompilerArguments>-W -Wall -Wno-unused-parameter  -Wno-unused-value -D_GNU_SOURCE -DTHREAD_SAFE -D_REENTRANT -Werror=unused-variable	-DPEGASUS_COMMON_INTERNAL  -DPEGASUS_PLATFORM_FREEBSD_GENERIC_GNU -DPEGASUS_OS_FREEBSD -DPEGASUS_PLATFORM_FREEBSD_X86_64_CLANG -DPEGASUS_USE_SYSLOGS -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPROVIDER_LIB_NS=\"namespace " + ClassName + "Lib {\"</ExtraCompilerArguments>");
			WriteLine("    <ExtraLinkerArguments>-L/usr/local/lib/pegasus/lib -L/usr/local/lib -lpegcommon -lpegprovider -lpegclient -lpegslp_client -lpegquerycommon -lpegqueryexpression -lpegcql -lpegwql -lpegrepository -lutil -lgeom -ljail -lkvm -pthread -lcam -lpkg" + AddLinks() + "</ExtraLinkerArguments>");
			WriteLine ("    <EnvironmentVariables>");
			WriteLine ("      <EnvironmentVariables>");
			System.Text.StringBuilder projectPaths = new System.Text.StringBuilder ();			
			projectPaths.Append ("../../../../bin/Debug:");
			AddLdLibs (projectPaths);
			WriteLine("        <Variable name=\"LD_LIBRARY_PATH\" value=\"" + projectPaths + "/usr/local/lib/pegasus/lib:/usr/local/lib:/usr/lib:/lib\" />");
			WriteLine("      </EnvironmentVariables>");
			WriteLine("    </EnvironmentVariables>");
			WriteLine ("    <Commandlineparameters></Commandlineparameters>");
			WriteLine ("    <ConsolePause>false</ConsolePause>");
			WriteLine ("    <LibPaths>");
			WriteLine ("      <LibPaths>");
			WriteLine ("        <LibPath>${ProjectDir}/../../bin/Debug</LibPath>");
			AddLibPaths ();
			WriteLine ("    </LibPaths>");
			WriteLine ("    </LibPaths>");
			WriteLine("  </PropertyGroup>");
			WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
			WriteLine("    <OutputPath>bin\\Release</OutputPath>");
			WriteLine("    <OutputName>Test{0}</OutputName>", ClassName);
			WriteLine("    <OptimizationLevel>3</OptimizationLevel>");
			WriteLine("    <DefineSymbols>MONODEVELOP</DefineSymbols>");
			WriteLine("    <SourceDirectory>.</SourceDirectory>");
			WriteLine("	   <ExtraCompilerArguments>-W -Wall -Wno-unused-parameter  -Wno-unused-value -D_GNU_SOURCE -DTHREAD_SAFE -D_REENTRANT -Werror=unused-variable	-DPEGASUS_COMMON_INTERNAL  -DPEGASUS_PLATFORM_FREEBSD_GENERIC_GNU -DPEGASUS_OS_FREEBSD -DPEGASUS_PLATFORM_FREEBSD_X86_64_CLANG -DPEGASUS_USE_SYSLOGS -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPROVIDER_LIB_NS=\"namespace " + ClassName + "Lib {\"</ExtraCompilerArguments>");
			WriteLine("    <ExtraLinkerArguments>-L/usr/local/lib/pegasus/lib -L/usr/local/lib -lpegcommon -lpegprovider -lpegclient -lpegslp_client -lpegquerycommon -lpegqueryexpression -lpegcql -lpegwql -lpegrepository -lutil -lgeom -ljail -lkvm -pthread -lcam -lpkg" + AddLinks() + "</ExtraLinkerArguments>");
			WriteLine("  </PropertyGroup>");
			string[] folders = System.IO.Directory.GetDirectories(basePath);
			WriteLine("  <ItemGroup>");
			WriteRootFiles();
			foreach(var dir in folders)
			{ 
				WriteFiles(dir);
			}
			WriteLine("  </ItemGroup>");
			WriteLine("  <ItemGroup>");
			foreach(var dir in folders)
			{
				WriteLine("    <Folder Include=\"{0}/\" />", new System.IO.DirectoryInfo(dir).Name);
			}
			WriteLine("  </ItemGroup>");
			WriteLine("</Project>");

		}

		void WriteRootFiles ()
		{
			string[] files = System.IO.Directory.GetFiles (basePath);
			foreach (var filePath in files) {
				string file = new System.IO.FileInfo (filePath).Name;
				if (file.EndsWith(".c") || file.EndsWith(".cpp"))
					WriteLine("    <Compile Include=\"{0}\" />", file);
			}
			WriteLine("  </ItemGroup>");
			WriteLine("  <ItemGroup>");
			foreach (var filePath in files) {
				string file = new System.IO.FileInfo (filePath).Name;
				if ((!file.EndsWith(".c") && !file.EndsWith(".cpp")))
					WriteLine("    <Node Include=\"{0}\" />", file);
			}
		}

		void WriteFiles (string dir)
		{
			string[] files = System.IO.Directory.GetFiles (dir);
			foreach (var filePath in files) {
				string file = new System.IO.FileInfo (filePath).Name;
				if ((file.EndsWith(".c") || file.EndsWith(".cpp")))
					WriteLine("    <Compile Include=\"{0}/{1}\" />", new System.IO.DirectoryInfo(dir).Name, file);
			}
			WriteLine ("  </ItemGroup>");
			WriteLine ("  <ItemGroup>");
			WriteLine ("   <None Include=\"RunTest" + ClassName + ".sh\" />");
			foreach (var filePath in files) {
				string file = new System.IO.FileInfo (filePath).Name;
				if ((!file.EndsWith(".c") && !file.EndsWith(".cpp")))
					WriteLine("    <Node Include=\"{0}/{1}\" />", new System.IO.DirectoryInfo(dir).Name, file);
			}
		}

		void AddPackages ()
		{
			bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
			bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			HashSet<string> deps = new HashSet<string>();

			if (Manifest.SuperClass != null && !Manifest.SuperClass.HaveChildren) {
				deps.Add(CodeWriterBase.GetClassName(Manifest.SuperClass));
			}
			if (isDependency) {

				System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			} else if (isComponent) {
				// Find GroupComponent Reference Class
				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;
				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			}
			AddMethodIncludes (Manifest, deps);

			if (deps.Count > 0) {
				foreach (var prj in deps) {
					WriteLine ("        <Package file=\"../../../{0}/{1}.md.pc\" name=\"{1}\" IsProject=\"True\" />", prj.Replace ("CIM_", "").Replace ("UNIX_", ""), prj);
				}
			}
		}

		void AddLdLibs (System.Text.StringBuilder sb)
		{
			bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
			bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			HashSet<string> deps = new HashSet<string>();

			if (Manifest.SuperClass != null && !Manifest.SuperClass.HaveChildren) {
				deps.Add(CodeWriterBase.GetClassName(Manifest.SuperClass));
			}
			if (isDependency) {

				System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			} else if (isComponent) {
				// Find GroupComponent Reference Class
				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;
				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			}
			AddMethodIncludes (Manifest, deps);

			if (deps.Count > 0) {
				foreach (var prj in deps) {
					sb.Append ("../../../../.." + prj.Replace ("CIM_", "").Replace ("UNIX_", "") + "/bin/Debug:");
				}
			}
		}


		string AddLinks ()
		{
			System.Text.StringBuilder links = new System.Text.StringBuilder (" -l" + ClassName + "Provider");
			bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
			bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			HashSet<string> deps = new HashSet<string>();

			if (Manifest.SuperClass != null && !Manifest.SuperClass.HaveChildren) {
				deps.Add(CodeWriterBase.GetClassName(Manifest.SuperClass));
			}
			if (isDependency) {

				System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			} else if (isComponent) {
				// Find GroupComponent Reference Class
				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;
				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			}
			AddMethodIncludes (Manifest, deps);

			if (deps.Count > 0) {

				foreach (var prj in deps) {
					links.Append (" -l" + prj + "Provider");
				}
			}
			return links.ToString ();
		}

		void AddLibPaths ()
		{
			bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
			bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			HashSet<string> deps = new HashSet<string>();

			if (Manifest.SuperClass != null && !Manifest.SuperClass.HaveChildren) {
				deps.Add(CodeWriterBase.GetClassName(Manifest.SuperClass));
			}
			if (isDependency) {

				System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			} else if (isComponent) {
				// Find GroupComponent Reference Class
				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;
				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
				foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
					deps.Add (el);
				}
			}
			AddMethodIncludes (Manifest, deps);

			if (deps.Count > 0) {
				foreach (var prj in deps) {
					WriteLine ("        <LibPath>${ProjectDir}/../../../" + prj.Replace ("CIM_", "").Replace ("UNIX_", "") + "/bin/Debug</LibPath>");
				}
			}
		}

		void AddMethodIncludes (ClassManifest target, HashSet<string> pkgs)
		{
			if (target.SuperClass != null)
				AddMethodIncludes (target.SuperClass, pkgs);

			foreach (var method in target.Class.Methods) {
				foreach (var parameter in method.Parameters) {
					System.Management.Internal.CimParameterReference refParam = parameter as System.Management.Internal.CimParameterReference;
					if (refParam != null) {
						var name = refParam.ReferenceClass.ToString ().Replace ("CIM_", "UNIX_");

						var refManifest = GeneratorFactory.Classes.FirstOrDefault (x => !x.HaveChildren && CodeWriterBase.GetClassName (x) == name);
						if (refManifest != null) {
							pkgs.Add (CodeWriterBase.GetClassName (refManifest));
						}
					}
				}
			}
		}

	}
}

