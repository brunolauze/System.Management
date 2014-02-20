using System;
using System.Linq;
using System.Management.Internal;
using System.Text;
using System.Collections.Generic;

namespace System.Management.Tests
{
	internal abstract class CodeWriterBase : ICodeWriter
	{
		private static string[] OverridenProperties = new string[] {
			"Name",
			"InstanceID",
			"Caption",
			"Description",
			"InstallDate",
			"Antecedent",
			"Dependent",
			"GroupComponent",
			"PartComponent"
		};

		private StringBuilder sb;
		private readonly ClassManifest manifest;

		public CodeWriterBase (ClassManifest manifest)
		{
			this.manifest = manifest;
			sb = new StringBuilder ();
		}

		public void Save(string filePath)
		{
			System.IO.File.WriteAllText (filePath, sb.ToString ());
		}

		public ClassManifest Manifest
		{
			get { return this.manifest; }
		}

		public CimClass Class
		{
			get { return this.manifest.Class; }
		}

		public string ClassName
		{
			get
			{
				return this.manifest.HaveChildren ? this.Class.ClassName.ToString () : FixExceptions(this.Class.ClassName.ToString ()).Replace ("CIM_", "UNIX_");
			}
		}

		public string FolderName
		{
			get 
			{
				return FixExceptions (this.Class.ClassName.ToString ()).Replace ("CIM_", "").Replace ("UNIX_", "");
			}
		}

		protected string FixExceptions (string str)
		{
			if (str == "CIM_UnitaryComputerSystem") return "CIM_ComputerSystem";
			if (str == "CIM_UnixProcess") return "CIM_Process";
			if (str == "CIM_UnixThread") return "CIM_Thread";
			if (str == "CIM_UnixProcessStatistics") return "CIM_ProcessStatistics";
			if (str == "CIM_UnixProcessStatisticalInformation") return "CIM_ProcessStatisticalInformation";
			if (str == "CIM_UnixLocalFileSystem") return "CIM_LocalFileSystem";
			if (str == "CIM_UnixFile") return "CIM_File";
			if (str == "CIM_UnixDeviceFile") return "CIM_DeviceFile";
			if (str == "CIM_Directory") return "CIM_Directory";
			return str;
		}

		public string SuperClass
		{
			get {
				if (string.IsNullOrEmpty (this.Class.SuperClass.ToString ())) {
					return "CIM_ClassBase";
				}
				return this.Class.SuperClass.ToString ();
			}
		}

		public abstract void Write();

		protected void Write(string value, params object[] args)
		{
			if (args == null || args.Length == 0) {
				sb.Append (value);
			} else {
				sb.AppendFormat (value, args);
			}
		}

		public bool IsDerived(CimProperty p)
		{
			return IsDerived (p, this.manifest.SuperClass);
		}

		public bool IsDerivedFromBase(CimProperty p)
		{
			if (OverridenProperties.Contains (p.Name.ToString ()))
				return false;
			return IsDerivedFromBase (p, this.manifest.SuperClass);
		}

		public bool IsDerived(CimProperty p, ClassManifest superClass)
		{
			bool found = false;
			if (superClass == null)
				return false;
			foreach (var sp in superClass.Class.Properties) {
				if (sp.Name == p.Name) {
					found = true;
					break;
				}
			}
			if (!found)
				return IsDerived (p, superClass.SuperClass);
			return found;
		}

		public bool IsDerivedFromBase(CimProperty p, ClassManifest superClass)
		{
			bool found = false;
			bool wasBase = false;
			if (superClass == null)
				return false;
			foreach (var sp in superClass.Class.Properties) {
				if (sp.Name == p.Name) {
					if (superClass.Class.ClassName.ToString () == "CIM_ManagedSystemElement" ||
						superClass.Class.ClassName.ToString () == "CIM_ManagedElement")
						wasBase = true;
					found = true;
					break;
				}
			}
			if (!found)
				return IsDerived (p, superClass.SuperClass);
			return found && wasBase;
		}


		protected  void DefineProperties(ClassManifest manifestItem)
		{
			foreach (var p in manifestItem.Class.Properties) {
				if (!IsDerived(p))
					DefineProperty (p.Name.ToString());
			}
		}

