using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cshapshiyan_1
{
    /// <summary>   
    /// Summary description for cilpButton.   
    /// </summary>   
    public class CircleButton : System.Windows.Forms.Button
    {
        /*
        private void roundButton_Paint(object sender,
  System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath buttonPath =
            new System.Drawing.Drawing2D.GraphicsPath();
            // Set a new rectangle to the same size as the button's  
            // ClientRectangle property.
            System.Drawing.Rectangle newRectangle = roundButton.ClientRectangle;
            // Decrease the size of the rectangle.
            newRectangle.Inflate(-10, -10);

            // Draw the button's border.
            e.Graphics.DrawEllipse(System.Drawing.Pens.Black, newRectangle);
            // Increase the size of the rectangle to include the border.
            newRectangle.Inflate(1, 1);
            // Create a circle within the new rectangle.
            buttonPath.AddEllipse(newRectangle);

            //设置按钮的Region.
            roundButton.Region = new System.Drawing.Region(buttonPath);
        } */
    }   
}
