using System;
using System.Linq;

namespace System.Management.Tests
{
	internal class FixtureMain : CodeWriterBase
	{   
		public FixtureMain ()
			: base(null)
		{
		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include <UNIX_Common.h>");
			WriteLine ("");
			foreach (var m in GeneratorFactory.Classes.OrderBy(x => x.Class.ClassName.ToString())) {
				if (!m.HaveChildren) {
					string className = FixExceptions(m.Class.ClassName.ToString ()).Replace ("CIM_", "UNIX_");
					WriteLine ("#include \"{0}Fixture.h\"", className);
				}
			}
			WriteLine ("");
			WriteLine ("#include <iostream>");
			WriteLine ("");
			WriteLine ("Boolean IsTarget(String s, String target)");
			WriteLine ("{");
			WriteLine ("\tif (String::equal(s, CIMHelper::EmptyString) ||");
			WriteLine ("\t\tString::equalNoCase(s, target))");
			WriteLine ("\t\t\treturn Boolean(true);");
			WriteLine ("\treturn Boolean(false);");
			WriteLine ("}");
			WriteLine ("");


			WriteLine ("int main (int argc, char *argv[])");
			WriteLine ("{");
			WriteLine ("\tstd::cout << \"Starting Pegasus Providers Testing Framework...\" << std::endl;");
			WriteLine ("");
			WriteLine ("\tString s;");
			WriteLine ("\tif (argc <= 1)");
			WriteLine ("\t\ts.assign(CIMHelper::EmptyString);");
			WriteLine ("\telse");
			WriteLine ("\t\ts.assign(argv[1]);");
			WriteLine ("");
			foreach (var m in GeneratorFactory.Classes.OrderBy(x => x.Class.ClassName.ToString())) {
				if (!m.HaveChildren)
				{
					string className = FixExceptions(m.Class.ClassName.ToString ()).Replace ("CIM_", "UNIX_");
					string paramName = className.Replace ("UNIX_", "");
					paramName = "__" + paramName.Substring (0, 1).ToLower () + paramName.Substring (1);
					WriteLine ("\tif (IsTarget(s, \"" + className + "\")) { " + className + "Fixture " + paramName + "; " + paramName + ".Run(); }");
				}
			}
			WriteLine ("");
			WriteLine ("}");
			WriteLine ("");
		}
	}
}

