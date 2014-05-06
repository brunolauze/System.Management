using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Management.Tests
{
	internal class TestScriptMaker : CodeWriterBase
	{
		public TestScriptMaker (ClassManifest manifest)
			:base(manifest)
		{

		}

		public override void Write ()
		{
			WriteLine ("#!/bin/sh");
			WriteLine ("");
			WriteLicense ("#");
			WriteLine ("");
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			AddLdLibs (sb);
			WriteLine ("cd bin/Debug && LD_LIBRARY_PATH=" + sb.ToString () + "/usr/local/lib/pegasus/lib:/usr/local/lib:/usr/lib:/lib ./Test" + ClassName);
		}

		void AddLdLibs (System.Text.StringBuilder sb)
		{
					bool isDependency = DeriveFrom (Manifest, "CIM_Dependency");
					bool isComponent = Class.ClassName.ToString() != "CIM_ConcreteComponent" && DeriveFrom (Manifest, "CIM_Component") && !Manifest.HaveChildren && ContainsProperties("GroupComponent", "PartComponent");
					HashSet<string> deps = new HashSet<string>();
					deps.Add (ClassName);
					if (Manifest.SuperClass != null && !Manifest.SuperClass.HaveChildren) {
						deps.Add(CodeWriterBase.GetClassName(Manifest.SuperClass));
					}
					if (isDependency) {

						System.Management.Internal.CimPropertyReference r1 = GetProperty (Manifest, "Antecedent") as System.Management.Internal.CimPropertyReference;
						System.Management.Internal.CimPropertyReference r2 = GetProperty (Manifest, "Dependent") as System.Management.Internal.CimPropertyReference;

						//* GET ALL GROUP NAMES DERIVING FROM GROUP */
						foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
							deps.Add (el);
						}
						foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
							deps.Add (el);
						}
					} else if (isComponent) {
						// Find GroupComponent Reference Class
						System.Management.Internal.CimPropertyReference r1 = GetProperty(Manifest, "GroupComponent") as System.Management.Internal.CimPropertyReference;
						System.Management.Internal.CimPropertyReference r2 = GetProperty(Manifest, "PartComponent") as System.Management.Internal.CimPropertyReference;
						//* GET ALL GROUP NAMES DERIVING FROM GROUP */
						foreach (var el in GetAllTopClassesOf (r1.ReferenceClass.ToString ())) {
							deps.Add (el);
						}
						foreach (var el in GetAllTopClassesOf (r2.ReferenceClass.ToString ())) {
							deps.Add (el);
						}
					}
					AddMethodIncludes (Manifest, deps);

					if (deps.Count > 0) {
						foreach (var prj in deps) {
							sb.Append ("../../../../../" + prj.Replace ("CIM_", "").Replace ("UNIX_", "") + "/bin/Debug:");
						}
					}
		}

		void AddMethodIncludes (ClassManifest target, HashSet<string> pkgs)
		{
			if (target.SuperClass != null)
				AddMethodIncludes (target.SuperClass, pkgs);

			foreach (var method in target.Class.Methods) {
				foreach (var parameter in method.Parameters) {
					System.Management.Internal.CimParameterReference refParam = parameter as System.Management.Internal.CimParameterReference;
					if (refParam != null) {
						var name = refParam.ReferenceClass.ToString ().Replace("CIM_", "UNIX_");

						var refManifest = GeneratorFactory.Classes.FirstOrDefault (x => !x.HaveChildren && CodeWriterBase.GetClassName (x) == name);
						if (refManifest != null) {
							pkgs.Add(CodeWriterBase.GetClassName (refManifest));
						}
					}
				}
			}

		}

	}
}

