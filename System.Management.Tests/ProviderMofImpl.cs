using System;

namespace System.Management.Tests
{
	internal class ProviderMofImpl : CodeWriterBase
	{
		private string schemasPath;

		public ProviderMofImpl (ClassManifest manifest, string schemasPath)
			: base (manifest)
		{
			this.schemasPath = schemasPath;
		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("instance of PG_Provider");
			WriteLine ("{");
			WriteLine ("\tProviderModuleName = \"{0}Module\";", ClassName);
			WriteLine ("\tName = \"{0}Provider\";", ClassName);
			WriteLine ("};");
			WriteLine ("");
			WriteLine ("instance of PG_ProviderCapabilities");
			WriteLine ("{");
			WriteLine ("\tProviderModuleName = \"{0}Module\";", ClassName);
			WriteLine ("\tProviderName = \"{0}Provider\";", ClassName);
			WriteLine ("\tCapabilityID = \"1\";");
			WriteLine ("\tClassName = \"{0}\";", Class.ClassName.ToString());
			WriteLine ("\tNamespaces = {\"root/cimv2\"};");
			WriteLine ("\tProviderType = { 2,5,7 };");
			WriteLine ("\tSupportedProperties = NULL; // All properties");
			WriteLine ("\tSupportedMethods = NULL; // All methods");
			WriteLine ("};");
			WriteLine("");
			WriteLine ("instance of PG_ProviderCapabilities");
			WriteLine ("{");
			WriteLine ("\tProviderModuleName = \"{0}Module\";", ClassName);
			WriteLine ("\tProviderName = \"{0}Provider\";", ClassName);
			WriteLine ("\tCapabilityID = \"2\";");
			WriteLine ("\tClassName = \"{0}\";");
			WriteLine ("\tNamespaces = {\"root/cimv2\"};");
			WriteLine ("\tProviderType = { 2,5,7 };");
			WriteLine ("\tSupportedProperties = NULL; // All properties");
			WriteLine ("\tSupportedMethods = NULL; // All methods");
			WriteLine ("};");
			WriteLine("");
			Save(System.IO.Path.Combine(schemasPath, ClassName + "20R.mof"));
		}
	}
}

