// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Used for the value of a <see cref="XamlProperty"/>.
	/// Can be a <see cref="XamlTextValue"/> or a <see cref="XamlObject"/>.
	/// </summary>
	public abstract class XamlPropertyValue
	{
		/// <summary>
		/// used internally by the XamlParser.
		/// </summary>
		internal abstract object GetValueFor(XamlPropertyInfo targetProperty);
		
		XamlProperty _parentProperty;
		
		/// <summary>
		/// Gets the parent property that this value is assigned to.
		/// </summary>
		public XamlProperty ParentProperty {
			get { return _parentProperty; }
			internal set {
				if (_parentProperty != value) {
					_parentProperty = value;
					OnParentPropertyChanged();
				}
			}
		}
		
		/// <summary>
		/// Occurs when the value of the ParentProperty property changes.
		/// </summary>
		public event EventHandler ParentPropertyChanged;
		
		internal virtual void OnParentPropertyChanged()
		{
			if (ParentPropertyChanged != null) {
				ParentPropertyChanged(this, EventArgs.Empty);
			}
		}
		
		internal abstract void RemoveNodeFromParent();
		
		internal abstract void AddNodeTo(XamlProperty property);
		
		internal abstract XmlNode GetNodeForCollection();
	}
}
