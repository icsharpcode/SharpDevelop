// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer
{
	sealed class DesignPanel : SingleVisualChildElement
	{
		UIElement _designedElement;
		
		public UIElement DesignedElement {
			get {
				return _designedElement;
			}
			set {
				_designedElement = value;
				this.VisualChild = value;
			}
		}
		
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			
			return base.HitTestCore(hitTestParameters);
		}
	}
}
