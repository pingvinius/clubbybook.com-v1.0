namespace ClubbyBook.Common
{
    using System;

    public class UrlRewriteAttribute : Attribute
    {
        public string UrlRewrite { get; set; }

        public UrlRewriteAttribute(string urlRewrite)
        {
            this.UrlRewrite = urlRewrite;
        }
    }
}