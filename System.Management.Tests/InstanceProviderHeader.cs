using System;

namespace System.Management.Tests
{
	internal class InstanceProviderHeader : CodeWriterBase
	{
		public InstanceProviderHeader (ClassManifest manifest)
			: base(manifest)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine("");
			WriteLine("");
			WriteLine("#ifndef __{0}_PROVIDER_H", ClassName);
			WriteLine("#define __{0}_PROVIDER_H", ClassName);

			WriteLine("#include \"{0}.h\"", ClassName);
			WriteLine("");
			WriteLine("#define UNIX_PROVIDER {0}Provider", ClassName);
			WriteLine("#define CLASS_IMPLEMENTATION {0}", ClassName);
			WriteLine("#define CLASS_IMPLEMENTATION_NAME \"{0}\"", ClassName);
			WriteLine ("#define BASE_CLASS_NAME \"{0}\"", Manifest.Class.ClassName.ToString());
			int keyCount = GetKeyCount (Manifest);
			WriteLine ("#define NUMKEYS_CLASS_IMPLEMENTATION {0}", keyCount);
			WriteLine("");
			WriteLine("");
			WriteLine("#include \"UNIXProviderBase.h\"");
			WriteLine("");
			WriteLine("#endif");
			WriteLine("");
			WriteLine ("#undef UNIX_PROVIDER");
			WriteLine ("#undef CLASS_IMPLEMENTATION");
			WriteLine ("#undef CLASS_IMPLEMENTATION_NAME");
			WriteLine ("#undef BASE_CLASS_NAME");
			WriteLine ("#undef NUMKEYS_CLASS_IMPLEMENTATION");
		}
	}
}

