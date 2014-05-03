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
			string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "Templates", className + "_" + platform + ".xml");
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
				}
				return template;
			}
			return null;
		}
	}
}

