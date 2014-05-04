using System;

namespace System.Management.Tests
{
	internal class InstanceIncludes : CodeWriterBase
	{
		private string os;

		public InstanceIncludes (ClassManifest manifest, string os)
			: base(manifest)
		{
			this.os = os;
		}

		public override void Write ()
		{
			WriteLicense ();
			if (string.IsNullOrEmpty (os)) {
				WriteLine ("");
				WriteLine ("#if defined(PEGASUS_OS_HPUX)");
				WriteLine ("#\tinclude \"{0}Deps_HPUX.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_LINUX)");
				WriteLine ("#\tinclude \"{0}Deps_LINUX.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_DARWIN)");
				WriteLine ("#\tinclude \"{0}Deps_DARWIN.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_AIX)");
				WriteLine ("#\tinclude \"{0}Deps_AIX.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_FREEBSD)");
				WriteLine ("#\tinclude \"{0}Deps_FREEBSD.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_SOLARIS)");
				WriteLine ("#\tinclude \"{0}Deps_SOLARIS.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_ZOS)");
				WriteLine ("#\tinclude \"{0}Deps_ZOS.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_VMS)");
				WriteLine ("#\tinclude \"{0}Deps_VMS.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_TRU64)");
				WriteLine ("#\tinclude \"{0}Deps_TRU64.h\"", ClassName);
				WriteLine ("#else");
				WriteLine ("#\tinclude \"{0}Deps_STUB.h\"", ClassName);
				WriteLine ("#endif");
			} else {
				WriteLine ("#if defined(PEGASUS_OS_{0})", os);
				WriteLine ("");

				string specific = TemplateFactory.GetDependencies (ClassName, os);
				if (!string.IsNullOrEmpty (specific)) {
					WriteLine (specific);
				}
				else 
					WriteLine ("");

				WriteLine ("");
				WriteLine ("#endif");
			}
		}
	}
}

