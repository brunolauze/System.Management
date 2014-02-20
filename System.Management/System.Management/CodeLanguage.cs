namespace System.Management
{
	public enum CodeLanguage
	{
		CSharp,
#if JSCRIPT
		JScript,
#endif
		VB,
		VJSharp,
		Mcpp
	}
}
