using System;
using System.Linq;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal static class GeneratorFactory
	{
		private static IEnumerable<ClassManifest> _classes;
		private static readonly object _lock = new object();

		public static IEnumerable<ClassManifest> Classes
		{
			get { return _classes; }
			set {
				lock (_lock) {
					_classes = value;
				}
			}
		}

		private const string BasePath = "/usr/home/bruno/workspace/pegasus-providers.tmp/UNIXProviders/";

		public static void Generate(ClassManifest manifest)
		{
			if (manifest.Class.ClassName.ToString ().StartsWith("PG_") ||
				manifest.Class.ClassName.ToString ().StartsWith("PRS_") ||
				manifest.Class.ClassName.ToString ().Contains("SNMP") ||
				manifest.Class.ClassName.ToString ().Contains("UnitOfWork") ||
				manifest.Class.ClassName.ToString ().Contains("UoW") ||
				manifest.Class.ClassName.ToString ().Contains("Telnet") ||
				manifest.Class.ClassName.ToString ().Contains ("J2ee") /* DISABLE JAVA */)
				return;

			InstanceHeader header = new InstanceHeader (manifest);
			//if (IsDone (manifest.Class.ClassName.ToString ()))
			//	return;

			InstanceImpl impl = new InstanceImpl (manifest);

			if (!manifest.HaveChildren) {
				if (!IsDone(manifest.Class.ClassName.ToString())) {
					AddPrivates (manifest, "HPUX");
					AddPrivates (manifest, "LINUX");
					AddPrivates (manifest, "FREEBSD");
					AddPrivates (manifest, "SOLARIS");
					AddPrivates (manifest, "ZOS");
					AddPrivates (manifest, "VMS");
					AddPrivates (manifest, "TRU64");
					AddPrivates (manifest, "DARWIN");
					AddPrivates (manifest, "AIX");
					AddPrivates (manifest, "STUB");

					AddIncludes (manifest, (string)null);
					AddIncludes (manifest, "HPUX");
					AddIncludes (manifest, "LINUX");
					AddIncludes (manifest, "FREEBSD");
					AddIncludes (manifest, "SOLARIS");
					AddIncludes (manifest, "ZOS");
					AddIncludes (manifest, "VMS");
					AddIncludes (manifest, "TRU64");
					AddIncludes (manifest, "DARWIN");
					AddIncludes (manifest, "AIX");
					AddIncludes (manifest, "STUB");

					AddHeaderPrivates (manifest, (string)null);
					AddHeaderPrivates (manifest, "HPUX");
					AddHeaderPrivates (manifest, "LINUX");
					AddHeaderPrivates (manifest, "FREEBSD");
					AddHeaderPrivates (manifest, "SOLARIS");
					AddHeaderPrivates (manifest, "ZOS");
					AddHeaderPrivates (manifest, "VMS");
					AddHeaderPrivates (manifest, "TRU64");
					AddHeaderPrivates (manifest, "DARWIN");
					AddHeaderPrivates (manifest, "AIX");
					AddHeaderPrivates (manifest, "STUB");
				}

				//Add Tests
				AddTests (manifest);
			}
			header.Write ();
			string strBasePath = GetBasePath (header.ClassName, manifest.HaveChildren);
			header.Save(System.IO.Path.Combine(strBasePath, header.ClassName + ".h"));
			if (manifest.HaveChildren) {
				// WILL BE TOTALLY ABSTRACT
				//impl.Write ();
				//impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + ".cpp"));
			} else {
				InstancePlatformImpl platformImpl = new InstancePlatformImpl (manifest);
				platformImpl.Write ();
				platformImpl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + ".cpp"));
				if (!IsDone(manifest.Class.ClassName.ToString())) {
					impl.Write ();
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_FREEBSD.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_LINUX.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_ZOS.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_WIN32.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_SOLARIS.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_HPUX.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_VMS.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_TRU64.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_DARWIN.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_AIX.hpp"));
					impl.Save (System.IO.Path.Combine (strBasePath, impl.ClassName + "_STUB.hpp"));
				}
			}

			if (!manifest.HaveChildren) {
				InstanceProviderHeader providerHeader = new InstanceProviderHeader (manifest);
				InstanceProviderImpl providerImpl = new InstanceProviderImpl (manifest);
				providerHeader.Write ();
				providerImpl.Write ();
				providerHeader.Save (System.IO.Path.Combine (strBasePath, providerHeader.ClassName + "Provider.h"));
				providerImpl.Save (System.IO.Path.Combine (strBasePath, providerImpl.ClassName + "Provider.cpp"));
			}
		}

		static void AddPrivates (ClassManifest manifest, string os)
		{
			InstancePlatformPrivate privateHeader = new InstancePlatformPrivate (manifest, os);
			InstancePlatformPrivateImpl privateHeaderImpl = new InstancePlatformPrivateImpl (manifest, os);
			string strBasePath = GetBasePath (privateHeader.ClassName, manifest.HaveChildren);
			string fileNameHeader = System.IO.Path.Combine (strBasePath, privateHeader.ClassName + "_" + os + ".hxx");
			string fileNameImpl = System.IO.Path.Combine (strBasePath, privateHeaderImpl.ClassName + "_" + os + ".cxx");
			if (!System.IO.File.Exists (fileNameHeader)) 
			{
				privateHeader.Write ();
				privateHeader.Save (fileNameHeader);
			}
			if (!System.IO.File.Exists (fileNameImpl)) 
			{
				privateHeaderImpl.Write ();
				privateHeaderImpl.Save (fileNameImpl);
			}
		}

		static void AddHeaderPrivates (ClassManifest manifest, string os)
		{
			InstanceHeaderPrivate privateHeader = new InstanceHeaderPrivate (manifest, os);
			string strBasePath = GetBasePath (privateHeader.ClassName, manifest.HaveChildren);
			string fileNameHeader = System.IO.Path.Combine (strBasePath, privateHeader.ClassName + "Private" + (string.IsNullOrEmpty(os) ? "" : "_" + os) + ".h");
			if (!System.IO.File.Exists (fileNameHeader)) 
			{
				privateHeader.Write ();
				privateHeader.Save (fileNameHeader);
			}
		}

		static void AddIncludes (ClassManifest manifest, string os)
		{
			InstanceIncludes privateHeader = new InstanceIncludes (manifest, os);
			string strBasePath = GetBasePath (privateHeader.ClassName, manifest.HaveChildren);
			string fileNameHeader = System.IO.Path.Combine (strBasePath, privateHeader.ClassName + "Deps" + (string.IsNullOrEmpty(os) ? "" : "_" + os) + ".h");
			if (!System.IO.File.Exists (fileNameHeader)) 
			{
				privateHeader.Write ();
				privateHeader.Save (fileNameHeader);
			}
		}

		static void AddTests (ClassManifest manifest)
		{
			//Add Tests
			string testPath = GetTestPath ();
			InstanceHeaderFixture headerFixture = new InstanceHeaderFixture (manifest);
			InstanceImplFixture implFixture = new InstanceImplFixture (manifest);
			headerFixture.Write ();
			implFixture.Write ();
			headerFixture.Save(System.IO.Path.Combine(testPath, headerFixture.ClassName + "Fixture.h"));
			implFixture.Save(System.IO.Path.Combine(testPath, implFixture.ClassName + "Fixture.cpp"));
		}

		static void DeleteFiles (string folderName)
		{
			if (IsDone ("CIM_" + folderName))
				return;
			var headers = System.IO.Directory.GetFiles (folderName, "*.h");
			var concretes = System.IO.Directory.GetFiles (folderName, "*.cpp");
			var concretesh = System.IO.Directory.GetFiles (folderName, "*.hpp");
			foreach (var h in headers) {
				System.IO.File.Delete (h);
			}
			foreach (var c in concretes) {
				System.IO.File.Delete (c);
			}
			foreach (var c in concretesh) {
				System.IO.File.Delete (c);
			}
		}

		public static void Generate(IEnumerable<ClassManifest> manifests)
		{
			Classes = manifests;
			string folderName = BasePath;
			var folders = System.IO.Directory.GetDirectories (BasePath);
			string testPath = GetTestPath ();
			DeleteFiles (testPath);
			DeleteFiles (folderName);
			foreach (var f in folders) {
				//System.IO.Directory.Delete (f, true);
				DeleteFiles (folderName);
			}
			System.IO.File.Copy (
				System.IO.Path.Combine (Environment.CurrentDirectory, "CIMFixtureBase.h"),
				System.IO.Path.Combine (testPath, "CIMFixtureBase.h"));

			System.IO.File.Copy (
				System.IO.Path.Combine (Environment.CurrentDirectory, "CIMHelper.h"),
				System.IO.Path.Combine (BasePath, "CIMHelper.h"));

			System.IO.File.Copy (
				System.IO.Path.Combine (Environment.CurrentDirectory, "CIMHelper.cpp"),
				System.IO.Path.Combine (BasePath, "CIMHelper.cpp"));

			System.IO.File.Copy (
				System.IO.Path.Combine (Environment.CurrentDirectory, "UNIX_Common.h"),
				System.IO.Path.Combine (BasePath, "UNIX_Common.h"));

			System.IO.File.Copy (
				System.IO.Path.Combine (Environment.CurrentDirectory, "UNIXProviderBase.h"),
				System.IO.Path.Combine (BasePath, "UNIXProviderBase.h"));

			System.IO.File.Copy (
				System.IO.Path.Combine (Environment.CurrentDirectory, "UNIXProviderBase.cpp"),
				System.IO.Path.Combine (BasePath, "UNIXProviderBase.hpp"));

			InstanceBaseHeader baseHeader = new InstanceBaseHeader ();
			//InstanceBaseImpl baseImpl = new InstanceBaseImpl ();

			baseHeader.Write ();
			//baseImpl.Write ();
			baseHeader.Save(System.IO.Path.Combine(BasePath, "CIM_ClassBase.h"));
			//baseImpl.Save(System.IO.Path.Combine(BasePath, "CIM_ClassBase.cpp"));

			foreach (var manifest in manifests) {
				Generate (manifest);
			}
			ProviderMain providerMain = new ProviderMain ();
			providerMain.Write ();
			providerMain.Save (System.IO.Path.Combine (BasePath, "UNIX_Main.cpp"));

			FixtureMain fixtureMain = new FixtureMain ();
			fixtureMain.Write ();
			fixtureMain.Save(System.IO.Path.Combine(testPath, "main.cpp"));
		}

		private static string GetBasePath(string name, bool haveChildren)
		{
			if (haveChildren)
				return BasePath;

			string folder = System.IO.Path.Combine(BasePath, name.Replace("CIM_", "").Replace("UNIX_", ""));
			if (!System.IO.Directory.Exists (folder)) {
				System.IO.Directory.CreateDirectory (folder);
			}
			return folder;
		}

		private static string GetTestPath()
		{
			return System.IO.Path.Combine(new System.IO.DirectoryInfo (BasePath).Parent.FullName, "tests", "UNIXProviders.Tests");
		}

		public static string[] ClassesDone = new string[] {
			"CIM_ComputerSystem",
			"CIM_UnitaryComputerSystem",
			"CIM_Account",
			"CIM_OperatingSystem",
			"CIM_UnixProcess",
			"CIM_Process"
		};

		public static bool IsDone (string className)
		{
			if (ClassesDone.Contains (className))
				return true;
			return false;
		}
	}
}

