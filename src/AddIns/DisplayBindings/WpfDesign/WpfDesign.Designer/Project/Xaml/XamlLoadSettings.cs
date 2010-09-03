// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	/// <summary>
	/// Settings used to load a XAML document.
	/// </summary>
	public sealed class XamlLoadSettings
	{
		public readonly ICollection<Assembly> DesignerAssemblies = new List<Assembly>();
		public readonly List<Action<XamlDesignContext>> CustomServiceRegisterFunctions = new List<Action<XamlDesignContext>>();
		public Action<XamlErrorService> ReportErrors;
		XamlTypeFinder typeFinder = XamlTypeFinder.CreateWpfTypeFinder();
		
		public XamlTypeFinder TypeFinder {
			get { return typeFinder; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				typeFinder = value;
			}
		}
		
		public XamlLoadSettings()
		{
			DesignerAssemblies.Add(typeof(XamlDesignContext).Assembly);
		}
	}
}
