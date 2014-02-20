using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Management.Classes
{
	[Guid("9ce7daf0-2af7-4a89-9eac-da61b7811dd8")]
	internal class UNIX_OperatingSystem : CIM_OperatingSystem
	{
		public UNIX_OperatingSystem ()
		{

		}
		
		protected override QueryParser Parser { 
			get { return new QueryParser<UNIX_OperatingSystem> (); } 
		}

		public override UNIX_MethodParameterClass UNIXShutdown (UNIX_MethodParameterClass inProperties)
		{
			//System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("shutdown", "-r \"Restart from Mono Management\"");
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("date");
			
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			System.Diagnostics.Process p = System.Diagnostics.Process.Start (startInfo);
			p.BeginOutputReadLine ();
			p.BeginErrorReadLine ();
			p.WaitForExit ();
			//TODO: See how things reacts on shutdown...
			return base.UNIXShutdown (inProperties);
		}
	}
}

