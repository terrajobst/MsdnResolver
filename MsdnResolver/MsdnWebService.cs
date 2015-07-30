using System;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MsdnResolver
{
    // NOTE: Types need to be public because otherwise calling the web service will fail.

    [WebServiceBinding(Name = "ContentServiceBinding", Namespace = "urn:msdn-com:public-content-syndication")]
    public sealed class MsdnWebService : SoapHttpClientProtocol
    {
        public MsdnWebService(string appId)
        {
            AppId = new AppId
            {
                Value = appId
            };
            SoapVersion = SoapProtocolVersion.Soap11;
            Url = "http://services.msdn.microsoft.com/ContentServices/ContentService.asmx";
        }

        [XmlElement("appIdValue")]
        public AppId AppId { get; set; }

        [SoapHeader("AppId")]
        [SoapDocumentMethod("urn:msdn-com:public-content-syndication/GetContent", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlElement("getContentResponse", Namespace = "urn:msdn-com:public-content-syndication")]
        public GetContentResponse GetContent([XmlElement(Namespace = "urn:msdn-com:public-content-syndication")] GetContentRequest getContentRequest)
        {
            var results = Invoke("GetContent", new object[] { getContentRequest });
            return (GetContentResponse)(results[0]);
        }
    }

    [Serializable]
    [XmlType(Namespace = "urn:msdn-com:public-content-syndication/2006/09/common")]
    [XmlRoot(Namespace = "urn:msdn-com:public-content-syndication/2006/09/common", IsNullable = false)]
    public sealed class AppId : SoapHeader
    {
        [XmlElement("value")]
        public string Value { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:msdn-com:public-content-syndication")]
    public sealed class GetContentRequest
    {
        [XmlElement("contentIdentifier")]
        public string ContentIdentifier { get; set; }

        [XmlElement("locale", Namespace = "urn:mtpg-com:mtps/2004/1/key")]
        public string Locale { get; set; }

        [XmlArray("requestedDocuments")]
        [XmlArrayItem("requestedDocument", IsNullable = false)]
        public RequestedDocument[] RequestedDocuments { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:msdn-com:public-content-syndication")]
    public sealed class GetContentResponse
    {
        [XmlElement("contentId", Namespace = "urn:mtpg-com:mtps/2004/1/key")]
        public string ContentId { get; set; }

        [XmlArray("primaryDocuments")]
        [XmlArrayItem("primary", Namespace = "urn:mtpg-com:mtps/2004/1/primary", IsNullable = false)]
        public Primary[] PrimaryDocuments { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "urn:msdn-com:public-content-syndication")]
    public class RequestedDocument
    {
        [XmlAttribute("selector")]
        public string Selector { get; set; }

        [XmlAttribute("type")]
        public DocumentTypes Type { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = "urn:msdn-com:public-content-syndication")]
    public enum DocumentTypes
    {
        primary,
        common,
        image,
        feature,
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:mtpg-com:mtps/2004/1/primary")]
    public class Primary
    {
        [XmlAnyElement]
        public XmlElement Any { get; set; }

        [XmlAttribute("primaryFormat", Form = XmlSchemaForm.Qualified, Namespace = "urn:mtpg-com:mtps/2004/1/primary/category")]
        public string PrimaryFormat { get; set; }

        [XmlAttribute("location")]
        public string Location { get; set; }
    }
}