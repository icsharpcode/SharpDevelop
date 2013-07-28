// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Static methods to help with <see cref="System.Windows.Markup.INameScope"/> operations on Xaml elements.
	/// </summary>
	internal static class NameScopeHelper
	{
		/// <summary>
		/// Finds the XAML namescope for the specified object and uses it to unregister the old name and then register the new name.
		/// </summary>
		/// <param name="namedObject">The object where the name was changed.</param>
		/// <param name="oldName">The old name.</param>
		/// <param name="newName">The new name.</param>
		public static void NameChanged(XamlObject namedObject, string oldName, string newName)
		{
			var obj = namedObject;
			while (obj != null) {
				var nameScope = obj.Instance as INameScope;
				if (nameScope == null) {
					var depObj = obj.Instance as DependencyObject;
					if (depObj != null)
						nameScope = NameScope.GetNameScope(depObj);
				}
				if (nameScope != null) {
					if (oldName != null) {
						try {
							nameScope.UnregisterName(oldName);
						} catch (Exception x) {
							Debug.WriteLine(x.Message);
						}
					}
					if (newName != null) {
						nameScope.RegisterName(newName, namedObject.Instance);
					}
					break;
				}
				obj = obj.ParentObject;
			}
		}
	}
}
