using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace System.Management.Tests
{
	public static class TemplateFactory
	{
		private static Dictionary<string, ClassTemplate> _templates = new Dictionary<string, ClassTemplate>();
		private static readonly object _lock = new object ();

		public static string GetDependencies(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Dependencies;
		}

		public static string GetPrivate(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Private;
		}

		public static string GetPublic(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Public;
		}

		public static string GetDeclaration(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Declaration;
		}

		public static string GetInitialize(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Initialize;
		}

		public static string GetLoad(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Load;
		}

		public static string GetFinalize(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.Finalize;
		}

		public static string GetCreateInstance(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.CreateInstance;
		}

		public static string GetDeleteInstance(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.DeleteInstance;
		}

		public static string GetModifyInstance(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return null;
			return template.ModifyInstance;
		}

		public static bool HasCreateInstance(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return false;
			return !string.IsNullOrEmpty(template.CreateInstance);
		}

		public static bool HasDeleteInstance(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return false;
			return !string.IsNullOrEmpty(template.DeleteInstance);
		}

		public static bool HasModifyInstance(string className, string platform)
		{
			ClassTemplate template = GetClassTemplate (className, platform);
			if (template == null)
				return false;
			return !string.IsNullOrEmpty(template.ModifyInstance);
		}


		public static ClassTemplate GetClassTemplate(string className, string platform)
		{
			string key = className + platform;
			if (!_templates.ContainsKey (key)) {
				ClassTemplate template = GenerateClassTemplate (className, platform);
				lock (_lock) {
					if (!_templates.ContainsKey (key)) {
						_templates.Add (key, template);
					}
				}
			}
			return _templates [key];
		}

		static ClassTemplate GenerateClassTemplate (string className, string platform)
		{
			string filename = System.IO.Path.Combine(Environment.CurrentDirectory, className + "_" + platform + ".xml");
			if (System.IO.File.Exists (filename)) {
				XDocument doc = XDocument.Load (filename);
				ClassTemplate template = new ClassTemplate ();
				foreach (var el in doc.Root.Elements()) {
					if (el.Name == "Dependencies")
						template.Dependencies = el.Value;
					if (el.Name == "Private")
						template.Private = el.Value;
					else if (el.Name == "Public")
						template.Public = el.Value;
					else if (el.Name == "Declaration")
						template.Declaration = el.Value;
					else if (el.Name == "Initialize")
						template.Initialize = el.Value;
					else if (el.Name == "Load")
						template.Load = el.Value;
					else if (el.Name == "Finalize")
						template.Finalize = el.Value;
					else if (el.Name == "CreateInstance")
						template.CreateInstance = el.Value;
					else if (el.Name == "DeleteInstance")
						template.DeleteInstance = el.Value;
					else if (el.Name == "ModifyInstance")
						template.ModifyInstance = el.Value;
				}
				return template;
			}
			return null;
		}
	}
}

