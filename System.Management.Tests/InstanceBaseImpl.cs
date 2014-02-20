using System;

namespace System.Management.Tests
{
	internal class InstanceBaseImpl : CodeWriterBase
	{
		public InstanceBaseImpl ()
			: base(null)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include \"CIM_ClassBase.h\"");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("CIM_ClassBase::CIM_ClassBase(void)");
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("CIM_ClassBase::~CIM_ClassBase(void)");
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("Boolean CIM_ClassBase::initialize()");
			WriteLine ("{");
			WriteLine ("\treturn false;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("Boolean CIM_ClassBase::load(int &pIndex)");
			WriteLine ("{");
			WriteLine ("\treturn false;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("Boolean CIM_ClassBase::finalize()");
			WriteLine ("{");
			WriteLine ("\treturn false;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("Boolean CIM_ClassBase::find(Array<CIMKeyBinding> &kbArray)");
			WriteLine ("{");
			WriteLine ("\treturn false;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("Boolean CIM_ClassBase::validateKey(CIMKeyBinding &kb) const");
			WriteLine ("{");
			WriteLine ("\treturn false;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("");
		}
	}
}

