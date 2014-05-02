using System;

namespace System.Management.Tests
{
	internal class LicenseFile : CodeWriterBase
	{
		public LicenseFile ()
			: base(null)
		{
		}

		public override void Write ()
		{
			WriteLicense ();
		}
	}
}