		protected void DeclareProperties(ClassManifest manifestItem, List<String> added, bool haveChildren)
		{
			if (!haveChildren) {
				if (manifestItem.SuperClass != null)
					DeclareProperties (manifestItem.SuperClass, added, haveChildren);
			}
			foreach (var p in manifestItem.Class.Properties) {
				if (added.Contains (p.Name.ToString ()))
					continue;
				if (manifestItem.HaveChildren) {
					//if (!IsDerived (p))
					{
						added.Add (p.Name.ToString ());
						DeclarePropertyGetter (p);
					}
				} else { //if (!IsDerivedFromBase(p))
					added.Add (p.Name.ToString ());
					DeclarePropertyGetter (p);
				}
			}
		}

		protected void DefinePropertiesGetter(ClassManifest manifestItem, List<String> added, bool haveChildren)
		{
			if (!haveChildren) {
				if (manifestItem.SuperClass != null)
					DefinePropertiesGetter (manifestItem.SuperClass, added, haveChildren);
			}
			foreach (var p in manifestItem.Class.Properties) {
				if (added.Contains (p.Name.ToString ()))
					continue;
				if (manifestItem.HaveChildren) {
					//if (!IsDerived (p))
					{
						added.Add (p.Name.ToString ());
						DefinePropertyGetter (p);
					}
				} else { //if (!IsDerivedFromBase(p))
					added.Add (p.Name.ToString ());
					DefinePropertyGetter (p);
				}
			}
		}

		protected void WriteLine(string value, params object[] args)
		{
			if (args == null || args.Length == 0) {
				sb.AppendLine (value);
			} else {
				sb.AppendFormat (value + "\n", args);
			}
		}

		protected void DefineProperty(string name)
		{
			sb.AppendFormat ("#define {0}\t\t\t\t\"{1}\"\n", GetPropertyDeclaration (name), name);
		}

		protected void DefinePropertyGetter(CimProperty p)
		{
			string name = p.Name.ToString();
			sb.AppendFormat ("\tvirtual Boolean get{0}(CIMProperty&) const{1};\n", name, DeclarationEnding);
			sb.AppendFormat ("\tvirtual {0} get{1}() const{2};", GetCodeType(p), name, DeclarationEnding);
			sb.AppendLine ();
		}

		protected void DeclarePropertyGetter(CimProperty p)
		{
			string name = p.Name.ToString();
			sb.AppendFormat ("Boolean {0}::get{1}(CIMProperty &p) const\n", ClassName, name);
			sb.AppendLine ("{");
			sb.AppendFormat ("\tp = CIMProperty({0}, get{1}());\n", GetPropertyDeclaration(name), name);
			sb.AppendLine ("\treturn true;");
			sb.AppendLine ("}");
			sb.AppendLine ();

			DeclarePropertyGetterValue (p);
		}

		protected IEnumerable<string> GetAllTopClassesOf(string superClassName)
		{
			if (superClassName == "CIM_System")
				return new string[] { "UNIX_ComputerSystem" };

			List<string> names = new List<string> ();
			if (GeneratorFactory.Classes.Any (x => x.Class.ClassName.ToString () == superClassName && !x.HaveChildren)) {
				var name = FixExceptions (superClassName.ToString ());
				if (!name.StartsWith ("UNIX_"))
					name = name.Replace ("CIM_", "UNIX_");
				names.Add (name);
				return names;
			}
			foreach (var manifestItem in GeneratorFactory.Classes) {
				if (DeriveFrom (manifestItem, superClassName) && !manifestItem.HaveChildren) {
					var name = FixExceptions (manifestItem.Class.ClassName.ToString ());
					if (!name.StartsWith ("UNIX_"))
						name = name.Replace ("CIM_", "UNIX_");
					names.Add (name);
				}
			}
			return names;
		}

		protected bool DeriveFrom(ClassManifest manifestItem, string name)
		{
			if (manifestItem.SuperClass == null)
				return false;
			if (manifestItem.SuperClass.Class.ClassName.ToString ().Equals (name))
				return true;
			return DeriveFrom (manifestItem.SuperClass, name);
		}
			
		protected bool ContainsProperties(params string[] properties)
		{
			int count = 0;
			foreach (var p in Manifest.Class.Properties) {
				foreach (var c in properties) {
					if (c == p.Name.ToString ()) {
						count++;
						break;
					}
				}
			}
			return count == properties.Length;
		}

		void DeclarePropertyGetterValue (CimProperty p)
		{
			string name = p.Name.ToString ();
			sb.AppendFormat ("{0} {1}::get{2}() const\n", GetCodeType (p), ClassName, name);
			sb.AppendLine ("{");
			sb.AppendFormat ("\t{0}\n", GetDefaultValue (p));
			sb.AppendLine ("}");
			sb.AppendLine ();
		}

