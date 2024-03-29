﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace COCAAPI.GetPRDetailsFromNAV {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="GetPRDetailsCOCA_Binding", Namespace="urn:microsoft-dynamics-schemas/codeunit/GetPRDetailsCOCA")]
    public partial class GetPRDetailsCOCA : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetPRForCOCAOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public GetPRDetailsCOCA() {
            this.Url = global::COCAAPI.Properties.Settings.Default.COCAAPI_GetPRDetailsFromNAV_GetPRDetailsCOCA;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetPRForCOCACompletedEventHandler GetPRForCOCACompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:microsoft-dynamics-schemas/codeunit/GetPRDetailsCOCA:GetPRForCOCA", RequestNamespace="urn:microsoft-dynamics-schemas/codeunit/GetPRDetailsCOCA", ResponseElementName="GetPRForCOCA_Result", ResponseNamespace="urn:microsoft-dynamics-schemas/codeunit/GetPRDetailsCOCA", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void GetPRForCOCA(ref XMLPortForCOCAPRHeader xMLCOCAPRHeader) {
            object[] results = this.Invoke("GetPRForCOCA", new object[] {
                        xMLCOCAPRHeader});
            xMLCOCAPRHeader = ((XMLPortForCOCAPRHeader)(results[0]));
        }
        
        /// <remarks/>
        public void GetPRForCOCAAsync(XMLPortForCOCAPRHeader xMLCOCAPRHeader) {
            this.GetPRForCOCAAsync(xMLCOCAPRHeader, null);
        }
        
        /// <remarks/>
        public void GetPRForCOCAAsync(XMLPortForCOCAPRHeader xMLCOCAPRHeader, object userState) {
            if ((this.GetPRForCOCAOperationCompleted == null)) {
                this.GetPRForCOCAOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPRForCOCAOperationCompleted);
            }
            this.InvokeAsync("GetPRForCOCA", new object[] {
                        xMLCOCAPRHeader}, this.GetPRForCOCAOperationCompleted, userState);
        }
        
        private void OnGetPRForCOCAOperationCompleted(object arg) {
            if ((this.GetPRForCOCACompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPRForCOCACompleted(this, new GetPRForCOCACompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:microsoft-dynamics-nav/xmlports/x50106")]
    public partial class XMLPortForCOCAPRHeader {
        
        private PRHeader[] pRHeaderField;
        
        private PRLine[] pRLineField;
        
        private string[] textField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PRHeader")]
        public PRHeader[] PRHeader {
            get {
                return this.pRHeaderField;
            }
            set {
                this.pRHeaderField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PRLine")]
        public PRLine[] PRLine {
            get {
                return this.pRLineField;
            }
            set {
                this.pRLineField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:microsoft-dynamics-nav/xmlports/x50106")]
    public partial class PRHeader {
        
        private string pRNoField;
        
        private string pRDateField;
        
        private string departmentField;
        
        private string intendedForField;
        
        private string plantNoField;
        
        private string workDescriptionField;
        
        private string purchaserCodeField;
        
        private string purchaserNameField;
        
        private string pRMonitoringStatusField;
        
        private string pRApprovingStatusField;
        
        private string pRStatusField;
        
        private string pRTypeField;
        
        private string outageCodeField;
        
        private string outageCodeDescriptionField;
        
        private string priorityField;
        
        private string plantRelatedField;
        
        private string technicalReportRequiredField;
        
        private string projectLocationField;
        
        private string projectInChargeField;
        
        private string budgetTotalAmountField;
        
        private string pRReleasedDateField;
        
        /// <remarks/>
        public string PRNo {
            get {
                return this.pRNoField;
            }
            set {
                this.pRNoField = value;
            }
        }
        
        /// <remarks/>
        public string PRDate {
            get {
                return this.pRDateField;
            }
            set {
                this.pRDateField = value;
            }
        }
        
        /// <remarks/>
        public string Department {
            get {
                return this.departmentField;
            }
            set {
                this.departmentField = value;
            }
        }
        
        /// <remarks/>
        public string IntendedFor {
            get {
                return this.intendedForField;
            }
            set {
                this.intendedForField = value;
            }
        }
        
        /// <remarks/>
        public string PlantNo {
            get {
                return this.plantNoField;
            }
            set {
                this.plantNoField = value;
            }
        }
        
        /// <remarks/>
        public string WorkDescription {
            get {
                return this.workDescriptionField;
            }
            set {
                this.workDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string PurchaserCode {
            get {
                return this.purchaserCodeField;
            }
            set {
                this.purchaserCodeField = value;
            }
        }
        
        /// <remarks/>
        public string PurchaserName {
            get {
                return this.purchaserNameField;
            }
            set {
                this.purchaserNameField = value;
            }
        }
        
        /// <remarks/>
        public string PRMonitoringStatus {
            get {
                return this.pRMonitoringStatusField;
            }
            set {
                this.pRMonitoringStatusField = value;
            }
        }
        
        /// <remarks/>
        public string PRApprovingStatus {
            get {
                return this.pRApprovingStatusField;
            }
            set {
                this.pRApprovingStatusField = value;
            }
        }
        
        /// <remarks/>
        public string PRStatus {
            get {
                return this.pRStatusField;
            }
            set {
                this.pRStatusField = value;
            }
        }
        
        /// <remarks/>
        public string PRType {
            get {
                return this.pRTypeField;
            }
            set {
                this.pRTypeField = value;
            }
        }
        
        /// <remarks/>
        public string OutageCode {
            get {
                return this.outageCodeField;
            }
            set {
                this.outageCodeField = value;
            }
        }
        
        /// <remarks/>
        public string OutageCodeDescription {
            get {
                return this.outageCodeDescriptionField;
            }
            set {
                this.outageCodeDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string Priority {
            get {
                return this.priorityField;
            }
            set {
                this.priorityField = value;
            }
        }
        
        /// <remarks/>
        public string PlantRelated {
            get {
                return this.plantRelatedField;
            }
            set {
                this.plantRelatedField = value;
            }
        }
        
        /// <remarks/>
        public string TechnicalReportRequired {
            get {
                return this.technicalReportRequiredField;
            }
            set {
                this.technicalReportRequiredField = value;
            }
        }
        
        /// <remarks/>
        public string ProjectLocation {
            get {
                return this.projectLocationField;
            }
            set {
                this.projectLocationField = value;
            }
        }
        
        /// <remarks/>
        public string ProjectInCharge {
            get {
                return this.projectInChargeField;
            }
            set {
                this.projectInChargeField = value;
            }
        }
        
        /// <remarks/>
        public string BudgetTotalAmount {
            get {
                return this.budgetTotalAmountField;
            }
            set {
                this.budgetTotalAmountField = value;
            }
        }
        
        /// <remarks/>
        public string PRReleasedDate {
            get {
                return this.pRReleasedDateField;
            }
            set {
                this.pRReleasedDateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:microsoft-dynamics-nav/xmlports/x50106")]
    public partial class PRLine {
        
        private string pRNo_LineField;
        
        private string no_LineField;
        
        /// <remarks/>
        public string PRNo_Line {
            get {
                return this.pRNo_LineField;
            }
            set {
                this.pRNo_LineField = value;
            }
        }
        
        /// <remarks/>
        public string No_Line {
            get {
                return this.no_LineField;
            }
            set {
                this.no_LineField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void GetPRForCOCACompletedEventHandler(object sender, GetPRForCOCACompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPRForCOCACompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPRForCOCACompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public XMLPortForCOCAPRHeader xMLCOCAPRHeader {
            get {
                this.RaiseExceptionIfNecessary();
                return ((XMLPortForCOCAPRHeader)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591