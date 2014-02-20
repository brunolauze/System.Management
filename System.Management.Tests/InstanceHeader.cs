using System;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal class InstanceHeader : CodeWriterBase
	{
		public InstanceHeader (ClassManifest manifest)
			: base (manifest)
		{

		}

		public override void Write()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#ifndef __{0}_H", ClassName.ToUpper());
			WriteLine ("#define __{0}_H", ClassName.ToUpper());
			WriteLine ("");
			WriteLine ("");

			bool isComponent = DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			IEnumerable<string> groupNames = null;
			IEnumerable<string> partNames = null;
			if (this.Manifest.SuperClass == null) {
				WriteLine ("#include \"CIM_ClassBase.h\"");
			}
			else {
				if (SuperClass.Contains("UNIX_"))
					WriteLine ("#include <{0}/{1}.h>", SuperClass.Replace("CIM_", "").Replace("UNIX_", ""), SuperClass);
				else
					WriteLine ("#include \"{0}.h\"", SuperClass);
			}
			if (isComponent) {
				// Find GroupComponent Reference Class

				System.Management.Internal.CimPropertyReference r1 = Manifest.Class.Properties ["GroupComponent"] as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = Manifest.Class.Properties ["PartComponent"] as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
				partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());

				foreach (var g in groupNames) {
					WriteLine ("#include <{0}/{1}.h>", g.Replace("CIM_", "").Replace("UNIX_", ""), g);
				}
				foreach (var p in partNames) {
					WriteLine ("#include <{0}/{1}.h>", p.Replace("CIM_", "").Replace("UNIX_", ""), p);
				}
			}

			WriteLine ("");
			if (!Manifest.HaveChildren) {
				WriteLine ("#include \"{0}Deps.h\"", ClassName);
				WriteLine ("");
			}
			WriteLine ("");
			DefineProperties (this.Manifest);

			WriteLine ("");
			WriteLine ("");
			WriteLine ("class {0} :", ClassName);
			WriteLine ("\tpublic {0}", SuperClass);
			WriteLine ("{");
			WriteLine ("public:");
			WriteLine ("");
			if (!Manifest.HaveChildren) {
				WriteLine ("\t{0}();", ClassName);
				WriteLine ("\t~{0}();", ClassName);
				WriteLine ("");
			}

			WriteLine ("\tvirtual Boolean initialize(){0};", DeclarationEnding);
			WriteLine ("\tvirtual Boolean load(int&){0};", DeclarationEnding);
			WriteLine ("\tvirtual Boolean finalize(){0};", DeclarationEnding);
			WriteLine ("\tvirtual Boolean find(Array<CIMKeyBinding>&){0};", DeclarationEnding);
			WriteLine ("\tvirtual Boolean validateKey(CIMKeyBinding&) const{0};", DeclarationEnding);
			WriteLine ("\tvirtual void setScope(CIMName){0};", DeclarationEnding);
			WriteLine ("");
			List<string> added = new List<string> ();
			DefinePropertiesGetter (Manifest, added, Manifest.HaveChildren);
			WriteLine ("");
			WriteLine ("private:");
			if (!Manifest.HaveChildren) {
				WriteLine ("\tCIMName currentScope;");
				WriteLine ("");
				if (!Manifest.HaveChildren) {
					WriteLine ("#\tinclude \"{0}Private.h\"", ClassName);
					WriteLine ("");
				}
				if (isComponent) {
					WriteLine ("\tint groupIndex;");
					WriteLine ("\tint partIndex;");

					foreach (var g in groupNames) {
						WriteLine ("\t{0} group_{0}_Component;", g);
						WriteLine ("\tint group_{0}_Index;", g);
						WriteLine ("\tbool endOf_{0}_Group;", g);
					}
					foreach (var p in partNames) {
						WriteLine ("\t{0} part_{0}_Component;", p);
						WriteLine ("\tint part_{0}_Index;", p);
						WriteLine ("\tbool endOf_{0}_Part;", p);
					}
						
					WriteLine ("");

				}
			}
			WriteLine ("");
			WriteLine ("};");
			WriteLine ("");
			WriteLine ("#endif /* {0} */", ClassName.ToUpper());
		}

		private string FixComponentClassName (string name)
		{
			if (name == "CIM_System") 
			{
				return "CIM_ComputerSystem";
			}
			return name;
		}
	}
}

