using System;

namespace System.Management.Tests
{
	internal class InstancePlatformImpl : CodeWriterBase
	{
		public InstancePlatformImpl (ClassManifest manifest)
			: base (manifest)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include \"{0}.h\"", ClassName);
			WriteLine ("");
			WriteLine ("#if defined(PEGASUS_OS_HPUX)");
			WriteLine ("#\tinclude \"{0}_HPUX.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_HPUX.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_LINUX)");
			WriteLine ("#\tinclude \"{0}_LINUX.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_LINUX.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_DARWIN)");
			WriteLine ("#\tinclude \"{0}_DARWIN.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_DARWIN.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_AIX)");
			WriteLine ("#\tinclude \"{0}_AIX.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_AIX.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_FREEBSD)");
			WriteLine ("#\tinclude \"{0}_FREEBSD.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_FREEBSD.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_SOLARIS)");
			WriteLine ("#\tinclude \"{0}_SOLARIS.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_SOLARIS.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_ZOS)");
			WriteLine ("#\tinclude \"{0}_ZOS.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_ZOS.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_VMS)");
			WriteLine ("#\tinclude \"{0}_VMS.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_VMS.hpp\"", ClassName);
			WriteLine ("#elif defined(PEGASUS_OS_TRU64)");
			WriteLine ("#\tinclude \"{0}_TRU64.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_TRU64.hpp\"", ClassName);
			WriteLine ("#else");
			WriteLine ("#\tinclude \"{0}_STUB.hxx\"", ClassName);
			WriteLine ("#\tinclude \"{0}_STUB.hpp\"", ClassName);
			WriteLine ("#endif");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("Boolean {0}::validateKey(CIMKeyBinding &kb) const", ClassName);
			WriteLine ("{");
			WriteLine ("");
			WriteLine ("\t/* Keys  */");
			if (IsKeyed(Manifest)) {
				var keys = GetKeyProperties ();
				foreach (var p in keys)
				{
					WriteLine ("\t//{0}", p.Name.ToString ());
				}
				WriteLine ("");
				WriteLine ("\tCIMName name = kb.getName();");
				Write ("\tif (");
				bool firstkey = true;
				foreach (var p in keys)
				{
					if (!firstkey)
						Write (" ||\n\t\t\t");
					Write ("name.equal({0})",  GetPropertyDeclaration(p.Name.ToString ()));
					firstkey = false;
				}
				Write ("\n\t\t)\n\t\t\treturn true;\n");
				WriteLine ("");
			} 
			WriteLine ("\treturn false;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("void {0}::setScope(CIMName scope)", ClassName);
			WriteLine ("{");
			WriteLine ("\tcurrentScope = scope;");
			WriteLine ("}");
			WriteLine ("");
		}
	}
}

