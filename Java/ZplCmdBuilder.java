import java.util.Locale;

public class ZplCmdBuilder {

    private  String START_ZPL = "^XA";
    private  String END_ZPL = "^XZ";

    public  String ORI_NORMAL = "N";    	//orientation Normal  旋轉參數，正常
    public  String ORI_90 = "R";          //orientation 90 degrees 旋轉參數，90度 (順時針)
    public  String ORI_180 = "I";         //orientation 180 degrees
    public  String ORI_270 = "B";         //orientation 270 degrees

    private StringBuffer zplCode;

    public  ZplCmdBuilder(){
        zplCode = new StringBuffer();
    }

    public String GetZplCode() { return zplCode.toString(); }

    public ZplCmdBuilder ClrZplCode() {
        zplCode.delete(0, zplCode.length());
        return this;
    }

    //region 一般通用函式
    /** 取得完整的 zpl 段，自動包含 ZPL 開頭和結尾 */
    public String GetZpl(String... strings) {
        if(strings != null) {
            StringBuffer result = new StringBuffer();
            result.append(START_ZPL);
            for (int i=0 ; i<strings.length ; i++) {
                result.append(strings[i]);
            }
            result.append(END_ZPL);
            return result.toString();
        }
        else
            return null;
    }

    /** ZPL 區段起始標號 (放在 ZPL 語言開頭) */
    public ZplCmdBuilder startZPL(){
        this.zplCode.append(START_ZPL);
        return this;
    }

    /** ZPL 區段終止標號 (放在 ZPL 語言結尾) */
    public ZplCmdBuilder endZPL(){
        this.zplCode.append(END_ZPL);
        return this;
    }
    //endregion

    //region 編碼指定

    /**
     * Select Encoding Table (選擇編碼) - 選擇編碼檔作為目前列印文字的編碼
     * @param deviceAndCodename  編碼檔存放的裝置識別字元 與 編碼檔名，格式 :  E:BIG5.DAT
     * @return
     */
    public ZplCmdBuilder SelectEncoding(String deviceAndCodename ){
        this.zplCode.append(String.format(Locale.TAIWAN,"^SE%s", deviceAndCodename ));
        return this;
    }
    /**
     * Change International Font/Encoding (改變編碼) - 選擇目前列印文字的編碼
     * @param encodingCode  編碼代號
     * @return
     */
    public ZplCmdBuilder ChgEncoding(int encodingCode ){
        this.zplCode.append(String.format(Locale.TAIWAN,"^CI%d", encodingCode ));
        return this;
    }

    //endregion

    //region 字型設定
    /**
     * Font Identifier (設定預備字型) - 預先設定會使用到的字型的代號
     * @param fontLetter    字型的代號字串，從 A-Z, 0-9
     * @param deviceAndFontname 字型存放的裝置識別字元 與 字型檔名，格式 :  E:BIG5.TTF
     * @return
     */
    public ZplCmdBuilder FontIdentifier(String fontLetter, String deviceAndFontname ){
        this.zplCode.append(String.format(Locale.TAIWAN,"^CW%s,%s", fontLetter, deviceAndFontname ));
        return this;
    }

    /**
     * Change Alphanumeric Default Font (指定字母/數字預設字型) - 指定印表機預設字型的大小, 與 ^CW 配合使用
     * @param fontLetter iMZ320 預設有 0-7 共八種字型
     * @param width     電源開啟時預設 5
     * @param height    電源開啟時預設 9
     * @return
     */
    public ZplCmdBuilder ChangeAlphanumericFont(String fontLetter, int width, int height ){
        this.zplCode.append(String.format(Locale.TAIWAN,"^CF%s,%d,%d", fontLetter, height, width ));
        return this;
    }

    /**
     * Use Font Name to Call Font (使用字型) - 在接下來的列印中使用某個預備字型
     * @param fontLetter    字型代號
     * @param width     字寬 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
     * @param height    字高 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
     * @return
     */
    public ZplCmdBuilder UseFontN(String fontLetter, int width, int height){
        this.zplCode.append(String.format(Locale.TAIWAN,"^A%sN,%d,%d",fontLetter, height, width));
        return this;
    }

    /**
     * Use Font Name to Call Font (使用字型) - 在接下來的列印中使用某個預備字型
     * @param fontLetter    字型代號
     * @param width     字寬 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
     * @param height    字高 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
     * @return
     */
    public ZplCmdBuilder UseFont(String fontLetter, int width, int height){
        this.zplCode.append(String.format(Locale.TAIWAN,"^A%s,%d,%d",fontLetter, height, width));
        return this;
    }

