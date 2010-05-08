/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 10.02.2009
 * Zeit: 14:31
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ExtendedCollectionEditor.
	/// </summary>
	public class ExtendedCollectionEditor:CollectionEditor
	{
		private Type[] types; 

		public ExtendedCollectionEditor(Type type):base(type)
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
