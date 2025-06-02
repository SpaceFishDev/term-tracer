namespace TermTracer
{
    class Ray
    {
        public Vector3 Origin, Direction;
        public Ray()
        {
            Origin = new();
            Direction = new();
        }
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
        public Vector3 at(double t)
        {
            return Origin + Direction*t;
        }
    }
}