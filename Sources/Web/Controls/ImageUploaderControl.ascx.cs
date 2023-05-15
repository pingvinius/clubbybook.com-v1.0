namespace ClubbyBook.Web.Controls
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using ClubbyBook.Common.Logging;

    public partial class ImageUploaderControl : UserControl
    {
        public string ImagePath
        {
            get
            {
                if (ViewState["iucImagePath"] == null)
                    ViewState["iucImagePath"] = string.Empty;

                return ViewState["iucImagePath"].ToString();
            }
            set
            {
                ViewState["iucImagePath"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ImagePath = string.Empty;

            if (IsPostBack)
            {
                string filePath;
                int imageWidth;
                int imageHeight;
                int thumbnailWidth;
                int thumbnailHeight;
                int thumbnailX;
                int thumbnailY;

                if (ValidateBeforeResize(out filePath, out imageWidth, out imageHeight, out thumbnailWidth, out thumbnailHeight,
                  out thumbnailX, out thumbnailY))
                {
                    filePath = Server.MapPath(filePath);

                    try
                    {
                        byte[] cropImage = null;

                        using (System.Drawing.Image origImage = System.Drawing.Image.FromFile(filePath))
                        {
                            double scaleX = (double)origImage.Width / (double)imageWidth;
                            double scaleY = (double)origImage.Height / (double)imageHeight;

                            thumbnailX = (int)(thumbnailX * scaleX);
                            thumbnailY = (int)(thumbnailY * scaleY);
                            thumbnailWidth = (int)(thumbnailWidth * scaleX);
                            thumbnailHeight = (int)(thumbnailHeight * scaleY);

                            // Prepare crop image
                            cropImage = CropImageToBytes(origImage, thumbnailWidth, thumbnailHeight, thumbnailX, thumbnailY);
                        }

                        // Delete old file
                        File.Delete(filePath);

                        // Save new image
                        if (cropImage != null)
                        {
                            using (MemoryStream ms = new MemoryStream(cropImage, 0, cropImage.Length))
                            {
                                ms.Write(cropImage, 0, cropImage.Length);
                                using (System.Drawing.Image croppedImage = System.Drawing.Image.FromStream(ms, true))
                                    croppedImage.Save(filePath, croppedImage.RawFormat);
                            }
                        }

                        ImagePath = hfImageTempPath.Value;
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                }
            }
        }

        private byte[] CropImageToBytes(System.Drawing.Image origImage, int width, int height, int x, int y)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    bmp.SetResolution(origImage.HorizontalResolution, origImage.VerticalResolution);

                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        graphic.DrawImage(origImage, new Rectangle(0, 0, width, height),
                          x, y, width, height, GraphicsUnit.Pixel);

                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, origImage.RawFormat);
                        return ms.GetBuffer();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            return null;
        }

        private bool ValidateBeforeResize(out string filePath, out int imageWidth, out int imageHeight,
          out int thumbnailWidth, out int thumbnailHeight, out int thumbnailX, out int thumbnailY)
        {
            filePath = string.Empty;
            imageWidth = 0;
            imageHeight = 0;
            thumbnailWidth = 0;
            thumbnailHeight = 0;
            thumbnailX = 0;
            thumbnailY = 0;

            filePath = hfImageTempPath.Value;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(Server.MapPath(filePath)))
                return false;

            imageWidth = GetIntValueByField(hfImageWidth);
            if (imageWidth <= 0)
                return false;

            imageHeight = GetIntValueByField(hfImageHeight);
            if (imageHeight <= 0)
                return false;

            thumbnailWidth = GetIntValueByField(hfThumbnailWidth);
            if (thumbnailWidth <= 0)
                return false;

            thumbnailHeight = GetIntValueByField(hfThumbnailHeight);
            if (thumbnailHeight <= 0)
                return false;

            thumbnailX = GetIntValueByField(hfThumbnailX);
            if (thumbnailX < 0)
                return false;

            thumbnailY = GetIntValueByField(hfThumbnailY);
            if (thumbnailY < 0)
                return false;

            return true;
        }

        private int GetIntValueByField(HiddenField hf)
        {
            if (!string.IsNullOrEmpty(hf.Value))
                return (int)Convert.ToDouble(hf.Value, CultureInfo.InvariantCulture);

            return -1;
        }
    }
}