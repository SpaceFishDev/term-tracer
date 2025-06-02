using System.Runtime.InteropServices;
namespace TermTracer
{
    class FrameBuffer
    {
        Vector3[] BufferRgb;
        const int STDOUT_FILENO = 1;
        const int TIOCGWINSZ = 0x5413;


        public int Width, Height;
        public FrameBuffer()
        {
            Width = Console.WindowWidth / 2;
            Height = Console.WindowHeight;
            BufferRgb = new Vector3[Width * Height];
            for (int i = 0; i < Width * Height; ++i)
            {
                BufferRgb[i] = new(0, 0, 0);
            }
        }
        public void PutPixel(int x, int y, Vector3 color)
        {
            BufferRgb[x + y * Width] = color;
        }
        public char MapRgbToChar(int r, int g, int b)
        {
            float total = (float)r + (float)g + (float)b;
            total /= 3.0f;
            total /= 255;
            string mappings = "_.,:;!rvL7Tictzlnxu14oYjsIVFJfeyCZa2APqhk3SXENUbwdp5HOGK96mD$R8BQ0MW@";
            total *= mappings.Length;
            if (total > mappings.Length)
            {
                total = mappings.Length - 1;
            }
            return mappings[(int)total];
        }
        public void ClearTerminal()
        {
            Console.WriteLine("\x1b[3J\x1b[H\x1b[2J");
        }
        public void DrawToTerminal()
        {
            string buffer = "";
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Vector3 color = BufferRgb[x + y * Width];
                    int r = (int)color.x;
                    int g = (int)color.y;
                    int b = (int)color.z;
                    char c = MapRgbToChar(r, g, b);
                    buffer += $"\x1b[38;2;{r};{g};{b}m";
                    buffer += c;
                    buffer += $"\x1b[38;2;{r};{g};{b}m";
                    buffer += c;
                }
                buffer += '\n';
            }
            Console.WriteLine(buffer);
        }
    }
}