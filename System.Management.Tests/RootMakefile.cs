using System;

namespace System.Management.Tests
{
	internal class RootMakefile : CodeWriterBase
	{
		public RootMakefile ()
			: base(null)
		{
		}

		public override void Write ()
		{
			WriteLicense ("#");
			WriteLine ("");
			WriteLine ("ROOT = .");
			WriteLine ("include $(ROOT)/mak/config.mak");
		
			WriteLine ("DIRS = \\");
			WriteLine ("\tsrc \\");
			WriteLine ("\tSchemas");
			WriteLine ("");
			WriteLine ("include $(ROOT)/mak/recurse.mak");

			WriteLine (".PHONY: FORCE");
			WriteLine ("FORCE:");

			WriteLine ("install:");
			WriteLine ("\t$(MKDIRHIER) $(DESTDIR)$(PREFIX)/lib/pegasus/lib");
			WriteLine ("\t$(foreach i, $(wildcard $(PEGASUS_HOME)/lib/*.so), install -s -m 444 $(i) $(DESTDIR)$(PREFIX)/lib/pegasus/lib;)");
			WriteLine ("\t$(foreach i, $(wildcard $(PEGASUS_HOME)/lib/*.so.1), install -s -m 444 $(i) $(DESTDIR)$(PREFIX)/lib/pegasus/lib;)");
			WriteLine ("\t$(MKDIRHIER) $(DESTDIR)$(PREFIX)/share/pegasus");
			WriteLine ("\tcd $(PEGASUS_ROOT)/Schemas && $(foreach i, $(wildcard *.mof), tar pzcf $(PEGASUS_HOME)/UNIXProviders.cimpkg $(i)");
			WriteLine ("\tinstall -m 444 $(PEGASUS_ROOT)/Schemas/UNIXProviders.cimpkg $(DESTDIR)$(PREFIX)/share/pegasus/UNIXProviders.cimpkg");
		}
	}
}

