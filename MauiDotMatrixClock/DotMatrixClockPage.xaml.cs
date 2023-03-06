using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Drawing;
using Color = Microsoft.Maui.Graphics.Color;
using Rectangle = Microsoft.Maui.Controls.Shapes.Rectangle;

namespace MauiDotMatrixClock;

public partial class DotMatrixClockPage : ContentPage
{
	const int horzDots = 41;
	const int vertDots = 7;

    static readonly int[,,] numberPatterns = new int[10, 7, 5]
    {
        {
            {0, 1, 1, 1,0 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 1, 1 }, { 1, 0, 1, 0, 1 },
            { 1, 1, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0}
        },
        {
            {0, 0, 1, 0,0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 1, 1, 0}
        },
        {
            {0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 0, 0, 0, 0, 1 }, { 0, 0, 0, 1, 0 },
            { 0, 0, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 1, 1, 1, 1, 1}
        },
        {
            {1, 1, 1, 1, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0}
        },
        {
            { 0, 0, 0, 1, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 0, 1, 0 }, { 1, 0, 0, 1, 0 },
            { 1, 1, 1, 1, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 1, 0}
        },
        {
            {  1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 1, 0 }, { 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0}
        },
        {
            {0, 0, 1, 1, 0 }, { 0, 1, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 1, 1, 1, 0 },
            { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0}
        },
        {
            { 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 1 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 },
            { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0}
        },
        {
            {0, 1, 1, 1,0 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 1, 1 }, { 1, 0, 1, 0, 1 },
            { 1, 1, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 0}
        },
        {
            { 0, 1, 1, 1, 0 }, { 1, 0, 0, 0, 1 }, { 1, 0, 0, 0, 1 }, { 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 1 }, { 0, 0, 0, 1, 0 }, { 0, 1, 1, 0, 0}
        }
     };

    static readonly int[,] colonPattern = new int[7, 2]
    {
        { 0, 0 }, { 1, 1 }, { 1, 1 }, { 0, 0 }, { 1, 1 }, { 1, 1}, { 0, 0 }
    };

    static readonly Microsoft.Maui.Graphics.Color colorOn = Colors.Red;
    static readonly Microsoft.Maui.Graphics.Color colorOff = new Color(0.5f, 0.5f, 0.5f, 0.25f);

    BoxView[,,] digitBoxViews = new BoxView[6, 7, 5];
    public DotMatrixClockPage()
    {
        InitializeComponent();
        // BoxView dot dimensions
        double height = 0.85 / vertDots;
        double width = 0.85 / horzDots;

        // Create and assemble the BoxViews.
        double xIncrement = 1.0 / (horzDots - 1);
        double yIncrement = 1.0 / (vertDots - 1);
        double x = 0;

        for (int digit = 0; digit < 6; digit++)
        {
            for (int col = 0; col < 5; col++)
            {
                double y = 0;

                for (int row = 0; row < 7; row++)
                {
                    // Create the digit BoxView and add to layout.
                    BoxView boxView = new BoxView();
                    digitBoxViews[digit, row, col] = boxView;

                    absoluteLayout.SetLayoutFlags(boxView, AbsoluteLayoutFlags.All);
                    absoluteLayout.SetLayoutBounds(boxView, new Rect(x, y, width, height));
                    
                    absoluteLayout.Children.Add(boxView);
                    y += yIncrement;
                }
                x += xIncrement;
            }
            x += xIncrement;

            // Colons between the hour, minutes, and seconds.
            if (digit == 1 || digit == 3)
            {
                int colon = digit / 2;

                for (int col = 0; col < 2; col++)
                {
                    double y = 0;

                    for (int row = 0; row < 7; row++)
                    {
                        // Create the BoxView and set the color.
                        BoxView boxView = new BoxView
                        {
                            Color = colonPattern[row, col] == 1 ? colorOn : colorOff
                        };
                        absoluteLayout.SetLayoutBounds(boxView, new Rect(x, y, width, height));
                        absoluteLayout.SetLayoutFlags(boxView, AbsoluteLayoutFlags.All);
                        absoluteLayout.Children.Add(boxView);
                        y += yIncrement;
                    }
                    x += xIncrement;
                }
                x += xIncrement;
            }
        }
        Device.StartTimer(TimeSpan.FromSeconds(1), OnTimer);
        OnTimer();

	}

    private bool OnTimer()
    {
        DateTime dateTime = DateTime.Now;

        // Convert 24-hour clock to 12-hour clock.
        int hour = (dateTime.Hour + 11) % 12 + 1;

        // Set the dot colors for each digit separately.
        SetDotMaxtrix(0, hour / 10);
        SetDotMaxtrix(1, hour % 10);
        SetDotMaxtrix(2, dateTime.Minute / 10);
        SetDotMaxtrix(3, dateTime.Minute % 10);
        SetDotMaxtrix(4, dateTime.Second / 10);
        SetDotMaxtrix(5, dateTime.Second % 10);
        return true;

    }

    private void SetDotMaxtrix(int index, int digit)
    {
       for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                bool isOn = numberPatterns[digit, row, col] == 1;
                Microsoft.Maui.Graphics.Color color = isOn ? colorOn : colorOff;
                digitBoxViews[index, row, col].Color = color;
            }
        }
    }

    private void ContentPage_SizeChanged(object sender, EventArgs e)
    {
      absoluteLayout.HeightRequest = vertDots * Width / horzDots;
    }
}

