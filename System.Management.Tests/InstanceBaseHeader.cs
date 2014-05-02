using System;

namespace System.Management.Tests
{
	internal class InstanceBaseHeader : CodeWriterBase
	{
		public InstanceBaseHeader ()
			: base(null)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#ifndef __CIM_CLASSBASE_H");
			WriteLine ("#define __CIM_CLASSBASE_H");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("#include \"UNIX_Common.h\"");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("class CIM_ClassBase");
			WriteLine ("{");
			WriteLine ("public:");
			WriteLine ("");
			WriteLine ("\tvirtual Boolean initialize()=0;");
			WriteLine ("\tvirtual Boolean load(int&)=0;");
			WriteLine ("\tvirtual Boolean finalize()=0;");
			WriteLine ("\tvirtual Boolean find(const Array<CIMKeyBinding>&)=0;");
			WriteLine ("\tvirtual Boolean validateKey(CIMKeyBinding&) const=0;");
			WriteLine ("");
			WriteLine ("private:");
			WriteLine ("");
			WriteLine ("};");
			WriteLine ("");
			WriteLine ("#endif /* __CIM_CLASSBASE_H */");
		}
	}
}

