using NUnit.Framework;
using System;
using System.Linq;
using System.Management.Internal;
using System.Collections.Generic;

namespace System.Management.Tests
{
	[TestFixture ()]
	public class WbemClientFixture
	{
		[Test ()]
		public void BuildProviders ()
		{
			WbemClient client = new WbemClient ("localhost", new System.Net.NetworkCredential("bruno", "P0l3n0rd"), "root/cimv2");
			client.IsSecure = false;

			var classes = new Dictionary<CimName, ClassManifest> ();

			ProcessClass (client, new CimName ("CIM_ManagedElement"), null, classes);
			ProcessClass (client, new CimName ("CIM_Indication"), null, classes);
			ProcessClass (client, new CimName ("CIM_AbstractComponent"), null, classes);
			ProcessClass (client, new CimName ("CIM_AbstractElementStatisticalData"), null, classes);
			ProcessClass (client, new CimName ("CIM_AbstractIndicationSubscription"), null, classes);
			ProcessClass (client, new CimName ("CIM_Component"), null, classes);
			ProcessClass (client, new CimName ("CIM_LogicalIdentity"), null, classes);
			ProcessClass (client, new CimName ("CIM_ElementSettingData"), null, classes);
			ProcessClass (client, new CimName ("CIM_Statistics"), null, classes);
			ProcessClass (client, new CimName ("CIM_MemberOfCollection"), null, classes);
			ProcessClass (client, new CimName ("CIM_Dependency"), null, classes);
			ProcessClass (client, new CimName ("CIM_CredentialManagementCapabilities"), null, classes);

			foreach(var c in client.EnumerateClasses ())
			{
				if (!classes.ContainsKey (c.ClassName)) {
					classes.Add (c.ClassName, new ClassManifest(c, null, false));
				}
			}


			Console.WriteLine ("Total Count: " + classes.Count);

			Console.WriteLine ("End Of Schema Count: " + classes.Count(x => !x.Value.HaveChildren));

			/*
			var parents = new Dictionary<string, string> ();
			foreach (var c in classes) {
				var tree = client.EnumerateClassHierarchy (c.ClassName);
				foreach (var t in tree.Children) {
					if (!parents.ContainsKey (t.Name.ToString())) {
						parents.Add (t.Name.ToString(), c.ClassName.ToString());	
					}
				}
			}
			*/

			/*
			foreach (var c in classes) {
				Console.WriteLine (c.Value.Class.ClassName);
				Console.WriteLine ("\tSuperClass: " + c.Value.Class.SuperClass.ToString ());
				foreach (var p in c.Value.Class.Properties) {
					Console.WriteLine ("\t" + p.Name + " Type: " + p.Type);
				}
			}
			*/

			GeneratorFactory.Generate(classes.Where(x => !x.Key.ToString().StartsWith("UNIX_") && 
				!x.Key.ToString ().StartsWith("PG_") &&
				!x.Key.ToString ().Contains("Picker") &&
				!x.Key.ToString ().Contains("ChangerDevice") &&
				!x.Key.ToString ().StartsWith("PRS_") &&
				!x.Key.ToString ().Contains("SNMP") &&
				!x.Key.ToString ().Contains("UnitOfWork") &&
				!x.Key.ToString ().Contains("UoW") &&
				!x.Key.ToString ().Contains("Telnet") &&
				!x.Key.ToString ().Contains ("J2ee")).Select(x => x.Value));
		}

		void ProcessClass (WbemClient client, CimName cimName, ClassManifest superClass, Dictionary<CimName, ClassManifest> classes)
		{
			var classItem = client.GetClass (cimName);
			var tree = client.EnumerateClassHierarchy (cimName);
			bool haveChildren = tree.TreeSize > 1;
			//Check if all are UNIX_ then no children
			if (haveChildren) {
				bool foundDep = false;
				foreach (var c in tree.Children) {
					if (!c.Name.ToString ().StartsWith ("UNIX_")) {
						foundDep = true;
						break;
					}
				}
				if (!foundDep) {
					haveChildren = false;
				}

				if (IsException (cimName)) {
					haveChildren = false;
				}
			} else if (tree.TreeSize == 1) {
				if (superClass == null) {
					/*
					foreach (var c in tree.Children) {
						if (c.Name.ToString () == cimName.ToString ()) {
							haveChildren = true;
							break;
						}
					}
					*/
				} else {
					foreach (var c in tree.Children) {
						if (c.Name.ToString () == superClass.Class.ClassName.ToString ()) {
							haveChildren = true;
							break;
						}
					}
				}
			}
			if (classItem.ClassName.ToString ().Contains("Abstract") || BaseClasses.Contains (classItem.ClassName.ToString ()))
				haveChildren = true;
			FixSuperClass (superClass);
			var manifest = new ClassManifest (classItem, superClass, haveChildren);
			if (!classes.ContainsKey(cimName))
				classes.Add (cimName, manifest);
			
			foreach (var item in tree.Children) {
				ProcessClass (client, item.Name, manifest, classes);
			}
		}

		private static string[] BaseClasses = new string[] {
			"CIM_StorageVolume",
			"CIM_POTSModem"
		};

		private static string[] ClassExceptions = new string[] {
			"CIM_ApplicationSystem",
			"CIM_SoftwareElement",
			"CIM_SoftwareFeature",
			"CIM_FileSystem",
			"CIM_NetworkPort",
			"CIM_EthernetPort",
			"CIM_CredentialManagementCapabilities",
			"CIM_KeyBasedCredentialManagementService",
			"CIM_DisplayController",
			"CIM_PolicyGroup",
			"CIM_PowerSupply",
			"CIM_USBDevice",
			"CIM_ConcreteJob"
		};

		static void FixSuperClass (ClassManifest superClass)
		{
			if (superClass == null)
				return;
			if (ClassExceptions.Contains(superClass.Class.ClassName.ToString ()))
				superClass.Class.ClassName = new System.Management.Internal.CimName(superClass.Class.ClassName.ToString().Replace("CIM_", "UNIX_"));
		}

		static bool IsException (CimName cimName)
		{
			if (ClassExceptions.Contains(cimName.ToString ()))
				return true;
			return false;
		}
	}
}

