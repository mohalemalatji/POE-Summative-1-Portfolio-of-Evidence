using System.Drawing.Drawing2D;

namespace CybersecurityAwarenessBot.Controls;

public class RoundedPanel : Panel
{
    public int CornerRadius { get; set; } = 18;
    public Color FillColor { get; set; } = Color.White;


    public RoundedPanel()
    {
        DoubleBuffered = true;
        BackColor = Color.Transparent;
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle bounds = ClientRectangle;
        bounds.Width -= 1;
        bounds.Height -= 1;

        using GraphicsPath path = CreateRoundedPath(bounds, CornerRadius);
        using SolidBrush brush = new(FillColor);
        e.Graphics.FillPath(brush, path);
    }


    protected override void OnResize(EventArgs eventArgs)
    {
        base.OnResize(eventArgs);
        Invalidate();
    }


    private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new();

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
