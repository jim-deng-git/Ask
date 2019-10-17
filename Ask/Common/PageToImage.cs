using System.Threading;

namespace WorkV3.Common
{
    public class PageToImage
    {
        private System.Drawing.Bitmap bitmap;        
        private string url;
        private int width;
        private int height;

        public PageToImage(string url) {
            this.width = 1280;
            this.url = url;            
        }

        private void setBitmap() {
            using (System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser()) {
                wb.ScrollBarsEnabled = false;
                wb.Width = width;
                wb.Navigate(url);
                //确保页面被解析完全                
                while (wb.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete) {
                    System.Windows.Forms.Application.DoEvents();                    
                    System.Threading.Thread.Sleep(100);
                }
                System.Threading.Thread.Sleep(1000);

                height = wb.Document.Body.ScrollRectangle.Height + 50;
                wb.Height = height;

                bitmap = new System.Drawing.Bitmap(width, height);
                wb.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, width, height));
                wb.Dispose();
            }
        }

        public void SaveImage(string fileName, System.Drawing.Imaging.ImageFormat format = null) {
            Thread thread = new Thread(new ThreadStart(setBitmap));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            while (thread.IsAlive)
               Thread.Sleep(1000);

            if (format == null)
                format = System.Drawing.Imaging.ImageFormat.Jpeg;
            bitmap.Save(fileName, format);
            bitmap.Dispose();
        }
    }    
}