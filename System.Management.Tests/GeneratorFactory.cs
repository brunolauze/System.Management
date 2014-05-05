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

		public static string RootPath
		{
			get { return System.IO.Path.Combine (Environment.CurrentDirectory, "openpegasus-providers"); }
		}

		public static string BasePath
		{
			get { return System.IO.Path.Combine (RootPath, "src", "Providers", "UNIXProviders"); }
		}

		public static string SchemaPath
		{
			get { return System.IO.Path.Combine (RootPath, "Schemas"); }
		}


		private static List<SolutionItem> projects = new List<SolutionItem> ();

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

			Console.WriteLine ("Processing {0}...", CodeWriterBase.GetClassName (manifest));

			ClassMofImpl mofImpl = new ClassMofImpl (manifest, SchemaPath);
			mofImpl.Write ();
			ProviderMofImpl provMofImpl = new ProviderMofImpl (manifest, SchemaPath);
			provMofImpl.Write ();
			InstanceHeader header = new InstanceHeader (manifest);


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

			} else {
				InstancePlatformImpl platformImpl = new InstancePlatformImpl (manifest);
				platformImpl.Write ();
				platformImpl.Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + ".cpp"));
				if (!IsDone(manifest.Class.ClassName.ToString())) {
					new InstanceImpl (manifest, "FREEBSD").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_FREEBSD.hpp"));
					new InstanceImpl (manifest, "LINUX").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_LINUX.hpp"));
					new InstanceImpl (manifest, "ZOS").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_ZOS.hpp"));
					new InstanceImpl (manifest, "WIN32").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_WIN32.hpp"));
					new InstanceImpl (manifest, "SOLARIS").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_SOLARIS.hpp"));
					new InstanceImpl (manifest, "HPUX").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_HPUX.hpp"));
					new InstanceImpl (manifest, "VMS").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_VMS.hpp"));
					new InstanceImpl (manifest, "TRU64").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_TRU64.hpp"));
					new InstanceImpl (manifest, "DARWIN").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_DARWIN.hpp"));
					new InstanceImpl (manifest, "AIX").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_AIX.hpp"));
					new InstanceImpl (manifest, "STUB").Save (System.IO.Path.Combine (strBasePath, platformImpl.ClassName + "_STUB.hpp"));
				}
			}

			if (!manifest.HaveChildren) {
				ProviderMakefile makefile = new ProviderMakefile (manifest);
				makefile.Write ();
				makefile.Save(System.IO.Path.Combine (strBasePath, "Makefile"));
				InstanceProviderHeader providerHeader = new InstanceProviderHeader (manifest);
				InstanceProviderImpl providerImpl = new InstanceProviderImpl (manifest);
				providerHeader.Write ();
				providerImpl.Write ();
				providerHeader.Save (System.IO.Path.Combine (strBasePath, providerHeader.ClassName + "Provider.h"));
				providerImpl.Save (System.IO.Path.Combine (strBasePath, providerImpl.ClassName + "Provider.cpp"));
				SingleProviderProjectMaker projectMaker = new SingleProviderProjectMaker (manifest, BasePath);
				projectMaker.Write ();
				projects.Add(projectMaker.Project);
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

		public static void Generate(IEnumerable<ClassManifest> manifests)
		{
			Classes = manifests;
			string folderName = BasePath;
			string testPath = GetTestPath ();
			if (System.IO.Directory.Exists (RootPath)) {
				System.IO.Directory.Delete (RootPath, true);
			}
			Console.WriteLine ("Creating Directory : " + RootPath);
			System.IO.Directory.CreateDirectory (RootPath);
			System.IO.Directory.CreateDirectory (System.IO.Path.Combine (RootPath, "src"));
			System.IO.Directory.CreateDirectory (System.IO.Path.Combine (RootPath, "src", "Providers"));
			System.IO.Directory.CreateDirectory (BasePath);
			System.IO.Directory.CreateDirectory (SchemaPath);
			System.IO.Directory.CreateDirectory (System.IO.Path.Combine (BasePath, "tests"));
			System.IO.Directory.CreateDirectory (testPath);
			List<string> baseFiles = new List<string> ();
			baseFiles.AddRange(System.IO.Directory.GetFiles (Environment.CurrentDirectory, "*.h"));
			baseFiles.AddRange(System.IO.Directory.GetFiles (Environment.CurrentDirectory, "*.c"));
			baseFiles.AddRange(System.IO.Directory.GetFiles (Environment.CurrentDirectory, "*.hpp"));
			baseFiles.AddRange(System.IO.Directory.GetFiles (Environment.CurrentDirectory, "*.hxx"));
			baseFiles.AddRange(System.IO.Directory.GetFiles (Environment.CurrentDirectory, "*.cpp"));

			foreach (var baseFile in baseFiles) {
				System.IO.File.Copy (baseFile, 
					System.IO.Path.Combine (BasePath, new System.IO.FileInfo (baseFile).Name));
			}

			InstanceBaseHeader baseHeader = new InstanceBaseHeader ();

			baseHeader.Write ();
			baseHeader.Save(System.IO.Path.Combine(BasePath, "CIM_ClassBase.h"));

			foreach (var manifest in manifests) {
				Generate (manifest);
			}
			ProviderMain providerMain = new ProviderMain ();
			providerMain.Write ();
			providerMain.Save (System.IO.Path.Combine (BasePath, "UNIX_Main.cpp"));

			FixtureMain fixtureMain = new FixtureMain ();
			fixtureMain.Write ();
			fixtureMain.Save(System.IO.Path.Combine(testPath, "main.cpp"));

			RootMakefile rootMakefile = new RootMakefile ();
			rootMakefile.Write ();
			rootMakefile.Save(System.IO.Path.Combine(RootPath, "Makefile"));

			FolderMakefile srcMakefile = new FolderMakefile ("..", "Providers");
			srcMakefile.Write ();
			srcMakefile.Save(System.IO.Path.Combine(RootPath, "src", "Makefile"));

			FolderMakefile provMakefile = new FolderMakefile ("../..", "UNIXProviders");
			provMakefile.Write ();
			provMakefile.Save(System.IO.Path.Combine(RootPath, "src", "Providers", "Makefile"));

			AddProviderMakefile (manifests);
			string makPath = System.IO.Path.Combine(new System.IO.DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "mak");
			MakFolderCopy.DirectoryCopy (makPath, System.IO.Path.Combine(RootPath, "mak"), true);

			SchemaProjectMaker schemaProject = new SchemaProjectMaker (SchemaPath);
			schemaProject.Write ();

			ClassManagedSchemaImpl classManagedSystem = new ClassManagedSchemaImpl (false);
			classManagedSystem.Write ();
			classManagedSystem.Save (System.IO.Path.Combine (SchemaPath, "UNIX_ManagedSystemSchema20.mof"));
			ClassManagedSchemaImpl provManagedSystem = new ClassManagedSchemaImpl (true);
			provManagedSystem.Write ();
			provManagedSystem.Save (System.IO.Path.Combine (SchemaPath, "UNIX_ManagedSystemSchema20R.mof"));

			ProviderProjectMaker providerProject = new ProviderProjectMaker (BasePath, false, projects);
			providerProject.Write ();
			ProviderProjectMaker providerTestProject = new ProviderProjectMaker (testPath, true, projects);
			providerTestProject.Write ();

			SolutionFileCreator slnCreator = new SolutionFileCreator (projects);
			slnCreator.Write ();
			slnCreator.Save (System.IO.Path.Combine (RootPath, "openpegasus-providers.sln"));
		
			LicenseFile license = new LicenseFile ();
			license.Write ();
			license.Save(System.IO.Path.Combine(RootPath, "LICENSE"));

			System.IO.File.WriteAllText (System.IO.Path.Combine (RootPath, "env_var.status"), "#Defines variables");
		}

		private static void AddProviderMakefile(IEnumerable<ClassManifest> manifests)
		{
			var sb = new System.Text.StringBuilder ("\\\n");
			foreach (var manifest in manifests) {
				sb.AppendLine ("\t" + manifest.Class.ClassName.ToString ().Replace ("CIM_", "").Replace ("UNIX_", "") + "\\");
			}
			FolderMakefile provMakefile = new FolderMakefile ("../../..", sb.ToString());
			provMakefile.Write ();
			provMakefile.Save(System.IO.Path.Combine(BasePath, "Makefile"));

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
			return System.IO.Path.Combine(BasePath, "tests", "UNIXProviders.Tests");
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
			return false;
			/*
			if (ClassesDone.Contains (className))
				return true;
			return false;
			*/
		}
	}
}