		private string GetDefaultValue (CimProperty p)
		{
			string name = p.Name.ToString ();
			if (name == "SystemCreationClassName") {
				return "return String(\"UNIX_ComputerSystem\");";
			}
			if (name == "CreationClassName") {
				return "return String(\"" + ClassName + "\");";
			}
			if (name == "CSCreationClassName") {
				return "return String(\"UNIX_ComputerSystem\");";
			}
			if (name == "OSCreationClassName") {
				return "return String(\"UNIX_OperatingSystem\");";
			}
			if (name == "FSCreationClassName") {
				return "return String(\"UNIX_FileSystem\");";
			}
			if (name == "ElementName") {
				/* UNIX_XXXX = XXXX */
				return "return String(\"" + ClassName.Replace("CIM_", "").Replace("UNIX_", "") + "\");";
			}
			if (name == "SystemName" ||
				name == "CSName") {
				return "return CIMHelper::HostName;";
			}
			if (name == "OSName") {
				return "return CIMHelper::OSName;";
			}
			if (name == "Status") {
				return "return String(DEFAULT_STATUS);";
			}
			if (name == "ComminucationStatus") {
				return "return Uint16(DEFAULT_COMMUNICATION_STATUS);";
			}
			if (name == "OperatingStatus") {
				return "return Uint16(DEFAULT_OPERATING_STATUS);";
			}
			if (name == "HealthState") {
				return "return Uint16(DEFAULT_HEALTH_STATE);";
			}
			if (name == "PrimaryStatus") {
				return "return Uint16(DEFAULT_PRIMARY_STATUS);";
			}
			if (name == "EnabledState") {
				return "return Uint16(DEFAULT_ENABLED_STATE);";
			}
			bool isArray = (p as CimPropertyArray) != null;
			var typeName = GetCodeType (p);
			CimType newType;
			bool parsed = Enum.TryParse (p.Type.ToString (), true, out newType);
			if (isArray) {
				/* Build default Array */
				return typeName + " as;\n" +
					"\t\n\n\treturn as;\n";
			} else {
				if (typeName.StartsWith ("Uint") || typeName.StartsWith ("Sint") || typeName.StartsWith ("Real")) {
					return "return " + typeName + "(0);";
				} else if (p.Type == System.Management.Internal.CimType.STRING) {
					return "return String (\"\");";
				} else if (p.Type == System.Management.Internal.CimType.DATETIME) {
					return "struct tm* clock;\t\t\t// create a time structure\n" +
						"\ttime_t val = time(NULL);\n" +
						"\tclock = gmtime(&(val));\t// Get the last modified time and put it into the time structure\n" +
						"\treturn CIMDateTime(\n" +
						"\t\tclock->tm_year + 1900,\n" +
						"\t\tclock->tm_mon + 1,\n" +
						"\t\tclock->tm_mday,\n" +
						"\t\tclock->tm_hour,\n" +
						"\t\tclock->tm_min,\n" +
						"\t\tclock->tm_sec,\n" +
						"\t\t0,0,\n" +
						"\t\tclock->tm_gmtoff);";
				}
				if (parsed) {
					if (newType == CimType.Boolean) {
						return "return Boolean(false);";
					} else if (newType == CimType.Reference) {
						return string.Format("return CIMInstance(CIMName(\"{0}\"));", GetReferenceClass());
					} else if (newType == CimType.Object) {
						return "return NULL;";
					}
				}
			}
			return "return NULL;";
		}

		private string GetReferenceClass ()
		{
			string ret = "CIM_ManagedElement";
			if (Manifest.Class.ClassName == "CIM_Dependency") {
				return "CIM_Dependency";
			} else if (Manifest.Class.ClassName == "CIM_Component") {
				return "CIM_Component";
			}
			string check;
			if (Manifest.SuperClass != null) {
				if (GetReferenceClass (Manifest.SuperClass, out check)) {
					ret = check;
				}
			}
			return ret;
		}

		private bool GetReferenceClass (ClassManifest superManifest, out string ret)
		{
			ret = "";
			if (superManifest.Class.ClassName == "CIM_Dependency") {
				ret = "CIM_Dependency";
				return true;
			} else if (superManifest.Class.ClassName == "CIM_Component") {
				ret = "CIM_Component";
				return true;
			}
			if (superManifest.SuperClass == null)
				return false;
			return GetReferenceClass (superManifest.SuperClass, out ret);
		}

