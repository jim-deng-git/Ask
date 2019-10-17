using System.Web;
using System.Web.Optimization;

namespace WorkV3
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Jquery
            bundles.Add(new ScriptBundle("~/JS/jquery").Include(
                        "~/Script/Jquery/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Script/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Script/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Script/bootstrap.js",
            //          "~/Script/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
            #region AjaxCore

            bundles.Add(new ScriptBundle("~/JS/Core").Include(
                                   "~/Script/base/Core.js"));
            #endregion

            #region 全域性的CSS (後端)
            bundles.Add(new StyleBundle("~/Css/BackCss").Include(
                      "~/css/normalize.css",
                      "~/css/ccwork.css",
                      "~/css/background.css",
                      "~/css/button.css",
                      "~/css/font-color.css",
                      "~/css/spacing.css"
                      ));
            #endregion

            #region 後端專屬
            bundles.Add(new StyleBundle("~/Css/BackCssLogin").Include(
                      "~/css/login-backend.css"));
            #endregion

            #region 全域性的CSS (前端)
            bundles.Add(new StyleBundle("~/Css/FrontCss").Include(
                      "~/css/normalize.css",
                      "~/css/ccwork.css",                      
                      "~/css/background.css",
                      "~/css/button.css",
                      "~/css/font-color.css",
                      "~/css/spacing.css"
                      ));
            #endregion

            #region 前端專屬

            #endregion
            
            #region Menu
            bundles.Add(new StyleBundle("~/Css/mmenu").Include(
              "~/css/vendor/mmenu/mmenu.css"));
            #endregion

            #region  tooltipster

            bundles.Add(new StyleBundle("~/Css/tooltipster").Include(
                "~/css/vendor/tooltipster/tooltipster.css"));

            bundles.Add(new ScriptBundle("~/JS/tooltipster").Include(
       "~/Script/tooltipster/tooltipster.bundle.js"));
            #endregion

            #region sweetalert 警告視窗

            bundles.Add(new StyleBundle("~/Css/sweetalert").Include(
             "~/css/vendor/sweetalert/sweetalert.css"));

            bundles.Add(new ScriptBundle("~/JS/sweetalert").Include(
       "~/Script/base/sweetalert2.js"));
            #endregion

            #region 驗證 ~/JS/validate
            //   bundles.Add(new ScriptBundle("~/JS/validate").Include(
            //"~/Script/validation/jquery.validationEngine.js",
            //"~/Script/validation/jquery.validationEngine-cn.js"));

            bundles.Add(new ScriptBundle("~/JS/validate").Include(
               "~/Script/validation/jquery.validationEngine.js",
               "~/Script/validation/jquery.validationEngine-cn.js"));
            //bundles.Add(new StyleBundle("~/Css/validate").Include(
            //            "~/Css/vendor/validation/validationEngine.jquery.css",
            //            "~/Css/vendor/validation/template.css"));
            #endregion

            #region jquery-ui

            bundles.Add(new StyleBundle("~/Css/jquery-ui").Include(
                      "~/css/vendor/jquery-ui/jquery-ui.css"));

            bundles.Add(new ScriptBundle("~/JS/jquery-ui").Include(
       "~/Script/jquery-ui/jquery-ui.js",
       "~/Script/jquery-ui/jquery.ui.touch-punch.js"));
            #endregion

            #region modernizr

            bundles.Add(new ScriptBundle("~/JS/modernizr").Include(
       "~/Script/base/modernizr.custom.js"));
            #endregion

            #region colorbox

            bundles.Add(new StyleBundle("~/Css/colorbox").Include(
                   "~/css/vendor/colorbox/colorbox.css"));

            bundles.Add(new ScriptBundle("~/JS/colorbox").Include(
       "~/Script/colorbox/jquery.colorbox.js"));
            #endregion

            #region  jquery-easing

            bundles.Add(new ScriptBundle("~/JS/jquery.easing").Include(
       "~/Script/base/jquery.easing.{version}.js"));
            #endregion

            #region jquery-mousewheel
            bundles.Add(new ScriptBundle("~/JS/jquery-mousewheel").Include(
   "~/Script/base/jquery.mousewheel.js"));
            #endregion

            #region classie
            bundles.Add(new ScriptBundle("~/JS/classie").Include(
   "~/Script/base/classie.js"));
            #endregion

            #region mmenu
            bundles.Add(new ScriptBundle("~/JS/mmenu").Include(
   "~/Script/base/jquery.mmenu.all.js",
   "~/Script/base/jquery.mmenu.lazysubmenus.js"));
            #endregion

            #region nestable
            bundles.Add(new ScriptBundle("~/JS/nestable").Include(
  "~/Script/nestable/jquery.nestable.js"));
            #endregion

            #region Materialize
            bundles.Add(new ScriptBundle("~/JS/Materialize").Include(
  "~/Script/base/materialize.initial.js", "~/Script/base/materialize.global.js"));
            #endregion

            #region admin
            bundles.Add(new ScriptBundle("~/JS/admin").Include(
  "~/Script/base/admin.js"));

            bundles.Add(new ScriptBundle("~/JS/admin-iframe").Include(
  "~/Script/base/admin-iframe.js"));

            bundles.Add(new ScriptBundle("~/JS/admin-slideTabs").Include(
"~/Script/base/admin-slideTabs.js"));

            bundles.Add(new StyleBundle("~/Css/admin").Include(
"~/css/admin.css"));

            #endregion

            #region forms

            bundles.Add(new StyleBundle("~/Css/forms").Include(
                 "~/css/forms.css", "~/css/dropdown.css"));

            bundles.Add(new ScriptBundle("~/JS/forms").Include(
 "~/Script/forms/forms.js", "~/Script/forms/dropdown.js", "~/Script/forms/flexdatalist.js", "~/Script/forms/moment-with-locales.js", "~/Script/forms/datetimepicker.js", "~/Script/forms/moment-with-locales.js"));

            #endregion

            #region base

            bundles.Add(new ScriptBundle("~/JS/base").Include("~/Script/base/velocity.min.js",
 "~/Script/base/buttons.js"));
            #endregion

            #region swiper
            bundles.Add(new ScriptBundle("~/JS/swiper").Include(
"~/Script/swiper/swiper.jquery.js"));

            bundles.Add(new StyleBundle("~/Css/swiper").Include(
               "~/css/vendor/swiper/swiper.css"));
            #endregion

            #region fileuploader
            bundles.Add(new ScriptBundle("~/JS/fileuploader").Include(
"~/Script/fileuploader/jquery.fileuploader.js"));
            #endregion

            #region flexslider
            bundles.Add(new StyleBundle("~/Css/flexslider").Include("~/css/vendor/flexslider/flexslider.css"));
            bundles.Add(new ScriptBundle("~/JS/flexslider").Include(
"~/Script/flexslider/jquery.flexslider.js"));
            #endregion

            #region Cards
            bundles.Add(new StyleBundle("~/Css/Card").Include(
              "~/css/card.css"));
            #endregion

            #region  StickyTableHeaders

            bundles.Add(new ScriptBundle("~/JS/StickyTableHeaders").Include(
"~/Script/StickyTableHeaders/jquery.stickytableheaders.js"));

            #endregion

            #region ckeditor
            bundles.Add(new ScriptBundle("~/JS/ckeditor").Include(
"~/Script/ckeditor/ckeditor.js", "~/Script/ckeditor/ckeditor.custom.js"));
            #endregion

            #region slide-pips
            bundles.Add(new StyleBundle("~/Css/slidePips").Include("~/css/vendor/slider-pips/slider-pips.css"));
            bundles.Add(new ScriptBundle("~/JS/slidePips").Include("~/script/slider-pips/jquery-ui-slider-pips.js"));
            #endregion

            #region jwplayer
            bundles.Add(new ScriptBundle("~/JS/jwplayer").Include("~/Script/jwplayer/jwplayer.js"));
            #endregion

            #region tagEditor
            bundles.Add(new StyleBundle("~/Css/tagEditor").Include("~/Css/vendor/tagEditor/tagEditor.css"));

            bundles.Add(new ScriptBundle("~/JS/tagEditor").Include(
                "~/Script/tagEditor/jquery.caret.min.js",
                "~/Script/tagEditor/jquery.tag-editor.js",
                "~/Script/tagEditor/jquery.ui.autocomplete.html.js"
            ));
            #endregion

            #region JS組件的封裝
            //bundles.Add(new ScriptBundle("~/JS/component").Include("~/Script/workV3/component.js"));
            bundles.Add(new ScriptBundle("~/JS/component").Include("~/Script/workV3/component.js",
                "~/Script/jquery-cookie/jquery.cookie.js"));
            #endregion

            #region 簡繁轉換
            bundles.Add(new ScriptBundle("~/JS/TrnTCSC").Include(
"~/Script/Base/chinese_convert.js"));
            #endregion

            #region 廣告
            bundles.Add(new ScriptBundle("~/JS/Advertising").Include("~/Script/Advertising/advertising.js"));
            #endregion  

            #region HighCharts
            bundles.Add(new StyleBundle("~/Css/HighCharts").Include("~/css/vendor/highcharts/highcharts.css"));
            bundles.Add(new ScriptBundle("~/JS/HighCharts").Include("~/Script/highcharts/highcharts.js", "~/Script/highcharts/highcharts-3d.js", "~/Script/highcharts/highcharts-more.js"));
            #endregion
            #region autocomplete(Flexdatalist)
            bundles.Add(new ScriptBundle("~/JS/Flexdatalist").Include("~/Script/flexdatalist/jquery.flexdatalist.js"));
            #endregion  
            //壓縮最佳化
            BundleTable.EnableOptimizations = WebInfo.EnableOptimizations;
        }

        public static IHtmlString Validate() {
            // string html = Styles.Render("~/Css/validate").ToHtmlString() + Scripts.Render("~/JS/validate").ToHtmlString();
            string html =  Scripts.Render("~/JS/validate").ToHtmlString();
            return new HtmlString(html);
        }

        public static IHtmlString FileUploader() {
            string html = Scripts.Render("~/JS/fileuploader").ToHtmlString();
            return new HtmlString(html);
        }

        public static IHtmlString Component() {
            return Scripts.Render("~/JS/component");
        }

        public static IHtmlString SlidePips() {
            string html = Styles.Render("~/Css/slidePips").ToHtmlString() + Scripts.Render("~/JS/slidePips").ToHtmlString();
            return new HtmlString(html);
        }
        public static IHtmlString Flexdatalist()
        {
            string html = Scripts.Render("~/JS/Flexdatalist").ToHtmlString();
            return new HtmlString(html);
        }

        public static IHtmlString StickyTableHeaders() {
            string html = Scripts.Render("~/JS/StickyTableHeaders").ToHtmlString();
            return new HtmlString(html);
        }
        //private static string JWPlayerKey = "JWPlayer";
        public static bool IsLoadJWPlayer = false;
        public static IHtmlString JWPlayer()
        {
            //bool IsLoadJWPlayer = false;

            //if (HttpContext.Current.Session[JWPlayerKey] == null)
            //{
            //    HttpContext.Current.Session[JWPlayerKey] = HttpContext.Current.Session.SessionID;
            //}
            //else
            //{
            //    IsLoadJWPlayer = true;
            //}
            string html = "";
            if (!IsLoadJWPlayer)
            {
                html =
                    Scripts.Render("~/JS/jwplayer").ToHtmlString() +
                    "<script type=\"text/javascript\">jwplayer.key='" + WorkLib.GetItem.appSet("JWPlayerKey") + "';</script>";
                IsLoadJWPlayer = true;
            }
            return new HtmlString(html);
        }

        public static IHtmlString Flexslider() {
            string html = Styles.Render("~/Css/flexslider").ToHtmlString() + Scripts.Render("~/JS/flexslider").ToHtmlString();
            return new HtmlString(html);
        }

        public static bool IsLoadColorbox = false;
        public static IHtmlString Colorbox()
        {
            string html = "";
            if (!IsLoadColorbox)
            {
                 html = Styles.Render("~/Css/colorbox").ToHtmlString() + Scripts.Render("~/JS/colorbox").ToHtmlString();
                IsLoadColorbox = true;
            }
            return new HtmlString(html);
        }

        public static bool IsLoadSwiper = false;
        public static IHtmlString Swiper()
        {
            string html = "";
            if (!IsLoadSwiper)
            {
                html = Styles.Render("~/Css/swiper").ToHtmlString() + Scripts.Render("~/JS/swiper").ToHtmlString();
                IsLoadSwiper = true;
            }
            return new HtmlString(html);
        }
        public static IHtmlString forms()
        {
            string html = Styles.Render("~/Css/forms").ToHtmlString() + Scripts.Render("~/JS/forms").ToHtmlString();
            return new HtmlString(html);
        }

        public static IHtmlString TagEditor() {
            string html = Styles.Render("~/Css/tagEditor").ToHtmlString() + Scripts.Render("~/JS/tagEditor").ToHtmlString();
            return new HtmlString(html);
        }
        public static IHtmlString SweetAlert() {
            string html = Styles.Render("~/Css/sweetalert").ToHtmlString() + Scripts.Render("~/JS/sweetalert").ToHtmlString();
            return new HtmlString(html);
        }
        public static IHtmlString TooltipSter() {
            string html = Styles.Render("~/Css/tooltipster").ToHtmlString() + Scripts.Render("~/JS/tooltipster").ToHtmlString();
            return new HtmlString(html);
        }
        public static IHtmlString HighCharts()
        {
            string html = Styles.Render("~/Css/HighCharts").ToHtmlString() + Scripts.Render("~/JS/HighCharts").ToHtmlString();
            return new HtmlString(html);
        }
    }
}
