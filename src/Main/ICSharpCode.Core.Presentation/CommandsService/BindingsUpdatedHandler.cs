using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.Core.Presentation;


namespace ICSharpCode.Core.Presentation
{
	public delegate void BindingsUpdatedHandler(object sender, BindingsUpdatedHandlerArgs args);
	
	public class BindingsUpdatedHandlerArgs
	{
		public ICollection<UIElement> RemovedInstances
		{
			get; set;
		}
		
		public ICollection<UIElement> AddedInstances
		{
			get; set;
		}
		
		public ICollection<Type> RemovedTypes
		{
			get; set;
		}
		
		public ICollection<Type> AddedTypes
		{
			get; set;
		}
	}
}
