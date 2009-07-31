using System;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{
    public class IBindingInfoTemplateEqualityComparer : IEqualityComparer<IBindingInfoTemplate>
    {
    	bool IEqualityComparer<IBindingInfoTemplate>.Equals(IBindingInfoTemplate key, IBindingInfoTemplate comparedValue) 
    	{
    		return key.OwnerInstanceName == comparedValue.OwnerInstanceName
    			&& key.OwnerTypeName == comparedValue.OwnerTypeName
    			&& key.RoutedCommandName == comparedValue.RoutedCommandName;
    	}
    	
		int IEqualityComparer<IBindingInfoTemplate>.GetHashCode(IBindingInfoTemplate value) 
		{
			var instanceNameHashCode = value.OwnerInstanceName != null ? value.OwnerInstanceName.GetHashCode() : 0;
			var typeNameHashCode = value.OwnerTypeName != null ? value.OwnerTypeName.GetHashCode() : 0;
			var routedCommandNameHashCode = value.RoutedCommandName != null ? value.RoutedCommandName.GetHashCode() : 0;
    		var groupsHashCode = 0;
    		if(value.Groups != null) {
    			foreach(var group in value.Groups) {
    				groupsHashCode ^= group != null ? group.GetHashCode() : 0;
    			}
    		}
			
    		return instanceNameHashCode ^ typeNameHashCode ^ routedCommandNameHashCode ^ groupsHashCode;
		}
    }
}
