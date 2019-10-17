using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3.Services.Interface
{
    interface IImageService
    {
        void GetImageWidthAndHeight(string source, out int width, out int height);
    }
}
