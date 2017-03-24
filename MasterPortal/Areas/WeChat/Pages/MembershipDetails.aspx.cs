using System;
using System.Collections.Generic;
using Adxstudio.Xrm.Web.UI.CrmEntityListView;
using Site.Pages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Query;
using Site.Areas.EntityList.Helpers;
using Microsoft.Xrm.Client.Diagnostics;
using Adxstudio.Xrm.Web.UI.JsonConfiguration;
using Adxstudio.Xrm.Web.UI;
using System.Linq;
using System.Web.Script.Serialization;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode;
using System.Drawing;
using System.Text;


namespace Site.Areas.WeChat.Pages
{
    public partial class MembershipDetails : PortalPage
    {
        OrganizationService service;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectToLoginIfAnonymous();
            RedirectToLoginIfNecessary();

            service = new OrganizationService("Xrm");
            Guid id;
            if (Guid.TryParse(Request.QueryString["id"], out id))
            {
                XrmContext.CreateQuery("adx_membership").FirstOrDefault(c => c.GetAttributeValue<Guid>("adx_membershipid") == id);
            }

            Guid redeemGuid = id;
            QueryExpression qe = new QueryExpression("adx_membership");
            qe.ColumnSet.AllColumns = true;
            EntityCollection ec = service.RetrieveMultiple(qe);
            if (ec.Entities.Count > 0)
            {
                // img_qrcode
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                string encoding = ec.Entities[0]["adx_membershipid"].ToString();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 3;
                qrCodeEncoder.QRCodeVersion = 7;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

                var bw = new ZXing.BarcodeWriter();
                var encOptions = new ZXing.Common.EncodingOptions() { Width = 150, Height = 150, Margin = 0 };
                bw.Options = encOptions;
                bw.Format = ZXing.BarcodeFormat.QR_CODE;
                var result = new Bitmap(bw.Write(encoding));
                MemoryStream _stream = new MemoryStream();
                result.Save(_stream, ImageFormat.Jpeg);
                result.Dispose();

                ImageConverter converter = new ImageConverter();
                byte[] qrByte = (byte[])converter.ConvertTo(new Bitmap(_stream), typeof(byte[]));
                string base64String = Convert.ToBase64String(qrByte, 0, qrByte.Length);
                membershipQrImage.ImageUrl = "data:image/jpeg;base64," + base64String;
            }
        }

    }
}