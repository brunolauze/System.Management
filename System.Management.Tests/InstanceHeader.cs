using System;
using System.Collections.Generic;
using System.Linq;

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

			foreach (var qualifier in Manifest.Class.Qualifiers) {
				if (qualifier != null) {
					WriteLine ("");
					WriteLine ("/* **** " + qualifier.Name + " *** */");
					WriteLine ("/*");
					foreach (var descVal in qualifier.Values) {
						WriteLine (descVal);
					}
					WriteLine ("*/");
					WriteLine ("");
				}
			}

			WriteLine ("#ifndef __{0}_H", ClassName.ToUpper());
			WriteLine ("#define __{0}_H", ClassName.ToUpper());
			WriteLine ("");
			WriteLine ("");
			List<string> added = new List<string> ();
			bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
			bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
			IEnumerable<string> groupNames = null;
			IEnumerable<string> partNames = null;

			if (this.Manifest.SuperClass == null) {
				WriteLine ("#include \"CIM_ClassBase.h\"");
			}
			else {
				added.Add (SuperClass);
				if (SuperClass.Contains ("UNIX_")) {
					WriteLine ("#include <{0}/{1}.h>", SuperClass.Replace ("CIM_", "").Replace ("UNIX_", ""), SuperClass);
				} else {
					WriteLine ("#include \"{0}.h\"", SuperClass);
				}
			}
			if (isDependency) {

				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
				partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());

				foreach (var g in groupNames) {
					if (added.Contains (g))
						continue;
					added.Add (g);
					WriteLine ("#include <{0}/{1}Provider.h>", g.Replace("CIM_", "").Replace("UNIX_", ""), g);
				}
				foreach (var p in partNames) {
					if (added.Contains (p))
						continue;
					added.Add (p);
					WriteLine ("#include <{0}/{1}Provider.h>", p.Replace ("CIM_", "").Replace ("UNIX_", ""), p);
				}
			}
			else if (isComponent) {
				// Find GroupComponent Reference Class

				System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
				System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;

				//* GET ALL GROUP NAMES DERIVING FROM GROUP */
				groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
				partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());

				foreach (var g in groupNames) {
					if (added.Contains (g))
						continue;
					added.Add (g);
					WriteLine ("#include <{0}/{1}Provider.h>", g.Replace("CIM_", "").Replace("UNIX_", ""), g);
				}
				foreach (var p in partNames) {
					if (added.Contains (p))
						continue;
					added.Add (p);
					WriteLine ("#include <{0}/{1}Provider.h>", p.Replace("CIM_", "").Replace("UNIX_", ""), p);
				}
			}
			AddMethodIncludes (Manifest, added);
			WriteLine ("");
			if (!Manifest.HaveChildren) {
				WriteLine ("#include \"{0}Deps.h\"", ClassName);
				WriteLine ("");
			}
			WriteLine ("");
			added.Clear ();
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
			if (DeriveFrom (Manifest, "CIM_ManagedSystemElement")) {
				WriteLine ("\tvirtual Boolean loadByName(String&){0};", DeclarationEnding);
			}
			WriteLine ("\tvirtual Boolean finalize(){0};", DeclarationEnding);
			WriteLine ("\tvirtual Boolean find(const Array<CIMKeyBinding>&){0};", DeclarationEnding);
			WriteLine ("\tvirtual Boolean validateKey(CIMKeyBinding&) const{0};", DeclarationEnding);
			WriteLine ("\tvirtual void setScope(CIMName){0};", DeclarationEnding);
			WriteLine ("");
			WriteLine ("\tvirtual Boolean loadInstance(CIMInstance&);");
			WriteLine ("");

			DefinePropertiesGetter (Manifest, added, Manifest.HaveChildren);
			added.Clear ();
			WriteLine ("");
			WriteMethods (Manifest, added);
			if (added.Count > 0) WriteLine ("");
			added.Clear();
			WriteLine ("private:");
			if (!Manifest.HaveChildren) {
				WriteLine ("\tCIMName currentScope;");
				AddPrivateProperties (Manifest, added);
				WriteLine ("");
				if (isDependency) {
					foreach (var g in groupNames) {
						string name = g.Replace ("CIM_", "").Replace ("UNIX_", "");
						name = "_antecedent" + name;
						WriteLine (g.Replace ("CIM_", "UNIX_") + " " + name + ";");
						WriteLine (g.Replace ("CIM_", "UNIX_") + "Provider " + name + "Provider;");
					}
					WriteLine ("");
					if (partNames != null) {
						foreach (var g in partNames) {
							string name = g.Replace ("CIM_", "").Replace ("UNIX_", "");
							name = "_dependent" + name;
							WriteLine (g.Replace ("CIM_", "UNIX_") + " " + name + ";");
							WriteLine (g.Replace ("CIM_", "UNIX_") + "Provider " + name + "Provider;");
						}
					}
					WriteLine ("");
				}
				else if (isComponent) {
					WriteLine ("\tint groupIndex;");
					WriteLine ("\tint partIndex;");

					foreach (var g in groupNames) {
						WriteLine ("\t{0} group_{0}_Component;", g);
						WriteLine ("\tint group_{0}_Index;", g);
						WriteLine ("\tbool endOf_{0}_Group;", g);
					}
					WriteLine ("");
					foreach (var p in partNames) {
						WriteLine ("\t{0} part_{0}_Component;", p);
						WriteLine ("\tint part_{0}_Index;", p);
						WriteLine ("\tbool endOf_{0}_Part;", p);
					}
						
					WriteLine ("");

				}
				if (!Manifest.HaveChildren) {
					WriteLine ("#\tinclude \"{0}Private.h\"", ClassName);
					WriteLine ("");
				}
			}
			WriteLine ("");
			WriteLine ("};");
			WriteLine ("");
			WriteLine ("#endif /* {0} */", ClassName.ToUpper());
		}

		void WriteMethods (ClassManifest target, List<string> added)
		{
			if (target.SuperClass != null)
				WriteMethods (target.SuperClass, added);
			foreach (var method in target.Class.Methods) {
				string methodName = "invoke" + method.Name.ToString ();
				if (added.Contains (methodName))
					continue;
				added.Add (methodName);
				WriteLine ("\tvirtual {0} {1}({2});\n", GetCodeType(method), methodName, GetMethodParameters(method));
			}
		}

		private string GetMethodParameters (System.Management.Internal.CimMethod method)
		{
			string paramResults = "";
			if (method.Parameters.Count > 0) {
				paramResults += "\n";
				for(int i = 0; i < method.Parameters.Count; i++) {
					var parameter = method.Parameters.ElementAt (i);
					string name = "in" + parameter.Name;
					paramResults += string.Format("\t\t{0} &{1}{2}\n", GetCodeType (parameter), name, i == method.Parameters.Count - 1 ? "" : ",");
				}
				paramResults += "\t";
			}
			return paramResults;
		}

		private string FixComponentClassName (string name)
		{
			if (name == "CIM_System") 
			{
				return "CIM_ComputerSystem";
			}
			return name;
		}

		void AddPrivateProperties (ClassManifest target, List<string> added)
		{
			if (target.SuperClass != null) {
				AddPrivateProperties (target.SuperClass, added);
			}
			foreach (var p in target.Class.Properties) {
				string name = p.Name.ToString ();
				if (added.Contains (name))
					continue;
				added.Add (name);
				name = "_" + name.Substring (0, 1).ToLower () + name.Substring (1);
				WriteLine ("\t{0} {1};", GetCodeType (p), name);
			}
		}

		void AddMethodIncludes (ClassManifest target, List<string> added)
		{
			if (target.SuperClass != null)
				AddMethodIncludes (target.SuperClass, added);

			foreach (var method in target.Class.Methods) {
				foreach (var parameter in method.Parameters) {
					System.Management.Internal.CimParameterReference refParam = parameter as System.Management.Internal.CimParameterReference;
					if (refParam != null) {
						var name = refParam.ReferenceClass.ToString ().Replace("CIM_", "UNIX_");
						if (added.Contains (name))
							continue;
						added.Add (name);
						var refManifest = GeneratorFactory.Classes.FirstOrDefault (x => !x.HaveChildren && CodeWriterBase.GetClassName (x) == name);
						if (refManifest != null) {
							WriteLine ("#include <{0}/{1}.h>", name.Replace ("CIM_", "").Replace("UNIX_", ""), name);
						}
					}
				}
			}

		}
	}
}

