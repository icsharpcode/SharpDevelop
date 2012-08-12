// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class FieldHelper
	{
		public IField Field;
		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
		
		/// <summary>
		/// Field name should include the class prefix (e.g. "Class1.MyField")
		/// </summary>
		public void CreateField(string fullyQualifiedName)
		{
			Field = MockRepository.GenerateMock<IField, IEntity>();
			Field.Stub(f => f.ProjectContent).Return(ProjectContentHelper.ProjectContent);
			Field.Stub(f => f.FullyQualifiedName).Return(fullyQualifiedName);
		}
		
		public void CreatePublicField(string fullyQualifiedName)
		{
			CreateField(fullyQualifiedName);
			Field.Stub(f => f.IsPublic).Return(true);
		}
		
		public void CreatePrivateField(string fullyQualifiedName)
		{
			CreateField(fullyQualifiedName);
			Field.Stub(f => f.IsPublic).Return(false);
			Field.Stub(f => f.IsPrivate).Return(true);
		}
		
		public void SetRegion(DomRegion region)
		{
			Field.Stub(f => f.Region).Return(region);
		}
		
		public void VariableStartsAtColumn(int column)
		{
			var region = new DomRegion(1, column);
			SetRegion(region);
		}
		
		public void VariableStartsAtLine(int line)
		{
			var region = new DomRegion(line, 1);
			SetRegion(region);
		}
		
		public void VariableEndsAtColumn(int column)
		{
			var region = new DomRegion(1, 1, 1, column);
			SetRegion(region);
		}
		
		public void VariableEndsAtLine(int line)
		{
			var region = new DomRegion(1, 1, line, 1);
			SetRegion(region);
		}
		
		public void SetCompilationUnitFileName(string fileName)
		{
			var helper = new CompilationUnitHelper();
			helper.SetFileName(fileName);
			Field.Stub(f => f.CompilationUnit).Return(helper.CompilationUnit);
		}
	}
}
