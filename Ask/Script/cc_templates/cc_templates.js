var path = getRootPath();
CKEDITOR.addTemplates(
'default',
{
    //圖片資料夾路徑，放在同目錄的images資料夾內
    imagesPath: CKEDITOR.getUrl(path+'/Script/cc_templates/images/'),
    templates: [
        {
            //標題
            title: '兩欄式',
            // image: 'images_01.jpg',//圖片
            // description: '', //樣板描述
            //自訂樣板內容
            html: '<div class="col-2">' +
                    '<ul class="unstyled">' +
                        '<li>鄉村樂園零錢包</li>' +
                        '<li>尺寸：約12cm X 9cm</li>' +
                        '<li>' +
                            '內附：' +
                            '<ul class="unstyled">' +
                                '<li>意大利製植鞣牛皮<span class="tilde">..........</span>一片</li>' +
                                '<li>裁好有孔皮革裁片<span class="tilde">..........</span>一片</li>' +
                            '</ul> '+
                        '</li>' +
                    '</ul>' +
                '</div>' +
                '<div class="col-2">' +
                    '<ul class="unstyled">' +
                        '<li>拉鍊<span class="tilde">..........</span>一根</li>' +
                        '<li>木框<span class="tilde">..........</span>一個</li>' +
                        '<li>標示筆<span class="tilde">..........</span>一隻</li>' +
                        '<li>布袋及心意咭<span class="tilde">..........</span>一組</li>' +
                    '</ul>' +
                '</div>'
        },
        //第二個樣板
        {
            //標題
            title: '引言樣式1',
            // image: 'images_02.png',//圖片
            // description: '', //樣板描述
            //自訂樣板內容
            html: '<div class="blockquote style1">' +
                    '<p>採訪當天我們的車拐進一個小小的巷子裡，很容易就發現葉老師的工作室，透明玻璃門裡面些微雜亂的空間，老師工作的身影就直接映入我們眼前。 葉明福老師 — 因為熱愛工藝，在高中時一股腦栽進了工藝世界，憑藉著對皮革的熱愛，靠著一口破日語到日本學成歸國，總是掛著笑容的臉與被時間擠壓的背影是他最大的特徵。</p>' +
                '</div>'
        },
        {
            //標題
            title: '引言樣式2',
            // image: 'images_02.png',//圖片
            // description: '', //樣板描述
            //自訂樣板內容
            html: '<div class="blockquote style2">' +
                    '<div class="blockquote-title">從工藝科學生到皮革老師</div>' +
                    '<p>身為台灣皮革藝術家出國學習的先驅，葉老師的皮革之路走的與眾不同。高中就讀工藝科的葉老師在家中經濟條件的選擇下，以原料最為便宜的陶藝當作主修，一直到了高工畢業之後在等兵單的那段時間，學姊給了葉老師一個皮革的工作機會，才讓葉老師真正認識到皮革這項工藝，在過程中學姊用以工代訓的方法來教導葉老師。</p>' +
                '</div>'
        },
        {
            //標題
            title: '引言樣式3',
            // image: 'images_02.png',//圖片
            // description: '', //樣板描述
            //自訂樣板內容
            html: '<div class="blockquote style3">' +
                    '<p>採訪當天我們的車拐進一個小小的巷子裡，很容易就發現葉老師的工作室，透明玻璃門裡面些微雜亂的空間，老師工作的身影就直接映入我們眼前。 </p>' +
                    '<p>葉明福老師 — 因為熱愛工藝，在高中時一股腦栽進了工藝世界，憑藉著對皮革的熱愛，靠著一口破日語到日本學成歸國，總是掛著笑容的臉與被時間擠壓的背影是他最大的特徵。</p>' +
                '</div>'
        },
        {
            //標題
            title: '引言樣式4',
            // image: 'images_02.png',//圖片
            // description: '', //樣板描述
            //自訂樣板內容
            html: '<div class="blockquote style4">' +
                    '<p>採訪當天我們的車拐進一個小小的巷子裡，很容易就發現葉老師的工作室，透明玻璃門裡面些微雜亂的空間，老師工作的身影就直接映入我們眼前。 </p>' +
                    '<p>葉明福老師 — 因為熱愛工藝，在高中時一股腦栽進了工藝世界，憑藉著對皮革的熱愛，靠著一口破日語到日本學成歸國，總是掛著笑容的臉與被時間擠壓的背影是他最大的特徵。</p>' +
                '</div>'
        },
        {
            //標題
            title: '引言樣式5',
            // image: 'images_02.png',//圖片
            // description: '', //樣板描述
            //自訂樣板內容
            html: '<div class="blockquote style5">' +
                    '<p>身為台灣皮革藝術家出國學習的先驅，葉老師的皮革之路走的與眾不同。高中就讀工藝科的葉老師在家中經濟條件的選擇下，以原料最為便宜的陶藝當作主修，一直到了高工畢業之後在等兵單的那段時間，學姊給了葉老師一個皮革的工作機會，才讓葉老師真正認識到皮革這項工藝，在過程中學姊用以工代訓的方法來教導葉老師。</p>' +
                '</div>'
        }
    ]
});

function getRootPath() {
    var hostName = window.location.host
    var strFullPath = window.document.location.href;
    var strPath = window.document.location.pathname;
    var pos = strFullPath.indexOf(strPath);
    var prePath = strFullPath.substring(0, pos);
    var postPath = strPath.substring(0, strPath.substr(1).indexOf('/') + 1);

    return postPath.replace("/Backend", "");
}