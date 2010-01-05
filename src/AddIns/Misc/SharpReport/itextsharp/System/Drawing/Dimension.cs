using System;

namespace System.Drawing {
    /// <summary>
    /// The <code>Dimension</code> class encapsulates the width and
    /// height of a component (in int precision) in a single object. 
    /// </summary>
    /// <remarks>
    /// The class is 
    /// associated with certain properties of components. Several methods 
    /// defined by the <code>Component</code> class and the 
    /// <code>LayoutManager</code> interface return a <code>Dimension</code> object.
    /// <p/>
    /// Normally the values of <code>width</code> 
    /// and <code>height</code> are non-negative ints. 
    /// The constructors that allow you to create a dimension do 
    /// not prevent you from setting a negative value for these properties. 
    /// If the value of <code>width</code> or <code>height</code> is 
    /// negative, the behavior of some methods defined by other objects is 
    /// undefined. 
    /// </remarks>
    public class Dimension : Dimension2D {
    
        /// <summary>
        /// The width dimension. Negative values can be used. 
        /// </summary>
        public int width;

        /// <summary>
        /// The height dimension. Negative values can be used. 
        /// </summary>
        public int height;

        /// <summary>
        /// Creates an instance of <code>Dimension</code> with a width 
        /// of zero and a height of zero. 
        /// </summary>
        public Dimension() : this(0, 0) {}

        /// <summary>
        /// Creates an instance of <code>Dimension</code> whose width 
        /// and height are the same as for the specified dimension. 
        /// </summary>
        /// <param name="d">
        /// the specified dimension for the 
        /// <code>width</code> and 
        /// <code>height</code> values.
        /// </param>
        public Dimension(Dimension d) : this(d.width, d.height) {}

        /// <summary>
        /// Constructs a Dimension and initializes it to the specified width and
        /// specified height.
        /// </summary>
        /// <param name="width">the specified width dimension</param>
        /// <param name="height">the specified height dimension</param>
        public Dimension(int width, int height) {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Returns the width of this dimension in double precision.
        /// </summary>
        /// <value>the width</value>
        public override double Width {
            get {
                return width;
            }
        }

        /// <summary>
        /// Returns the height of this dimension in double precision.
        /// </summary>
        /// <value>the height</value>
        public override double Height {
            get {
                return height;
            }
        }

        /// <summary>
        /// Set the size of this Dimension object to the specified width
        /// and height in double precision.
        /// </summary>
        /// <param name="width">the new width for the Dimension object</param>
        /// <param name="height">the new height for the Dimension object</param>
        public override void SetSize(double width, double height) {
            width = (int) Math.Ceiling(width);
            height = (int) Math.Ceiling(height);
        }

        /// <summary>
        /// Get/set the size of this <code>Dimension</code> object.
        /// </summary>
        /// <value>the size</value>
        public new Dimension Size {
            get {
                return new Dimension(width, height);
            }

            set {
                SetSize(value.width, value.height);
            }
        }    

        /// <summary>
        /// Set the size of this <code>Dimension</code> object 
        /// to the specified width and height.
        /// </summary>
        /// <param name="width">the new width for this <code>Dimension</code> object.</param>
        /// <param name="height">the new height for this <code>Dimension</code> object.</param>
        public void SetSize(int width, int height) {
            this.width = width;
            this.height = height;
        }    

        /// <summary>
        /// Checks whether two dimension objects have equal values.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj) {
            if (obj is Dimension) {
                Dimension d = (Dimension)obj;
                return (width == d.width) && (height == d.height);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this Dimension.
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode() {
            int sum = width + height;
            return sum * (sum + 1)/2 + width;
        }

        /// <summary>
        /// Returns a string representation of the values of this 
        /// <code>Dimension</code> object's <code>height</code> and 
        /// <code>width</code> fields.
        /// </summary>
        /// <remarks>
        /// This method is intended to be used only 
        /// for debugging purposes, and the content and format of the returned 
        /// string may vary between implementations. The returned string may be 
        /// empty but may not be <code>null</code>.
        /// </remarks>
        /// <returns>a string representation of this <code>Dimension</code>
        /// object.
        /// </returns>
        public override string ToString() {
            return this.GetType().Name + "[width=" + width + ",height=" + height + "]";
        }
    }
}
