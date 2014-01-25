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
//using ICSharpCode.PackageManagement;
//using ICSharpCode.SharpDevelop.Dom;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class FieldHelper
//	{
//		public IField Field;
//		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
//		
//		/// <summary>
//		/// Field name should include the class prefix (e.g. "Class1.MyField")
//		/// </summary>
//		public void CreateField(string fullyQualifiedName)
//		{
//			Field = MockRepository.GenerateMock<IField, IEntity>();
//			Field.Stub(f => f.ProjectContent).Return(ProjectContentHelper.ProjectContent);
//			Field.Stub(f => f.FullyQualifiedName).Return(fullyQualifiedName);
//		}
//		
//		public void CreatePublicField(string fullyQualifiedName)
//		{
//			CreateField(fullyQualifiedName);
//			Field.Stub(f => f.IsPublic).Return(true);
//		}
//		
//		public void CreatePrivateField(string fullyQualifiedName)
//		{
//			CreateField(fullyQualifiedName);
//			Field.Stub(f => f.IsPublic).Return(false);
//			Field.Stub(f => f.IsPrivate).Return(true);
//		}
//		
//		public void SetRegion(DomRegion region)
//		{
//			Field.Stub(f => f.Region).Return(region);
//		}
//		
//		public void VariableStartsAtColumn(int column)
//		{
//			var region = new DomRegion(1, column);
//			SetRegion(region);
//		}
//		
//		public void VariableStartsAtLine(int line)
//		{
//			var region = new DomRegion(line, 1);
//			SetRegion(region);
//		}
//		
//		public void VariableEndsAtColumn(int column)
//		{
//			var region = new DomRegion(1, 1, 1, column);
//			SetRegion(region);
//		}
//		
//		public void VariableEndsAtLine(int line)
//		{
//			var region = new DomRegion(1, 1, line, 1);
//			SetRegion(region);
//		}
//		
//		public void SetCompilationUnitFileName(string fileName)
//		{
//			var helper = new CompilationUnitHelper();
//			helper.SetFileName(fileName);
//			Field.Stub(f => f.CompilationUnit).Return(helper.CompilationUnit);
//		}
//	}
//}
