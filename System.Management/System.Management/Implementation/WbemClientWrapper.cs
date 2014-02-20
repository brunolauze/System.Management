using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Management
{
	internal static class WbemClientFactory
	{
		public static IWbemClassObject_DoNotMarshal Get(string strObjectPath)
		{
			return new UnixWbemClassObject(new UnixWbemClass (null));
		}

		public static IEnumerable<IWbemClassObject_DoNotMarshal> Get(string nameSpace, string strQuery)
		{
			strQuery = Regex.Replace (strQuery, "Win32", "UNIX", RegexOptions.IgnoreCase);
			strQuery = Regex.Replace (strQuery, "unix_", "UNIX_", RegexOptions.IgnoreCase);

			System.Management.Internal.WbemClient client = new System.Management.Internal.WbemClient ("localhost", null, nameSpace);
			client.IsSecure = false;
			var obj = client.ExecQuery (new System.Management.Internal.ExecQueryOpSettings ("WQL", strQuery));
			System.Management.Internal.CimInstancePathList list = obj as System.Management.Internal.CimInstancePathList;
			foreach (var item in list) {
				yield return new UnixWbemClassObject(new UnixWbemClass (item));
			}
		}

	}
}

