using System;

namespace System.Management.Tests
{
	internal class InstanceHeaderPrivate : CodeWriterBase
	{
		private string os;

		public InstanceHeaderPrivate (ClassManifest manifest, string os)
			: base(manifest)
		{
			this.os = os;
		}

		public override void Write ()
		{
			if (string.IsNullOrEmpty (os)) {
				WriteLine ("");
				WriteLine ("#if defined(PEGASUS_OS_HPUX)");
				WriteLine ("#\tinclude \"{0}Private_HPUX.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_LINUX)");
				WriteLine ("#\tinclude \"{0}Private_LINUX.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_DARWIN)");
				WriteLine ("#\tinclude \"{0}Private_DARWIN.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_AIX)");
				WriteLine ("#\tinclude \"{0}Private_AIX.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_FREEBSD)");
				WriteLine ("#\tinclude \"{0}Private_FREEBSD.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_SOLARIS)");
				WriteLine ("#\tinclude \"{0}Private_SOLARIS.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_ZOS)");
				WriteLine ("#\tinclude \"{0}Private_ZOS.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_VMS)");
				WriteLine ("#\tinclude \"{0}Private_VMS.h\"", ClassName);
				WriteLine ("#elif defined(PEGASUS_OS_TRU64)");
				WriteLine ("#\tinclude \"{0}Private_TRU64.h\"", ClassName);
				WriteLine ("#else");
				WriteLine ("#\tinclude \"{0}Private_STUB.h\"", ClassName);
				WriteLine ("#endif");
			} else {
				WriteLine ("#if defined(PEGASUS_OS_{0})", os);
				WriteLine ("");
				WriteLine ("");
				WriteLine ("");
				WriteLine ("#endif");
			}
		}
	}
}

