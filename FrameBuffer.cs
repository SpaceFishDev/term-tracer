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
            string mappings = "HOGK96D$R8BQ0MW@";
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
            char[] buffer = new char["\x1b[38;2;255;255;255m".Length*Width*Height*2 + (Width*Height*2)+Height ];
            int idx = 0;
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Vector3 color = BufferRgb[x + y * Width];
                    int r = (int)color.x;
                    int g = (int)color.y;
                    int b = (int)color.z;
                    char c = MapRgbToChar(r, g, b);
                    string temp = "";
                    temp += $"\x1b[38;2;{r};{g};{b}m";
                    temp += c;
                    temp += $"\x1b[38;2;{r};{g};{b}m";
                    temp += c;
                    foreach (var chr in temp)
                    {
                        buffer[idx] = chr;
                        ++idx;
                    }
                }
                buffer[idx] = '\n';
                ++idx;
            }
            buffer[idx] = '\0';
            Console.WriteLine(buffer);
        }
    }
}