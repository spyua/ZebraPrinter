using System.Globalization;
using System.Text;

/// <summary>
/// ZPL 語言封裝類別
/// </summary>
namespace ZebraUDP
{
    public class ZplCmdBuilder
    {
        private static readonly string START_ZPL = "^XA";
        private static readonly string END_ZPL = "^XZ";

        public static string ORI_NORMAL = "N";                                  //orientation Normal  旋轉參數，正常
        public static string ORI_90 = "R";                                      //orientation 90 degrees 旋轉參數，90度 (順時針)
        public static string ORI_180 = "I";                                     //orientation 180 degrees
        public static string ORI_270 = "B";                                     //orientation 270 degrees

        private readonly StringBuilder _zplCode = new StringBuilder();          //內置 Zpl 碼組合字串
        private readonly CultureInfo _format = new CultureInfo("zh-TW");


        /** 取得目前組合的 zpl 碼 */
        public string GetZplCode() { return _zplCode.ToString(); }
        /** 清空目前組合的 zpl 碼 */
        public ZplCmdBuilder ClrZplCode()
        {
            _zplCode.Clear();
            return this;
        }

        #region 一般通用函式
        /** 取得完整的 zpl 段，自動包含 ZPL 開頭和結尾 */
        public string GetZpl(params string[] strings)
        {
            if (strings != null)
            {
                StringBuilder result = new StringBuilder();
                result.Append(START_ZPL);
                for (int i = 0; i < strings.Length; i++)
                {
                    result.Append(strings[i]);
                }
                result.Append(END_ZPL);
                return result.ToString();
            }
            else
                return null;
        }

        /** ZPL 區段起始標號 (放在 ZPL 語言開頭) */
        public ZplCmdBuilder StartZPL()
        {
            _zplCode.Append(START_ZPL);
            return this;
        }
        /** ZPL 區段終止標號 (放在 ZPL 語言結尾) */
        public ZplCmdBuilder EndZPL()
        {
            _zplCode.Append(END_ZPL);
            return this;
        }
        #endregion

        #region 編碼指定
        /**
        * Select Encoding Table (選擇編碼) - 選擇編碼檔作為目前列印文字的編碼
        * @param deviceAndCodename  編碼檔存放的裝置識別字元 與 編碼檔名，格式 :  E:BIG5.DAT
        * @return
        */
        public ZplCmdBuilder SelectEncoding(string deviceAndCodename)
        {
            _zplCode.Append(string.Format(_format, "^SE{0}", deviceAndCodename));
            return this;
        }
        /**
         * Change International Font/Encoding (改變編碼) - 選擇目前列印文字的編碼
         * @param encodingCode  編碼代號
         * @return
         */
        public ZplCmdBuilder ChgEncoding(int encodingCode)
        {
            _zplCode.Append(string.Format(_format, "^CI{0}", encodingCode));
            return this;
        }



        #endregion

        #region 字型設定
        /**
             * Font Identifier (設定預備字型) - 預先設定會使用到的字型的代號
             * @param fontLetter    字型的代號字串，從 A-Z, 0-9
             * @param deviceAndFontname 字型存放的裝置識別字元 與 字型檔名，格式 :  E:BIG5.TTF
             * @return
             */
        public ZplCmdBuilder FontIdentifier(string fontLetter, string deviceAndFontname)
        {
            _zplCode.Append(string.Format(_format, "^CW{0},{1}", fontLetter, deviceAndFontname));
            return this;
        }

        /**
         * Change Alphanumeric Default Font (指定字母/數字預設字型) - 指定印表機預設字型的大小, 與 ^CW 配合使用
         * @param fontLetter iMZ320 預設有 0-7 共八種字型
         * @param width     電源開啟時預設 5
         * @param height    電源開啟時預設 9
         * @return
         */
        public ZplCmdBuilder ChangeAlphanumericFont(string fontLetter, int width, int height)
        {
            _zplCode.Append(string.Format(_format, "^CF{0},{1},{2}", fontLetter, height, width));
            return this;
        }

        /**
         * Use Font Name to Call Font (使用字型) - 在接下來的列印中使用某個預備字型
         * @param fontLetter    字型代號
         * @param width     字寬 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
         * @param height    字高 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
         * @return
         */
        public ZplCmdBuilder UseFontN(string fontLetter, int width, int height)
        {
            _zplCode.Append(string.Format(_format, "^A{0}N,{1},{2}", fontLetter, height, width));
            return this;
        }

        /**
         * Use Font Name to Call Font (使用字型) - 在接下來的列印中使用某個預備字型
         * @param fontLetter    字型代號
         * @param width     字寬 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
         * @param height    字高 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
         * @return
         */
        public ZplCmdBuilder UseFont(string fontLetter, int width, int height)
        {
            _zplCode.Append(string.Format(_format, "^A{0},{1},{2}", fontLetter, height, width));
            return this;
        }

        /**
         * Use Font Name to Call Font (帶入字型檔名來使用字型)
         * @param fontName  字型存放的裝置識別字元 與 字型檔名，格式 :  E:BIG5.TTF
         * @param width     字寬 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
         * @param height    字高 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
         * @return
         */
        public ZplCmdBuilder UseFontByName(string fontName, int width, int height)
        {
            _zplCode.Append(string.Format(_format, "^A@N,{0},{1},{2}", height, width, fontName));
            return this;
        }
        #endregion

        #region Label 設定

        /**
             * Label Length (設定標籤長度)
             * @param y 長度 (dots, 1 - 32000)
             * @return
             */
        public ZplCmdBuilder LabelLength(int y)
        {
            _zplCode.Append(string.Format(_format, "^LL{0}", y));
            return this;
        }

