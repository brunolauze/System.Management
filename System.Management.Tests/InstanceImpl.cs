using System;
using System.Linq;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal class InstanceImpl : CodeWriterBase
	{
		private string os;

		public InstanceImpl (ClassManifest manifest, string os)
			: base(manifest)
		{
			this.os = os;
		}

		public override void Save (string filePath)
		{
			Write ();
			base.Save (filePath);
		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("");
			if (Manifest.HaveChildren) {
				WriteLine ("#include \"{0}.h\"", ClassName);
				WriteLine ("");
			}
			string declSpecific = TemplateFactory.GetDeclaration (ClassName, os);
			if (!string.IsNullOrEmpty (declSpecific)) {
				WriteLine (declSpecific);
				WriteLine ("");
			}

			WriteLine ("{0}::{0}(void)", ClassName);
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("{0}::~{0}(void)", ClassName);
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			List<string> added = new List<string> ();
			DeclareProperties (Manifest, added, Manifest.HaveChildren);
			WriteLine ("");
			added.Clear ();
			WriteLine ("Boolean {0}::loadInstance(CIMInstance &instance)", ClassName);
			WriteLine ("{");
			WriteLine ("\tUint32 propertyCount = instance.getPropertyCount();");
			WriteLine ("\tfor(Uint32 i = 0; i < propertyCount; i++) {");
			WriteLine ("\t\tCIMConstProperty property = instance.getProperty(i);");
			added.Clear ();
			WriteLoadInstanceProperties (Manifest, added);
			added.Clear ();
			WriteLine ("\t}");
			WriteLine ("\treturn true;");
			WriteLine ("}");
			WriteLine ("");
			added.Clear ();
			WriteMethods (Manifest, added);
			added.Clear ();
			WriteLine ("");
			if (!this.Manifest.HaveChildren)
			{
				if (DeriveFrom (Manifest, "CIM_Dependency")) {
					WriteLine ("Boolean {0}::initialize()", ClassName);
					WriteLine ("{");
					IEnumerable<string> partNames = null;
					IEnumerable<string> groupNames = null;
					if (!AddPlatformInitialize ()) {

						System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
						System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

						//* GET ALL GROUP NAMES DERIVING FROM GROUP */
						groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
						partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());
						if (groupNames.Count () == 0 || partNames.Count () == 0) {
							WriteLine ("\t/* Related Instances are Abstract */");
							WriteLine ("\treturn false;");
						}
						else {
							foreach (var g in groupNames) {
								string name = g.Replace ("CIM_", "").Replace ("UNIX_", "");
								name = "_antecedent" + name;
								WriteLine ("\t" + name + ".initialize();");
							}
							if (partNames != null) {
								foreach (var g in partNames) {
									string name = g.Replace ("CIM_", "").Replace ("UNIX_", "");
									name = "_dependent" + name;
									WriteLine ("\t" + name + ".initialize();");
								}
							}
						}
						WriteLine ("\treturn true;");
					}
					WriteLine ("}");
					WriteLine ("Boolean {0}::load(int &pIndex)", ClassName);
					WriteLine ("{");
					added.Clear ();
					if (!AddPlatformLoad ()) {
						WriteLine ("");

						WriteLoadProperties (Manifest, added);
						added.Clear ();
						WriteLine ("");
						WriteLine ("\treturn false;");
					}
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::finalize()", ClassName);
					WriteLine ("{");
					if (!AddPlatformFinalize ()) {
						foreach (var g in groupNames) {
							string name = g.Replace ("CIM_", "").Replace ("UNIX_", "");
							name = "_antecedent" + name;
							WriteLine ("\t" + name + ".finalize();");
						}
						if (partNames != null) {
							foreach (var g in partNames) {
								string name = g.Replace ("CIM_", "").Replace ("UNIX_", "");
								name = "_dependent" + name;
								WriteLine ("\t" + name + ".finalize();");
							}
						}
						WriteLine ("\treturn true;");
					}
					WriteLine ("}");


				} else if (Manifest.Class.ClassName != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties ("GroupComponent", "PartComponent")) {

					// Find GroupComponent Reference Class
					System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
					System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;

					/* GET ALL GROUP NAMES DERIVING FROM GROUP */
					var groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
					var partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());

					WriteLine ("Boolean {0}::initialize()", ClassName);
					WriteLine ("{");
					if (!AddPlatformInitialize ()) {
						WriteLine ("\tgroupIndex = 0;");
						WriteLine ("\tpartIndex = 0;");
						foreach (var g in groupNames) {
							WriteLine ("\tgroup_{0}_Index = -1;", g);
							WriteLine ("\tendOf_{0}_Group = !group_{0}_Component.initialize();", g);
						}
						foreach (var p in partNames) {
							WriteLine ("\tpart_{0}_Index = -1;", p);
							WriteLine ("\tendOf_{0}_Part = !part_{0}_Component.initialize();", p);
						}
						WriteLine ("\treturn true;");
					}
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::load(int &pIndex)", ClassName);
					WriteLine ("{");
					if (!AddPlatformLoad())
					{
						if (partNames.Count () == 0 || groupNames.Count () == 0) {
							WriteLine ("\t/* Related Instances are Abstract */");
							WriteLine ("\treturn false;");
						} else {
							int groupIndex = 0;
							int partIndex = 0;
							string groupIncrementStatement = "\tif (pIndex == 0 || (";
							bool partEvalFirst = true;
							foreach (var p in partNames) {
								if (!partEvalFirst)
									groupIncrementStatement += " &&\n\t\t\t";
								groupIncrementStatement += "endOf_" + p + "_Part";
								partEvalFirst = false;
							}
							groupIncrementStatement += "))\n\t{\n";
							Write (groupIncrementStatement);
							bool groupState = true;
							foreach (var g in groupNames) {
								WriteLine ("\t\t{0}if (groupIndex == {1})", groupState ? "" : "else ", groupIndex);
								WriteLine ("\t\t{");
								WriteLine ("\t\t\tgroup_{0}_Index++;", g);
								WriteLine ("\t\t\tendOf_{0}_Group = !group_{0}_Component.load(group_{0}_Index);", g);

								WriteLine ("\t\t\tif (endOf_{0}_Group)", g);
								WriteLine ("\t\t\t{");
								foreach (var p in partNames) {
									WriteLine ("\t\t\t\tendOf_{0}_Part = false;", p);
									WriteLine ("\t\t\t\tpart_{0}_Component.setScope(CIMName(\"{1}\"));", p, g);
									WriteLine ("\t\t\t\tpart_{0}_Component.initialize();", p);
								}
								WriteLine ("\t\t\t\tpartIndex = 0;");
								WriteLine ("\t\t\t\tgroupIndex++;");
								WriteLine ("\t\t\t}");
								WriteLine ("\t\t}");
								groupState = false;
								groupIndex++;
							}
							WriteLine ("\t}");
							partIndex = 0;
							foreach (var p in partNames) {
								WriteLine ("\tif (partIndex == {0})", partIndex);
								WriteLine ("\t{");
								WriteLine ("\t\tpart_{0}_Index++;", p);
								WriteLine ("\tendOf_{0}_Part = !part_{0}_Component.load(part_{0}_Index);", p);
								WriteLine ("\t}");
								partIndex++;
							}

							partIndex = 0;
							foreach (var p in partNames) {
								WriteLine ("\tif (partIndex == {0} && endOf_{1}_Part)", partIndex, p);
								WriteLine ("\t{");
								WriteLine ("\t\tpart_{0}_Component.finalize();", p);
								WriteLine ("\t\tpartIndex++;");
								WriteLine ("\t}");
								partIndex++;
							}
							WriteLine ("");
							bool firstOfReturn = true;
							string returnStatement = "\tif (";
							foreach (var g in groupNames) {
								if (!firstOfReturn)
									returnStatement += " &&\n\t\t";
								returnStatement += "endOf_" + g + "_Group";
								firstOfReturn = false;
							}
							foreach (var p in partNames) {
								if (!firstOfReturn)
									returnStatement += " &&\n\t\t";
								returnStatement += "endOf_" + p + "_Part";
								firstOfReturn = false;
							}
							returnStatement += ")\t\treturn false;\n\treturn true;\n";
							Write (returnStatement);
						}
						WriteLine ("}");
						WriteLine ("");
						WriteLine ("Boolean {0}::finalize()", ClassName);
						WriteLine ("{");
						if (!AddPlatformFinalize())
						{
							foreach (var g in groupNames) {
								WriteLine ("\tgroup_{0}_Component.finalize();", g);
							}
							foreach (var p in partNames) {
								WriteLine ("\tpart_{0}_Component.finalize();", p);
							}
						}
						WriteLine ("\treturn true;");
					}
					WriteLine ("}");
				} else {
					WriteLine ("Boolean {0}::initialize()", ClassName);
					WriteLine ("{");
					if (!AddPlatformInitialize ()) {
						WriteLine ("\treturn false;");
					}
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::load(int &pIndex)", ClassName);
					WriteLine ("{");
					added.Clear ();
					if (!AddPlatformLoad ()) {
						WriteLine ("\t");
						WriteLoadProperties (Manifest, added);
						added.Clear ();
						WriteLine ("\t");
						WriteLine ("\treturn false;");
					}
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::finalize()", ClassName);
					WriteLine ("{");
					if (!AddPlatformFinalize ()) {
						WriteLine ("\treturn false;");
					}
					WriteLine ("}");
				}
				WriteLine ("");
				if (DeriveFrom (Manifest, "CIM_ManagedSystemElement")) {
					WriteLine ("Boolean {0}::loadByName(String &name)", ClassName);
					WriteLine ("{");
					WriteLine ("\t");
					WriteLine ("\treturn false;");
					WriteLine ("\t");
					WriteLine ("}");
				}
				WriteLine ("");
				WriteLine ("Boolean {0}::find(const Array<CIMKeyBinding> &kbArray)", ClassName);
				WriteLine ("{");
				if (IsKeyed(Manifest)) {
					WriteLine ("\tCIMKeyBinding kb;");
					var keys = GetKeyProperties ();
					foreach (var p in keys) {
						var keyType = "String"; //GetCodeType (p);
						string keyName = p.Name.ToString ();
						keyName = keyName.Substring (0, 1).ToLower () + keyName.Substring (1); 
						WriteLine ("\t{0} {1}Key;", keyType, keyName);
					}
					WriteLine ("");
					WriteLine ("");
					WriteLine ("\tfor(Uint32 i = 0; i < kbArray.size(); i++)");
					WriteLine ("\t{");
					WriteLine ("\t\tkb = kbArray[i];");
					WriteLine ("\t\tCIMName keyName = kb.getName();");
					bool firstKeyLoop = true;
					foreach (var p in keys) {
						string keyName = p.Name.ToString ();
						keyName = keyName.Substring (0, 1).ToLower () + keyName.Substring (1); 
						if (firstKeyLoop)
							WriteLine ("\t\tif (keyName.equal({0})) {1}Key = kb.getValue();", GetPropertyDeclaration (p.Name.ToString ()), keyName);
						else
							WriteLine ("\t\telse if (keyName.equal({0})) {1}Key = kb.getValue();", GetPropertyDeclaration (p.Name.ToString ()), keyName);
						firstKeyLoop = false;
					}

					WriteLine ("\t}");
					WriteLine ("\t");
					WriteLine ("\t");
					WriteLine ("\t");
					WriteLine ("\t/* Execute find with extracted keys */");
					WriteLine ("\tfor(int i = 0; load(i); i++) {");
					int i = 0;
					firstKeyLoop = true;
					foreach(var p in keys) {
						string keyName = p.Name.ToString ();
						keyName = keyName.Substring (0, 1).ToLower () + keyName.Substring (1) + "Key"; 
						//WriteLine ("\t\t{3}(String::equalNoCase(get{0}(), {1})){2}", p.Name, keyName, i == keys.Count () - 1 ? ")" : " &&", i == 0 ? "if (" : "\t");

						if (p.Type == System.Management.Internal.CimType.STRING) {
							WriteLine ("\t\t{3}(String::equalNoCase(get{0}(), {1})){2}", p.Name, keyName, i == keys.Count () - 1 ? ")" : " && ", i == 0 ? "if (" : "\t");
						} else if (p.Type == System.Management.Internal.CimType.DATETIME) {
							WriteLine ("\t\t{3}(String::equalNoCase(get{0}().toString(), {1})){2}", p.Name, keyName, i == keys.Count () - 1 ? ")" : " && ", i == 0 ? "if (" : "\t");
						}
						else if (p.Type != System.Management.Internal.CimType.CIMNULL && p.Type != System.Management.Internal.CimType.REFERENCE) {
							WriteLine ("\t\t{3}(String::equalNoCase(String(std::to_string(get{0}()).c_str()), {1})){2}", p.Name, keyName, i == keys.Count () - 1 ? ")" : " && ", i == 0 ? "if (" : "\t");
							//WriteLine ("\t\t{3}(get{0}() == {1}){2}", p.Name, keyName, i == keys.Count () - 1 ? " && \n" : ")", i == 0 ? "if (" : "\t");
						} else {
							WriteLine ("\t\t{3}(String::equalNoCase(get{0}().getPath().toString(), {1})){2}", p.Name, keyName, i == keys.Count () - 1 ? ")" : " && ", i == 0 ? "if (" : "\t");
						}
						i++;
					}
					WriteLine ("\t\t{");
					WriteLine ("\t\t\treturn true;");
					WriteLine ("\t\t}");
					WriteLine ("\t}");
					WriteLine ("\t");
					WriteLine ("\treturn false;");
				} else {
					WriteLine ("\treturn true;");
				}
				WriteLine ("}");
			}
		}

		void WriteLoadProperties (ClassManifest target, List<string> added)
		{
			if (target.SuperClass != null)
				WriteLoadProperties (target.SuperClass, added);

			foreach (var property in target.Class.Properties) {
				string name = property.Name.ToString ();
				if (added.Contains (name))
					continue;
				added.Add (name);
				var newProp = GetProperty(Manifest, property.Name.ToString());
				name = "_" + name.Substring (0, 1).ToLower () + name.Substring (1);
				/*
				System.Management.Internal.CimPropertyReference refProp = newProp as System.Management.Internal.CimPropertyReference;
				if (refProp != null) 
					WriteLine ("\t/" + "*");
				*/
				WriteLine ("\t{0}", GetDefaultValue (name, newProp));
				/*
				if (refProp != null)
					WriteLine ("\t*" + "/");
				*/
			}
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
				WriteLine ("{0} {1}::{2}({3})", GetCodeType(method), ClassName, methodName, GetMethodParameters(method));
				WriteLine ("{");
				WriteLine ("\t{0} {1}", GetCodeType(method), GetDefaultValue("returnValue", method));
				WriteLine ("\t");

				foreach (var parameter in method.Parameters) {
					System.Management.Internal.CimParameterReference refParam = parameter as System.Management.Internal.CimParameterReference;
					if (refParam != null) {
						var name = refParam.ReferenceClass.ToString ().Replace("CIM_", "UNIX_");
						var refManifest = GeneratorFactory.Classes.FirstOrDefault (x => !x.HaveChildren && CodeWriterBase.GetClassName (x) == name);
						if (refManifest != null) {
							string pName = "in" + parameter.Name;
							WriteLine ("\t{0} {1}Object;", name, pName);
							WriteLine ("\t{0}Object.loadInstance({0});", pName);
							WriteLine ("\t");
						}
					}
				}

				WriteLine ("\t/* Execute method {0} */", method.Name);
				WriteLine ("\t");
				WriteLine ("\t");
				WriteLine ("\t");
				WriteLine ("\treturn returnValue;");
				WriteLine ("}");
				WriteLine ("");
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

		void WriteLoadInstanceProperties (ClassManifest target, List<string> added)
		{
			if (target.SuperClass != null)
				WriteLoadInstanceProperties (target.SuperClass, added);

			foreach(var property in target.Class.Properties)
			{
				if (added.Contains (property.Name.ToString()))
					continue;
				WriteLine ("\t\t\t{1}if (String::equal(property.getName().getString(), \"{0}\"))", property.Name, added.Count == 0 ? "" : "else ");
				WriteLine ("\t\t\t{");
				string name = property.Name.ToString ().Substring (0, 1).ToLower () + property.Name.ToString ().Substring (1) + "Value";
				WriteLine ("\t\t\t\t{0} {1};", GetCodeType (property), name);
				WriteLine ("\t\t\t\tproperty.getValue().get({0});", name);
				WriteLine ("\t\t\t\tset{0}({1});", property.Name, name);
				WriteLine ("\t\t\t}");
				added.Add (property.Name.ToString());
			}
		}

		bool AddPlatformInitialize ()
		{
			string template = TemplateFactory.GetInitialize (ClassName, os);
			if (!string.IsNullOrEmpty (template)) {
				WriteLine (template);
				return true;
			}
			return false;
		}

		bool AddPlatformLoad ()
		{
			string template = TemplateFactory.GetLoad (ClassName, os);
			if (!string.IsNullOrEmpty (template)) {
				WriteLine (template);
				return true;
			}
			return false;
		}

		bool AddPlatformFinalize ()
		{
			string template = TemplateFactory.GetFinalize (ClassName, os);
			if (!string.IsNullOrEmpty (template)) {
				WriteLine (template);
				return true;
			}
			return false;
		}
	}
}

