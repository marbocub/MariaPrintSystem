﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MariaPrintManager.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MariaPrintManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   (アイコン) に類似した型 System.Drawing.Icon のローカライズされたリソースを検索します。
        /// </summary>
        internal static System.Drawing.Icon Icon1 {
            get {
                object obj = ResourceManager.GetObject("Icon1", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   mariaprint に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PrintProcessor {
            get {
                return ResourceManager.GetString("PrintProcessor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   MariaPrintSystem に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Product {
            get {
                return ResourceManager.GetString("Product", resourceCulture);
            }
        }
        
        /// <summary>
        ///   HKEY_LOCAL_MACHINE\SOFTWARE\Marbocub\MariaPrintSystem に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RegKeyMariaPrintSystem {
            get {
                return ResourceManager.GetString("RegKeyMariaPrintSystem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   BaseUrl に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RegValueBaseUrl {
            get {
                return ResourceManager.GetString("RegValueBaseUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   AllowDocumentNames に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RegValueDocumentNames {
            get {
                return ResourceManager.GetString("RegValueDocumentNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Printer に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RegValuePrinter {
            get {
                return ResourceManager.GetString("RegValuePrinter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   RoomID に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RegValueRoomID {
            get {
                return ResourceManager.GetString("RegValueRoomID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Maria Print Manager に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Marbocub に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Vendor {
            get {
                return ResourceManager.GetString("Vendor", resourceCulture);
            }
        }
    }
}
