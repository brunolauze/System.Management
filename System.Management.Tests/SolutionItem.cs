﻿using System;

namespace System.Management.Tests
{
	public class SolutionItem
	{
		public SolutionItem (string className, Guid projectId, bool isTest)
		{
			Name = className;
			if (isTest) {
				Path = "src\\Providers\\UNIXProviders\\" + className.Replace ("CIM_", "").Replace ("UNIX_", "") + "\\tests\\" + className.Replace ("CIM_", "").Replace ("UNIX_", "")  + ".Tests\\" + className + ".Tests.cproj";
			}
			else {
				Path = "src\\Providers\\UNIXProviders\\" + className.Replace ("CIM_", "").Replace ("UNIX_", "") + "\\" + className + ".cproj";
			}
			ProjectId = projectId;
			SchemaId = Guid.NewGuid ();
		}

		public string Name {
			get;
			set;
		}

		public string Path {
			get;
			set;
		}

		public Guid ProjectId {
			get;
			set;
		}
		public Guid SchemaId {
			get;
			set;
		}

	}
}

