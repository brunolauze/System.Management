using System;

namespace System.Management.Tests
{
	internal class FolderMakefile : CodeWriterBase
	{
		private string root; 
		private string folderNames;

		public FolderMakefile (string root, string folderNames)
			: base(null)
		{
			this.root = root;
			this.folderNames = folderNames;
		}

		public override void Write ()
		{
			WriteLicense ("#");
			WriteLine ("");
			WriteLine ("ROOT = {0}", root);
			WriteLine ("include $(ROOT)/mak/config.mak");
			WriteLine ("DIRS = ", folderNames);
			WriteLine ("include $(ROOT)/mak/recurse.mak");
		}

	}
}

