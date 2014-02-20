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
			WriteLine ("#include \"CIMFixtureBase.h\"");
			WriteLine ("");
			WriteLine ("class {0}Fixture :", ClassName);
			WriteLine ("\tpublic CIMFixtureBase");
			WriteLine ("{");
			WriteLine ("public:");
			WriteLine ("\t{0}Fixture();", ClassName);
			WriteLine ("\t~{0}Fixture();", ClassName);
			WriteLine ("\tvirtual void Run();");
			WriteLine ("");
			WriteLine ("};");
		}
	}
}

