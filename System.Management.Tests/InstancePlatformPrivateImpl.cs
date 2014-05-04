using System;

namespace System.Management.Tests
{
	internal class InstancePlatformPrivateImpl : CodeWriterBase
	{
		private string os;

		public InstancePlatformPrivateImpl (ClassManifest manifest, string os)
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
			WriteLine ("");
			WriteLine ("#endif");
		}
	}
}

