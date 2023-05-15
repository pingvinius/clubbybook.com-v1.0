namespace ClubbyBook.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class ImageUploaderService : WebService
    {
        private static string[] validExtensions = new string[] { ".jpg", ".jpeg", ".png", ".bmp" };
        private static int fileMaxSize = 1024 * 1024 * 1; // 1 Mb

        [WebMethod]
        [ScriptMethod]
        public void Upload()
        {
            if (!UserManagement.IsAuthenticated)
                throw new UnauthorizedAccessException();

            string jSONResponse = string.Empty;

            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile file = files.Count > 0 ? files[files.Count - 1] : null;

            ImageUploadResultType result = ValidateImageFile(file);
            if (result == ImageUploadResultType.OK)
            {
                string tempFileExtension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(tempFileExtension))
                    tempFileExtension = ".jpg";
                string tempFileName = string.Format("{0}_{1}{2}", UserManagement.CurrentUser.Id,
                  file.FileName.GetHashCode().ToString("x"), tempFileExtension);
                string tempFilePath = Path.Combine(Settings.ImagesTempPath, tempFileName);

                Image origImage = Image.FromStream(file.InputStream);

                file.SaveAs(Server.MapPath(tempFilePath));

                jSONResponse = GetServiceResponse(result, origImage.Width, origImage.Height, VirtualPathUtility.ToAbsolute(tempFilePath));
            }
            else
                jSONResponse = GetServiceResponse(result);

            HttpContext.Current.Response.Write(jSONResponse);
        }

        private ImageUploadResultType ValidateImageFile(HttpPostedFile file)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName) || file.InputStream == null || file.InputStream.Length == 0)
                return ImageUploadResultType.NoFile;

            if (Array.IndexOf<string>(validExtensions, Path.GetExtension(file.FileName).ToLower()) == -1)
                return ImageUploadResultType.InvalidExtension;

            if (file.ContentLength >= fileMaxSize)
                return ImageUploadResultType.InvalidSize;

            return ImageUploadResultType.OK;
        }

        private string GetServiceResponse(ImageUploadResultType result)
        {
            if (result == ImageUploadResultType.OK)
                throw new InvalidOperationException("The \"result\" variable should be not OK.");

            return GetServiceResponse(result, 0, 0, string.Empty);
        }

        private string GetServiceResponse(ImageUploadResultType result, int width, int height, string tempFilePath)
        {
            List<JSONKeyValuePair> resultPairs = new List<JSONKeyValuePair>();

            resultPairs.Add(new JSONKeyValuePair("result", (int)result));

            if (result == ImageUploadResultType.OK)
            {
                resultPairs.Add(new JSONKeyValuePair("width", width));
                resultPairs.Add(new JSONKeyValuePair("height", height));
                resultPairs.Add(new JSONKeyValuePair("filePath", tempFilePath));
            }

            return JSONHelper.FromArray(resultPairs.ToArray());
        }
    }
}