using System;
using System.Linq;

namespace System.Management.Tests
{
	internal class SingleFixtureCreator : CodeWriterBase
	{   
		public SingleFixtureCreator (ClassManifest manifest)
			: base(manifest)
		{
		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include <UNIX_Common.h>");
			WriteLine ("");
			string className = ClassName;
			WriteLine ("#include \"{0}Fixture.h\"", className);
			WriteLine ("");
			WriteLine ("#include <iostream>");
			WriteLine ("");

			WriteLine ("int main (int argc, char *argv[])");
			WriteLine ("{");
			WriteLine ("\tstd::cout << \"Starting Pegasus Providers Testing Framework...\" << std::endl;");
			WriteLine ("");
			WriteLine ("std::cout << \"Testing {0} Provider...\" << endl;", ClassName);
			WriteLine ("");
			string paramName = className.Replace ("UNIX_", "");
			paramName = "__" + paramName.Substring (0, 1).ToLower () + paramName.Substring (1);
			WriteLine ("\t" + className + "Fixture " + paramName + ";\n\t" + paramName + ".Run();");
			WriteLine ("");
			WriteLine ("}");
			WriteLine ("");
		}
	}
}

