/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 29.05.2015
 * Time: 19:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Addin.Dialogs
{
	/// <summary>
	/// Description of GroupingCollectionEditor.
	/// </summary>
	public class GroupingCollectionEditor:CollectionEditor
	{
		Type[] types;
		
		public GroupingCollectionEditor(Type type):base(type){
			types = new Type[] {typeof(GroupColumn)};
		}
		
		
		protected override Type[] CreateNewItemTypes(){
			return types;
		}
			
			
		protected override object CreateInstance(Type itemType)
		{
//			if (itemType == typeof(SqlParameter)) {
//				return new SqlParameter();
//			}
			return base.CreateInstance(typeof(GroupColumn));
		}

		
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context){
			return  UITypeEditorEditStyle.Modal;
		}
	}
}
