using System.Data.Common;
using System.Numerics;
using System.Runtime.InteropServices;
namespace TermTracer
{
    class HitRecord
    {
        public Vector3 p;
        public Vector3 Normal;
        public double t;
        public HitRecord()
        {
            p = new();
            Normal = new();
        }
    }
    abstract class Object
    {
        public abstract bool hit(Ray r, double ray_tmin, double ray_tmax, out HitRecord rec);
    }
    class Sphere : Object
    {
        public Vector3 Center;
        public double Radius;
        public Sphere(Vector3 c, double r)
        {
            Center = c;
            Radius = r;
        }
        public override bool hit(Ray r, double ray_tmin, double ray_tmax, out HitRecord rec)
        {

            Vector3 oc = Center - r.Origin;
            var a = Vector3.dot(r.Direction, r.Direction);
            var h = Vector3.dot(r.Direction, oc);
            var c = Vector3.dot(oc, oc) - Radius * Radius;
            var discriminant = h * h - a * c;
            if (discriminant < 0)
            {
                rec = new();
                return false;
            }
            var sqrtd = Math.Sqrt(discriminant);
            var root = (h - sqrtd) / a;
            if (root <= ray_tmin || root >= ray_tmax)
            {
                root = (h + sqrtd) / a;
                if (root <= ray_tmin || root >= ray_tmax)
                {
                    rec = new();
                    return false;
                }
            }
            rec = new();
            rec.t = root;
            rec.p = r.at(rec.t);
            rec.Normal = (rec.p - Center) / Radius;
            return true;

        }

    }
    class Program
    {
        public static Vector3 RayColor(Ray r)
        {
            var t = HitSphere(new(0, 0, -1), 0.5, r);
            if(t > 0.0)
            {
                var N = (r.at(t) - new Vector3(0, 0, -1)).UnitVector();
                return new Vector3(N.x + 1, N.y + 1, N.z + 1) * 0.5;
            }
            Vector3 unitDirection = r.Direction.UnitVector();
            var a = 0.5 * (unitDirection.y + 1.0);
            return new Vector3(1, 1, 1) * (1 - a) + new Vector3(0.5, 0.7, 1.0) * a;
        }
        public static void Main()
        {
            FrameBuffer fb = new();
            double aspectRatio = (float)fb.Width / (float)fb.Height;
            double viewportHeight = 2.0;
            double viewportWidth = viewportHeight * aspectRatio;
            Vector3 CameraCenter = new(0,0,0);
            Vector3 viewportU = new Vector3(viewportWidth, 0, 0);
            Vector3 viewportV = new Vector3(0, -viewportHeight, 0);

            double focalLength = 1;
            Vector3 pixelDeltaU = viewportU / (double)fb.Width;
            Vector3 pixelDeltaV = viewportV / (double)fb.Height;

            Vector3 viewPortUpperLeft = CameraCenter - (new Vector3(0, 0, focalLength) + viewportU/2 + viewportV/2);
            Vector3 pixel00Loc = viewPortUpperLeft + (pixelDeltaU + pixelDeltaV)*0.5;


            for (int x = 0; x < fb.Width; ++x)
            {
                for (int y = 0; y < fb.Height; ++y)
                {
                    var pixelCenter = pixel00Loc + (pixelDeltaU * (double)x) + (pixelDeltaV * (double)y);
                    var rayDir = pixelCenter - CameraCenter;
                    Ray r = new(CameraCenter, rayDir);
                    var Color = RayColor(r);
                    Color *= 255;
                    fb.PutPixel(x, y, Color);
                }
            }
            fb.ClearTerminal();
            fb.DrawToTerminal();
        }
    }
}