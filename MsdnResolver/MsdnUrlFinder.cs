using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Xml;

namespace MsdnResolver
{
    internal sealed class MsdnUrlFinder
    {
        private const string UrlFormat = "http://msdn.microsoft.com/{0}/library/{1}";
        private const string Locale = "en-us";

        private readonly MsdnWebService _msdnWebService = new MsdnWebService("Microsoft.Fx.Msdn");

        public Task<string> GetUrlAsync(string documentationId)
        {
            return Task.Run(() =>
            {
                var msdnRequest = new GetContentRequest
                {
                    ContentIdentifier = "AssetId:" + documentationId,
                    Locale = Locale
                };

                try
                {
                    var response = _msdnWebService.GetContent(msdnRequest);
                    var contentId = response.ContentId;
                    var msdnUrl = string.Format(UrlFormat, Locale, contentId);
                    return msdnUrl;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (SoapException)
                {
                    return null;
                }
            });
        }

        public Task<string> GetSummaryAsync(string documentationId)
        {
            return Task.Run(() =>
            {
                var msdnRequest = new GetContentRequest
                {
                    ContentIdentifier = "AssetId:" + documentationId,
                    Locale = Locale,
                    RequestedDocuments = new[]
                    {
                        new RequestedDocument
                        {
                            Type = DocumentTypes.primary,
                            Selector = "Mtps.Xhtml",
                        }
                    }
                };

                try
                {
                    var response = _msdnWebService.GetContent(msdnRequest);
                    var summary = GetSummary(response);
                    return summary;
                }
                catch (WebException)
                {
                    return null;
                }
                catch (SoapException)
                {
                    return null;
                }
            });
        }

        private static string GetSummary(GetContentResponse response)
        {
            var documents = response.PrimaryDocuments;
            if (documents == null)
                return null;

            var primary = documents.FirstOrDefault();
            if (primary == null || primary.Any == null)
                return null;

            var nameTable = new NameTable();
            var manager = new XmlNamespaceManager(nameTable);
            manager.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
            var nodeList = primary.Any.SelectNodes("//xhtml:div[@class='summary']", manager);
            if (nodeList == null)
                return null;

            var node = nodeList.OfType<XmlElement>().FirstOrDefault();
            if (node == null)
                return null;

            // Remove all non C# markup

            var nonCSharpNodes = node.SelectNodes(".//xhtml:variation[@devLang!='cs']", manager);
            if (nonCSharpNodes != null)
            {
                var xmlElements = nonCSharpNodes.Cast<XmlElement>().ToArray();
                foreach (var element in xmlElements)
                    element.ParentNode.RemoveChild(element);
            }

            return node.InnerText;
        }
    }
}