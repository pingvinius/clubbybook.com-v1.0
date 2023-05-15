namespace ClubbyBook.Common.Mail
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public abstract class MailTemplate
    {
        private readonly Dictionary<string, string> parameters;
        private readonly string templateFileName;
        private readonly string template;

        public MailTemplate(string templateFileName)
        {
            this.templateFileName = templateFileName;
            this.parameters = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(templateFileName, Encoding.UTF8))
                template = sr.ReadToEnd();
        }

        public string Body
        {
            get
            {
                int index = Template.IndexOf(Environment.NewLine);
                if (index == -1)
                    throw new InvalidOperationException("Invalid template format.");

                string body = Template.Substring(index + Environment.NewLine.Length);
                return ReplaceParameters(body);
            }
        }

        public string Subject
        {
            get
            {
                int index = Template.IndexOf(Environment.NewLine);
                if (index == -1)
                    throw new InvalidOperationException("Invalid template format.");

                string subject = Template.Substring(0, index);
                return ReplaceParameters(subject);
            }
        }

        public string Template
        {
            get
            {
                return template;
            }
        }

        protected IDictionary<string, string> Parameters
        {
            get
            {
                return parameters;
            }
        }

        private string ReplaceParameters(string text)
        {
            foreach (var pair in Parameters)
            {
                var oldText = text;
                while ((text = oldText.Replace(pair.Key, pair.Value)) != oldText)
                    oldText = text;
            }

            return text;
        }
    }
}