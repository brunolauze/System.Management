using System;
using System.Management.Internal;

namespace System.Management.Tests
{
	internal class ClassManifest
	{
		public ClassManifest (CimClass classItem, ClassManifest superClass, bool haveChildren)
		{
			this.Class = classItem;
			this.SuperClass = superClass;
			this.HaveChildren = haveChildren;
		}

		public CimClass Class {
			get;
			private set;
		}

		public ClassManifest SuperClass {
			get;
			private set;
		}

		public bool HaveChildren
		{
			get; set;
		}
	}
}

