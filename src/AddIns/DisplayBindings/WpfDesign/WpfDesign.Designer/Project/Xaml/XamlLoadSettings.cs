// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Reflection;
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
