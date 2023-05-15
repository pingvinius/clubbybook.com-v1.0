namespace ClubbyBook.Web.SiteMap
{
    using System;
    using System.Text;
    using System.Web;
    using System.Xml;
    using ClubbyBook.Common.Logging;

    public static class SiteMapGenerator
    {
        public const string SiteMapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public const string XmlElementSiteMap = "urlset";

        public const string XmlElementSiteMapNode = "url";
        public const string XmlElementSiteMapNodeLoc = "loc";
        public const string XmlElementSiteMapNodeLastmod = "lastmod";
        public const string XmlElementSiteMapNodeChangefreq = "changefreq";
        public const string XmlElementSiteMapNodePriority = "priority";

        private static XmlWriterSettings XmlFileSettings = new XmlWriterSettings()
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = false
        };

        public static void SaveSiteMapToXml(string filePath)
        {
            SaveSiteMapToXml(SiteMap.Provider, filePath);
        }

        public static void SaveSiteMapToXml(SiteMapProvider siteMapPrivider, string filePath)
        {
            if (siteMapPrivider == null)
                throw new ArgumentNullException("siteMapPrivider");

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            // Remove site map from cache, so provider should recreate site map again
            var clubbyBookSiteMapProvider = siteMapPrivider as ClubbyBookSiteMapProvider;
            if (clubbyBookSiteMapProvider != null)
                clubbyBookSiteMapProvider.ResetSiteMap();

            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                XmlElement rootElement = xmlDocument.CreateElement(XmlElementSiteMap, SiteMapNamespace);
                foreach (SiteMapNode node in siteMapPrivider.RootNode.GetAllNodes())
                    rootElement.AppendChild(CreateXmlElement(xmlDocument, node));

                xmlDocument.AppendChild(rootElement);

                using (XmlWriter xmlWriter = XmlWriter.Create(filePath, XmlFileSettings))
                    xmlDocument.Save(xmlWriter);
            }
            catch (Exception ex)
            {
                Logger.Write("The error has been occupied while creating SiteMap xml file.", ex);
            }
        }

        private static XmlElement CreateXmlElement(XmlDocument xmlDocument, SiteMapNode node)
        {
            if (xmlDocument == null)
                throw new ArgumentNullException("xmlDocument");

            if (node == null)
                throw new ArgumentNullException("node");

            XmlElement itemElement = xmlDocument.CreateElement(XmlElementSiteMapNode, SiteMapNamespace);
            XmlElement tmpElement = null;

            tmpElement = xmlDocument.CreateElement(XmlElementSiteMapNodeLoc, SiteMapNamespace);
            tmpElement.InnerText = SiteMapHelper.ValidateUrl(node.Url);
            itemElement.AppendChild(tmpElement);

            if (!string.IsNullOrEmpty(node[SiteMapHelper.LastModifiedDateKey]))
            {
                tmpElement = xmlDocument.CreateElement(XmlElementSiteMapNodeLastmod, SiteMapNamespace);
                tmpElement.InnerText = node[SiteMapHelper.LastModifiedDateKey];
                itemElement.AppendChild(tmpElement);
            }

            if (!string.IsNullOrEmpty(node[SiteMapHelper.ChangeFrequentlyKey]))
            {
                tmpElement = xmlDocument.CreateElement(XmlElementSiteMapNodeChangefreq, SiteMapNamespace);
                tmpElement.InnerText = node[SiteMapHelper.ChangeFrequentlyKey];
                itemElement.AppendChild(tmpElement);
            }

            if (!string.IsNullOrEmpty(node[SiteMapHelper.PriorityKey]))
            {
                tmpElement = xmlDocument.CreateElement(XmlElementSiteMapNodePriority, SiteMapNamespace);
                tmpElement.InnerText = node[SiteMapHelper.PriorityKey];
                itemElement.AppendChild(tmpElement);
            }

            return itemElement;
        }
    }
}