using System;

namespace System.Management.Tests
{
	internal class ClassManagedSchemaImpl : CodeWriterBase
	{
		private bool isProvider;

		public ClassManagedSchemaImpl (bool isProvider)
			: base(null)
		{
			this.isProvider = isProvider;
		}

		public override void Write ()
		{
			WriteLicense ("");
			WriteLine ("");
			WriteLine ("#pragma include (\"Meta_Class{0}.mof\")", isProvider ? "20R" : "20");

			foreach (var obj in GeneratorFactory.Classes) {
				WriteLine ("#pragma include (\"{0}{1}.mof\")", CodeWriterBase.GetClassName(obj), isProvider ? "20R" : "20");
			}
			WriteLine ("");
		}
	}
}

