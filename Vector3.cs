namespace TermTracer
{
    class Vector3
    {
        public double[] e = new double[3]{0,0,0};
        public double x
        {
            get
            {
                return e[0];
            }
            set
            {
                e[0] = value;
            }
        }
        public double y
        {
            get
            {
                return e[1];
            }
            set
            {
                e[1] = value;
            }
        }
        public double z
        {
            get
            {
                return e[2];
            }
            set
            {
                e[2] = value;
            }
        }
        public Vector3()
        {
            
        }
        public Vector3(double x, double y, double z)
        {
            e[0] = x;
            e[1] = y;
            e[2] = z;
        }
        public static Vector3 operator +(Vector3 a, Vector3 v)
        {
            return new(a.x + v.x, a.y + v.y, a.z + v.z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 v)
        {
            return new(a.x - v.x, a.y - v.y, a.z - v.z);
        }
        public static Vector3 operator /(Vector3 a, Vector3 v)
        {
            return new(a.x / v.x, a.y / v.y, a.z / v.z);
        }
        public static Vector3 operator *(Vector3 a, Vector3 v)
        {
            return new(a.x * v.x, a.y * v.y, a.z * v.z);
        }
        public static Vector3 operator +(Vector3 b, double a)
        {
            return new Vector3(b.x + a, b.y + a, b.z + a);
        }
        public static Vector3 operator -(Vector3 b, double a)
        {
            return new Vector3(b.x - a, b.y - a, b.z - a);
        }
        public static Vector3 operator /(Vector3 b, double a)
        {
            return new Vector3(b.x / a, b.y / a, b.z / a);
        }
        public static Vector3 operator *(Vector3 b, double a)
        {
            return new Vector3(b.x * a, b.y * a, b.z * a);
        }
        public double length_squared()
        {
            return x * x + y * y + z * z;
        }
        public double length()
        {
            return Math.Sqrt(length_squared());
        }
        public override string ToString()
        {
            return $"{x} {y} {z}";
        }
        public static double dot(Vector3 u, Vector3 v)
        {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }
        public static Vector3 cross(Vector3 u, Vector3 v)
        {
            return new Vector3(u.e[1]*v.e[2]-u.e[2]*v.e[1],u.e[2]*u.e[0]-u.e[0]*v.e[2], u.e[0]*v.e[1]-u.e[1]*v.e[0]);
        }
        public Vector3 UnitVector()
        {
            if (this.e[0] == 0 && this.e[1] == 0 && this.e[2] == 0)
            {
                return this;
            }
            return this / this.length();
        }
    }
}