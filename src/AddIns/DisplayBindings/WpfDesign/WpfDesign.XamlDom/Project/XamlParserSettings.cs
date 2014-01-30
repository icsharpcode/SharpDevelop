// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
