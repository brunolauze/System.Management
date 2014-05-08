using System;

namespace System.Management.Tests
{
	internal class InstanceHeaderFixture : CodeWriterBase
	{
		public InstanceHeaderFixture (ClassManifest manifest)
			: base(manifest)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include <MockResponseHandler.h>");
			WriteLine ("");
			WriteLine ("#ifndef __CIMFIXTUREBASE_H");
			WriteLine ("#define __CIMFIXTUREBASE_H");
			WriteLine ("");
			WriteLine ("class CIMFixtureBase");
			WriteLine ("{");
			WriteLine ("public:");
			WriteLine ("\tvirtual void Run()=0;");
			WriteLine ("");
			WriteLine ("};");
			WriteLine ("");
			WriteLine ("#endif");
			WriteLine ("");
			//WriteLine ("#include \"CIMFixtureBase.h\"");
			WriteLine ("");
			WriteLine ("class {0}Fixture :", ClassName);
			WriteLine ("\tpublic CIMFixtureBase");
			WriteLine ("{");
			WriteLine ("public:");
			WriteLine ("\t{0}Fixture();", ClassName);
			WriteLine ("\t~{0}Fixture();", ClassName);
			WriteLine ("\tvirtual void Run();");
			AddCustomTests ();
			WriteLine ("");
			WriteLine ("};");
		}

		void AddCustomTests ()
		{
			bool isFirst = true;
			AddCustomTests ("FREEBSD", ref isFirst);
			AddCustomTests ("LINUX", ref isFirst);
			AddCustomTests ("SOLARIS", ref isFirst);
			AddCustomTests ("ZOS", ref isFirst);
			AddCustomTests ("HPUX", ref isFirst);
			AddCustomTests ("TRU64", ref isFirst);
			AddCustomTests ("DARWIN", ref isFirst);
			AddCustomTests ("AIX", ref isFirst);
			AddCustomTests ("STUB", ref isFirst);
			if (!isFirst) {
				WriteLine ("#endif");
			}
		}

		void AddCustomTests(string os, ref bool isFirst)
		{
			var tests = TemplateFactory.GetTests (ClassName, os);
			if (tests == null)
				return;
			WriteLine ("#{0} PEGASUS_OS_{1}", isFirst ? "if" : "elif", os);
			foreach (var test in tests) {
				WriteLine ("\tvirtual void {0}();", test.Name);
			}

			isFirst = false;

		}
	}
}

