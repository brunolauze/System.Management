using System;
using System.Collections.Generic;
using System.Linq;

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
			WriteLine ("using " + ClassName + "Lib::CIMHelper;");
			WriteLine ("");
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
			WriteLine ("\tconst {0} &instanceObject) const", ClassName);
			WriteLine ("{");
			WriteLine ("\tCIMProperty p;");
			WriteLine ("");
			WriteLine ("\tCIMInstance inst(className);");
			WriteLine ("");
			WriteLine ("\t// Set path");
			WriteLine ("\tinst.setPath(CIMObjectPath(String(\"\"), // hostname");
			WriteLine ("\t\t\tnameSpace,");
			WriteLine ("\t\t\tclassName,");
			WriteLine ("\t\t\tconstructKeyBindings(instanceObject)));");
			WriteLine ("");

			List<string> added = new List<string> ();
			AddProperties (Manifest, added);
			added.Clear ();
			WriteLine ("");
			WriteLine ("\treturn inst;");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("Array<CIMKeyBinding> {0}Provider::constructKeyBindings(const {0}& instanceObject) const", ClassName);
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
						WriteLine ("\t\tCIMValue(instanceObject.get{0}().getPath()));", key.Name.ToString ());
						WriteLine ("\t{0}Key.setType(CIMKeyBinding::REFERENCE);", key.Name.ToString ());
						WriteLine ("\tkeys.append({0}Key);", key.Name.ToString ());
					} else {
						WriteLine ("\tkeys.append(CIMKeyBinding(");
						WriteLine ("\t\t{0},", GetPropertyDeclaration (key.Name.ToString ()));
						if (key.Type == System.Management.Internal.CimType.STRING) {
							WriteLine ("\t\tinstanceObject.get{0}(),", key.Name.ToString ());
						} else {
							WriteLine ("\t\tCIMValue(instanceObject.get{0}()).toString(),", key.Name.ToString ());
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

			if (GetMethodCount(Manifest) > 0) {
				WriteLine ("#define __invokeMethod_H");
				WriteLine ("/*");
				WriteLine ("================================================================================");
				WriteLine ("NAME              : invokeMethod");
				WriteLine ("DESCRIPTION       : invokeMethod for the current instance;");
				WriteLine ("ASSUMPTIONS       : None");
				WriteLine ("PRE-CONDITIONS    : ");
				WriteLine ("POST-CONDITIONS   : ");
				WriteLine ("NOTES             : ");
				WriteLine ("================================================================================");
				WriteLine ("*/");
				WriteLine ("");
				WriteLine ("void {0}Provider::invokeMethod(", ClassName);
				WriteLine ("\tconst OperationContext& context,");
				WriteLine ("\tconst CIMObjectPath& objectReference,");
				WriteLine ("\tconst CIMName& methodName,");
				WriteLine ("\tconst Array<CIMParamValue>& inParameters,");
				WriteLine ("\tMethodResultResponseHandler& handler)");
				WriteLine ("{");
				WriteLine ("\tif (!objectReference.getClassName().equal(\"" + ClassName + "\") && !objectReference.getClassName().equal(\"" + Class.ClassName.ToString() + "\")) {");
				WriteLine ("\t\tString classMessage;");
				WriteLine ("\t\tclassMessage.append(\"{0} Provider\");", ClassName);
				WriteLine ("\t\tclassMessage.append (\" does not support class \");");
				WriteLine ("\t\tclassMessage.append (objectReference.getClassName().getString());");
				WriteLine ("\t\tthrow CIMNotSupportedException(classMessage);");
				WriteLine ("\t}");
				WriteLine ("");
				WriteLine ("");
				WriteLine ("\thandler.processing();");

				WriteLine ("\t// Make cimom handle invokeMethod request with input parameters.");
				WriteLine ("\tCIMObjectPath localReference = CIMObjectPath(");
				WriteLine ("\t\tString(\"\"),");
				WriteLine ("\t\tCIMNamespaceName(\"root/cimv2\"),");
				WriteLine ("\t\tobjectReference.getClassName(),");
				WriteLine ("\t\tobjectReference.getKeyBindings());");
				WriteLine ("\t");

				bool firstMethod = true;
				WriteMethods (Manifest, firstMethod);

				WriteLine ("\telse {");
				WriteLine ("\t\tString message;");
				WriteLine ("\t\tmessage.append(\"{0}\");", ClassName);
				WriteLine ("\t\tmessage.append (\" does not support invokeMethod\");");
				WriteLine ("\t\tthrow CIMNotSupportedException(message);");
				WriteLine ("\t}");
				WriteLine ("}");
			}

			WriteLine ("");
			WriteLine ("");
			WriteLine("#define UNIX_PROVIDER {0}Provider", ClassName);
			WriteLine("#define UNIX_PROVIDER_NAME \"{0}Provider\"", ClassName);
			WriteLine("#define CLASS_IMPLEMENTATION {0}", ClassName);
			WriteLine("#define CLASS_IMPLEMENTATION_NAME \"{0}\"", ClassName);
			WriteLine ("#define BASE_CLASS_NAME \"{0}\"", Manifest.Class.ClassName.ToString());
			int keyCount = GetKeyCount(Manifest);
			WriteLine ("#define NUMKEYS_CLASS_IMPLEMENTATION {0}", keyCount);

			if (DeriveFrom (Manifest, "CIM_ManagedSystemElement")) {
				WriteLine ("#define CLASS_LOADABLE_BY_NAME 1");
			}
			AddCreateInstance ();
			AddDeleteInstance ();
			AddModifyInstance ();
			WriteLine("");
			WriteLine("");
			WriteLine("#include \"UNIXProviderBase.hpp\"");
			WriteLine("");
		}

		int GetMethodCount (ClassManifest target)
		{
			int i = 0;
			if (target.SuperClass != null)
				i += GetMethodCount (target.SuperClass);
			i += target.Class.Methods.Count;
			return i;
		}

		void WriteMethods (ClassManifest target, bool firstMethod)
		{
			foreach (var method in target.Class.Methods) {
				WriteLine ("\t" + (firstMethod ? "" : "else ") + "if (methodName.equal(\"" + method.Name + "\")) {");
				WriteLine ("");
				WriteLine ("\t\tif (inParameters.size() != " +method.Parameters.Count(x => x.Qualifiers.Any(y => y.Name == "In")).ToString() + ")");
				WriteLine ("\t\t{");
				WriteLine ("\t\t\tthrow new CIMOperationFailedException(\"Incorrect in parameters\");");
				WriteLine ("\t\t}");
				WriteLine ("\t\t");

				WriteLine ("\t\t//Invoking {0} method.", method.Name);
				string methodName = "invoke" + method.Name.ToString ();
				string returnValue = methodName + "ReturnValue";
				if (method.Type != System.Management.Internal.CimType.CIMNULL) {
					WriteLine ("\t\t{0} {1};", GetCodeType (method), returnValue);
				}
				WriteLine ("");
				if (method.Parameters.Count > 0) {
					foreach (var parameter in method.Parameters) {
						string name = "in" + parameter.Name;

						WriteLine ("\t\t{0} {1};", GetCodeType (parameter), name);
					}
					WriteLine ("\t\t");

					WriteLine ("\t\tfor(Uint32 pi = 0; pi < inParameters.size(); pi++) {");
					WriteLine ("\t\t\tCIMParamValue p = inParameters[pi];");
					bool firstParam = true;
					foreach (var parameter in method.Parameters) {
						string name = "in" + parameter.Name;
						WriteLine ("\t\t\t{0}if (String::equalNoCase(p.getParameterName(), \"{1}\"))", firstParam ? "" : "else ", parameter.Name);
						WriteLine ("\t\t\t{");
						WriteLine ("\t\t\t\tp.getValue().get({0});", name);
						WriteLine ("\t\t\t}");
						firstParam = true;
					}
					WriteLine ("\t\t}");
				}
				WriteLine ("\t\t_p.initialize();");
				WriteLine ("\t\t_p.find(localReference.getKeyBindings());");
				WriteLine ("\t\t{0} = _p.{1}({2}", returnValue, methodName, method.Parameters.Count == 0 ? ");" : "\n");
				bool firstParameter = true;
				foreach (var parameter in method.Parameters) {
					string name = "in" + parameter.Name;
					if (!firstParameter)
						Write (",\n");
					Write ("\t\t\t{0}", name);
					firstParameter = false;
				}
				if (method.Parameters.Count > 0)
					WriteLine ("\n\t\t);");

				WriteLine ("\t\t_p.finalize();");
				if (method.Type == System.Management.Internal.CimType.CIMNULL) 
					WriteLine ("\t\thandler.deliver();", returnValue);
				else
					WriteLine ("\t\thandler.deliver({0});", returnValue);

				WriteLine ("");
				WriteLine ("\t}");
				firstMethod = false;
			}
			if (target.SuperClass != null) {
				WriteMethods (target.SuperClass, firstMethod);
			}
		}



		void AddProperties (ClassManifest manifestItem, List<string> added)
		{
			if (manifestItem.SuperClass != null)
				AddProperties (manifestItem.SuperClass, added);
			WriteLine ("\t//{0} Properties", manifestItem.Class.ClassName.ToString ());
			foreach (var p in manifestItem.Class.Properties) {
				if (!added.Contains (p.Name.ToString ())) {
					WriteLine ("\tif (instanceObject.get{0}(p)) inst.addProperty(p);", p.Name.ToString ());
					added.Add (p.Name.ToString ());
				}
			}
			string currentClassName = CodeWriterBase.GetClassName (manifestItem);
			if (currentClassName != ClassName)
				WriteLine ("\tif (className.equal(\"{0}\")) return inst;", currentClassName);

			WriteLine ("\t");
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

		void AddDeleteInstance()
		{
			if (AddDeleteInstance ("FREEBSD")) return;
			if (AddDeleteInstance ("LINUX")) return;
			if (AddDeleteInstance ("ZOS")) return;
			if (AddDeleteInstance ("WIN32")) return;
			if (AddDeleteInstance ("SOLARIS")) return;
			if (AddDeleteInstance ("HPUX")) return;
			if (AddDeleteInstance ("VMS")) return;
			if (AddDeleteInstance ("TRU64")) return;
			if (AddDeleteInstance ("DARWIN")) return;
			if (AddDeleteInstance ("AIX")) return;
			if (AddDeleteInstance ("STUB")) return;
		}

		void AddCreateInstance()
		{
			if (AddCreateInstance ("FREEBSD")) return;
			if (AddCreateInstance ("LINUX")) return;
			if (AddCreateInstance ("ZOS")) return;
			if (AddCreateInstance ("WIN32")) return;
			if (AddCreateInstance ("SOLARIS")) return;
			if (AddCreateInstance ("HPUX")) return;
			if (AddCreateInstance ("VMS")) return;
			if (AddCreateInstance ("TRU64")) return;
			if (AddCreateInstance ("DARWIN")) return;
			if (AddCreateInstance ("AIX")) return;
			if (AddCreateInstance ("STUB")) return;
		}

		bool AddCreateInstance(string os)
		{
			if (!TemplateFactory.HasCreateInstance(ClassName, os)) return false;
			WriteLine ("");
			WriteLine ("#define __createInstance_H");
			WriteLine ("/*");
			WriteLine ("================================================================================");
			WriteLine ("NAME              : createInstance");
			WriteLine ("DESCRIPTION       : Create {0} instance;", ClassName);
			WriteLine ("ASSUMPTIONS       : None");
			WriteLine ("PRE-CONDITIONS    : ");
			WriteLine ("POST-CONDITIONS   : ");
			WriteLine ("NOTES             : ");
			WriteLine ("================================================================================");
			WriteLine ("*/");
			WriteLine ("");
			WriteLine ("void {0}Provider::createInstance(", ClassName);
			WriteLine ("\tconst OperationContext& context,");
			WriteLine ("\tconst CIMObjectPath& ref,");
			WriteLine ("\tconst CIMInstance& instanceObject,");
			WriteLine ("\tObjectPathResponseHandler& handler)");
			WriteLine ("{");
			WriteLine ("\tif (!ref.getClassName().equal(\"" + ClassName + "\") && !ref.getClassName().equal(\"" + Class.ClassName.ToString() + "\")) {");
			WriteLine ("\t\tString classMessage;");
			WriteLine ("\t\tclassMessage.append(\"{0} Provider\");", ClassName);
			WriteLine ("\t\tclassMessage.append (\" does not support class \");");
			WriteLine ("\t\tclassMessage.append (ref.getClassName().getString());");
			WriteLine ("\t\tthrow CIMNotSupportedException(classMessage);");
			WriteLine ("\t}");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("\thandler.processing();");

			WriteLine ("\t// Make cimom handle deleteInstance request from object path reference.");
			WriteLine ("\tCIMObjectPath localReference = CIMObjectPath(");
			WriteLine ("\t\tString(\"\"),");
			WriteLine ("\t\tCIMNamespaceName(\"root/cimv2\"),");
			WriteLine ("\t\tref.getClassName(),");
			WriteLine ("\t\tref.getKeyBindings());");
			WriteLine ("\t");
			WriteLine ("\tif (_p.find(localReference.getKeyBindings())) {");
			WriteLine ("\t\tthrow CIMObjectAlreadyExistsException(");
			WriteLine ("\t\t\t\tlocalReference.toString());");
			WriteLine ("\t}");
			WriteLine ("\telse {");
			WriteLine ("\t\tif (_p.loadInstance(instanceObject) {");
			WriteLine ("\t\t\tif (_p.createInstance()) {");
			WriteLine ("\t\t\t\t/* Deliver Instance */");
			WriteLine ("\t\t\t\tCIMInstance creationResult = constructInstance(");
			WriteLine ("\t\t\t\t\t\t\tlocalReference.getClassName(), ");
			WriteLine ("\t\t\t\t\t\t\tCIMNamespaceName(\"root/cimv2\"), ");
			WriteLine ("\t\t\t\t\t\t\t_p);");
			WriteLine ("\t\t\t\thandler.deliver(creationResult);");
			WriteLine ("\t\t\t\t/* Send Create Indication */");
			WriteLine ("\t\t\t}");
			WriteLine ("\t\t\telse {");
			WriteLine ("\t\t\t\t/* Raise Creating Exception */");
			WriteLine ("\t\t\t}");
			WriteLine ("\t\t}");
			WriteLine ("\t\telse {");
			WriteLine ("\t\t\t/* Raise Loading Exception */");
			WriteLine ("\t\t}");
			WriteLine ("\t}");
			WriteLine ("\thandler.complete();");
			WriteLine ("}");
			return true;
		}


		bool AddDeleteInstance(string os)
		{
			if (!TemplateFactory.HasDeleteInstance(ClassName, os)) return false;
			WriteLine ("");
			WriteLine ("#define __deleteInstance_H");
			WriteLine ("/*");
			WriteLine ("================================================================================");
			WriteLine ("NAME              : deleteInstance");
			WriteLine ("DESCRIPTION       : Delete {0} instance;", ClassName);
			WriteLine ("ASSUMPTIONS       : None");
			WriteLine ("PRE-CONDITIONS    : ");
			WriteLine ("POST-CONDITIONS   : ");
			WriteLine ("NOTES             : ");
			WriteLine ("================================================================================");
			WriteLine ("*/");
			WriteLine ("");
			WriteLine ("void {0}Provider::deleteInstance(", ClassName);
			WriteLine ("\tconst OperationContext& context,");
			WriteLine ("\tconst CIMObjectPath& ref,");
			WriteLine ("\tResponseHandler& handler)");
			WriteLine ("{");
			WriteLine ("\tif (!ref.getClassName().equal(\"" + ClassName + "\") && !ref.getClassName().equal(\"" + Class.ClassName.ToString() + "\")) {");
			WriteLine ("\t\tString classMessage;");
			WriteLine ("\t\tclassMessage.append(\"{0} Provider\");", ClassName);
			WriteLine ("\t\tclassMessage.append (\" does not support class \");");
			WriteLine ("\t\tclassMessage.append (ref.getClassName().getString());");
			WriteLine ("\t\tthrow CIMNotSupportedException(classMessage);");
			WriteLine ("\t}");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("\thandler.processing();");

			WriteLine ("\t// Make cimom handle deleteInstance request from object path reference.");
			WriteLine ("\tCIMObjectPath localReference = CIMObjectPath(");
			WriteLine ("\t\tString(\"\"),");
			WriteLine ("\t\tCIMNamespaceName(\"root/cimv2\"),");
			WriteLine ("\t\tref.getClassName(),");
			WriteLine ("\t\tref.getKeyBindings());");
			WriteLine ("\t");

			WriteLine ("\tif (_p.find(localReference.getKeyBindings()) {");
			WriteLine ("\t\tif (!_p.deleteInstance()) {");
			WriteLine ("\t\t\t/* Send Delete Indication */");
			WriteLine ("\t\t}");
			WriteLine ("\t\telse {");
			WriteLine ("\t\t\t/* Raise Delete Exception */");
			WriteLine ("\t\t}");
			WriteLine ("\t}");
			WriteLine ("\telse {");
			WriteLine ("\t\tthrow CIMObjectNotFoundException(");
			WriteLine ("\t\t\t\tlocalReference.toString());");
			WriteLine ("\t}");

			WriteLine ("\thandler.complete();");
			WriteLine ("}");
			return true;
		}

		void AddModifyInstance()
		{
			if (AddModifyInstance ("FREEBSD")) return;
			if (AddModifyInstance ("LINUX")) return;
			if (AddModifyInstance ("ZOS")) return;
			if (AddModifyInstance ("WIN32")) return;
			if (AddModifyInstance ("SOLARIS")) return;
			if (AddModifyInstance ("HPUX")) return;
			if (AddModifyInstance ("VMS")) return;
			if (AddModifyInstance ("TRU64")) return;
			if (AddModifyInstance ("DARWIN")) return;
			if (AddModifyInstance ("AIX")) return;
			if (AddModifyInstance ("STUB")) return;
		}

		bool AddModifyInstance(string os)
		{
			if (!TemplateFactory.HasModifyInstance(ClassName, os)) return false;
			WriteLine ("");
			WriteLine ("#define __modifyInstance_H");
			WriteLine ("/*");
			WriteLine ("================================================================================");
			WriteLine ("NAME              : modifyInstance");
			WriteLine ("DESCRIPTION       : Modify {0} instance;", ClassName);
			WriteLine ("ASSUMPTIONS       : None");
			WriteLine ("PRE-CONDITIONS    : ");
			WriteLine ("POST-CONDITIONS   : ");
			WriteLine ("NOTES             : ");
			WriteLine ("================================================================================");
			WriteLine ("*/");
			WriteLine ("");
			WriteLine ("void {0}Provider::modifyInstance(", ClassName);
			WriteLine ("\tconst OperationContext& context,");
			WriteLine ("\tconst CIMObjectPath& ref,");
			WriteLine ("\tconst CIMInstance& instanceObject,");
			WriteLine ("\tconst Boolean includeQualifiers,");
			WriteLine ("\tconst CIMPropertyList& propertyList,");
			WriteLine ("\tResponseHandler& handler)");
			WriteLine ("{");
			WriteLine ("\tif (!ref.getClassName().equal(\"" + ClassName + "\") && !ref.getClassName().equal(\"" + Class.ClassName.ToString() + "\")) {");
			WriteLine ("\t\tString classMessage;");
			WriteLine ("\t\tclassMessage.append(\"{0} Provider\");", ClassName);
			WriteLine ("\t\tclassMessage.append (\" does not support class \");");
			WriteLine ("\t\tclassMessage.append (ref.getClassName().getString());");
			WriteLine ("\t\tthrow CIMNotSupportedException(classMessage);");
			WriteLine ("\t}");
			WriteLine ("");
			WriteLine ("");
			WriteLine ("\thandler.processing();");

			WriteLine ("\t// Make cimom handle deleteInstance request from object path reference.");
			WriteLine ("\tCIMObjectPath localReference = CIMObjectPath(");
			WriteLine ("\t\tString(\"\"),");
			WriteLine ("\t\tCIMNamespaceName(\"root/cimv2\"),");
			WriteLine ("\t\tref.getClassName(),");
			WriteLine ("\t\tref.getKeyBindings());");
			WriteLine ("\t");

			WriteLine ("\tif (_p.find(localReference.getKeyBindings()) {");
			WriteLine ("\t\tif (_p.loadInstance(instanceObject) {");
			WriteLine ("\t\t\tif (!_p.modifyInstance()) {");
			WriteLine ("\t\t\t\t/* Send Modify Indication */");
			WriteLine ("\t\t\t}");
			WriteLine ("\t\t\telse {");
			WriteLine ("\t\t\t\t/* Raise Modify Exception */");
			WriteLine ("\t\t\t}");
			WriteLine ("\t\t}");
			WriteLine ("\t}");
			WriteLine ("\telse {");
			WriteLine ("\t\tthrow CIMObjectNotFoundException(");
			WriteLine ("\t\t\t\tlocalReference.toString());");
			WriteLine ("\t}");

			WriteLine ("\thandler.complete();");
			WriteLine ("}");
			return true;
		}
	}
}

