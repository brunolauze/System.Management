using System;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal class ProviderProjectMaker : CodeWriterBase
	{
		private string basePath;
		private bool isTest;
		private IEnumerable<SolutionItem> projects;

		public ProviderProjectMaker (string basePath, bool isTest, IEnumerable<SolutionItem> projects)
			: base(null)
		{
			this.basePath = basePath;
			this.isTest = isTest;
			this.projects = projects;
		}

		public override void Write ()
		{
			WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			WriteLine("<Project DefaultTargets=\"Build\" ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
			WriteLine("  <PropertyGroup>");
			WriteLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
			WriteLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
			WriteLine("    <ProjectGuid>{0}</ProjectGuid>", isTest ? "{FB6D38B1-B280-4DC5-BFA8-4709837175EF}" : "{4DC6B981-CE60-4CD1-9DF5-7AEE2873EBDD}");
			WriteLine("    <Compiler>");
			WriteLine("      <Compiler ctype=\"ClangppCompiler\" />");
			WriteLine("    </Compiler>");
			WriteLine("    <Language>CPP</Language>");
			WriteLine("    <Target>Bin</Target>");
			if (isTest)
			{
				WriteLine ("    <Packages>");
				WriteLine("      <Packages>");
				foreach (var prj in projects) {
					WriteLine ("        <Package file=\"../../{0}/{1}.md.pc\" name=\"{1}\" IsProject=\"True\" />", prj.Name.Replace("CIM_", "").Replace("UNIX_", ""), prj.Name);
				}
				WriteLine("      </Packages>");
				WriteLine("    </Packages>");
			}
			WriteLine("  </PropertyGroup>");
			WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
			WriteLine("    <DebugSymbols>true</DebugSymbols>");
			WriteLine("    <OutputPath>bin\\Debug</OutputPath>");
			WriteLine("    <OutputName>{0}</OutputName>", isTest ? "TestUNIXProviders" : "UNIXProviders");
			if (!isTest) WriteLine("    <CompileTarget>SharedLibrary</CompileTarget>");
			WriteLine("    <DefineSymbols>DEBUG MONODEVELOP</DefineSymbols>");
			WriteLine("    <SourceDirectory>.</SourceDirectory>");
			WriteLine("    <Includes>");
			WriteLine("      <Includes>");
			WriteLine("        <Include>/usr/local/include</Include>");
			WriteLine("        <Include>${ProjectDir}</Include>");
			WriteLine("        <Include>${ProjectDir}/*</Include>");
			if (isTest)
			{
				WriteLine("        <Include>${ProjectDir}/../..</Include>");
			}
			WriteLine("      </Includes>");
			WriteLine("    </Includes>");
			/*
			System.Text.StringBuilder projectLink = new System.Text.StringBuilder ();
			if (isTest) {
				foreach (var prj in projects) {
					projectLink.Append (" -L../../" + prj.Name.Replace ("CIM_", "").Replace ("UNIX_", "") + "/bin/Debug -l" + prj.Name + "Provider");
				}
			}
			*/
			WriteLine("    <ExtraCompilerArguments>-W -Wall -Wno-unused-parameter  -Wno-unused-value -D_GNU_SOURCE -DTHREAD_SAFE -D_REENTRANT -Werror=unused-variable	-DPEGASUS_COMMON_INTERNAL  -DPEGASUS_PLATFORM_FREEBSD_GENERIC_GNU -DPEGASUS_OS_FREEBSD -DPEGASUS_PLATFORM_FREEBSD_X86_64_CLANG -DPEGASUS_USE_SYSLOGS -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPROVIDER_LIB_NS=\"UNIXProvidersLib\" -DPROVIDER_LIB_NS_BEGIN=\"namespace UNIXProvidersLib {\"</ExtraCompilerArguments>");
			WriteLine("    <ExtraLinkerArguments>-L/usr/local/lib/pegasus/lib -L/usr/local/lib -lpegcommon -lpegprovider -lpegclient -lpegslp_client -lpegquerycommon -lpegqueryexpression -lpegcql -lpegwql -lpegrepository -lutil -lgeom -ljail -lkvm -pthread -lcam -lpkg</ExtraLinkerArguments>");
			System.Text.StringBuilder projectPaths = new System.Text.StringBuilder ();
			foreach (var prj in projects) {
				projectPaths.Append ("../../../../" + prj.Name.Replace ("CIM_", "").Replace ("UNIX_", "") + "/bin/Debug:");
			}
			if (isTest)
			{
				WriteLine ("    <EnvironmentVariables>");
				WriteLine ("      <EnvironmentVariables>");
				WriteLine ("        <Variable name=\"LD_LIBRARY_PATH\" value=\"" + projectPaths + ":/usr/local/lib/pegasus/lib:/usr/local/lib:/usr/lib:/lib\" />");
				WriteLine ("      </EnvironmentVariables>");
				WriteLine ("    </EnvironmentVariables>");
				WriteLine ("    <Commandlineparameters></Commandlineparameters>");
				WriteLine ("    <ConsolePause>false</ConsolePause>");
				WriteLine ("    <LibPaths>");
				WriteLine ("      <LibPaths>");
				WriteLine ("        <LibPath>${ProjectDir}/../../bin/Debug</LibPath>");
				WriteLine ("    </LibPaths>");
				WriteLine ("    </LibPaths>");
			}
			WriteLine("  </PropertyGroup>");
			WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
			WriteLine("    <OutputPath>bin\\Release</OutputPath>");
			WriteLine("    <OutputName>{0}</OutputName>", isTest ? "TestUNIXProviders" : "UNIXProviders");
			if (!isTest) WriteLine("    <CompileTarget>SharedLibrary</CompileTarget>");
			WriteLine("    <OptimizationLevel>3</OptimizationLevel>");
			WriteLine("    <DefineSymbols>MONODEVELOP</DefineSymbols>");
			WriteLine("    <SourceDirectory>.</SourceDirectory>");
			WriteLine("	   <ExtraCompilerArguments>-W -Wall -Wno-unused-parameter  -Wno-unused-value -D_GNU_SOURCE -DTHREAD_SAFE -D_REENTRANT -Werror=unused-variable	-DPEGASUS_COMMON_INTERNAL  -DPEGASUS_PLATFORM_FREEBSD_GENERIC_GNU -DPEGASUS_OS_FREEBSD -DPEGASUS_PLATFORM_FREEBSD_X86_64_CLANG -DPEGASUS_USE_SYSLOGS -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPEGASUS_ARCH_LIB=\"lib\" -DPEGASUS_ENABLE_USERGROUP_AUTHORIZATION -DPEGASUS_ENABLE_CQL -DPEGASUS_DEFAULT_ENABLE_OOP -DPEGASUS_ENABLE_EXECQUERY -DPEGASUS_HAS_SSL -DPEGASUS_SSL_RANDOMFILE -DPEGASUS_ENABLE_SSL_CRL_VERIFICATION -DPEGASUS_ENABLE_AUDIT_LOGGER -DPEGASUS_ENABLE_IPV6 -DPEGASUS_ENABLE_INDICATION_COUNT -DPEGASUS_ENABLE_SLP -DPEGASUS_USE_EXTERNAL_SLP_TYPE=1 -DPEGASUS_ENABLE_INTEROP_PROVIDER -DPEGASUS_USE_EXPERIMENTAL_INTERFACES -DPEGASUS_ENABLE_CMPI_PROVIDER_MANAGER -DPEGASUS_DEST_LIB_DIR=\"lib\" -DPEGASUS_ENABLE_PROTOCOL_WSMAN -DPEGASUS_PAM_AUTHENTICATION -DPEGASUS_NO_PASSWORDFILE -DPEGASUS_ENABLE_PROTOCOL_BINARY -DPEGASUS_EXTRA_PROVIDER_LIB_DIR=\"\" -DPROVIDER_LIB_NS=\"UNIXProvidersLib\" -DPROVIDER_LIB_NS_BEGIN=\"namespace UNIXProvidersLib {\"</ExtraCompilerArguments>");
			WriteLine("    <ExtraLinkerArguments>-L/usr/local/lib/pegasus/lib -L/usr/local/lib -lpegcommon -lpegprovider -lpegclient -lpegslp_client -lpegquerycommon -lpegqueryexpression -lpegcql -lpegwql -lpegrepository -lutil -lgeom -ljail -lkvm -pthread -lcam -lpkg</ExtraLinkerArguments>");
			if (isTest)
			{
				WriteLine ("    <EnvironmentVariables>");
				WriteLine ("      <EnvironmentVariables>");


				WriteLine("        <Variable name=\"LD_LIBRARY_PATH\" value=\"" + projectPaths + ":/usr/local/lib/pegasus/lib:/usr/local/lib:/usr/lib:/lib\" />");
				WriteLine("      </EnvironmentVariables>");
				WriteLine("    </EnvironmentVariables>");
				WriteLine ("    <Commandlineparameters></Commandlineparameters>");
				WriteLine ("    <ConsolePause>false</ConsolePause>");
				WriteLine ("    <LibPaths>");
				WriteLine ("      <LibPaths>");
				WriteLine ("        <LibPath>${ProjectDir}/../../bin/Debug</LibPath>");
				WriteLine ("    </LibPaths>");
				WriteLine ("    </LibPaths>");
			}
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
			if (isTest)
				Save(System.IO.Path.Combine(basePath, "UNIXProviders.Tests.cproj"));
			else
				Save(System.IO.Path.Combine(basePath, "UNIXProviders.cproj"));

			string mdString = "Name: UNIXProviders\nDescription: \nVersion: 0.1\nLibs: -L\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/bin/Debug\" -lUNIXProviders\nCflags: -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Account\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ComputerSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/OperatingSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SoftwareElement\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Process\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AccountManagementCapabilities\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CreateDirectoryAction\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DeviceFile\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/InstalledSoftwareElement\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/InstalledOS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Cluster\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VirtualComputerSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/OSProcess\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/FileSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DiskDrive\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/EthernetAdapter\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/EthernetPort\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/MessageLog\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/LogEntry\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/LogicalDisk\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Mount\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DiskPartition\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Directory\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/UsersAccess\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DatabaseSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DatabaseService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ApplicationSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Meta_Class\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Processor\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/_External/smbios\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BIOSString\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BIOSElement\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BIOSFeature\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SoftwareFeature\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ComputerSystemPackage\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Group\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AccountOnSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemBIOS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/NFS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/RemoteFileSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/LocalFileSystem\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CurrentTime\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ServiceProcess\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AccountManagementService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/UsersAccount\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ActiveConnection\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/IPNetworkConnection\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/TCPProtocolEndpoint\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/UDPProtocolEndpoint\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/PhysicalMemory\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/FileSpecification\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/RebootAction\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DNSSettingData\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DNSProtocolEndpoint\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemSetting\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Watchdog\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VLAN\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ProcessExecutable\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VirtualSystemSettingData\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/AllocationCapabilities\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/VirtualSystemManagementService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ResourcePool\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/DataFile\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/Service\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemAdministratorGroup\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SystemAdministrator\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/USBController\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/_External/pciconf\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/SCSIController\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/TimeService\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/TimeZone\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/ManagementController\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/BootOSFromFS\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CertificateAuthority\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CertificateManagementCapabilities\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CAHasPublicCertificate\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/PublicKeyCertificate\" -I\"/home/bruno/Projects/openpegasus-providers/src/Providers/UNIXProviders/CredentialManagementCapabilities\"";
			System.IO.File.WriteAllText(System.IO.Path.Combine(basePath, "UNIXProviders.md.pc"), mdString);

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
				if ((!file.EndsWith(".c") && !file.EndsWith(".cpp")) || file.EndsWith("Main.cpp"))
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
	}
}

