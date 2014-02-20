/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Management.Classes
{
	[Guid("c7d94751-8c0a-4775-ac4c-0b778fef4eeb")]
	internal abstract class CIMWbemClassBase : IUnixWbemClassHandler
	{
		private Dictionary<string, object> _prop;
		private Dictionary<string, UnixCimMethodInfo> _methods;
		private readonly Dictionary<string, UnixWbemPropertyInfo> _propInfos;
		private readonly Dictionary<string, UnixWbemPropertyInfo> _systemPropInfos;
		private readonly Dictionary<string, UnixWbemQualiferInfo> _qualifiers;
		private IEnumerator<UnixCimMethodInfo> _methodEnumerator;
		private bool _isInstance = false;
		private readonly Type _baseType;

		public CIMWbemClassBase (Type baseType)
		{
			_baseType = baseType;
			_propInfos = new Dictionary<string, UnixWbemPropertyInfo>();
			_methods = new Dictionary<string, UnixCimMethodInfo>();
			_systemPropInfos = new Dictionary<string, UnixWbemPropertyInfo>();
			_qualifiers = new Dictionary<string, UnixWbemQualiferInfo>();
			RegisterProperies();
			RegisterMethods();
		}
		
		public virtual IUnixWbemClassHandler New()
		{
			var obj = Activator.CreateInstance (this.GetType ());
			var managed = obj as CIMWbemClassBase;
			if (managed != null) managed._isInstance = true;
			return (IUnixWbemClassHandler)obj;
		}

		protected virtual void RegisterMethods ()
		{

		}
		
		protected virtual void RegisterProperies()
		{
			RegisterSystemProperty ("__GENUS", CimType.SInt32, 0);
			RegisterSystemProperty ("__UID", CimType.Object, 0);
			RegisterSystemProperty ("__SERVER", CimType.String, 0);
			RegisterSystemProperty ("__NAMESPACE", CimType.String, 0);
			RegisterSystemProperty ("__CLASS", CimType.String, 0);
			RegisterSystemProperty ("__DYNASTY", CimType.String, 0);
			RegisterSystemProperty ("__DERIVATION", CimType.Object, 0);
			RegisterSystemProperty ("__RELATIVE_PATH", CimType.String, 0);
			RegisterSystemProperty ("__PATH", CimType.String, 0);
			RegisterSystemProperty ("__KEY_FIELD", CimType.String, 0);
		}
		
		public virtual void RegisterProperty(string name, CimType type, int flavor)
		{
			if (_propInfos.ContainsKey (name))
				_propInfos[name] = new UnixWbemPropertyInfo { Name = name, Type = type, Flavor = flavor};
			else 
				_propInfos.Add (name, new UnixWbemPropertyInfo { Name = name, Type = type, Flavor = flavor});
		}

		public virtual void RegisterProperty(UnixWbemPropertyInfo info)
		{
			if (_propInfos.ContainsKey (info.Name))
				_propInfos[info.Name] = info;
			else 
				_propInfos.Add (info.Name, info);
		}
		
		public IEnumerable<UnixWbemQualiferInfo> GetQualifiers ()
		{
			return _qualifiers.Values.Where (x => ShouldSendQualifier(x));
		}
		
		private bool ShouldSendQualifier (UnixWbemQualiferInfo qualifier)
		{
			bool toInstance = _isInstance ? qualifier.PropagateToInstance : true;
			if (!IsMetaImplementation && qualifier.OriginType != this.GetType ()) 
			{
				if (qualifier.PropagateToDerivedClasses) qualifier.Origin = QualifierOrigin.Propagated;
				return toInstance && qualifier.PropagateToDerivedClasses;
			}
			return toInstance;
		}
		
		protected virtual bool IsMetaImplementation
		{
			get { return false; }
		}
		
		public virtual void RegisterQualifiers (IEnumerable<UnixWbemQualiferInfo> qualifiers)
		{
			foreach (var qualifier in qualifiers)
			{
				RegisterQualifier (qualifier);
			}
		}
		
		public virtual void RegisterQualifier (UnixWbemQualiferInfo qualifier)
		{
			if (_qualifiers.ContainsKey (qualifier.Name)) {
				if (_qualifiers[qualifier.Name].Overridable)
				{
					_qualifiers[qualifier.Name] = qualifier;
				}
			}
			else 
			{
				_qualifiers.Add (qualifier.Name, qualifier);
			}
		}
		
		protected virtual void RegisterSystemProperty(string name, CimType type, int flavor)
		{	
			if (_propInfos.ContainsKey (name))
				_systemPropInfos[name] = new UnixWbemPropertyInfo { Name = name, Type = type, Flavor = flavor};
			else 
				_systemPropInfos.Add (name, new UnixWbemPropertyInfo { Name = name, Type = type, Flavor = flavor});
		}
		
		public abstract string Namespace { get; }
		
		#region IUnixWbemClassHandler implementation
		
		public virtual System.Collections.Generic.IEnumerable<object> Get (string strQuery)
		{
			return Get (new object[] { null }, strQuery);
		}

		protected virtual System.Collections.Generic.IEnumerable<object> Get (System.Collections.Generic.IEnumerable<object> source, string strQuery)
		{
			if (source == null) return new object[0];
			return Filter(source.Select (x => Get (x)), strQuery);
		}


		public virtual IEnumerable<object> Filter (IEnumerable<object> obj, string strQuery)
		{
			return Parser.Parse (obj, strQuery).OfType<object>();
		}
		
		public virtual object Get (object nativeObj)
		{
			var newObj = (CIMWbemClassBase)Build (nativeObj);
			return newObj.Format(newObj.GetInternal (nativeObj));
		}

		protected virtual IUnixWbemClassHandler Build (object nativeObj)
		{
			var obj = (CIMWbemClassBase)New ();
			obj.BuildInternal ();
			return obj; 
		}

		internal void BuildInternal()
		{
			this._prop = new Dictionary<string, object>();
			this._prop.Add ("__GENUS", _isInstance ? 0 : 1);
			this._prop.Add ("__UID", GetUid ());
			this._prop.Add ("__SERVER", System.Net.Dns.GetHostName ().ToLower ());
			this._prop.Add ("__NAMESPACE", this.Namespace);
			this._prop.Add ("__DYNASTY", _baseType.Name);
			this._prop.Add ("__CLASS", this.GetType().Name);
			this._prop.Add ("__DERIVATION", GetDerivations());
			this._prop.Add ("__PATH", string.Format ("//{0}/{1}/{2}", this._prop["__SERVER"],this._prop["__NAMESPACE"],this._prop["__CLASS"])); 
			this._prop.Add ("__KEY_FIELD", PathField);
		}

		
		protected virtual Guid GetUid ()
		{
			return GetClassUid ();
		}

		internal virtual IUnixWbemClassHandler GetInternal(object nativeObj) {

			return this;
		}
		
		protected Guid GetClassUid ()
		{
			GuidAttribute att = this.GetType ().GetCustomAttributes(typeof( .GetCustomAttribute<GuidAttribute>(false);
			if (att == null) return Guid.NewGuid ();
			return new Guid(att.Value);
		}
		
		internal IUnixWbemClassHandler Format (IUnixWbemClassHandler obj)
		{
			return obj.WithProperty ("__RELATIVE_PATH", string.Format ("{0}/{1}={2}", obj.Properties ["__CLASS"], PathField, FormatPropertyValue(obj.Properties [PathField])))
				.WithProperty ("__PATH",string.Format ("//{0}/{1}/{2}/{3}={4}", obj.Properties ["__SERVER"], obj.Properties ["__NAMESPACE"], obj.Properties ["__CLASS"], PathField, FormatPropertyValue(obj.Properties [PathField])))
					  .WithProperty ("__PROPERTY_COUNT", obj.Properties.Count + 1);
		}

		private static string FormatPropertyValue(object obj)
		{
			if (obj is String)
			{
				if (obj == null)
				{
					return "\"\"";
				}
				return "\"" + obj.ToString () + "\"";
			}
			if (obj == null) return "";
			return obj.ToString ();
		}

		public virtual IEnumerable<string> GetMethods()
		{
			return _methods.Keys.ToList ();
		}
		
		/// <summary>
		/// Adds the property.
		/// </summary>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public virtual void AddProperty (string key, object obj)
		{
			_isInstance = true;
			_prop["__GENUS"] = 1;
			if (_prop.ContainsKey (key)) {
				_prop [key] = obj;
			} else {
				_prop.Add (key, obj);
			}
		}

		public IUnixWbemClassHandler WithProperty (string key, object obj)
		{
			AddProperty (key,  obj);
			return this;
		}

		public IUnixWbemClassHandler WithMethod (string key, UnixCimMethodInfo methodInfo)
		{
			AddMethod (key, methodInfo);
			return this;
		}

		
		public virtual void AddMethod (string key, UnixCimMethodInfo method)
		{
			if (_methods.ContainsKey (key)) {
				_methods [key] = method;
			} else {
				_methods.Add (key, method);
			}
		}

		public UnixCimMethodInfo NextMethod ()
		{
			if (_methodEnumerator == null)
				_methodEnumerator = Methods.GetEnumerator ();
			if (_methodEnumerator.MoveNext ()) {
				return _methodEnumerator.Current;
			} else {
				_methodEnumerator = null;
			}
			return default(UnixCimMethodInfo);
		}
		
		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <returns>
		/// The property.
		/// </returns>
		/// <param name='key'>
		/// Key.
		/// </param>
		public virtual object GetProperty (string key)
		{
			if (_prop == null) {
				return null;
			}
			var realKey = _prop.Keys.FirstOrDefault(x => x.Equals (key, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty (realKey))
			{
				return _prop[realKey];
			}
			return  null;
		}

		public virtual T GetPropertyAs<T> (string key)
		{
			return (T)GetProperty (key);
		}

		/// <summary>
		/// Invokes the method.
		/// </summary>
		/// <returns>
		/// The method.
		/// </returns>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public virtual IUnixWbemClassHandler InvokeMethod (string methodName, IUnixWbemClassHandler inParams)
		{
			UnixCimMethodInfo methodInfo = this.Methods.FirstOrDefault (x => x.Name == methodName);
			if  (methodInfo.IsInstanceMethod && !this._isInstance)
			{
				throw new InvalidOperationException("Cannot execute instance method with class.");
			}
			MethodInfo method = GetType ().GetMethod (methodName, BindingFlags.Public | BindingFlags.Instance);
			IUnixWbemClassHandler outParams = null;
			if (method != null)
			{
				outParams = (IUnixWbemClassHandler)method.Invoke (this, new object[] { (UNIX_MethodParameterClass)inParams });
			}
			return outParams;
		}
		
		public virtual IDictionary<string, object> Properties { get { return _prop; } }

		public virtual IEnumerable<string> PropertyNames { get { return _propInfos.Keys; } }
		
		public virtual IEnumerable<UnixWbemPropertyInfo> PropertyInfos { get { return _propInfos.Values; } }
		
		public virtual IEnumerable<UnixWbemPropertyInfo> SystemPropertyInfos { get { return _systemPropInfos.Values; } }
		
		public virtual IEnumerable<string> SystemPropertyNames { get { return _systemPropInfos.Keys; } }
		
		public virtual IEnumerable<string> QualifierNames { get { return GetQualifiers ().Select (x => x.Name); } }

		public IEnumerable<string> MethodNames { get { return _methods.Keys; } }
		
		public IEnumerable<UnixCimMethodInfo> Methods { get { return _methods.Values; } }

		public UnixWbemQualiferInfo GetQualifier(string name)
		{
			var obj = _qualifiers[name];
			return ShouldSendQualifier (obj) ? obj : null;
		}
		
		public UnixWbemQualiferInfo GetQualifier (int index)
		{
			return GetQualifiers ().ElementAt (index);
		}

		protected abstract QueryParser Parser { get; }
		
		private string[] GetDerivations ()
		{
			var list = new List<string> ();
			Type type = IsMetaImplementation ? this.GetType ().BaseType : this.GetType ();
			if (type.BaseType != null && type.BaseType != typeof(object) && type.BaseType != typeof(CIMWbemClassBase)) {

				do {
					list.Add (type.BaseType.Name);
					type = type.BaseType;
				} while(type.BaseType != null && type.BaseType != typeof(object) && type.BaseType != typeof(CIMWbemClassBase));
			}
			return list.ToArray ();
		}
		 
		public abstract string PathField { get; }
		#endregion
	}
}
*/