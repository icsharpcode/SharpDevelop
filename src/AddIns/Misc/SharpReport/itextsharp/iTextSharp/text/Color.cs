using System;

namespace iTextSharp.text {
    /// <summary>
    /// Base class for Color, serves as wrapper class for <see cref="T:System.Drawing.Color"/>
    /// to allow extension.
    /// </summary>
    public class Color {
        public static readonly Color WHITE = new Color(255, 255, 255);
        public static readonly Color LIGHT_GRAY = new Color(192, 192, 192);
        public static readonly Color GRAY      = new Color(128, 128, 128);
        public static readonly Color DARK_GRAY  = new Color(64, 64, 64);
        public static readonly Color BLACK     = new Color(0, 0, 0);
        public static readonly Color RED       = new Color(255, 0, 0);
        public static readonly Color PINK      = new Color(255, 175, 175);
        public static readonly Color ORANGE     = new Color(255, 200, 0);
        public static readonly Color YELLOW     = new Color(255, 255, 0);
        public static readonly Color GREEN     = new Color(0, 255, 0);
        public static readonly Color MAGENTA    = new Color(255, 0, 255);
        public static readonly Color CYAN     = new Color(0, 255, 255);
        public static readonly Color BLUE     = new Color(0, 0, 255);
        private const double FACTOR = 0.7;
        System.Drawing.Color color;

        /// <summary>
        /// Constuctor for Color object.
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 255.</param>
        public Color(int red, int green, int blue) {
            color = System.Drawing.Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// Constuctor for Color object.
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="alpha">The transparency component value for the new Color structure. Valid values are 0 through 255.</param>
        public Color(int red, int green, int blue, int alpha) {
            color = System.Drawing.Color.FromArgb(alpha, red, green, blue);
        }

        /// <summary>
        /// Constructor for Color object
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 1.</param>
        public Color(float red, float green, float blue) {
            color = System.Drawing.Color.FromArgb((int)(red * 255 + .5), (int)(green * 255 + .5), (int)(blue * 255 + .5));
        }

        /// <summary>
        /// Constructor for Color object
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="alpha">The transparency component value for the new Color structure. Valid values are 0 through 1.</param>
        public Color(float red, float green, float blue, float alpha) {
            color = System.Drawing.Color.FromArgb((int)(alpha * 255 + .5), (int)(red * 255 + .5), (int)(green * 255 + .5), (int)(blue * 255 + .5));
        }

        public Color(int argb) {
            color = System.Drawing.Color.FromArgb(argb);
        }

        /// <summary>
        /// Constructor for Color object
        /// </summary>
        /// <param name="color">a Color object</param>
        /// <overloads>
        /// Has three overloads.
        /// </overloads>
        public Color(System.Drawing.Color color) {
            this.color = color;
        }

        /// <summary>
        /// Gets the red component value of this <see cref="T:System.Drawing.Color"/> structure.
        /// </summary>
        /// <value>The red component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
        public int R {
            get {
                return color.R;
            }
        }

        /// <summary>
        /// Gets the green component value of this <see cref="T:System.Drawing.Color"/> structure.
        /// </summary>
        /// <value>The green component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
        public int G {
            get {
                return color.G;
            }
        }

        /// <summary>
        /// Gets the blue component value of this <see cref="T:System.Drawing.Color"/> structure.
        /// </summary>
        /// <value>The blue component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
        public int B {
            get {
                return color.B;
            }
        }

        public Color Brighter() {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            int i = (int)(1.0/(1.0-FACTOR));
            if ( r == 0 && g == 0 && b == 0) {
            return new Color(i, i, i);
                }
            if ( r > 0 && r < i ) r = i;
            if ( g > 0 && g < i ) g = i;
            if ( b > 0 && b < i ) b = i;

            return new Color(Math.Min((int)(r/FACTOR), 255), 
                    Math.Min((int)(g/FACTOR), 255),
                    Math.Min((int)(b/FACTOR), 255));
        }

        public Color Darker() {
            return new Color(Math.Max((int)(color.R * FACTOR), 0), 
                    Math.Max((int)(color.G * FACTOR), 0),
                    Math.Max((int)(color.B * FACTOR), 0));
        }
    
        public override bool Equals(object obj) {
            if (!(obj is Color))
                return false;
            return color.Equals(((Color)obj).color);
        }
    
        public override int GetHashCode() {
            return color.GetHashCode();
        }

        public int ToArgb() {
            return color.ToArgb();
        }
    }
}
