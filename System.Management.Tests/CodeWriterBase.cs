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

		public virtual void Save(string filePath)
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

		public static string GetClassName (ClassManifest target)
		{
			return target.HaveChildren ? target.Class.ClassName.ToString () : FixExceptions(target.Class.ClassName.ToString ()).Replace ("CIM_", "UNIX_");
		}

		public string FolderName
		{
			get 
			{
				return FixExceptions (this.Class.ClassName.ToString ()).Replace ("CIM_", "").Replace ("UNIX_", "");
			}
		}

		protected static string FixExceptions (string str)
		{
			if (str == "CIM_UnitaryComputerSystem") return "CIM_ComputerSystem";
			if (str == "CIM_UnixProcess") return "CIM_Process";
			if (str == "CIM_UnixThread") return "CIM_Thread";
			if (str == "CIM_UnixProcessStatistics") return "CIM_ProcessStatistics";
			if (str == "CIM_UnixProcessStatisticalInformation") return "CIM_ProcessStatisticalInformation";
			if (str == "CIM_UnixLocalFileSystem") return "CIM_LocalFileSystem";
			if (str == "CIM_UnixFile") return "CIM_File";
			if (str == "CIM_UnixDeviceFile") return "CIM_DeviceFile";
			if (str == "CIM_UnixDirectory") return "CIM_Directory";
			return str;
		}

		public string SuperClass
		{
			get {
				if (Manifest.SuperClass == null || string.IsNullOrEmpty (this.Class.SuperClass.ToString ())) {
					return "CIM_ClassBase";
				}
				return CodeWriterBase.GetClassName (Manifest.SuperClass);
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

		public bool HasProperty(string name)
		{
			CimProperty property = GetProperty (Manifest, name);
			return property != null;
		}

		public static CimProperty GetProperty(ClassManifest target, string propertyName)
		{
			var p = target.Class.Properties [propertyName];
			if (p == null) {
				if (target.SuperClass != null) {
					return GetProperty (target.SuperClass, propertyName);
				}
			}
			return p;
		}

		public bool IsDerivedFromBase(CimProperty p)
		{
			if (OverridenProperties.Contains (p.Name.ToString ()))
				return false;
			return IsDerivedFromBase (p, this.manifest.SuperClass);
		}

		public static bool IsDerived(CimProperty p, ClassManifest superClass)
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

		public static bool IsDerivedFromBase(CimProperty p, ClassManifest superClass)
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


		protected void DefineProperties(ClassManifest manifestItem)
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

		protected void DefinePropertiesAssign(ClassManifest manifestItem, List<String> added, bool haveChildren)
		{
			if (!haveChildren) {
				if (manifestItem.SuperClass != null)
					DefinePropertiesAssign (manifestItem.SuperClass, added, haveChildren);
			}
			foreach (var p in manifestItem.Class.Properties) {
				if (added.Contains (p.Name.ToString ()))
					continue;
				if (manifestItem.HaveChildren) {
					//if (!IsDerived (p))
					{
						added.Add (p.Name.ToString ());
						DefinePropertyAssign (p);
					}
				} else { //if (!IsDerivedFromBase(p))
					added.Add (p.Name.ToString ());
					DefinePropertyAssign (p);
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
			string decl = GetPropertyDeclaration (name);
			sb.AppendFormat ("#ifndef {0}\n", decl);
			sb.AppendFormat ("#define {0} \\\n\t\t\t\t\t\"{1}\"\n", decl, name);
			sb.AppendLine ("#endif");
			sb.AppendLine ();
		}

		protected void DefinePropertyGetter(CimProperty p)
		{
			string name = p.Name.ToString();
			sb.AppendFormat ("\tvirtual Boolean get{0}(CIMProperty&) const{1};\n", name, DeclarationEnding);
			sb.AppendFormat ("\tvirtual {0} get{1}() const{2};\n", GetCodeType(p), name, DeclarationEnding);
			sb.AppendFormat ("\tvirtual void set{1}({0}&){2};", GetCodeType(p), name, DeclarationEnding);
			sb.AppendLine ();
		}
		protected void DefinePropertyAssign(CimProperty p)
		{
			string name = p.Name.ToString();
			WriteLine(GetDefaultValue("\t_" + name.Substring(0, 1)  + name.Substring(1), p));
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

		public static bool IsOrDeriveFrom(ClassManifest manifestItem, string name)
		{
			if (manifestItem.Class.ClassName == name) return true;
			if (DeriveFrom(manifestItem,  name))
			{
				return true;
			}
			return false;
		}

		protected static bool DeriveFrom(ClassManifest manifestItem, string name)
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
			sb.AppendFormat ("\treturn {0};\n", "_" + name.Substring (0, 1).ToLower() + name.Substring (1));
			sb.AppendLine ("}");
			sb.AppendLine ();
			sb.AppendFormat ("void {1}::set{2}({0} &value)\n", GetCodeType (p), ClassName, name);
			sb.AppendLine ("{");
			sb.AppendFormat ("\t{0} = value;\n", "_" + name.Substring (0, 1).ToLower() + name.Substring(1));
			sb.AppendLine ("}");
			sb.AppendLine ();
		}

		protected string GetDefaultValue (string privateProperty, CimProperty p)
		{
			string name = p.Name.ToString ();
			if (name == "SystemCreationClassName") {
				return privateProperty + " = String(\"UNIX_ComputerSystem\");";
			}
			if (name == "CreationClassName") {
				return privateProperty + " = String(\"" + ClassName + "\");";
			}
			if (name == "CSCreationClassName") {
				return privateProperty + " = String(\"UNIX_ComputerSystem\");";
			}
			if (name == "OSCreationClassName") {
				return privateProperty + " = String(\"UNIX_OperatingSystem\");";
			}
			if (name == "FSCreationClassName") {
				return privateProperty + " = String(\"UNIX_FileSystem\");";
			}
			if (name == "ElementName") {
				/* UNIX_XXXX = XXXX */
				return privateProperty + " = String(\"" + ClassName.Replace("CIM_", "").Replace("UNIX_", "") + "\");";
			}
			if (name == "SystemName" ||
				name == "CSName") {
				return privateProperty + " = CIMHelper::HostName;";
			}
			if (name == "OSName") {
				return privateProperty + " = CIMHelper::OSName;";
			}
			if (name == "Status" && p.Type == System.Management.Internal.CimType.STRING) {
				return privateProperty + " = String(DEFAULT_STATUS);";
			}
			if (name == "ComminucationStatus" && p.Type == System.Management.Internal.CimType.UINT16) {
				return privateProperty + " = Uint16(DEFAULT_COMMUNICATION_STATUS);";
			}
			if (name == "OperatingStatus" && p.Type == System.Management.Internal.CimType.UINT16) {
				return privateProperty + " = Uint16(DEFAULT_OPERATING_STATUS);";
			}
			if (name == "HealthState" && p.Type == System.Management.Internal.CimType.UINT16) {
				return privateProperty + " = Uint16(DEFAULT_HEALTH_STATE);";
			}
			if (name == "PrimaryStatus" && p.Type == System.Management.Internal.CimType.UINT16) {
				return privateProperty + " = Uint16(DEFAULT_PRIMARY_STATUS);";
			}
			if (name == "EnabledState" && p.Type == System.Management.Internal.CimType.UINT16) {
				return privateProperty + " = Uint16(DEFAULT_ENABLED_STATE);";
			}
			bool isArray = (p as CimPropertyArray) != null;
			var typeName = GetCodeType (p);
			CimType newType;
			bool parsed = Enum.TryParse (p.Type.ToString (), true, out newType);
			if (isArray) {
				/* Build default Array */
				/*
				return typeName + " as;\n" +
					"\t\n\n\treturn as;\n";
				*/
				return privateProperty + ".clear();";
			} else {
				if (typeName.StartsWith ("Uint") || typeName.StartsWith ("Sint") || typeName.StartsWith ("Real")) {
					return privateProperty + " = " + typeName + "(0);";
				} else if (p.Type == System.Management.Internal.CimType.STRING) {
					return privateProperty + " = String (\"\");";
				} else if (p.Type == System.Management.Internal.CimType.DATETIME) {
					/*
					return "struct tm* clock" + privateProperty + ";\t\t\t// create a time structure\n" +
						"\ttime_t val" + privateProperty + " = time(NULL);\n" +
						"\tclock" + privateProperty + " = gmtime(&(val" + privateProperty + "));\t// Get the last modified time and put it into the time structure\n" +
						"\t" + privateProperty + " = CIMDateTime(\n" +
						"\t\tclock" +  privateProperty + "->tm_year + 1900,\n" +
						"\t\tclock" + privateProperty + "->tm_mon + 1,\n" +
						"\t\tclock" + privateProperty + "->tm_mday,\n" +
						"\t\tclock" + privateProperty + "->tm_hour,\n" +
						"\t\tclock" + privateProperty + "->tm_min,\n" +
						"\t\tclock" + privateProperty + "->tm_sec,\n" +
						"\t\t0,0,\n" +
						"\t\tclock" + privateProperty + "->tm_gmtoff);";
					*/
					return privateProperty + " = CIMHelper::getCurrentTime();";
				}
				if (parsed) {
					if (newType == CimType.Boolean) {
						return privateProperty + " = Boolean(false);";
					} else if (newType == CimType.Reference) {
						System.Management.Internal.CimPropertyReference refProp = p as System.Management.Internal.CimPropertyReference;
						var refName = GetConcreteClass (refProp.ReferenceClass.ToString ());
						if (!string.IsNullOrEmpty (refName)) {
							if (DeriveFrom (Manifest, "CIM_Dependency")) {
								string nameDep = refName.Replace ("CIM_", "").Replace ("UNIX_", "");
								nameDep = privateProperty + nameDep;
								return privateProperty + " = " + nameDep + "Provider.constructInstance(\n\t\tCIMName(\"" + refName.Replace ("CIM_", "UNIX_") + "\"),\n\t\tCIMNamespaceName(\"root/cimv2\"),\n\t\t" + nameDep + ");";
							} else {
								return string.Format (privateProperty + " = CIMInstance(CIMName(\"{0}\"));", refName);
							}
						}
						return "";
					} else if (newType == CimType.Object) {
						return privateProperty + " = NULL;";
					}
				}
			}
			return privateProperty + " = NULL;";
		}

		private string GetConcreteClass (string str)
		{
			if (str == "CIM_System")
				return "UNIX_ComputerSystem";

			var current = GeneratorFactory.Classes.FirstOrDefault (x => x.Class.ClassName == str);
			if (current != null) {
				if (current.HaveChildren) {
					foreach (var manifestItem in GeneratorFactory.Classes) {
						if (DeriveFrom (manifestItem, str) && !manifestItem.HaveChildren) {
							return FixExceptions(manifestItem.Class.ClassName.ToString());
						}
					}
				}
				return FixExceptions(current.Class.ClassName.ToString());
			}
			return null;
		}

		protected string GetDefaultValue (string privateProperty, CimMethod method)
		{
			var typeName = GetCodeType (method);
			CimType newType;
			bool parsed = Enum.TryParse (method.Type.ToString (), true, out newType);
			if (typeName.StartsWith ("Uint") || typeName.StartsWith ("Sint") || typeName.StartsWith ("Real")) {
				return privateProperty + " = " + typeName + "(0);";
			} else if (method.Type == System.Management.Internal.CimType.STRING) {
				return privateProperty + " = String (\"\");";
			} else if (method.Type == System.Management.Internal.CimType.DATETIME) {
				return "struct tm* clock;\t\t\t// create a time structure\n" +
					"\ttime_t val = time(NULL);\n" +
					"\tclock = gmtime(&(val));\t// Get the last modified time and put it into the time structure\n" +
					privateProperty + " = CIMDateTime(\n" +
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
					return privateProperty + " = Boolean(false);";
				} else if (newType == CimType.Reference) {
					return string.Format (privateProperty + " = CIMInstance(CIMName(\"{0}\"));", "CIM_ManagedElement");
				} else if (newType == CimType.Object) {
					return privateProperty + " = NULL;";
				}
			}
			return privateProperty + " = NULL;";
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

		protected string GetCodeType (System.Management.Internal.CimMethod p)
		{
			var type = p.Type;
			CimType newType;
			if (type == System.Management.Internal.CimType.REFERENCE)
				return "CIMInstance";
			if (type == System.Management.Internal.CimType.DATETIME) {
				return "CIMDateTime";
			}
			if (Enum.TryParse (type.ToString (), true, out newType))
				return newType.ToString ().Replace ("UInt", "Uint").Replace ("SInt", "Sint");
			else
				throw new NotImplementedException ("Not implemented yet.");	
		}

		protected string GetCodeType (System.Management.Internal.CimParameter p)
		{
			bool isArray = (p as CimParameterArray) != null;
			var type = p.Type;
			CimType newType;
			if (type == System.Management.Internal.CimType.REFERENCE)
				return "CIMInstance";
			if (type == System.Management.Internal.CimType.DATETIME) {
				return "CIMDateTime";
			}
			System.Management.Internal.CimParameterReference refParam = p as System.Management.Internal.CimParameterReference;
			if (refParam != null) {
				return "CIMInstance";
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
			return c.Replace ("_I_D", "_ID")
				.Replace ("_F_S", "_FS")
				.Replace ("_C_S", "_CS")
				.Replace ("_Qo_S", "_QOS")
				.Replace ("_O_O_B", "_OOB")
				.Replace ("_R_E_D", "_RED")
				.Replace("_F_C_H_B_A", "_FCHBA")
				.Replace("_W_B_E_M", "_WBEM")
				.Replace("_I_B", "_IB")
				.Replace("_B_I_O_S", "_BIOS")
				.Replace("_U_S_B", "_USB")
				.Replace("_I_E_E_E", "_IEEE")
				.Replace("_V_L_A_N", "_VLAN")
				.Replace("_G_A_R_P", "_GARP")
				.Replace("_A_R_P", "_ARP")
				.Replace("_L_A_G", "_LAG")
				.Replace("_E_S_P", "_ESP")
				.Replace("_S_A_P", "_SAP")
				.Replace("_C_R_L", "_CRL")
				.Replace("_G_U_I_D", "_GUID")
				.Replace("_B_G_P", "_BGP")
				.Replace("_M_P_L_S", "_MPLS")
				.Replace("_S_L_P", "_SLP")
				.Replace("_C_L_P", "_CLP")
				.Replace("_R_A_I_D", "_RAID")
				.Replace("_S_C_S_I", "_SCSI")
				.Replace("_S_A_T_A", "_SATA")
				.Replace("_D_M_A", "_DMA")
				.Replace("_X_5", "_X5")
				.Replace("_P_O_T_S", "_POTS")
				.Replace("_L_L_D_P", "_LLDP")
				.Replace("_L_D_A_P", "_LDAP")
				.Replace("_I_S_D_N", "_ISDN")
				.Replace("_I_P_S_O", "_IPSO")
				.Replace("_S_A_S", "_SAS")
				.Replace("_W_O_R_M", "_WORM")
				.Replace("_C_D_R_O_M", "_CDROM")
				.Replace("_D_V_D", "_DVD")
				.Replace("_A_D_S_L", "_ADSL")
				.Replace("_V_D_S_L", "_VDSL")
				.Replace("_H_D_S_L", "_HDSL")
				.Replace("_S_D_S_L", "_SDSL")
				.Replace("_D_S_L", "_DSL")
				.Replace("_S_P_I", "_SPI")
				.Replace("_P_C_Ie", "_PCI_e")
				.Replace("_P_C_I", "_PCI")
				.Replace("_P_C_M_C_I", "_PCMCI")
				.Replace("_E_S_C_O_N", "_ESCON")
				.Replace("_S_S_A", "_SSA")
				.Replace("_A_T_A", "_ATA")
				.Replace("_N_F_S", "_NFS")
				.Replace("_M_P_L_S_L_S_P", "_MPLSLSP")
				.Replace("_M_P_L_S_L_S", "_MPLSLS")
				.Replace("_M_P_L_S_C_R_L_S_P", "_MPLSCRLSP")
				.Replace("_M_P_L", "_MPL")
				.Replace("_O_S_P_F", "_OSPF")
				.Replace("_C_P_U", "_CPU")
				.Replace("_O_S", "_OS")
				.Replace("_F_C_H_B_A", "_FCHBA")
				.Replace("_F_C", "_FC")
				.Replace("_T_C_P", "_TCP")
				.Replace("_U_D_P", "_UDP")
				.Replace("_D_N_S", "_DNS")
				.Replace("_D_H_C_P", "_DHCP")
				.Replace("_D_S_C_P", "_DSCP")
				.Replace("_N_A_T", "_NAT")
				.Replace("_VRF", "_VRF")
				.Replace("_V_L_T", "_VLT")
				.Replace("_A_H_T", "_AHT")
				.Replace("_I_K_E", "_IKE")
				.Replace("_I_P_C_O_M_P", "_IPCOMP")
				.Replace("_I_P", "_IP")
				.Replace("_T_P_M", "_TPM")
				.Replace("_I_R_Q", "_IRQ")
				.Replace("_I_O", "_IO")
				.Replace("_C_I_F_S", "_CIFS")
				.Replace("_S_W_R", "_SWR")
				.Replace("_A_F", "_AF")
				.Replace("_E_F", "_EF")
				.Replace("_O_U", "_OU")
				.Replace("_A_G_P", "_AGP")
				.Replace("_Wi_Fi", "_WiFi");
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

		protected void WriteLicense(string linePrefix = "")
		{
			sb.AppendLine (linePrefix + "//%LICENSE////////////////////////////////////////////////////////////////");
			sb.AppendLine (linePrefix + "//");
			sb.AppendLine (linePrefix + "// Licensed to The Open Group (TOG) under one or more contributor license");
			sb.AppendLine (linePrefix + "// agreements.  Refer to the OpenPegasusNOTICE.txt file distributed with");
			sb.AppendLine (linePrefix + "// this work for additional information regarding copyright ownership.");
			sb.AppendLine (linePrefix + "// Each contributor licenses this file to you under the OpenPegasus Open");
			sb.AppendLine (linePrefix + "// Source License; you may not use this file except in compliance with the");
			sb.AppendLine (linePrefix + "// License.");
			sb.AppendLine (linePrefix + "//");
			sb.AppendLine (linePrefix + "// Permission is hereby granted, free of charge, to any person obtaining a");
			sb.AppendLine (linePrefix + "// copy of this software and associated documentation files (the \"Software\"),");
			sb.AppendLine (linePrefix + "// to deal in the Software without restriction, including without limitation");
			sb.AppendLine (linePrefix + "// the rights to use, copy, modify, merge, publish, distribute, sublicense,");
			sb.AppendLine (linePrefix + "// and/or sell copies of the Software, and to permit persons to whom the");
			sb.AppendLine (linePrefix + "// Software is furnished to do so, subject to the following conditions:");
			sb.AppendLine (linePrefix + "//");
			sb.AppendLine (linePrefix + "// The above copyright notice and this permission notice shall be included");
			sb.AppendLine (linePrefix + "// in all copies or substantial portions of the Software.");
			sb.AppendLine (linePrefix + "//");
			sb.AppendLine (linePrefix + "// THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS");
			sb.AppendLine (linePrefix + "// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF");
			sb.AppendLine (linePrefix + "// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.");
			sb.AppendLine (linePrefix + "// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY");
			sb.AppendLine (linePrefix + "// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,");
			sb.AppendLine (linePrefix + "// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE");
			sb.AppendLine (linePrefix + "// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
			sb.AppendLine (linePrefix + "//");
			sb.AppendLine (linePrefix + "//////////////////////////////////////////////////////////////////////////");
			sb.AppendLine (linePrefix + "//");
			sb.AppendLine (linePrefix + "//%/////////////////////////////////////////////////////////////////////////");
		}

		protected int GetKeyCount(ClassManifest target)
		{
			int keyCount = 0;
			if (target == null)
				return keyCount;
			if (target.Class.HasKeyProperty) {
				foreach (var p in target.Class.Properties) {
					if (p.IsKeyProperty) {
						keyCount++;
					}
				}
			}
			if (keyCount == 0 && target.SuperClass != null) {
				return GetKeyCount (target.SuperClass);
			}
			return keyCount;
		}
	}
}

