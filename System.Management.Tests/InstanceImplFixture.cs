﻿using System;

namespace System.Management.Tests
{
	internal class InstanceImplFixture : CodeWriterBase
	{
		public InstanceImplFixture (ClassManifest manifest)
			: base(manifest)
		{

		}

		public override void Write ()
		{
			WriteLicense ();
			WriteLine ("");
			WriteLine ("#include \"{0}Fixture.h\"", ClassName);
			WriteLine ("#include <{0}/{1}Provider.h>", FolderName, ClassName);
			WriteLine ("");
			WriteLine ("{0}Fixture::{0}Fixture()", ClassName);
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("{0}Fixture::~{0}Fixture()", ClassName);
			WriteLine ("{");
			WriteLine ("}");
			WriteLine ("");
			WriteLine ("void {0}Fixture::Run()", ClassName);
			WriteLine ("{");
			WriteLine ("\tCIMName className(\"{0}\");", ClassName);
			WriteLine ("\tCIMNamespaceName nameSpace(\"root/cimv2\");");
			WriteLine ("\t{0} _p;", ClassName);
			WriteLine ("\t{0}Provider _provider;", ClassName);
			WriteLine ("\tUint32 propertyCount;");
			WriteLine ("\tCIMOMHandle omHandle;");
			WriteLine ("\t_provider.initialize(omHandle);");
			WriteLine ("\t_p.initialize();");
			WriteLine ("");
			WriteLine ("\tfor(int pIndex = 0; _p.load(pIndex); pIndex++)");
			WriteLine ("\t{");
			WriteLine ("\t\tCIMInstance instance = _provider.constructInstance(className,");
			WriteLine ("\t\t\t\t\tnameSpace,");
			WriteLine ("\t\t\t\t\t_p);");
			WriteLine ("\t\tCIMObjectPath path = instance.getPath();");
			WriteLine ("\t\tcout << path.toString() << endl;");
			WriteLine ("\t\tpropertyCount = instance.getPropertyCount();");
			WriteLine ("\t\tfor(Uint32 i = 0; i < propertyCount; i++)");
			WriteLine ("\t\t{");
			WriteLine ("");
			WriteLine ("\t\t\tCIMProperty propertyItem = instance.getProperty(i);");
			WriteLine ("\t\t\tif (propertyItem.getType() == CIMType::REFERENCE) {");
			WriteLine ("\t\t\t\tCIMValue subValue = propertyItem.getValue();");
			WriteLine ("\t\t\t\tCIMInstance subInstance;");
			WriteLine ("\t\t\t\tsubValue.get(subInstance);");
			WriteLine ("\t\t\t\tCIMObjectPath subPath = subInstance.getPath();");
			WriteLine ("\t\t\t\tcout << \"\tName: \" << propertyItem.getName().getString() << \": \" << subPath.toString() << endl;");
			WriteLine ("\t\t\t\tUint32 subPropertyCount = subInstance.getPropertyCount();");
			WriteLine ("\t\t\t\tfor(Uint32 j = 0; j < subPropertyCount; j++)");
			WriteLine ("\t\t\t\t{");
			WriteLine ("\t\t\t\t\tCIMProperty subPropertyItem = subInstance.getProperty(j);");
			WriteLine ("\t\t\t\t\tcout << \"\t\tName: \" << subPropertyItem.getName().getString() << \" - Value: \" << subPropertyItem.getValue().toString() << endl;");
			WriteLine ("\t\t\t\t}");
			WriteLine ("\t\t\t}");
			WriteLine ("\t\t\telse {");
			WriteLine ("\t\t\t\tcout << \"\tName: \" << propertyItem.getName().getString() << \" - Value: \" << propertyItem.getValue().toString() << endl;");
			WriteLine ("\t\t\t}");
			WriteLine ("");
			WriteLine ("\t\t}");
			WriteLine ("\t\tcout << \"------------------------------------\" << endl;");
			WriteLine ("\t\tcout << endl;");
			WriteLine ("\t}");
			WriteLine ("");
			WriteLine ("\t_p.finalize();");
			WriteLine ("}");
			WriteLine ("");
		}
	}
}
	