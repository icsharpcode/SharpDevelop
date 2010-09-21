// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
