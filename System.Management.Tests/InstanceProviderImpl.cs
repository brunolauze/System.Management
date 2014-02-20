using System;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal class InstanceProviderImpl : CodeWriterBase
	{
		public InstanceProviderImpl (ClassManifest manifest)
			: base(manifest)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine("");
			WriteLine("");
			WriteLine("#include \"{0}Provider.h\"", ClassName);
			WriteLine("");
			WriteLine("{0}Provider::{0}Provider()", ClassName);
			WriteLine("{");
			WriteLine("}");
			WriteLine("");
			WriteLine("{0}Provider::~{0}Provider()", ClassName);
			WriteLine("{");
			WriteLine("}");
			WriteLine("");
			WriteLine ("CIMInstance {0}Provider::constructInstance(", ClassName);
			WriteLine ("\tconst CIMName &className,");
			WriteLine ("\tconst CIMNamespaceName &nameSpace,");
			WriteLine ("\tconst {0} &_p) const", ClassName);
			WriteLine ("{");
			WriteLine ("\tCIMProperty p;");
			WriteLine ("");
			WriteLine ("\tCIMInstance inst(className);");
			WriteLine ("");
			WriteLine ("\t// Set path");
			WriteLine ("\tinst.setPath(CIMObjectPath(String(\"\"), // hostname");
			WriteLine ("\t\t\tnameSpace,");
			WriteLine ("\t\t\tCIMName(\"{0}\"),", ClassName);
			WriteLine ("\t\t\tconstructKeyBindings(_p)));");
			WriteLine ("");

			List<string> added = new List<string> ();
			AddProperties (Manifest, added);

			WriteLine ("");
			WriteLine ("\treturn inst;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("Array<CIMKeyBinding> {0}Provider::constructKeyBindings(const {0}& _p) const", ClassName);
			WriteLine ("{");
			WriteLine ("");
			WriteLine ("\tArray<CIMKeyBinding> keys;");
			WriteLine ("");
			if (IsKeyed (Manifest)) {
				var keys = GetKeyProperties ();
				foreach (var key in keys) {

					if (key.Type.ToCimType () == System.Management.Internal.CimType.REFERENCE) {
						WriteLine ("\tCIMKeyBinding {0}Key(", key.Name.ToString ());
						WriteLine ("\t\t{0},", GetPropertyDeclaration (key.Name.ToString ()));
						WriteLine ("\t\tCIMValue(_p.get{0}().getPath()));", key.Name.ToString ());
						WriteLine ("\t{0}Key.setType(CIMKeyBinding::REFERENCE);");
						WriteLine ("\tkeys.append({0}Key);", key.Name.ToString ());
					} else {
						WriteLine ("\tkeys.append(CIMKeyBinding(");
						WriteLine ("\t\t{0},", GetPropertyDeclaration (key.Name.ToString ()));
						if (key.Type == System.Management.Internal.CimType.STRING) {
							WriteLine ("\t\t_p.get{0}(),", key.Name.ToString ());
						} else {
							WriteLine ("\t\tCIMValue(_p.get{0}()).toString(),", key.Name.ToString ());
						}
						WriteLine ("\t\tCIMKeyBinding::{0}));", GetBindingType (key.Type.ToCimType ()));
					}
				}
			}
			WriteLine ("");
			WriteLine ("");
			WriteLine ("\treturn keys;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("");
			WriteLine("#define UNIX_PROVIDER {0}Provider", ClassName);
			WriteLine("#define UNIX_PROVIDER_NAME \"{0}Provider\"", ClassName);
			WriteLine("#define CLASS_IMPLEMENTATION {0}", ClassName);
			WriteLine("#define CLASS_IMPLEMENTATION_NAME \"{0}\"", ClassName);
			WriteLine ("#define BASE_CLASS_NAME \"{0}\"", Manifest.Class.ClassName.ToString());
			int keyCount = 0;
			if (Class.HasKeyProperty) {
				foreach (var p in Class.Properties) {
					if (p.IsKeyProperty) {
						keyCount++;
					}
				}
			}

			WriteLine ("#define NUMKEYS_CLASS_IMPLEMENTATION {0}", keyCount);
			WriteLine("");
			WriteLine("");
			WriteLine("#include \"UNIXProviderBase.hpp\"");
			WriteLine("");
		}

		void AddProperties (ClassManifest manifestItem, List<string> added)
		{
			if (manifestItem.SuperClass != null)
				AddProperties (manifestItem.SuperClass, added);
			WriteLine ("\t//{0} Properties", manifestItem.Class.ClassName.ToString ());
			foreach (var p in manifestItem.Class.Properties) {
				if (!added.Contains (p.Name.ToString ())) {
					WriteLine ("\tif (_p.get{0}(p)) inst.addProperty(p);", p.Name.ToString ());
					added.Add (p.Name.ToString ());
				}
			}
			WriteLine ("");
		}

		static string GetBindingType(System.Management.Internal.CimType type)
		{
			if (type == System.Management.Internal.CimType.REFERENCE)
				return "REFERENCE";
			else if (type == System.Management.Internal.CimType.BOOLEAN)
				return "BOOLEAN";
			else if (type == System.Management.Internal.CimType.DATETIME)
				return "STRING";
			else if (type == System.Management.Internal.CimType.STRING)
				return "STRING";
			else if (type == System.Management.Internal.CimType.CHAR16)
				return "STRING";
			else if (type == System.Management.Internal.CimType.CIMNULL)
				return "STRING";
			return "NUMERIC";
		}
	}
}

