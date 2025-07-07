using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Online_Shop_Pet_Project.UI
{
    public static class UIComponentsFactory
    {
        public static PictureBox CreateLogoPictureBox()
        {
            var logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 150),
                Location = new Point(125, 20)
            };

            string imagePath = Path.Combine(Application.StartupPath, "art", "logo.png");
            logo.Image = LoadImageOrDefault(imagePath, 150, 150);

            return logo;
        }

        public static Image LoadImageOrDefault(string imagePath, int width, int height)
        {
            if (File.Exists(imagePath))
            {
                try
                {
                    return Image.FromFile(imagePath);
                }
                catch
                {
                    return CreatePlaceholderImage(width, height);
                }
            }
            return CreatePlaceholderImage(width, height);
        }

        public static Bitmap CreatePlaceholderImage(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.LightGray))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
                gfx.DrawString("No Image", new Font("Segoe UI", 10), Brushes.DarkGray,
                    new PointF(width / 2 - 30, height / 2 - 10));
            }
            return bmp;
        }

        public static Label CreateTitleLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(120, 180)
            };
        }

        public static TextBox CreateTextBox(string placeholder, Point location, bool isPassword = false)
        {
            return new TextBox
            {
                Text = placeholder,
                Size = new Size(300, 30),
                Location = location,
                Font = new Font("Segoe UI", 12),
                PasswordChar = isPassword ? '*' : '\0'
            };
        }

        public static Button CreateButton(string text, Point location)
        {
            var button = new Button
            {
                Text = text,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(300, 40),
                Location = location,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        public static LinkLabel CreateLinkLabel(string text, Point location, LinkLabelLinkClickedEventHandler handler)
        {
            var linkLabel = new LinkLabel
            {
                Text = text,
                AutoSize = true,
                Location = location,
                Font = new Font("Segoe UI", 10)
            };
            linkLabel.LinkClicked += handler;
            return linkLabel;
        }
    }
}