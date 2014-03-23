/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 18:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of AbstractDesigner.
	/// </summary>
	public class AbstractDesigner:ControlDesigner
	{
		
		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize(component);
			SelectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			ComponentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
		}
		
		
		protected ISelectionService SelectionService {get; private set;}
		
		protected IComponentChangeService ComponentChangeService {get;private set;}
	}
}
