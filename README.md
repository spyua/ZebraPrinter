# ZebraPrinter

Support the simple usage of zebra printer by UDP connection.

## Usage

```
var zplCmd = new ZplCmdBuilder();

var zplCode = zplCmd.LabelLength(100)
                    .LabelHome(5, 50)
                    .FieldOrigin(2, 2, 2)
                    .FieldBlock("C")
                    .FieldSeparator()
                    .EndZPL()
                    .GetZplCode();
TryFlow(() =>{
        var zebra = new ZebraPrinter("IP", Port);
        zebra.Send(zplCode);
});

```

## ZplBuilder API

 - 一般通用函式
  - GetZpl:取得完整的 zpl 段
  - StartZPL:ZPL 區段起始標號 (放在 ZPL 語言開頭)
  - EndZPL:ZPL區段終止標號 (放在 ZPL 語言結尾)
 - 編碼指定
  - SelectEncoding:選擇編碼檔作為目前列印文字的編碼
  - ChgEncoding:選擇目前列印文字的編碼
 - 字型設定
  - FontIdentifier:設定預備字型
  - ChangeAlphanumericFont:指定印表機預設字型的大小
  - UseFontN:使用字型
  - UseFont:使用字型
  - UseFontByName:入字型檔名來使用字型
 - Label 設定
  - LabelLength:設定標籤長度
  - LabelHome:設定標籤起點座標
 - Field 設定
  - FieldParam:(Field參數設定)設定列印的方向以及字距
  - FieldOrigin:(Field初始設定) 設定 ^LH 後的第一個 field (最左上角)
  - FieldType:(Field格式設定) 與 ^FO 相似，但是針對後續要列印的欄位做設定
  - FieldBlock:(Field區塊設定)，主要用來做文字對齊用
  - FieldData:欄位資料
  - FieldSeparator:換行
  - FieldOrientation:選轉
 - Graph
  - BarCode128:BarCode 128 (包含換行)
  - BarCode39:BarCode 39 (包含換行)
  - QrCode: QRCode (包含換行)
  - GraphicBox:畫框框
