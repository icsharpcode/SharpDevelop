#if false
using System;
using System.Collections.Generic;
using ICSharpCode.CppBinding.Interop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.CppBinding.Parser
{
    /// <summary>
    /// Builds the compilation unit based on events reported by c++ parser.
    /// </summary>
    public class CppCompilationUnitBuilder : IParserEventConsumer
    {
        public CppCompilationUnitBuilder(IProjectContent projectContent)
        {
            cu = new DefaultCompilationUnit(projectContent);
        }

        public ICompilationUnit CompilationUnit
        {
            get { return cu; }
        }

        #region IParserEventConsumer Members

        /// <summary>
        /// Adds a new class to the compilation unit and sets it as current.
        /// </summary>
        /// <param name="loc">location (line/column) in the source code</param>
        /// <param name="name">name of added class</param>
        /// <param name="ts">class type specifier</param>
        public void BeginClassDefinition(Location loc, string name, TypeSpecifier ts)
        {
            DefaultClass clazz = new DefaultClass(cu, name);  //TODO: namespace where class is declared should be added here
            clazz.Region = new DomRegion(loc.line, loc.column);
            cu.Classes.Add(clazz);
            currentClass.Push(clazz);
        }

        public void ClassForwardDeclaration(Location loc, string name, TypeSpecifier ts, int fs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the end of class definition and forgets about the current class.
        /// </summary>
        public void EndClassDefinition(Location loc)
        {
            DefaultClass c = currentClass.Peek();
            int beginColumn = c.Region.BeginColumn;
            int beginLine = c.Region.BeginLine;
            c.Region = new DomRegion(beginLine, beginColumn, loc.line, loc.column);

            currentClass.Pop();
        }

        public void Test(TypeSpecifier ts)
        {
            throw new NotImplementedException();
        }

        #endregion

        ICompilationUnit cu;
        Stack<DefaultClass> currentClass = new Stack<DefaultClass>();
    }
}
#endif