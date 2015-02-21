using ExtensionHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageHelper
{
    public class ImageResizer
    {

        #region Private members

        private int _maxWidth;
        private int _maxHeight;


        #endregion

        #region Constructors

        public ImageResizer(int maxWidth, int maxHeight)
        {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        #endregion

        #region Public members

        public static string GenerateFileName(string fileName, string path)
        {
            bool fileExists;
            int i = 0;
            string name = TransliterationHelper.Front(System.IO.Path.GetFileNameWithoutExtension(fileName));//Транслитированное имя файла
            string extn = System.IO.Path.GetExtension(fileName); // Разрешение


            if (name.Length > 30)
                name = name.Remove(30);
            string result = path + "\\" + name + extn;
            fileExists = System.IO.File.Exists(result);

            while (fileExists)
            {
                i++;
                result = path + "\\" + name + i.ToString("X") + extn;
                fileExists = System.IO.File.Exists(result);
            }

            return result;
        }

        public int MaxWidth
        {
            get { return _maxWidth; }
        }

        public int MaxHeight
        {
            get { return _maxHeight; }
        }

        public Stream Resize(HttpPostedFileBase SourceFilePath, string DestFilePath, System.Drawing.Drawing2D.InterpolationMode imode, bool ForceJPEG = false)
        {
            return Resize(SourceFilePath.InputStream, DestFilePath, imode, ForceJPEG);
        }

        public Stream Resize(Stream SourceFilePath, string DestFilePath, System.Drawing.Drawing2D.InterpolationMode imode, bool ForceJPEG = false)
        {
            Image image = Image.FromStream(SourceFilePath, true, true);

            if (image == null)
                return null;

            ImageFormat format = image.RawFormat;
            int resultHeight = 0;
            int resultWidth = 0;
            if (image.Width > MaxWidth || image.Height > MaxHeight)
            {
                double widthPercent = _computePercent(MaxWidth, image.Width);
                double heightPercent = _computePercent(MaxHeight, image.Height);



                if (heightPercent > widthPercent)
                {
                    double resultPercent = 100 / heightPercent * 100;
                    resultWidth = Convert.ToInt32(image.Width / 100.0 * resultPercent);
                    resultHeight = MaxHeight;
                }
                else
                {
                    double resultPercent = 100 / widthPercent * 100;
                    resultHeight = Convert.ToInt32(image.Height / 100.0 * resultPercent);
                    resultWidth = MaxWidth;
                }
            }
            else
            {
                resultWidth = image.Width;
                resultHeight = image.Height;

            }
            Bitmap result;
            result = new Bitmap(resultWidth, resultHeight);
            result.MakeTransparent();
            Graphics graphics = Graphics.FromImage(result);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.FillRegion(Brushes.White, graphics.Clip);
            graphics.DrawImage(image, 0, 0, resultWidth, resultHeight);
            graphics.Flush();
            image.Dispose();


            System.IO.FileStream stream = new System.IO.FileStream(DestFilePath, System.IO.FileMode.Create);
            if (format.Guid == ImageFormat.Jpeg.Guid || ForceJPEG)
            {
                // Create parameters
                EncoderParameters paramets = new EncoderParameters(1);

                // Set quality (50)
                paramets.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);

                // Create encoder info
                ImageCodecInfo codec = null;
                foreach (ImageCodecInfo codectemp in ImageCodecInfo.GetImageDecoders())
                    if (codectemp.MimeType == "image/jpeg")
                        codec = codectemp;

                // Check
                if (codec == null)
                    throw new Exception("Codec not found for image/jpeg");

                result.Save(stream, codec, paramets);
            }
            else result.Save(stream, format);
            stream.Close();

            MemoryStream memoryStream = new MemoryStream();
            result.Save(memoryStream, format);
            return memoryStream;
        }

        #endregion

        #region Private methods

        private double _computePercent(int maxLength, int existLength)
        {
            return existLength / Convert.ToDouble(maxLength) * 100;
        }

        #endregion
    }
}
