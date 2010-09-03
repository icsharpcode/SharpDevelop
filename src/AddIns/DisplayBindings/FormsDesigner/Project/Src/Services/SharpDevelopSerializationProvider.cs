// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Provides a custom serialization service for the designer.
	/// </summary>
	internal sealed class SharpDevelopSerializationProvider : IDesignerSerializationProvider
	{
		internal SharpDevelopSerializationProvider()
		{
		}
		
		object IDesignerSerializationProvider.GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
		{
			if (currentSerializer == null) {
				// We need a base serializer to proceed.
				return null;
			}
			
			
			if (serializerType == typeof(MemberCodeDomSerializer)) {
				
				if (typeof(PropertyDescriptor).IsAssignableFrom(objectType) && !(currentSerializer is ProjectResourcesMemberCodeDomSerializer)) {
					return new ProjectResourcesMemberCodeDomSerializer((MemberCodeDomSerializer)currentSerializer);
				}
				
			} else if (serializerType == typeof(CodeDomSerializer)) {
				
				if (typeof(IComponent).IsAssignableFrom(objectType) && !(currentSerializer is ProjectResourcesComponentCodeDomSerializer)) {
					return new ProjectResourcesComponentCodeDomSerializer((CodeDomSerializer)currentSerializer);
				}
				
			}
			
			return null;
		}
	}
}
