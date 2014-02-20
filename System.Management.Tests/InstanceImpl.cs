using System;
using System.Linq;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal class InstanceImpl : CodeWriterBase
	{
		public InstanceImpl (ClassManifest manifest)
			: base(manifest)
		{

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
			WriteLine ("{0}::{0}(void)", ClassName);
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("{0}::~{0}(void)", ClassName);
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("");
			List<string> added = new List<string> ();
			DeclareProperties (Manifest, added, Manifest.HaveChildren);
			WriteLine ("");
			WriteLine ("");
			if (!this.Manifest.HaveChildren)
			{
				if (DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent")) {

					// Find GroupComponent Reference Class

					System.Management.Internal.CimPropertyReference r1 = Manifest.Class.Properties ["GroupComponent"] as System.Management.Internal.CimPropertyReference;
					System.Management.Internal.CimPropertyReference r2 = Manifest.Class.Properties ["PartComponent"] as System.Management.Internal.CimPropertyReference;

					//* GET ALL GROUP NAMES DERIVING FROM GROUP */
					var groupNames = GetAllTopClassesOf (r1.ReferenceClass.ToString ());
					var partNames = GetAllTopClassesOf (r2.ReferenceClass.ToString ());


					WriteLine ("Boolean {0}::initialize()", ClassName);
					WriteLine ("{");

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
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::load(int &pIndex)", ClassName);
					WriteLine ("{");

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
						returnStatement +="endOf_" + g + "_Group";
						firstOfReturn = false;
					}
					foreach (var p in partNames) {
						if (!firstOfReturn)
							returnStatement += " &&\n\t\t";
						returnStatement +="endOf_" + p + "_Part";
						firstOfReturn = false;
					}
					returnStatement += ")\t\treturn false;\n\treturn true;\n";
					Write (returnStatement);
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::finalize()", ClassName);
					WriteLine ("{");

					foreach (var g in groupNames) {
						WriteLine ("\tgroup_{0}_Component.finalize();", g);
					}
					foreach (var p in partNames) {
						WriteLine ("\tpart_{0}_Component.finalize();", p);
					}
					WriteLine ("\treturn true;");
					WriteLine ("}");

				} else {
					WriteLine ("Boolean {0}::initialize()", ClassName);
					WriteLine ("{");
					WriteLine ("\treturn false;");
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::load(int &pIndex)", ClassName);
					WriteLine ("{");
					WriteLine ("\treturn false;");
					WriteLine ("}");
					WriteLine ("");
					WriteLine ("Boolean {0}::finalize()", ClassName);
					WriteLine ("{");
					WriteLine ("\treturn false;");
					WriteLine ("}");
				}
				WriteLine ("");
				WriteLine ("Boolean {0}::find(Array<CIMKeyBinding> &kbArray)", ClassName);
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
					WriteLine ("");
					WriteLine ("");
					WriteLine ("");
					WriteLine ("/* EXecute find with extracted keys */");
					WriteLine ("");
					WriteLine ("\treturn false;");
				} else {
					WriteLine ("\treturn true;");
				}
				WriteLine ("}");
			}
		}
	}
}

