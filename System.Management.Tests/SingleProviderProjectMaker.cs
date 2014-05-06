using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Management.Tests
{
	internal class SingleProviderProjectMaker : CodeWriterBase
	{
		private string basePath;
		private Guid id;
		public SingleProviderProjectMaker (ClassManifest manifest, string basePath)
			: base(manifest)
		{
			this.id = Guid.NewGuid ();
			this.basePath = System.IO.Path.Combine (basePath, ClassName.Replace ("CIM_", "").Replace ("UNIX_", ""));
		}

		public SolutionItem Project
		{
			get { return new SolutionItem(ClassName, this.id, false); }
		}

		public override void Write ()
		{
			WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			WriteLine("<Project DefaultTargets=\"Build\" ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
			WriteLine("  <PropertyGroup>");
			WriteLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
			WriteLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
			WriteLine("    <ProjectGuid>{" + id.ToString().ToUpper() + "}</ProjectGuid>");
			WriteLine("    <Compiler>");
			WriteLine("      <Compiler ctype=\"ClangppCompiler\" />");
			WriteLine("    </Compiler>");
			WriteLine("    <Language>CPP</Language>");
			WriteLine("    <Target>Bin</Target>");
			AddPackages ();
			AddLibPaths ();
			WriteLine("  </PropertyGroup>");
			WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
			WriteLine("    <DebugSymbols>true</DebugSymbols>");
			WriteLine("    <OutputPath>bin\\Debug</OutputPath>");
			WriteLine("    <OutputName>{0}Provider</OutputName>", ClassName);
			WriteLine("    <CompileTarget>SharedLibrary</CompileTarget>");
			WriteLine("    <DefineSymbols>DEBUG MONODEVELOP</DefineSymbols>");
			WriteLine("    <SourceDirectory>.</SourceDirectory>");
			WriteLine("    <Includes>");
			WriteLine("      <Includes>");
			WriteLine("        <Include>/usr/local/include</Include>");
			WriteLine("        <Include>${ProjectDir}</Include>");
			WriteLine("        <Include>${ProjectDir}/..</Include>");
			WriteLine("      </Includes>");
			WriteLine("    </Includes>");
			WriteLine("    <ExtraCompilerArguments>-W -Wall -Wno-unused-parameter  -Wno-unused-value -D_GNU_SOURCE -DTHREAD_SAFE -D_REENTRANT -Werror=unused-variable	-DPEGASUS_COMMON_INTERNAL  -DPEGASUS_PLATFORM_FREEBSD_GENERIC_GNU -DPEGASUS_OS_FREEBSD -DPEGASUS_PLATFORM_FREEBSD_X86_64_CLANG -DPEGASUS_USE_SYSLOGS -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPROVIDER_LIB_NS=\"namespace " + ClassName + "Lib {\"</ExtraCompilerArguments>");
			WriteLine("    <ExtraLinkerArguments>-L/usr/local/lib/pegasus/lib -L/usr/local/lib -lpegcommon -lpegprovider -lpegclient -lpegslp_client -lpegquerycommon -lpegqueryexpression -lpegcql -lpegwql -lpegrepository -lutil -lgeom -ljail -lkvm -pthread -lcam -lpkg" + AddLinks() + "</ExtraLinkerArguments>");
			WriteLine("  </PropertyGroup>");
			WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
			WriteLine("    <OutputPath>bin\\Release</OutputPath>");
			WriteLine("    <OutputName>{0}Provider</OutputName>", ClassName);
			WriteLine("    <CompileTarget>SharedLibrary</CompileTarget>");
			WriteLine("    <OptimizationLevel>3</OptimizationLevel>");
			WriteLine("    <DefineSymbols>MONODEVELOP</DefineSymbols>");
			WriteLine("    <SourceDirectory>.</SourceDirectory>");
			WriteLine("	   <ExtraCompilerArguments>-W -Wall -Wno-unused-parameter  -Wno-unused-value -D_GNU_SOURCE -DTHREAD_SAFE -D_REENTRANT -Werror=unused-variable	-DPEGASUS_COMMON_INTERNAL  -DPEGASUS_PLATFORM_FREEBSD_GENERIC_GNU -DPEGASUS_OS_FREEBSD -DPEGASUS_PLATFORM_FREEBSD_X86_64_CLANG -DPEGASUS_USE_SYSLOGS -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPROVIDER_LIB_NS=\"namespace " + ClassName + "Lib {\"</ExtraCompilerArguments>");
			WriteLine("    <ExtraLinkerArguments>-L/usr/local/lib/pegasus/lib -L/usr/local/lib -lpegcommon -lpegprovider -lpegclient -lpegslp_client -lpegquerycommon -lpegqueryexpression -lpegcql -lpegwql -lpegrepository -lutil -lgeom -ljail -lkvm -pthread -lcam -lpkg" + AddLinks() + "</ExtraLinkerArguments>");
			WriteLine("  </PropertyGroup>");
			string[] folders = System.IO.Directory.GetDirectories(basePath);
			WriteLine ("  <ItemGroup>");
			WriteLine ("    <Compile Include=\"{0}\">", "../CIMHelper.cpp");
			WriteLine ("        <Link>CIMHelper.cpp</Link>");
			WriteLine ("    </Compile>");
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

			Save(System.IO.Path.Combine(basePath, ClassName +  ".cproj"));

			string mdString = "Name: " + ClassName + "Provider\nDescription: \nVersion: 0.1\nLibs: "; //-L\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/bin/Debug\" -lUNIXProviders\nCflags: -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Account\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ComputerSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/OperatingSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SoftwareElement\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Process\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AccountManagementCapabilities\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CreateDirectoryAction\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DeviceFile\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/InstalledSoftwareElement\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/InstalledOS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Cluster\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VirtualComputerSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/OSProcess\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/FileSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DiskDrive\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/EthernetAdapter\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/EthernetPort\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/MessageLog\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/LogEntry\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/LogicalDisk\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Mount\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DiskPartition\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Directory\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/UsersAccess\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DatabaseSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DatabaseService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ApplicationSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Meta_Class\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Processor\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/_External/smbios\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BIOSString\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BIOSElement\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BIOSFeature\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SoftwareFeature\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ComputerSystemPackage\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Group\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AccountOnSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemBIOS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/NFS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/RemoteFileSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/LocalFileSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CurrentTime\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ServiceProcess\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AccountManagementService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/UsersAccount\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ActiveConnection\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/IPNetworkConnection\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/TCPProtocolEndpoint\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/UDPProtocolEndpoint\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/PhysicalMemory\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/FileSpecification\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/RebootAction\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DNSSettingData\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DNSProtocolEndpoint\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemSetting\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Watchdog\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VLAN\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ProcessExecutable\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VirtualSystemSettingData\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AllocationCapabilities\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VirtualSystemManagementService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ResourcePool\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DataFile\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Service\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemAdministratorGroup\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemAdministrator\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/USBController\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/_External/pciconf\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SCSIController\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/TimeService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/TimeZone\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ManagementController\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BootOSFromFS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CertificateAuthority\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CertificateManagementCapabilities\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CAHasPublicCertificate\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/PublicKeyCertificate\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CredentialManagementCapabilities\"";
			System.IO.File.WriteAllText(System.IO.Path.Combine(basePath, ClassName + ".md.pc"), mdString);

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
				if ((file.EndsWith(".c") || file.EndsWith(".cpp")) && !file.EndsWith("Main.cpp"))
					WriteLine("    <Compile Include=\"{0}/{1}\" />", new System.IO.DirectoryInfo(dir).Name, file);
			}
			WriteLine("  </ItemGroup>");
			WriteLine("  <ItemGroup>");
			foreach (var filePath in files) {
				string file = new System.IO.FileInfo (filePath).Name;
				if ((!file.EndsWith(".c") && !file.EndsWith(".cpp")) || file.EndsWith("Main.cpp"))
					WriteLine("    <Node Include=\"{0}/{1}\" />", new System.IO.DirectoryInfo(dir).Name, file);
			}
		}


		string AddLinks ()
		{
			System.Text.StringBuilder links = new System.Text.StringBuilder ();
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
				WriteLine ("    <LibPaths>");
				WriteLine ("      <LibPaths>");
				foreach (var prj in deps) {
					WriteLine ("        <LibPath>${ProjectDir}/../" + prj.Replace ("CIM_", "").Replace ("UNIX_", "") + "/bin/Debug</LibPath>");
				}
				WriteLine ("    </LibPaths>");
				WriteLine ("    </LibPaths>");
			}
		}		void AddPackages ()
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
				WriteLine ("    <Packages>");
				WriteLine ("      <Packages>");
				foreach (var prj in deps) {
					WriteLine ("        <Package file=\"../{0}/{1}.md.pc\" name=\"{1}\" IsProject=\"True\" />", prj.Replace ("CIM_", "").Replace ("UNIX_", ""), prj);
				}
				WriteLine ("      </Packages>");
				WriteLine ("    </Packages>");
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
						var name = refParam.ReferenceClass.ToString ().Replace("CIM_", "UNIX_");

						var refManifest = GeneratorFactory.Classes.FirstOrDefault (x => !x.HaveChildren && CodeWriterBase.GetClassName (x) == name);
						if (refManifest != null) {
							pkgs.Add(CodeWriterBase.GetClassName (refManifest));
						}
					}
				}
			}

		}
	}
}

