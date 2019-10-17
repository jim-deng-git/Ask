using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc {
    public static class HtmlHelperExt {
        public static HtmlString Captcha(this HtmlHelper helper, string name) {
            Controller ctrl = (Controller)helper.ViewContext.Controller;
            Random rand = new Random();            
            string imageUrl = ctrl.Url.Action("CaptchaImage", "Common");
            string refreshUrl = ctrl.Url.Action("CaptchaCode", "Common");

            WorkV3.Common.Captcha captcha = WorkV3.Common.Captcha.Create();                       
            string html =
                $"<div id=\"div{ name }\">" +
                $"  <div class=\"captcha\">" +
                $"      <a href=\"javascript:\" class=\"login-code-img\">" +
                $"          <img src=\"{ imageUrl }?rand={ rand.Next() }\" alt=\"不清楚，换一張\" />" +
                $"          <i class=\"cc cc-reload2\"></i>" +
                $"      </a>" +
                $"  </div>" + 
                $"  <div class=\"input-field\">" +
                $"      <input type=\"text\" id=\"{ name }\" name=\"{ name }.Value\" placeholder=\"驗證碼\" class=\"text validate[required]\" />" +
                $"  </div>" +
                $"  <input type=\"hidden\" name=\"{ name }.Code\" value=\"{ captcha.Code }\" />" +               
                $"</div>" +                
                $"<script type=\"text/javascript\">" +
                $"  (function() {{" +
                $"      var outerElm = $('#div{ name }');" +
                $"      outerElm.on('click', 'a', function() {{" +
                $"          var rand = Math.random();" +
                $"          $.get('{ refreshUrl }?rand=' + rand, function(code) {{" +
                $"              outerElm.find('input[type=\"hidden\"]').val(code);" +
                $"              outerElm.find('img').prop('src', '{ imageUrl }?rand=' + rand);" +
                $"          }});" +
                $"      }});" +
                $"  }})();" +
                $"</script>";

            return new HtmlString(html);
        }

        public static HtmlString CaptchaOnBar(this HtmlHelper helper, string name) {
            Controller ctrl = (Controller)helper.ViewContext.Controller;
            Random rand = new Random();
            string imageUrl = ctrl.Url.Action("CaptchaImage", "Common");
            string refreshUrl = ctrl.Url.Action("CaptchaCode", "Common");

            WorkV3.Common.Captcha captcha = WorkV3.Common.Captcha.Create();
            string html =
                $"<span id=\"span{ name }\">" +
                $"  <input type=\"\" id=\"{ name }\" name=\"{ name }.Value\" placeholder=\"驗證碼\" class=\"text validate[required]\" />" +                
                $"  <a href=\"javascript:\" class=\"login-code-img\">" +
                $"      <img src=\"{ imageUrl }?rand={ rand.Next() }\" alt=\"不清楚，换一張\" />" +
                $"  </a>" +
                $"  <input type=\"hidden\" name=\"{ name }.Code\" value=\"{ captcha.Code }\" />" +
                $"</span>" +                
                $"<script type=\"text/javascript\">" +
                $"  (function() {{" +
                $"      var outerElm = $('#span{ name }');" +
                $"      outerElm.on('click', 'a', function() {{" +
                $"          var rand = Math.random();" +
                $"          $.get('{ refreshUrl }?rand=' + rand, function(code) {{" +
                $"              outerElm.find('input[type=\"hidden\"]').val(code);" +
                $"              outerElm.find('img').prop('src', '{ imageUrl }?rand=' + rand);" +
                $"          }});" +
                $"      }});" +
                $"  }})();" +
                $"</script>";

            return new HtmlString(html);
        }
    }
}