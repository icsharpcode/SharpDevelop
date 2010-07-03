using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Type : IDependency
    {
        /// <summary>
        /// Nested types like inner classes, interfaces and so on.
        /// </summary>
        public ISet<Type> InnerTypes { get; set; }

        /// <summary>
        /// Methods within type
        /// </summary>
        public ISet<Method> Methods { get; set; }

        /// <summary>
        /// Fields within type
        /// </summary>
        public ISet<Field> Fields { get; set; }

        /// <summary>
        /// Name of type with a name of namespace.
        /// </summary>
        public string FullName
        {
            get
            {
                return Namespace.Name + "." + Name;
            }
        }

        /// <summary>
        /// Name of type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Namespace where type is located
        /// </summary>
        public Namespace Namespace { get; set; }

        public Type()
        {
            Methods = new HashSet<Method>();
            Fields = new HashSet<Field>();
        }

        /// <summary>
        /// Returns all types which are used in this type
        /// </summary>
        /// <returns>A set of types</returns>
        public ISet<Type> GetUses()
        {
            var set = new HashSet<Type>();

            foreach (var method in Methods)
            {
                set.UnionWith(method.TypeUses);
            }

            foreach (var field in Fields)
            {
                if (field.Type != null) // if it is null so it is type from outside of this assembly
                    set.Add(field.Type); // TODO: find solution to handle them
            }

            return set;
        }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            foreach (var method in Methods)
            {
                g.AddVertex(method.Name);
            }

            foreach (var field in Fields)
            {
                g.AddVertex(field.Name);
            }

            foreach (var method in Methods)
            {
                foreach (var methodUse in method.MethodUses)
                {
                    g.AddEdge(new Edge<object>(method.Name, methodUse.Name));
                }

                foreach (var fieldUse in method.FieldUses)
                {
                    g.AddEdge(new Edge<object>(method.Name, fieldUse.Name));
                }
            }

            return g;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
