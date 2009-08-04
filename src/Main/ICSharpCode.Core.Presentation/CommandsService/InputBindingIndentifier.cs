using System;

namespace ICSharpCode.Core.Presentation
{	
	public struct InputBindingIdentifier 
	{
		public string OwnerInstanceName {
			get; set;
		}
		
		public string OwnerTypeName {
			get; set;
		}
		
		public string RoutedCommandName {
			get; set;
		}
	}
}
