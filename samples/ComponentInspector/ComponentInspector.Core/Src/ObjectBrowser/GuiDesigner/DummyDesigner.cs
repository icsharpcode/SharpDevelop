// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace NoGoop.ObjBrowser
{
	/// <summary>
	/// Class that works around a null reference exception when the Behaviour
	/// service is null.
	/// </summary>
	public class DummyDesigner : ParentControlDesigner
	{
		public DummyDesigner()
		{
		}
		
		/// <summary>
		/// HACK: Checks that the behaviour service otherwise we get a null 
		/// reference exception. The behaviour service is created when a control
		/// is added to the design surface or if the DocumentDesigner is used 
		/// instead of the ParentControlDesigner. We cannot use the DocumentDesigner
		/// since nothing seems to be shown in the design surface when we
		/// create an object. Until the designer code is reworked this hack
		/// fixes the problem.
		/// </summary>
		protected override void OnMouseDragBegin(int x, int y)
		{
			BehaviorService behaviourService = (BehaviorService)GetService(typeof(BehaviorService));
			if (behaviourService != null) {
				base.OnMouseDragBegin(x, y);
			}
		}
	}
}
