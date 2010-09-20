// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Delegate used for XamlParserSettings.CreateInstanceCallback.
	/// </summary>
	public delegate object CreateInstanceCallback(Type type, object[] arguments);
	
	/// <summary>
	/// Settings used for the XamlParser.
	/// </summary>
	public sealed class XamlParserSettings
	{
		CreateInstanceCallback _createInstanceCallback = Activator.CreateInstance;
		XamlTypeFinder _typeFinder = XamlTypeFinder.CreateWpfTypeFinder();
		IServiceProvider _serviceProvider = DummyServiceProvider.Instance;
		
		/// <summary>
		/// Gets/Sets the method used to create object instances.
		/// </summary>
		public CreateInstanceCallback CreateInstanceCallback {
			get { return _createInstanceCallback; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				_createInstanceCallback = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets the type finder to do type lookup.
		/// </summary>
		public XamlTypeFinder TypeFinder {
			get { return _typeFinder; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				_typeFinder = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets the service provider to use to initialize markup extensions.
		/// </summary>
		public IServiceProvider ServiceProvider {
			get { return _serviceProvider; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				_serviceProvider = value;
			}
		}
		
		sealed class DummyServiceProvider : IServiceProvider
		{
			public static readonly DummyServiceProvider Instance = new DummyServiceProvider();
			
			public object GetService(Type serviceType)
			{
				return null;
			}
		}
	}
}
