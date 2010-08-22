/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.08.2010
 * Time: 13:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace ICSharpCode.Reports.Core.Dialogs
{
	/// <summary>
	/// Description of SortingCollectionEditor.
	/// </summary>
	public class GroupingCollectionEditor:CollectionEditor
	{
		private Type[] types;
		
		public GroupingCollectionEditor(Type type):base(type)
		{
			types = new Type[] {typeof(GroupColumn)};
		}
		
			protected override Type[] CreateNewItemTypes()
		{
			return types;
		}
			
			
		protected override object CreateInstance(Type itemType)
		{
//			if (itemType == typeof(SqlParameter)) {
//				return new SqlParameter();
//			}
			return base.CreateInstance(typeof(GroupColumn));
		}

		
		
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			return base.EditValue(context, provider, value);
		}
	
		
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return  UITypeEditorEditStyle.Modal;
		}
	}
}
