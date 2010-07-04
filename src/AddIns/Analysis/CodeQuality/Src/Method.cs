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
        public Type ReturnType { get; set; }

        /// <summary>
        /// Type which owns this method
        /// </summary>
        public Type Owner { get; set; }

        /// <summary>
        /// Whether the method is constructor or not
        /// </summary>
        public bool IsConstructor { get; set; }

        /// <summary>
        /// Whether the method is public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the method is private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Whether the method is protected
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// Whether the method is static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Whether the method is sealed
        /// </summary>
        public bool IsSealed { get; set; }

        /// <summary>
        /// Whether the method is abstract
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Whether the method is setter
        /// </summary>
        public bool IsSetter { get; set; }

        /// <summary>
        /// Whether the method is getter
        /// </summary>
        public bool IsGetter { get; set; }

        /// <summary>
        /// Whether the method is virtual
        /// </summary>
        public bool IsVirtual { get; set; }

        public Method()
        {
            TypeUses = new HashSet<Type>();
            MethodUses = new HashSet<Method>();
            FieldUses = new HashSet<Field>();

            ReturnType = null;
            Owner = null;
        }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
