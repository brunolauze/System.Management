using NUnit.Framework;
using System;

namespace System.Management.Tests
{
	[TestFixture ()]
	public class ManagementObjectFixture
	{
		[Test ()]
		public void TestSimpleQuery ()
		{
			string wmiQuery = "SELECT * FROM UNIX_LogicalDisk";
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher (wmiQuery)) {
				ManagementObjectCollection retObjectCollection = searcher.Get ();
				foreach (ManagementObject obj in retObjectCollection) {
					Console.WriteLine (obj.Path.Path);
					foreach (var p in obj.Properties) {
						Console.WriteLine (p.Name + ": " + p.Value);
					}
				}
			}
		}
	}
}

