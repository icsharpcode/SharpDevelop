// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Control used to view a property grid category.
	/// </summary>
	public sealed class PropertyEditorCategoryView : HeaderedContentControl
	{
		static PropertyEditorCategoryView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyEditorCategoryView), new FrameworkPropertyMetadata(typeof(PropertyEditorCategoryView)));
		}
	}
}