		protected string GetCodeType (System.Management.Internal.CimProperty p)
		{
			bool isArray = (p as CimPropertyArray) != null;
			var type = p.Type;
			CimType newType;
			if (type == System.Management.Internal.CimType.REFERENCE)
				return "CIMInstance";
			if (type == System.Management.Internal.CimType.DATETIME) {
				return "CIMDateTime";
			}
			if (Enum.TryParse (type.ToString (), true, out newType))
				return (isArray ? "Array<" : "") + newType.ToString ().Replace ("UInt", "Uint").Replace ("SInt", "Sint") + (isArray ? ">" : "");
			else
				throw new NotImplementedException ("Not implemented yet.");	
		}

		protected static string GetPropertyDeclaration (string name)
		{
			string c = "PROPERTY_";
			for (int i = 0; i < name.Length; i++) {
				if (i == 0) {
					c += name [i].ToString ().ToUpper ();
				}
				else if (name[i].ToString().ToUpper() == name[i].ToString()) {
					c += "_" + name[i].ToString().ToUpper();
				}
				else {
					c += name[i].ToString().ToUpper();
				}
			}
			return c.Replace("_I_D", "_ID")
				.Replace("_F_S", "_FS")
				.Replace("_C_S", "_CS")
				.Replace("_O_S", "_OS");
		}

		protected static bool IsKeyed(ClassManifest manifestItem)
		{
			if (manifestItem.Class.HasKeyProperty)
				return true;
			if (manifestItem.SuperClass != null)
				return IsKeyed (manifestItem.SuperClass);
			return false;
		}

		protected IEnumerable<System.Management.Internal.CimProperty> GetKeyProperties()
		{
			Dictionary<string, System.Management.Internal.CimProperty> keys = new Dictionary<string, CimProperty> ();
			FillKeyProperties (Manifest, keys);
			return keys.Values;
		}

		private static void FillKeyProperties(ClassManifest manifestItem, Dictionary<string, System.Management.Internal.CimProperty> keys)
		{
			if (manifestItem.SuperClass != null) {
				FillKeyProperties (manifestItem.SuperClass, keys);
			}
			foreach (var p in manifestItem.Class.Properties) {
				if (p.IsKeyProperty && !keys.ContainsKey(p.Name.ToString())) {
					keys.Add (p.Name.ToString(), p);
				}
			}
		}

		public string DeclarationEnding
		{
			get {
				if (Manifest.HaveChildren)
					return "=0";
				return "";
			}
		}

		protected void WriteLicense()
		{
			sb.AppendLine ("//%LICENSE////////////////////////////////////////////////////////////////");
			sb.AppendLine ("//");
			sb.AppendLine ("// Licensed to The Open Group (TOG) under one or more contributor license");
			sb.AppendLine ("// agreements.  Refer to the OpenPegasusNOTICE.txt file distributed with");
			sb.AppendLine ("// this work for additional information regarding copyright ownership.");
			sb.AppendLine ("// Each contributor licenses this file to you under the OpenPegasus Open");
			sb.AppendLine ("// Source License; you may not use this file except in compliance with the");
			sb.AppendLine ("// License.");
			sb.AppendLine ("//");
			sb.AppendLine ("// Permission is hereby granted, free of charge, to any person obtaining a");
			sb.AppendLine ("// copy of this software and associated documentation files (the \"Software\"),");
			sb.AppendLine ("// to deal in the Software without restriction, including without limitation");
			sb.AppendLine ("// the rights to use, copy, modify, merge, publish, distribute, sublicense,");
			sb.AppendLine ("// and/or sell copies of the Software, and to permit persons to whom the");
			sb.AppendLine ("// Software is furnished to do so, subject to the following conditions:");
			sb.AppendLine ("//");
			sb.AppendLine ("// The above copyright notice and this permission notice shall be included");
			sb.AppendLine ("// in all copies or substantial portions of the Software.");
			sb.AppendLine ("//");
			sb.AppendLine ("// THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS");
			sb.AppendLine ("// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF");
			sb.AppendLine ("// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.");
			sb.AppendLine ("// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY");
			sb.AppendLine ("// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,");
			sb.AppendLine ("// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE");
			sb.AppendLine ("// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
			sb.AppendLine ("//");
			sb.AppendLine ("//////////////////////////////////////////////////////////////////////////");
			sb.AppendLine ("//");
			sb.AppendLine ("//%/////////////////////////////////////////////////////////////////////////");
		}
	}
}

