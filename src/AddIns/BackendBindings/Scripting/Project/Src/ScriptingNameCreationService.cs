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

//using System;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.ComponentModel.Design.Serialization;
//using ICSharpCode.FormsDesigner;
//
//namespace ICSharpCode.Scripting
//{
//	public class ScriptingNameCreationService : INameCreationService
//	{
//		IDesignerHost host;
//		XmlDesignerNameCreationService nameCreationService;
//		
//		public ScriptingNameCreationService(IDesignerHost host)
//		{
//			this.host = host;
//			nameCreationService = new XmlDesignerNameCreationService(host);
//		}
//		
//		public string CreateName(IContainer container, Type dataType)
//		{
//			return nameCreationService.CreateName(container, dataType);
//		}
//		
//		public bool IsValidName(string name)
//		{
//			if (!nameCreationService.IsValidName(name)) {
//				return false;
//			}
//			
//			if (host.Container != null) {
//				return host.Container.Components[name] == null;
//			}
//			return true;
//		}
//		
//		public void ValidateName(string name)
//		{
//			if (!IsValidName(name)) {
//				throw new Exception("Invalid name " + name);
//			}
//		}
//	}
//}
