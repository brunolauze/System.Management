using System;
using System.Collections.Generic;

namespace System.Management
{
	internal struct UnixCimMethodInfo
	{
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is instance method.
		/// </summary>
		/// <value><c>true</c> if this instance is instance method; otherwise, <c>false</c>.</value>
		public bool IsInstanceMethod
		{
			get;set;
		}

		public string InSignatureType
		{
			get;set;
		}

		public string OutSignatureType
		{
			get;set;
		}

		public IEnumerable<UnixWbemPropertyInfo> InProperties
		{
			get;set;
		}

		
		public IEnumerable<UnixWbemPropertyInfo> OutProperties
		{
			get;set;
		}

	}
}

