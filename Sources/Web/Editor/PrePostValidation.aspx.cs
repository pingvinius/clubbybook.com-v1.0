namespace ClubbyBook.Web.Editor
{
    using System;
    using System.Text;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;

    public partial class PrePostValidation : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Валидация текста перед постом", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbPreparedText.Text = string.Empty;
                tbPrePostText.Text = string.Empty;
            }
        }

        protected void btnTransform_Click(object sender, EventArgs e)
        {
            string text = tbPrePostText.Text.Trim();

            while (text.IndexOf("  ") != -1)
                text = text.Replace("  ", " ");

            while (text.IndexOf('…') != -1)
                text = text.Replace("…", "...");

            while (text.IndexOf('–') != -1)
                text = text.Replace("–", "-");

            while (text.IndexOf('—') != -1)
                text = text.Replace("—", "-");

            while (text.IndexOf('”') != -1)
                text = text.Replace("”", "\"");

            while (text.IndexOf('“') != -1)
                text = text.Replace("“", "\"");

            while (text.IndexOf('«') != -1)
                text = text.Replace("«", "\"");

            while (text.IndexOf('»') != -1)
                text = text.Replace("»", "\"");

            const int russianStart = 0x0400;
            const int russianEnd = 0x04FF;

            StringBuilder sb = new StringBuilder();
            foreach (char ch in text)
                if ((int)ch <= 255 || ((int)ch >= russianStart && (int)ch <= russianEnd))
                    sb.Append(ch);

            lbPreparedText.Text = UIUtilities.PrepareTextContent(sb.ToString());
        }
    }
}