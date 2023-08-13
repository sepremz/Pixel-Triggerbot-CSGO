using System.Drawing;
using System.Runtime.InteropServices;

namespace pixeltrigger
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]

        static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, IntPtr dwExtraInfo);

        const uint LEFTDOWN = 0x02;
        const uint LEFTUP = 0x04;

        int x = Screen.PrimaryScreen.Bounds.Width / 2 + 1;
        int y = Screen.PrimaryScreen.Bounds.Height / 2 + 1;
        Keys hotkey = Keys.XButton2;
        Point coordinates;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            coordinates = new Point(x, y);
            CheckForIllegalCrossThreadCalls = false;
            label1.Text = "Trigger hotkey -> " + hotkey.ToString() + " <-";
            Thread th = new Thread(BackgroundLogic) { IsBackground = true };
            th.Start();
        }
        void BackgroundLogic()
        {
            while (true)
            {
                if (KeyPressed(hotkey))
                {
                    bool colorChanged = false;
                    var oldColor = GetColorAt(coordinates);

                    while (!colorChanged)
                    {
                        var newColor = GetColorAt(coordinates);

                        if (!AreColorsClose(oldColor,newColor, 40))
                        {
                            colorChanged = true;
                        }
                    }

                    if (KeyPressed(hotkey))
                    {
                        Thread.Sleep(20);
                        PerformClick(0, 0);
                    }
                }


            }
        }
        Color GetColorAt(Point coordinates)
        {
            using (Bitmap pixelContainer = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(pixelContainer))
                {
                    g.CopyFromScreen(coordinates, Point.Empty, pixelContainer.Size);
                }
                return pixelContainer.GetPixel(0, 0);
            }
        }

        bool AreColorsClose(Color color1, Color color2, int maxColorDifference)
        {
            int redDiff = Math.Abs(color1.R - color2.R);
            int greenDiff = Math.Abs(color1.G - color2.G);
            int blueDiff = Math.Abs(color1.B - color2.B);

            return redDiff <= maxColorDifference && greenDiff <= maxColorDifference && blueDiff <= maxColorDifference;
        }


        void PerformClick(int x, int y)
        {
            mouse_event(LEFTDOWN, x, y, 0, IntPtr.Zero);
            Thread.Sleep(2);
            mouse_event(LEFTUP, x, y, 0, IntPtr.Zero);
        }


        bool KeyPressed(Keys vKey)
        {
            return GetAsyncKeyState(vKey) < 0;
        }
    }

        
}