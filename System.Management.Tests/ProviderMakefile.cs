using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Management.Tests
{
	internal class ProviderMakefile : CodeWriterBase
	{
		public ProviderMakefile (ClassManifest manifest)
			: base(manifest)
		{

		}

		public override void Write ()
		{
			WriteLicense ("#");

			WriteLine ("");
			WriteLine ("");
			WriteLine ("ROOT = ../../../..");
			WriteLine ("DIR = Providers/UNIXProviders/" + ClassName.Replace("CIM_", "").Replace("UNIX_", ""));
			WriteLine ("include $(ROOT)/mak/config.mak");
			WriteLine ("LIBRARY = {0}Provider", ClassName);
			WriteLine ("EXTRA_INCLUDES += -I/usr/local/include -I./..");
			WriteLine ("");
			WriteLine ("SOURCES = \\");
			WriteLine ("\t../CIMHelper.cpp \\");
			WriteLine ("\t{0}.cpp", ClassName);
			WriteLine ("\t{0}Provider.cpp", ClassName);
			WriteLine ("\t{0}Main.cpp", ClassName);
			WriteLine ("");

			AddPlatformSources (ClassName);
			List<string> added = new List<string> ();
			AddExtraSources (Manifest, added);


			WriteLine ("EXTRA_LIBRARIES = \\");
			WriteLine ("\t-lpegprovider \\");
			WriteLine ("\t-lpegclient \\");
			WriteLine ("\t-lpegcommon \\");
			WriteLine ("\t-lpegquerycommon \\");
			WriteLine ("\t-lpegqueryexpression \\");
			WriteLine ("\t-lpegcql \\");
			WriteLine ("\t-lpegwql \\");
			WriteLine ("\t-lpthread \\");

			//By Provider Libraries

			WriteLine ("\t-lutil \\");
			WriteLine ("\t-lkvm");


			WriteLine ("include $(ROOT)/mak/dynamic-library.mak");
			WriteLine ("");


			WriteLine ("");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("");
		}

		void AddPlatformSources(string className)
		{
			WriteLine ("ifeq ($(OS), aix)");
			WriteLine ("\tSOURCES += {0}_AIX.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), darwin)");
			WriteLine ("\tSOURCES += {0}_DARWIN.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), freebsd)");
			WriteLine ("\tSOURCES += {0}_FREEBSD.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), hpux)");
			WriteLine ("\tSOURCES += {0}_HPUX.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), linux)");
			WriteLine ("\tSOURCES += {0}_LINUX.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), solaris)");
			WriteLine ("\tSOURCES += {0}_SOLARIS.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), tru64)");
			WriteLine ("\tSOURCES += {0}_TRU64.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), vms)");
			WriteLine ("\tSOURCES += {0}_VMS.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), win32)");
			WriteLine ("\tSOURCES += {0}_WIN32.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq ($(OS), zos)");
			WriteLine ("\tSOURCES += {0}_ZOS.cpp", className);
			WriteLine ("endif");
			WriteLine ("ifeq (, $(filter aix,darwin,freebsd,hpux,linux,solaris,tru64,vms,win32,zos $(OS)))");
			WriteLine ("");
			WriteLine ("else");
			WriteLine ("\tSOURCES += {0}_STUB.cpp", className);
			WriteLine ("endif");
			WriteLine ("");
		}

		void AddExtraSources (ClassManifest manifest, List<string> added)
		{
			bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
			bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			IEnumerable<string> groupNames = null;
			IEnumerable<string> partNames = null;

			if (this.Manifest.SuperClass == null) {

			}
			else {
				added.Add (SuperClass);
				if (SuperClass.Contains ("UNIX_")) {
					AddExtraSources (SuperClass);
				} else {

				}
			}
			if (isDependency) {

				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
				partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());

				foreach (var g in groupNames) {
					if (added.Contains (g))
						continue;
					added.Add (g);
					AddExtraSources(g);
				}
				foreach (var p in partNames) {
					if (added.Contains (p))
						continue;
					added.Add (p);
					AddExtraSources(p);
				}
			}
			else if (isComponent) {
				// Find GroupComponent Reference Class

				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
				partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());

				foreach (var g in groupNames) {
					if (added.Contains (g))
						continue;
					added.Add (g);
					AddExtraSources(g);
				}
				foreach (var p in partNames) {
					if (added.Contains (p))
						continue;
					added.Add (p);
					AddExtraSources(p);
				}
			}
			AddMethodSources (Manifest, added);
		}

		void AddExtraSources(string className)
		{
			WriteLine ("SOURCES += \\");
			WriteLine ("\t../{0}/{1}.cpp", className.Replace("CIM_", "").Replace("UNIX_", ""), className);
			WriteLine ("\t../{0}/{1}Provider.cpp", className.Replace("CIM_", "").Replace("UNIX_", ""), className);
			WriteLine ("");
			AddPlatformSources ("../" + className.Replace("CIM_", "").Replace("UNIX_", "") + "/" + className);
			WriteLine ("");
			WriteLine ("");
		}

		void AddMethodSources (ClassManifest target, List<string> added)
		{
			if (target.SuperClass != null)
				AddMethodSources (target.SuperClass, added);

			foreach (var method in target.Class.Methods) {
				foreach (var parameter in method.Parameters) {
					System.Management.Internal.CimParameterReference refParam = parameter as System.Management.Internal.CimParameterReference;
					if (refParam != null) {
						var name = refParam.ReferenceClass.ToString ().Replace ("CIM_", "UNIX_");
						if (added.Contains (name))
							continue;
						added.Add (name);
						var refManifest = GeneratorFactory.Classes.FirstOrDefault (x => !x.HaveChildren && CodeWriterBase.GetClassName (x) == name);
						if (refManifest != null) {
							AddExtraSources (name);
						}
					}
				}
			}
		}
	}
}