        /**
         * Label Home (設定標籤起點座標)
         * @param x 標籤 x 起點
         * @param y 標籤 y 起點
         * @return
         */
        public ZplCmdBuilder LabelHome(int x, int y)
        {
            _zplCode.Append(string.Format(_format, "^LH{0},{1}", x, y));
            return this;
        }
        #endregion

        #region Field 設定
        /**
             * Field Parameter (Field參數設定) - 設定列印的方向以及字距
             * align H: horizontal printing,  V: vertical printing,  R: reverse printing
             * gap 字距
             * */
        public ZplCmdBuilder FieldParam(int gap)
        {
            _zplCode.Append(string.Format(_format, "^FP{0},{1}", "H", gap));
            return this;
        }
        /**
             * Field Origin (Field初始設定) - 設定 ^LH 後的第一個 field (最左上角)
             * align 0: left ,  1: right,  2: auto
             * */
        public ZplCmdBuilder FieldOrigin(int x, int y, int align)
        {
            _zplCode.Append(string.Format(_format, "^FO{0},{1},{2}", x, y, align));
            return this;
        }
        /**
             * Field Typeset (Field格式設定) - 與 ^FO 相似，但是針對後續要列印的欄位做設定
             * align 0: left ,  1: right,  2: auto
             * */
        public ZplCmdBuilder FieldType(int x, int y, int align)
        {
            _zplCode.Append(string.Format(_format, "^FT{0},{1},{2}", x, y, align));
            return this;
        }
        /**
             * Field Block (Field區塊設定)，主要用來做文字對齊用
             * align:  L: left ,  R: right,  C:center, J:justified
             * */
        public ZplCmdBuilder FieldBlock(string align)
        {
            _zplCode.Append(string.Format(_format, "^FB576,1,0,{0},0", align));
            return this;
        }
        /**
             * Field Data (欄位資料)
             * data:  要印出的資料
             * */
        public ZplCmdBuilder FieldData(string data)
        {
            _zplCode.Append(string.Format(_format, "^FD{0}", data));
            return this;
        }
        /**
             * Field Separator (換行)
             * */
        public ZplCmdBuilder FieldSeparator()
        {
            _zplCode.Append(string.Format(_format, "^FS"));
            return this;
        }

        /**
         * Field Orientation  (選轉)
         * @param rotation :  N:normal, R:rotation 90, I:Inverted 180, B:Bottom-Up 270(read from bottom up)
         * @return
         */
        public ZplCmdBuilder FieldOrientation(string rotation)
        {
            _zplCode.Append(string.Format(_format, "^FW{0}", rotation));
            return this;
        }
        #endregion

        /**
        * 印出  BarCode 128 (包含換行)
        * ^FO20,10               ^FO 設定條碼左上角的位置 ,   0, 0代表完全不留邊距
        * ^BCN,,Y,N             ^BC 印 code128 的指令
        * ^FD01008D004Q-0^FS    ^FD 設定要印的内容,  ^FS表示換行
        * @param start_x   起始 x 座標
        * @param start_y   起始 y 座標
        * @param height  條碼高度
        * @param code  條碼字串
        * @return
        */
        public ZplCmdBuilder BarCode128(int start_x, int start_y, int height, string code)
        {
            _zplCode.Append(string.Format(_format, "^FO{0},{1},2^BCN,{2},N,N,N^FD{3}^FS", start_x, start_y, height, code));
            return this;
        }
        /**  印出  BarCode 39 (包含換行) */
        public ZplCmdBuilder BarCode39(int start_x, int start_y, int height, string code)
        {
            _zplCode.Append(string.Format(_format, "^FO{0},{1},2^B3N,N,{2},N,N^FD{3}^FS", start_x, start_y, height, code));
            return this;
        }

        /**
         * 印出  QRCode (包含換行)
         * @param start_x   起點 x
         * @param start_y   起點 y
         * @param size    大小 ( 1 to 10, 共10種等級)
         * @param code  要編碼的文字 (注意，zebra 的 QRcode有附加設定，可在 FD 欄位中另外設定，故底下在 FD 後帶的 QA 即是附加設定)
         * @return
         */
        public ZplCmdBuilder QrCode(int start_x, int start_y, int size, string code)
        {
            _zplCode.Append($"^FO{start_x},{start_y},2^BQN,2,{size}^FDMA,{code}^FS");
            return this;
        }

        /// <summary>
        /// 打印英文
        /// </summary>
        /// <param name="EnText">待打印文本</param>
        /// <param name="ZebraFont">打印機字體 A-Z</param>
        /// <param name="px">終點X坐標</param>
        /// <param name="py">終點Y坐標</param>
        /// <param name="Orient">扭轉角度N = normal，R = rotated 90 degrees (clockwise)，I = inverted 180 degrees，B = read from bottom up, 270 degrees</param>
        /// <param name="Height">字體高度</param>
        /// <param name="Width">字體寬度</param>
        /// <returns>前往ZPL敕令</returns>
        public ZplCmdBuilder Text(string EnText, string ZebraFont, int px, int py, string Orient, int Height, int Width)
        {
            //ZPL打印英文敕令：^FO50,50^A0N,32,25^FDZEBRA^FS
            _zplCode.Append($"^FO{px},{py}^A { ZebraFont} {Orient},{Height},{Width}^FD{EnText}^FS");
            return this;
        }

        /**
         * Graphic Box 畫框框
         * @param width 寬 (邊框寬度 to 32000)
         * @param height 高 (邊框寬度 to 32000)
         * @param border 邊框寬度 ( 1 to 32000)
         * @param degree 圓角
         * @return
         */
        public ZplCmdBuilder GraphicBox(int width, int height, int border, int degree)
        {
            _zplCode.Append(string.Format(_format, "^GB{0},{1}, {2}, B, {3}", width, height, border, degree));
            return this;
        }

    }
}
