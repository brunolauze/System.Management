using System;

namespace System.Management.Tests
{
	internal class InstancePlatformPrivate : CodeWriterBase
	{
		private string os;

		public InstancePlatformPrivate (ClassManifest manifest, string os)
			: base(manifest)
		{
			this.os = os;
		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#ifdef PEGASUS_OS_" + os.ToUpper ());
			WriteLine ("");
			WriteLine ("#ifndef __{0}_PRIVATE_H", ClassName.ToUpper());
			WriteLine ("#define __{0}_PRIVATE_H", ClassName.ToUpper());
			WriteLine ("");
			WriteLine ("");


			WriteLine ("#endif");

			WriteLine ("");
			WriteLine ("");
			WriteLine ("#endif");
		}
	}
}

