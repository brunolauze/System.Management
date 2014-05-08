using System;
using System.Collections.Generic;

namespace System.Management.Tests
{
	public class ClassTemplate
	{

		public string Dependencies
		{
			get;set;
		}

		public string Public
		{
			get;set;
		}

		public string Private
		{
			get;set;
		}

		public string Declaration
		{
			get;set;
		}

		public string Initialize
		{
			get;set;
		}

		public string Load
		{
			get;set;
		}

		public string LoadByName {
			get;
			set;
		}

		public string Finalize
		{
			get;set;
		}

		public string CreateInstance
		{
			get;set;
		}

		public string DeleteInstance
		{
			get;set;
		}

		public string ModifyInstance
		{
			get;set;
		}

		public string ValidateInstance
		{
			get;set;
		}

		public IEnumerable<TestTemplate> Tests
		{
			get;set;
		}
	}
}

