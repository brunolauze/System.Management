using System;
using System.Linq;

namespace System.Management.Tests
{
	internal class ProviderMain : CodeWriterBase
	{
		public ProviderMain ()
			: base(null)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include <Pegasus/Common/Config.h>");
			WriteLine ("#include <Pegasus/Common/String.h>");
			WriteLine ("#include <Pegasus/Common/PegasusVersion.h>");
			WriteLine ("#include <UNIX_Common.h>");
			WriteLine ("");
			WriteLine ("PEGASUS_USING_PEGASUS;");
			WriteLine ("PEGASUS_USING_STD;");
			WriteLine ("");
			foreach (var m in GeneratorFactory.Classes.OrderBy(x => x.Class.ClassName.ToString())) {
				if (IsEnabled(m)) {
					WriteLine ("{0}#include <{1}/{2}Provider.h>", GeneratorFactory.IsDone(FixExceptions(m.Class.ClassName.ToString())) ? "" : "//", FixExceptions(m.Class.ClassName.ToString()).Replace("CIM_", "").Replace("UNIX_", ""),  FixExceptions(m.Class.ClassName.ToString()).Replace("CIM_", "UNIX_"));
				}
			}
			WriteLine ("");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("extern \"C\"");
			WriteLine ("PEGASUS_EXPORT CIMProvider* PegasusCreateProvider(const String& providerName)");
			WriteLine ("{");
			WriteLine ("\tif (String::equalNoCase(providerName, CIMHelper::EmptyString)) return NULL;");
			foreach (var m in GeneratorFactory.Classes.OrderBy(x => x.Class.ClassName.ToString())) {
				if (IsEnabled(m)) {
					WriteLine ("\t{0}else if (String::equalNoCase(providerName, \"{1}Provider\")) return new {1}Provider();", GeneratorFactory.IsDone(FixExceptions(m.Class.ClassName.ToString())) ? "" : "//", FixExceptions(m.Class.ClassName.ToString()).Replace("CIM_", "UNIX_"));
				}
			}
			WriteLine ("\treturn NULL;");
			WriteLine ("}");
		}

		private bool IsEnabled(ClassManifest manifestItem)
		{
			if (manifestItem.HaveChildren)
				return false;
			/*
			if (GeneratorFactory.ClassesDone.Contains (manifestItem.Class.ClassName.ToString ().Replace ("CIM_", "UNIX_")))
				return true;
			return false;
			*/
			return true;
		}
	}
}

