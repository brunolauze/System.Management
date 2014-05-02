using System;
using System.Linq;

namespace System.Management.Tests
{
	internal class ClassMofImpl : CodeWriterBase
	{
		private string schemasPath;

		public ClassMofImpl (ClassManifest manifest, string schemasPath)
			: base(manifest)
		{
			this.schemasPath = schemasPath;
		}

		private void WriteMof(string targetName)
		{
			targetName = targetName.Replace ("UNIX_", "CIM_")
				.Replace ("CIM_ElectricalSwitch", "CIM_ElectricalSwtich")
				.Replace ("CIM_OpticalDriveDiagnosticTest", "CIM_OpticalDiskDiagnosticTest")
				.Replace ("CIM_ResourcePoolExtentDependency", "CIM_ResourcePoolDriveDependency");
			string dmtfPath = "/usr/ports/net-mgmt/openpegasus/work/openpegasus-2.14.0/Schemas/CIM240/DMTF";
			string[] files = System.IO.Directory.GetFiles (dmtfPath, targetName + ".mof", System.IO.SearchOption.AllDirectories);
			if (files.Length == 0)
				throw new NotImplementedException ("Missing class: " + targetName);
			WriteLine ("");
			WriteLine (System.IO.File.ReadAllText (files [0]));
			WriteLine ("");
		}


		public void WriteMof(ClassManifest target)
		{
			if (target.SuperClass != null) {
				WriteMof (target.SuperClass);
			}
			WriteMof(target.Class.ClassName.ToString());
		}


		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("");
			WriteMof (Manifest);
			WriteLine ("");
			WriteLine ("   [Version( \"2.40.0\" ), Description(\"{0}\")]");
			WriteLine ("class {0} : {1}", ClassName, Class.ClassName);
			WriteLine ("{");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("};");
			WriteLine("");
			Save(System.IO.Path.Combine(schemasPath, ClassName + "20.mof"));}

	}
}

