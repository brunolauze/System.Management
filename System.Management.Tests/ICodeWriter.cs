using System;

namespace System.Management.Tests
{
	internal interface ICodeWriter
	{
		void Write();
		void Save(string filePath);
	}
}

