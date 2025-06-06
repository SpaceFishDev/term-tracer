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
        bool frontFace;
        public Material mat;
        public HitRecord()
        {
            p = new();
            Normal = new();
        }
        public void SetFrontFace(Ray r, Vector3 outwardNormal)
        {
            frontFace = Vector3.dot(r.Direction, outwardNormal) < 0;
            Normal = frontFace ? outwardNormal : outwardNormal*-1;
        }
    }
    abstract class Material
    {
        public abstract bool scatter(Ray rIn, HitRecord rec, out Vector3 attenuation, out Ray scattered);
    }
    class Lambertian : Material
    {
        public Vector3 Albedo = new();
        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }
        public override bool scatter(Ray rIn, HitRecord rec, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 scatterDir = rec.Normal + Vector3.randomUnitVector();
            if (scatterDir.NearZero())
            {
                scatterDir = rec.Normal;
            }
            scattered = new Ray(rec.p, scatterDir);
            attenuation = Albedo;
            return true;
        }
    }
    class Metal : Material
    {
        public Vector3 Albedo;
        public Metal(Vector3 albedo)
        {
            Albedo = albedo;
        }
        public override bool scatter(Ray rIn, HitRecord rec, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 reflected = Vector3.Reflect(rIn.Direction, rec.Normal);
            scattered = new Ray(rec.p, reflected);
            attenuation = Albedo;
            return true;
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
        public Material mat;
        public Sphere(Vector3 c, double r, Material mat)
        {
            Center = c;
            Radius = r;
            this.mat = mat;
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
            rec.mat = this.mat;
            rec.Normal = (rec.p - Center) / Radius;
            Vector3 outwardNormal = (rec.p - Center) / Radius;
            rec.SetFrontFace(r, outwardNormal);
            return true;

        }

    }
    class World : Object
    {
        public List<Object> Objects = new();
        public void Add(Object obj)
        {
            Objects.Add(obj);
        }
        public override bool hit(Ray r, double ray_tmin, double ray_tmax, out HitRecord rec)
        {
            rec = new();
            HitRecord temp = new();
            bool hitAnything = false;
            double closestSoFar = ray_tmax;
            foreach (var Obj in Objects)
            {
                if (Obj.hit(r, ray_tmin, closestSoFar, out temp))
                {
                    hitAnything = true;
                    closestSoFar = temp.t;
                    rec = temp;
                }
            }
            return hitAnything;
        }
    }
    class Program
    {
        private static Random r = new();
        public static double RandomDouble()
        {
            return r.NextDouble();
        }
        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * RandomDouble();
        }

        public static Vector3 RayColor(Ray r, World world, int depth)
        {
            if (depth <= 0)
            {
                return new(0, 0, 0);
            }
            HitRecord rec = new();
            if (world.hit(r, 0.0001, double.PositiveInfinity, out rec))
            {
                Ray scattered;
                Vector3 attenuation;
                if (rec.mat.scatter(r, rec, out attenuation, out scattered))
                {
                    return attenuation * RayColor(scattered, world, depth-1);
                }
                return new Vector3(0,0,0);
            }
            Vector3 unitDirection = r.Direction.UnitVector();
            var a = 0.5 * (unitDirection.y + 1.0);
            return new Vector3(1, 1, 1) * (1 - a) + new Vector3(0.5, 0.7, 1.0) * a;
        }
        public const int SPP = 40;
        public const int MaxDepth = 20;
        public static Ray GetRay(double x, double y)
        {
            var offset = SampleSquare();
            var pixelSample = pixel00Loc + (pixelDeltaU * (x + offset.x)) + (pixelDeltaV*(y + offset.y));
            var origin = CameraCenter;
            var direction = pixelSample - CameraCenter;

            return new(origin, direction);
        }
        public static Vector3 SampleSquare()
        {
            return new(RandomDouble() - 0.5, RandomDouble() - 0.5, RandomDouble() - 0.5);
        }
        public static Vector3 pixel00Loc = new();
        public static Vector3 pixelDeltaU = new();
        public static Vector3 pixelDeltaV = new();
        public static Vector3 CameraCenter = new();
        public static void Main()
        {
            FrameBuffer fb = new();
            double aspectRatio = (float)fb.Width / (float)fb.Height;
            double viewportHeight = 2.0;
            double viewportWidth = viewportHeight * aspectRatio;
            CameraCenter = new(0, 0, 0);
            Vector3 viewportU = new Vector3(viewportWidth, 0, 0);
            Vector3 viewportV = new Vector3(0, -viewportHeight, 0);

            double focalLength = 1;
            pixelDeltaU = viewportU / (double)fb.Width;
            pixelDeltaV = viewportV / (double)fb.Height;

            Vector3 viewPortUpperLeft = CameraCenter - (new Vector3(0, 0, focalLength) + viewportU / 2 + viewportV / 2);
            pixel00Loc = viewPortUpperLeft + (pixelDeltaU + pixelDeltaV) * 0.5;
            World world = new();
            world.Add(new Sphere(new(0, 0, -1), 0.5, new Lambertian(new(0.8,0.2,0.4))));
            world.Add(new Sphere(new(0, -100.5, -1), 100, new Lambertian(new(0.2,0.6,0.6))));



            for (int x = 0; x < fb.Width; ++x)
            {
                for (int y = 0; y < fb.Height; ++y)
                {
                    var Color = new Vector3();
                    for (int i = 0; i < SPP; ++i)
                    {
                        Ray r = GetRay(x,y);
                        Color += RayColor(r, world, MaxDepth);
                    }
                    Color /= (double)SPP;
                    Color *= 255.0;
                    fb.PutPixel(x, y, Color);
                }
            }
            fb.ClearTerminal();
            fb.DrawToTerminal();
        }
    }
}