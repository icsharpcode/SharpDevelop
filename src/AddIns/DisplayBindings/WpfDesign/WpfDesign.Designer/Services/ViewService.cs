// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2258 $</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	class ViewService : IViewService
	{
		public ViewService(DesignContext context)
		{
			context.SubscribeToService<IModelService>(delegate(IModelService modelService) {
				modelService.ItemCreated += OnItemCreated;
			});
		}

		void OnItemCreated(object sender, DesignItemEventArgs e)
		{
			if (e.Item.View != null) {
				SetAttachedModel(e.Item.View, e.Item);
			}
		}

		static readonly DependencyProperty AttachedModelProperty =
			DependencyProperty.RegisterAttached("AttachedModel",
			typeof(DesignItem), typeof(ViewService));

		static DesignItem GetAttachedModel(DependencyObject obj)
		{
			return (DesignItem)obj.GetValue(AttachedModelProperty);
		}

		static void SetAttachedModel(DependencyObject obj, DesignItem value)
		{
			obj.SetValue(AttachedModelProperty, value);
		}

		#region IViewService Members

		public DesignItem GetModel(DependencyObject view)
		{
			return GetAttachedModel(view);
		}

		public DependencyObject GetView(DesignItem model)
		{
			return model.View;
		}

		#endregion
	}
}
