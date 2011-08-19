// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace ICSharpCode.Reports.Core.Dialogs
{
	/// <summary>
	/// Description of ExtendedCollectionEditor.
	/// </summary>
	public class ParameterCollectionEditor:CollectionEditor
	{
		private Type[] types; 

		public ParameterCollectionEditor(Type type):base(type)
		{
			types = new Type[] {typeof(BasicParameter),typeof(SqlParameter)};
		}
		
		protected override Type[] CreateNewItemTypes()
		{
			return types;
		}
		protected override object CreateInstance(Type itemType)
		{
			if (itemType == typeof(SqlParameter)) {
				return new SqlParameter();
			}
			
			if (itemType == typeof(BasicParameter)) {
				return new BasicParameter();
			}
			
			
			return base.CreateInstance(itemType);
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
