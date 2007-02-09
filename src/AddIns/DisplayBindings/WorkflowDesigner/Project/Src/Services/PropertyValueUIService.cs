// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of PropertyValueUIService.
	/// </summary>
	public class PropertyValueUIService : IPropertyValueUIService
	{
		private PropertyValueUIHandler handler;
		
		public PropertyValueUIService()
		{
		}
	
		public event EventHandler PropertyUIValueItemsChanged;
		
		public void AddPropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			if (newHandler == null)
				throw new ArgumentNullException("newHandler");
			
			handler += newHandler;
		}
		
		public PropertyValueUIItem[] GetPropertyUIValueItems(ITypeDescriptorContext context, PropertyDescriptor propDesc)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (propDesc == null)
				throw new ArgumentNullException("propDesc");

			if (handler != null){
				ArrayList valueUIItemList = new ArrayList();
				handler(context, propDesc, valueUIItemList);

				if (valueUIItemList.Count > 0) {
					PropertyValueUIItem[] valueItems = new PropertyValueUIItem[valueUIItemList.Count];
					valueUIItemList.CopyTo(valueItems, 0);
					return valueItems;
				}
			}
			
			return null;
		}
		
		public void NotifyPropertyValueUIItemsChanged()
		{
			if (PropertyUIValueItemsChanged != null)
				PropertyUIValueItemsChanged(this, EventArgs.Empty);
		}
		
		public void RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			if (newHandler == null)
				throw new ArgumentNullException("newHandler");

			handler -= newHandler;
		}
		
	}
}
