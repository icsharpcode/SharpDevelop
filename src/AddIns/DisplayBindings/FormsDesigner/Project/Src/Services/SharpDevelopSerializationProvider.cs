// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