    /**
     * Use Font Name to Call Font (帶入字型檔名來使用字型)
     * @param fontName  字型存放的裝置識別字元 與 字型檔名，格式 :  E:BIG5.TTF
     * @param width     字寬 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
     * @param height    字高 (10 to 32000)，沒設定的話，會以最後一次呼叫 ^CF 的設定值為主
     * @return
     */
    public ZplCmdBuilder UseFontByName(String fontName, int width, int height){
        this.zplCode.append(String.format(Locale.TAIWAN,"^A@N,%d,%d,%s", height, width, fontName));
        return this;
    }
    //endregion

    //region Label 設定

    /**
     * Label Length (設定標籤長度)
     * @param y 長度 (dots, 1 - 32000)
     * @return
     */
    public ZplCmdBuilder LabelLength(int y){
        this.zplCode.append(String.format(Locale.TAIWAN,"^LL%d", y));
        return this;
    }

    /**
     * Label Home (設定標籤起點座標)
     * @param x 標籤 x 起點
     * @param y 標籤 y 起點
     * @return
     */
    public ZplCmdBuilder LabelHome(int x, int y){
        this.zplCode.append(String.format(Locale.TAIWAN,"^LH%d,%d", x, y));
        return this;
    }
    //endregion

    //region Field 設定
    /**
     * Field Parameter (Field參數設定) - 設定列印的方向以及字距
     * align H: horizontal printing,  V: vertical printing,  R: reverse printing
     * gap 字距
     * */
    public ZplCmdBuilder fieldParam(int gap){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FP%s,%d", "H", gap));
        return this;
    }
    /**
     * Field Origin (Field初始設定) - 設定 ^LH 後的第一個 field (最左上角)
     * align 0: left ,  1: right,  2: auto
     * */
    public ZplCmdBuilder fieldOrigin(int x, int y, int align){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FO%d,%d,%d", x, y, align));
        return this;
    }
    /**
     * Field Typeset (Field格式設定) - 與 ^FO 相似，但是針對後續要列印的欄位做設定
     * align 0: left ,  1: right,  2: auto
     * */
    public ZplCmdBuilder fieldType(int x, int y, int align){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FT%d,%d,%d", x, y, align));
        return this;
    }
    /**
     * Field Block (Field區塊設定)，主要用來做文字對齊用
     * align:  L: left ,  R: right,  C:center, J:justified
     * */
    public ZplCmdBuilder fieldBlock(String align){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FB576,1,0,%s,0", align));
        return this;
    }
    /**
     * Field Data (欄位資料)
     * data:  要印出的資料
     * */
    public ZplCmdBuilder fieldData(String data){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FD%s", data));
        return this;
    }
    /**
     * Field Separator (換行)
     * */
    public ZplCmdBuilder fieldSeparator(){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FS"));
        return this;
    }

    /**
     * Field Orientation  (選轉)
     * @param rotation :  N:normal, R:rotation 90, I:Inverted 180, B:Bottom-Up 270(read from bottom up)
     * @return
     */
    public ZplCmdBuilder fieldOrientation(String rotation){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FW%s", rotation));
        return this;
    }
    //endregion

    // region BarCode && QR Code
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
    public ZplCmdBuilder BarCode128(int start_x, int start_y, int height, String code){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FO%d,%d,2^BCN,%d,N,N,N^FD%s^FS", start_x, start_y, height, code));
        return this;
    }
    /**  印出  BarCode 39 (包含換行) */
    public ZplCmdBuilder BarCode39(int start_x, int start_y, int height, String code){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FO%d,%d,2^B3N,N,%d,N,N^FD%s^FS", start_x, start_y, height, code));
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
    public ZplCmdBuilder qrCode(int start_x, int start_y, int size, String code){
        this.zplCode.append(String.format(Locale.TAIWAN,"^FO%d,%d,2^BQN,2,%d^FDMA,%s^FS", start_x, start_y, size, code));
        return this;
    }
    //endregion

    /**
     * Graphic Box 畫框框
     * @param width 寬 (邊框寬度 to 32000)
     * @param height 高 (邊框寬度 to 32000)
     * @param border 邊框寬度 ( 1 to 32000)
     * @param degree 圓角
     * @return
     */
    public ZplCmdBuilder GraphicBox(int width, int height, int border, int degree){
        this.zplCode.append(String.format(Locale.TAIWAN,"^GB%d,%d, %d, B, %d", width, height, border, degree));
        return this;
    }

}
