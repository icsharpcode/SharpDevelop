/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.08.2010
 * Time: 13:25
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
	public class SortingCollectionEditor:CollectionEditor
	{
		private Type[] types;
		
		public SortingCollectionEditor(Type type):base(type)
		{
			types = new Type[] {typeof(SortColumn)};
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
			return base.CreateInstance(typeof(SortColumn));
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
