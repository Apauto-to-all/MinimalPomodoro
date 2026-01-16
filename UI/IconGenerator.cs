using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using MinimalPomodoro.Models;

namespace MinimalPomodoro.UI;

public static class IconGenerator
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    public static Icon GenerateTomatoIcon(float progress, PomodoroState state)
    {
        Color baseColor = state switch
        {
            PomodoroState.Working => Color.FromArgb(231, 76, 60),    // Vivid Red
            PomodoroState.ShortBreak => Color.FromArgb(39, 174, 96), // Strong Green
            PomodoroState.LongBreak => Color.FromArgb(41, 128, 185),  // Strong Blue
            PomodoroState.Paused => Color.FromArgb(140, 140, 140),   // Neutral Medium Gray
            _ => Color.Gray
        };

        // Darker border color
        Color borderColor = (state == PomodoroState.Paused)
            ? Color.FromArgb(80, 80, 80) // Darker gray for pause border
            : Color.FromArgb(
                Math.Max(0, baseColor.R - 50),
                Math.Max(0, baseColor.G - 50),
                Math.Max(0, baseColor.B - 50)
            );

        // Progress logic: Work empties, Break fills
        float fillLevel = (state == PomodoroState.Working) ? progress : (1.0f - progress);

        using Bitmap bitmap = new Bitmap(32, 32);
        using Graphics g = Graphics.FromImage(bitmap);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.Clear(Color.Transparent);

        // Tomato main body area (slightly flatter than a perfect circle)
        // 30x25 ellipse centered in 32x32
        RectangleF bodyRect = new RectangleF(1, 5, 30, 25);

        // 1. Draw Background
        // Paused: Deeper gray to make it look "shutdown"
        // Active: Pure White for clarity
        using (Brush bgBrush = (state == PomodoroState.Paused)
            ? new SolidBrush(Color.FromArgb(210, 210, 210))
            : new SolidBrush(Color.White))
        {
            g.FillEllipse(bgBrush, bodyRect);
        }

        // 2. Fill progress
        float fillHeight = bodyRect.Height * fillLevel;
        if (fillHeight > 0)
        {
            RectangleF clipRect = new RectangleF(bodyRect.X, bodyRect.Y + (bodyRect.Height - fillHeight), bodyRect.Width, fillHeight);
            g.SetClip(clipRect);
            using (SolidBrush fillBrush = new SolidBrush(baseColor))
            {
                g.FillEllipse(fillBrush, bodyRect);
            }
            g.ResetClip();
        }

        // 3. Draw Body Border
        using (Pen borderPen = new Pen(borderColor, 1.8f))
        {
            g.DrawEllipse(borderPen, bodyRect);
        }

        // 4. Highlight/Reflection
        // Nearly hide the highlight when paused to look "flat/dead"
        int highlightAlpha = (state == PomodoroState.Paused) ? 40 : 160;
        using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(highlightAlpha, Color.White)))
        {
            g.FillEllipse(highlightBrush, 7, 9, 7, 4);
        }

        // 5. Draw Leaves (A spiky star-shaped crown like the reference)
        Color leafColor = (state == PomodoroState.Paused)
            ? Color.FromArgb(100, 110, 100)
            : Color.FromArgb(0, 120, 0); // Darker, richer green

        using (SolidBrush leafBrush = new SolidBrush(leafColor))
        {
            // Define a spiky polygon for the leaf crown
            PointF[] leafPoints = new PointF[]
            {
                new PointF(16, 1),   // Top tip
                new PointF(13, 5),
                new PointF(8, 4),    // Left tip
                new PointF(13, 8),
                new PointF(10, 12),  // Bottom-left tip
                new PointF(16, 9),   // Bottom center indent
                new PointF(22, 12),  // Bottom-right tip
                new PointF(19, 8),
                new PointF(24, 4),   // Right tip
                new PointF(19, 5)
            };
            g.FillPolygon(leafBrush, leafPoints);

            // Add a tiny stem at the very top
            using (Pen stemPen = new Pen(leafColor, 1.5f))
            {
                g.DrawLine(stemPen, 16, 0, 16, 3);
            }
        }

        // 6. Pause Overlay
        if (state == PomodoroState.Paused)
        {
            using Pen pausePen = new Pen(Color.White, 3);
            g.DrawLine(pausePen, 13, 14, 13, 22);
            g.DrawLine(pausePen, 19, 14, 19, 22);
        }

        IntPtr hIcon = bitmap.GetHicon();
        Icon icon = Icon.FromHandle(hIcon);
        Icon clonedIcon = (Icon)icon.Clone();
        DestroyIcon(hIcon);
        return clonedIcon;
    }

    public static Bitmap GetPlayIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            PointF[] points = { new PointF(4, 3), new PointF(4, 13), new PointF(13, 8) };
            g.FillPolygon(Brushes.DarkSlateGray, points);
        }
        return bmp;
    }

    public static Bitmap GetPauseIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.FillRectangle(Brushes.DarkSlateGray, 4, 3, 3, 10);
            g.FillRectangle(Brushes.DarkSlateGray, 9, 3, 3, 10);
        }
        return bmp;
    }

    public static Bitmap GetResetIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen p = new Pen(Color.DarkSlateGray, 1.6f))
            {
                p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawArc(p, 3, 3, 10, 10, 45, 300);
                // arrow head
                g.FillPolygon(Brushes.DarkSlateGray, new PointF[] { new PointF(12, 4), new PointF(14, 6), new PointF(10, 7) });
            }
        }
        return bmp;
    }

    public static Bitmap GetOpenConfigIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush b = new SolidBrush(Color.FromArgb(255, 200, 80)))
            {
                g.FillRectangle(b, 2, 5, 12, 7);
                g.FillRectangle(Brushes.BurlyWood, 4, 3, 6, 4);
            }
            g.DrawRectangle(Pens.SaddleBrown, 2, 5, 12, 7);
        }
        return bmp;
    }

    public static Bitmap GetAutoStartIcon(bool enabled = false)
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color c = enabled ? Color.FromArgb(39, 174, 96) : Color.FromArgb(160, 160, 160);
            using (Pen p = new Pen(c, 1.8f))
            {
                g.DrawEllipse(p, 3, 3, 10, 10);
                g.DrawLine(p, 8, 5, 8, 9);
            }
        }
        return bmp;
    }

    public static Bitmap GetLanguageIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen p = new Pen(Color.FromArgb(100, 100, 100), 1.2f))
            {
                // Draw a simple globe-like circle with some lines
                g.DrawEllipse(p, 2, 2, 12, 12);
                g.DrawEllipse(p, 5, 2, 6, 12);
                g.DrawLine(p, 2, 8, 14, 8);
            }
        }
        return bmp;
    }

    public static Bitmap GetExitIcon()
    {
        Bitmap bmp = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen p = new Pen(Color.FromArgb(200, 60, 60), 2f))
            {
                g.DrawLine(p, 4, 4, 12, 12);
                g.DrawLine(p, 12, 4, 4, 12);
            }
        }
        return bmp;
    }
}
