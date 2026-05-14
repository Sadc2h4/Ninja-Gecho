# Ninja_Gecho
<!-- .NET 8 / Windows x64 -->
![.NET](https://img.shields.io/badge/language-.NET%208-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Platform](https://img.shields.io/badge/platform-Windows%20x64-0078D4?style=flat-square&logo=windows&logoColor=white)
![Architecture](https://img.shields.io/badge/arch-x64-gray?style=flat-square)

Ninja_Gecho is a lightweight Windows tool for making image backgrounds transparent.  
画像を開いて、クリック操作で背景や指定範囲を透過 PNG として保存できます。

## Download

配布時は `Ninja_Gecho.exe` を同梱してください。  
`Ninja_Gecho.exe` is a self-contained Windows x64 executable.

## Features / 主な機能

| Feature | Description |
|--------|-------------|
| 塗りつぶしモード / Bucket | クリックした点と同じ色がつながっている範囲を透過します。 |
| 主線保護塗りつぶし / Protected Bucket | Broken line art の線抜けを保護し、内側への透過漏れを軽減します。 |
| 色選択モード / Color Select | クリックした色に近いピクセルを画像全体から透過します。 |
| 四角形モード / Rectangle | 指定した rectangular area を透過します。 |
| 消しゴムモード / Eraser | 16x16 pixel eraser で部分的に透過します。 |
| フィニッシュ / Finish | 透明部分のふちに残った半端な色をなじませます。 |
| 現在の拡大率で保存 / Save Current Zoom | 25%, 50%, 200%, 400% など、表示倍率に合わせて pixel-perfect resized PNG を保存します。 |

## Setup

特別なインストールは不要です。  
Place `Ninja_Gecho.exe` in any folder and run it.

```text
Ninja_Gecho.exe
```

## Usage / 使い方

1. `ファイル > 開く` から画像を開きます。
2. ツールバーから透過モードを選び、画像上をクリックします。
3. `誤差許容度(0-255)` で同色判定の幅を調整します。
4. `不透明度(0-255)` で透過後の alpha を指定します。完全透過は `0` です。
5. `ファイル > 名前を付けて保存` で通常 PNG 保存します。
6. `ファイル > 現在の拡大率で保存` で、表示倍率のサイズにピクセルを保ったままリサイズして PNG 保存します。

## Protected Bucket Notes

主線保護塗りつぶしは、クリックした背景色と異なる暗い線を temporary barrier として扱います。  
It expands detected line pixels slightly, so small gaps in broken outlines are treated as closed while filling.

線が薄い場合や背景とのコントラストが低い場合は、通常の塗りつぶしと誤差許容度を使い分けてください。

## Deletion Method / 削除方法

`Ninja_Gecho.exe` が入ったフォルダごと削除してください。  
No registry entries are created by this application.

## Credits

Ninja_Gecho is created by `C2H4`.  
The transparency workflow and UI behavior were implemented with reference to `手軽に透明png`.

## Disclaimer / 免責事項

本ソフトウェアの使用によって生じたいかなる損害についても、作者は一切の責任を負いません。  
I assume no responsibility whatsoever for any damages incurred through the use of this software.
