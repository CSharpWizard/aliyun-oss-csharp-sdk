﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aliyun.OSS.Test.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProxyHost {
            get {
                return ((string)(this["ProxyHost"]));
            }
            set {
                this["ProxyHost"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProxyPort {
            get {
                return ((string)(this["ProxyPort"]));
            }
            set {
                this["ProxyPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DisabledAccessKeyId")]
        public string DisabledAccessKeyId {
            get {
                return ((string)(this["DisabledAccessKeyId"]));
            }
            set {
                this["DisabledAccessKeyId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\temp\\UploadFiles\\testupload.zip")]
        public string UploadSampleFile {
            get {
                return ((string)(this["UploadSampleFile"]));
            }
            set {
                this["UploadSampleFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\temp\\DownloadFiles")]
        public string DownloadFolder {
            get {
                return ((string)(this["DownloadFolder"]));
            }
            set {
                this["DownloadFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\temp\\UploadFiles\\testmultipartupload.zip")]
        public string MultiUploadSampleFile {
            get {
                return ((string)(this["MultiUploadSampleFile"]));
            }
            set {
                this["MultiUploadSampleFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DisabledAccessKeySecret")]
        public string DisabledAccessKeySecret {
            get {
                return ((string)(this["DisabledAccessKeySecret"]));
            }
            set {
                this["DisabledAccessKeySecret"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://oss-cn-hangzhou.aliyuncs.com")]
        public string SecondEndpoint {
            get {
                return ((string)(this["SecondEndpoint"]));
            }
            set {
                this["SecondEndpoint"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\temp\\UploadFiles")]
        public string UploadSampleFolder {
            get {
                return ((string)(this["UploadSampleFolder"]));
            }
            set {
                this["UploadSampleFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("your access key id")]
        public string AccessKeyId {
            get {
                return ((string)(this["AccessKeyId"]));
            }
            set {
                this["AccessKeyId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("your access key secret")]
        public string AccessKeySecret {
            get {
                return ((string)(this["AccessKeySecret"]));
            }
            set {
                this["AccessKeySecret"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("your endpoint")]
        public string Endpoint {
            get {
                return ((string)(this["Endpoint"]));
            }
            set {
                this["Endpoint"] = value;
            }
        }
    }
}
