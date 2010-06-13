using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Method : IDependency
    {
        /// <summary>
        /// Types which are used in body of method
        /// </summary>
        public ISet<Type> TypeUses { get; set; }

        /// <summary>
        /// Methods which are called in body of method
        /// </summary>
        public ISet<Method> MethodUses { get; set; }

        /// <summary>
        /// Fields which are accesed in body of method
        /// </summary>
        public ISet<Field> FieldUses { get; set; }
        
        /// <summary>
        /// A name of method
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A return type of method
        /// </summary>
        public Type Type { get; set; }

        public Method()
        {
            TypeUses = new HashSet<Type>();
            MethodUses = new HashSet<Method>();
            FieldUses = new HashSet<Field>();
        }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            return null;
        }
    }
}
