using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ButtonDemo
{
    // sample taken from "How to Create a Microsoft .NET Compact Framework-based Image Button" whitepaper
    // http://msdn.microsoft.com/en-us/library/aa446518.aspx
    public class ImageButton : Control
    {
        private Image image;
        private Color? imageTransparencyColor;
        private Image imageGrayScale;
        private Color? imageGrayScaleTransparencyColor;

        private bool bTransparent = true;

        private bool bPushed;

        private Bitmap m_bmpOffscreen;

        public ImageButton()
        {
            bPushed = false;

            // default to center
            this.TextAlign = ContentAlignment.MiddleCenter;

            // default to center
            this.ImageAlign = ContentAlignment.MiddleCenter;

            //default minimal size
            this.Size = new Size(21, 21);

            this.EnabledChanged += new EventHandler(ImageButton_EnabledChanged);
        }

        void ImageButton_EnabledChanged(object sender, EventArgs e)
        {
            if (!this.Enabled)
            {
                SetPushed(false);
            }
        }

        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                imageTransparencyColor = BackgroundImageColor(value);

                imageGrayScale = null;
                imageGrayScaleTransparencyColor = null;
            }
        }

        [DefaultValue(true)]
        public bool Transparent
        {
            get { return bTransparent; }
            set { bTransparent = value; this.Invalidate(); }
        }

        //public override Color BackColor
        //{
        //    get { return base.BackColor; }
        //    set { m_bmpOffscreen = null; base.BackColor = value; this.Invalidate(); }
        //}

        private TextImageRelation m_TextImageRelation;
        [DefaultValue(TextImageRelation.Overlay)]
        public TextImageRelation TextImageRelation
        {
            get { return this.m_TextImageRelation; }
            set { this.m_TextImageRelation = value; this.Invalidate(); }
        }

        private ContentAlignment m_TextAlign;
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get { return this.m_TextAlign; }
            set { this.m_TextAlign = value; this.Invalidate(); }
        }

        private ContentAlignment m_ImageAlign;
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment ImageAlign
        {
            get { return this.m_ImageAlign; }
            set { this.m_ImageAlign = value; this.Invalidate(); }
        }

        private bool m_ReadOnly;
        /// <summary>
        /// Wenn True, kann Button nicht geklickt werden. Im Gegensatz zu Enabled
        /// wird der Button aber nicht in GreyStyle dargestellt.
        /// </summary>
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return m_ReadOnly; }
            set
            {
                SetPushed(false);
                m_ReadOnly = value;
            }
        }

        private bool m_ClickOnHold;
        private new System.Windows.Forms.Timer clickOnHoldTimer;
        [DefaultValue(false)]
        public bool ClickOnHold
        {
            get { return m_ClickOnHold; }
            set
            {
                m_ClickOnHold = value;

                if (value == false && clickOnHoldTimer != null)
                {
                    clickOnHoldTimer.Enabled = false;
                    clickOnHoldTimer.Tick -= new EventHandler(clickOnHoldTimer_Tick);
                    clickOnHoldTimer.Dispose();
                    clickOnHoldTimer = null;
                }
                else if (value == true && clickOnHoldTimer == null)
                {
                    clickOnHoldTimer = new System.Windows.Forms.Timer() { Enabled = false, Interval = 500 };
                    clickOnHoldTimer.Tick += new EventHandler(clickOnHoldTimer_Tick);
                }
            }
        }

        void clickOnHoldTimer_Tick(object sender, EventArgs e)
        {
            this.PerformClick();
            if (clickOnHoldTimer.Interval >= 200) clickOnHoldTimer.Interval -= 100;
        }

        // Zusammenfassung:
        //     Ruft einen Wert ab, der beim Klicken auf die Schaltfläche an das übergeordnete
        //     Formular zurückgegeben wird, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     Einer der System.Windows.Forms.DialogResult-Werte. Der Standardwert ist None.
        //
        // Ausnahmen:
        //   System.ComponentModel.InvalidEnumArgumentException:
        //     Der zugewiesene Wert ist keiner der System.Windows.Forms.DialogResult-Werte.
        public virtual DialogResult DialogResult { get; set; }

        //public override string Text
        //{
        //    get
        //    {
        //        return base.Text;
        //    }
        //    set
        //    {
        //        base.Text = value;
        //        this.Invalidate();
        //    }
        //}

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }

        // offset for image/text in pused state
        private const int offsetX = 3;
        private const int offsetY = 3;

        // space between image/text
        private const int padding = 10;

        #region Painting

        protected override void OnResize(EventArgs e)
        {
            // invalidate bitmpap for painting on 
            m_bmpOffscreen = null;
            base.OnResize(e);
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            //Do nothing
        }


        private Rectangle imageRect;
        private Rectangle textRect;

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

            Graphics gxOff;       //Offscreen graphics
            Rectangle imgRect; //image rectangle
            Brush backBrush;   //brush for filling a backcolor
            Brush foreBrush;   //brush for painting the text

            if (m_bmpOffscreen == null) //Bitmap for doublebuffering
            {
                try
                {
                    // Sonderbehandlung, falls Width od. Height = 0
                    var width = Math.Max(ClientSize.Width, 1);
                    var heigth = Math.Max(ClientSize.Height, 1);
                    m_bmpOffscreen = new Bitmap(width, heigth);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} {1}", ClientSize.Width, ClientSize.Height));
                    MessageBox.Show(ex.ToString());
                    m_bmpOffscreen = new Bitmap(1, 1);
                }
            }

            gxOff = Graphics.FromImage(m_bmpOffscreen);

            gxOff.Clear(this.BackColor);

            //if (!bPushed) 
            //    backBrush = new SolidBrush(Parent.BackColor);
            //else //change the background when it's pressed
            //    backBrush = new SolidBrush(Color.LightGray);

            backBrush = new SolidBrush(Parent.BackColor);
            foreBrush = this.Enabled ? new SolidBrush(this.ForeColor) : new SolidBrush(SystemColors.GrayText);

            gxOff.FillRectangle(backBrush, this.ClientRectangle);


            // the available size for drawing
            var size = new Size(this.Width - offsetX, this.Height - offsetY);

            // calculate TextSize
            var textSizeF = e.Graphics.MeasureString(this.Text, this.Font);
            var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
            var imageSize = image != null ? image.Size : new Size(0, 0);

            // calculate available space for image/text
            var rect = GetTextImageRelation(this.TextImageRelation, size, imageSize);

            // image location inside control
            Point imageLocation = GetAbsoluteContentLocation(this.ImageAlign, rect.ImageRectangle, imageSize);

            // text location inside control
            Point textLocation = GetAbsoluteContentLocation(this.TextAlign, rect.TextRectangle, textSize);

            if (image != null)
            {
                ////Center the image relativelly to the control
                //int imageLeft = (this.Width - image.Width) / 2;
                //int imageTop = (this.Height - image.Height) / 2;
                //int imageLeft = imageLocation.X;
                //int imageTop = imageLocation.Y;

                if (bPushed)
                {
                    //The button was pressed
                    //Shift the image by x pixel
                    imgRect = new Rectangle(imageLocation.X + offsetX, imageLocation.Y + offsetY, image.Width, image.Height);
                }
                else
                {
                    imgRect = new Rectangle(imageLocation.X, imageLocation.Y, image.Width, image.Height);
                }

                // create grayscale image if enabled = false
                if (!this.Enabled && imageGrayScale == null)
                {
                    imageGrayScale = ConvertGrayscale(image);
                    imageGrayScaleTransparencyColor = BackgroundImageColor(imageGrayScale);
                }

                //Set transparent key
                ImageAttributes imageAttr = new ImageAttributes();
                if (this.Transparent)
                {
                    if (!this.Enabled)
                        imageAttr.SetColorKey(imageGrayScaleTransparencyColor.Value, imageGrayScaleTransparencyColor.Value);
                    else
                        imageAttr.SetColorKey(imageTransparencyColor.Value, imageTransparencyColor.Value);
                }

                // image to draw
                Image drawImage = this.Enabled ? image : imageGrayScale;

                //Draw image
                gxOff.DrawImage(drawImage, imgRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);

                // set global rect var for click detection
                this.imageRect = new Rectangle(imgRect.Left, imgRect.Top, image.Width, image.Height);
            }

            if (bPushed) //The button was pressed
            {
                //Prepare rectangle
                Rectangle rc = this.ClientRectangle;
                rc.Width--;
                rc.Height--;
                //Draw rectangle
                //gxOff.DrawRectangle(new Pen(Color.Black), rc);
            }

            //Draw from the memory bitmap
            e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);

            //Draw the text
            if (!String.IsNullOrEmpty(this.Text))
            {
                int textLeft = imageLocation.X;
                int textTop = imageLocation.Y;

                if (bPushed)
                {
                    //e.Graphics.DrawString(this.Text, this.Font, foreBrush, this.Width / 2 - size.Width / 2, this.Height / 2 - size.Height / 2);
                    e.Graphics.DrawString(this.Text, this.Font, foreBrush, textLocation.X + offsetX, textLocation.Y + offsetY);
                }
                else
                {
                    //e.Graphics.DrawString(this.Text, this.Font, foreBrush, this.Width / 2 - size.Width / 2 + offsetX, this.Height / 2 - size.Height / 2 + offsetY);
                    e.Graphics.DrawString(this.Text, this.Font, foreBrush, textLocation.X, textLocation.Y);
                }

                // set global rect var for click detection
                this.textRect = new Rectangle(textLocation.X, textLocation.Y, textSize.Width, textSize.Height);

            }

            base.OnPaint(e);
        }

        private struct TextImageRelationResult
        {
            public TextImageRelationResult(Rectangle imageRectangle, Rectangle textRectangle)
            {
                ImageRectangle = imageRectangle;
                TextRectangle = textRectangle;
            }

            public Rectangle ImageRectangle;
            public Rectangle TextRectangle;
        }

        private static TextImageRelationResult GetTextImageRelation(TextImageRelation relation, Size size, Size imageSize)
        {

            Rectangle imageRectangle;
            Rectangle textRectangle;

            switch (relation)
            {
                case TextImageRelation.Overlay:
                    imageRectangle = new Rectangle(0, 0, size.Width, size.Height);
                    textRectangle = new Rectangle(0, 0, size.Width, size.Height);
                    break;
                case TextImageRelation.ImageAboveText:
                    imageRectangle = new Rectangle(0, 0, size.Width, imageSize.Height);
                    textRectangle = new Rectangle(0, imageSize.Height, size.Width, size.Height - imageSize.Height);
                    break;
                case TextImageRelation.TextAboveImage:
                    imageRectangle = new Rectangle(0, size.Height - imageSize.Height, size.Width, imageSize.Height);
                    textRectangle = new Rectangle(0, 0, size.Width, size.Height - imageSize.Height);
                    break;
                case TextImageRelation.ImageBeforeText:
                    imageRectangle = new Rectangle(0, 0, imageSize.Width, size.Height);
                    textRectangle = new Rectangle(imageSize.Width + padding, 0, size.Width - imageSize.Width - padding, size.Height);
                    break;
                case TextImageRelation.TextBeforeImage:
                    imageRectangle = new Rectangle(size.Width - imageSize.Width, 0, imageSize.Width, size.Height);
                    textRectangle = new Rectangle(0, 0, size.Width - imageSize.Width, size.Height);
                    break;
                default:
                    throw new NotImplementedException(relation.ToString());
            }

            return new TextImageRelationResult(imageRectangle, textRectangle);
        }

        private static Point GetRelativeContentLocation(ContentAlignment alignment, Rectangle rectangle, SizeF contentSize)
        {
            return GetRelativeContentLocation(alignment, rectangle, new Size((int)contentSize.Width, (int)contentSize.Height));
        }

        private static Point GetRelativeContentLocation(ContentAlignment alignment, Rectangle rectangle, Size contentSize)
        {
            Point result;
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    result = new Point(0, 0);
                    break;
                case ContentAlignment.TopCenter:
                    result = new Point(rectangle.Width / 2 - contentSize.Width / 2, 0);
                    break;
                case ContentAlignment.TopRight:
                    result = new Point(rectangle.Width - contentSize.Width, 0);
                    break;
                case ContentAlignment.MiddleLeft:
                    result = new Point(0, rectangle.Height / 2 - contentSize.Height / 2);
                    break;
                case ContentAlignment.MiddleCenter:
                    result = new Point(rectangle.Width / 2 - contentSize.Width / 2, rectangle.Height / 2 - contentSize.Height / 2);
                    break;
                case ContentAlignment.MiddleRight:
                    result = new Point(rectangle.Width - contentSize.Width, rectangle.Height / 2 - contentSize.Height / 2);
                    break;
                case ContentAlignment.BottomLeft:
                    result = new Point(0, rectangle.Height - contentSize.Height);
                    break;
                case ContentAlignment.BottomCenter:
                    result = new Point(rectangle.Width / 2 - contentSize.Width / 2, rectangle.Height - contentSize.Height);
                    break;
                case ContentAlignment.BottomRight:
                    result = new Point(rectangle.Width - contentSize.Width, rectangle.Height - contentSize.Height);
                    break;
                default:
                    throw new NotImplementedException(alignment.ToString());
            }
            return result;
        }

        private static Point GetAbsoluteContentLocation(ContentAlignment alignment, Rectangle rectangle, SizeF contentSize)
        {
            return GetAbsoluteContentLocation(alignment, rectangle, new Size((int)contentSize.Width, (int)contentSize.Height));
        }

        private static Point GetAbsoluteContentLocation(ContentAlignment alignment, Rectangle rectangle, Size contentSize)
        {
            // get relative location
            var relativeLocation = GetRelativeContentLocation(alignment, rectangle, contentSize);

            // return absolute location
            return new Point(rectangle.X + relativeLocation.X, rectangle.Y + relativeLocation.Y);
        }

        private static Image ConvertGrayscale(Image image)
        {
            Bitmap original = new Bitmap(image);

            //make an empty bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    Color originalColor = original.GetPixel(i, j);

                    if (originalColor.R == 0 && originalColor.G == 0 && originalColor.B == 0)
                    {
                        originalColor = SystemColors.ControlDark;
                    }
                    else if (originalColor.R == 255 && originalColor.G == 255 && originalColor.B == 255)
                    {
                        originalColor = SystemColors.ControlLight;
                    }

                    //create the grayscale version of the pixel
                    int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59) + (originalColor.B * .11));

                    //create the color object
                    Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

                    //set the new image's pixel to the grayscale version
                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            original.Dispose();

            return newBitmap;
        }

        #endregion

        public void PerformClick()
        {
            PerformClick(0);
        }

        public void PerformClick(int duration)
        {
            if (this.Enabled && !this.ReadOnly)
            {
                var pushed = bPushed;
                SetPushed(true);

                try
                {
                    var start = DateTime.Now;
                    this.OnClick(EventArgs.Empty);
                    var stop = DateTime.Now;
                    duration -= (int)((stop - start).TotalMilliseconds);
                }
                finally
                {
                    if (duration > 0)
                    {
                        Task.Run(async () => await Task.Delay(duration))
                            .ContinueWith(t =>
                                SetPushed(false), TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        SetPushed(pushed);
                    }
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            // Click Event nur innerhalb des Contents (entweder image oder text akzeptieren)
            if (bPushed)
            {
                OnBeforeClick();
                base.OnClick(e);
                OnAfterClick();

                if (this.DialogResult != DialogResult.None && !this.IsDisposed && this.TopLevelControl != null && !this.TopLevelControl.IsDisposed)
                    ((Form)this.TopLevelControl).DialogResult = this.DialogResult;

            }
        }

        private bool CoordinateIsInRectangle(Rectangle rect, int x, int y)
        {
            return x >= rect.Left && x <= rect.Left + rect.Width && y >= rect.Top && y <= rect.Top + rect.Height;
        }

        private bool CoordinateIsInRectangle(Rectangle rect, Point p)
        {
            return CoordinateIsInRectangle(rect, p.X, p.Y);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            this.PerformClick();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {

            if (CoordinateIsInRectangle(imageRect, e.X, e.Y) || CoordinateIsInRectangle(textRect, e.X, e.Y))
            {
                SetPushed(true);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            SetPushed(false);

            base.OnMouseUp(e);
        }

        public void SetPushed(bool pushed)
        {
            if (!bPushed && pushed && this.Enabled && !this.ReadOnly)
            {
                bPushed = true;
                this.Invalidate();

                if (clickOnHoldTimer != null)
                {
                    clickOnHoldTimer.Enabled = true;
                }
            }
            else if (bPushed && !pushed)
            {
                bPushed = false;
                this.Invalidate();

                if (clickOnHoldTimer != null)
                {
                    clickOnHoldTimer.Enabled = false;
                    clickOnHoldTimer.Interval = 500;
                }
            }
        }

        private Color? BackgroundImageColor(Image image)
        {
            if (image == null) return null;
            Bitmap bmp = new Bitmap(image);
            return bmp.GetPixel(0, 0);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ImageButton
            // 
            this.ResumeLayout(false);

        }

        public event EventHandler BeforeClick;

        protected virtual void OnBeforeClick()
        {
            var eh = this.BeforeClick;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        public event EventHandler AfterClick;

        protected virtual void OnAfterClick()
        {
            var eh = this.AfterClick;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        #region IHelpText Member

        public string HelpText { get; set; }

        #endregion
    }
}
