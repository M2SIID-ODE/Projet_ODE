﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApplication2.CalculsSimples {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://emiage.org/", ConfigurationName="CalculsSimples.CalculsSimples")]
    public interface CalculsSimples {
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="http://emiage.org/CalculsSimples/decomposerRequest", ReplyAction="http://emiage.org/CalculsSimples/decomposerResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(additionnerResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(additionner))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(decomposer))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(premierResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(premier))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        WpfApplication2.CalculsSimples.decomposerResponse decomposer(WpfApplication2.CalculsSimples.decomposerRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://emiage.org/CalculsSimples/decomposerRequest", ReplyAction="http://emiage.org/CalculsSimples/decomposerResponse")]
        System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.decomposerResponse> decomposerAsync(WpfApplication2.CalculsSimples.decomposerRequest request);
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="http://emiage.org/CalculsSimples/additionnerRequest", ReplyAction="http://emiage.org/CalculsSimples/additionnerResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(additionnerResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(additionner))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(decomposer))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(premierResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(premier))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        WpfApplication2.CalculsSimples.additionnerResponse1 additionner(WpfApplication2.CalculsSimples.additionnerRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://emiage.org/CalculsSimples/additionnerRequest", ReplyAction="http://emiage.org/CalculsSimples/additionnerResponse")]
        System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.additionnerResponse1> additionnerAsync(WpfApplication2.CalculsSimples.additionnerRequest request);
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="http://emiage.org/CalculsSimples/premierRequest", ReplyAction="http://emiage.org/CalculsSimples/premierResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(additionnerResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(additionner))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(decomposer))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(premierResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(premier))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        WpfApplication2.CalculsSimples.premierResponse1 premier(WpfApplication2.CalculsSimples.premierRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://emiage.org/CalculsSimples/premierRequest", ReplyAction="http://emiage.org/CalculsSimples/premierResponse")]
        System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.premierResponse1> premierAsync(WpfApplication2.CalculsSimples.premierRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://emiage.org/")]
    public partial class additionnerResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private double returnField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public double @return {
            get {
                return this.returnField;
            }
            set {
                this.returnField = value;
                this.RaisePropertyChanged("return");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://emiage.org/")]
    public partial class additionner : object, System.ComponentModel.INotifyPropertyChanged {
        
        private double nombreAField;
        
        private double nombreBField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public double nombreA {
            get {
                return this.nombreAField;
            }
            set {
                this.nombreAField = value;
                this.RaisePropertyChanged("nombreA");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public double nombreB {
            get {
                return this.nombreBField;
            }
            set {
                this.nombreBField = value;
                this.RaisePropertyChanged("nombreB");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://emiage.org/")]
    public partial class decomposer : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int entierField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public int entier {
            get {
                return this.entierField;
            }
            set {
                this.entierField = value;
                this.RaisePropertyChanged("entier");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://emiage.org/")]
    public partial class premierResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private bool returnField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public bool @return {
            get {
                return this.returnField;
            }
            set {
                this.returnField = value;
                this.RaisePropertyChanged("return");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://emiage.org/")]
    public partial class premier : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int entierField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public int entier {
            get {
                return this.entierField;
            }
            set {
                this.entierField = value;
                this.RaisePropertyChanged("entier");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="decomposer", WrapperNamespace="http://emiage.org/", IsWrapped=true)]
    public partial class decomposerRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int entier;
        
        public decomposerRequest() {
        }
        
        public decomposerRequest(int entier) {
            this.entier = entier;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="decomposerResponse", WrapperNamespace="http://emiage.org/", IsWrapped=true)]
    public partial class decomposerResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public object[] @return;
        
        public decomposerResponse() {
        }
        
        public decomposerResponse(object[] @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="additionner", WrapperNamespace="http://emiage.org/", IsWrapped=true)]
    public partial class additionnerRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double nombreA;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double nombreB;
        
        public additionnerRequest() {
        }
        
        public additionnerRequest(double nombreA, double nombreB) {
            this.nombreA = nombreA;
            this.nombreB = nombreB;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="additionnerResponse", WrapperNamespace="http://emiage.org/", IsWrapped=true)]
    public partial class additionnerResponse1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double @return;
        
        public additionnerResponse1() {
        }
        
        public additionnerResponse1(double @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="premier", WrapperNamespace="http://emiage.org/", IsWrapped=true)]
    public partial class premierRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int entier;
        
        public premierRequest() {
        }
        
        public premierRequest(int entier) {
            this.entier = entier;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="premierResponse", WrapperNamespace="http://emiage.org/", IsWrapped=true)]
    public partial class premierResponse1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://emiage.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool @return;
        
        public premierResponse1() {
        }
        
        public premierResponse1(bool @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CalculsSimplesChannel : WpfApplication2.CalculsSimples.CalculsSimples, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CalculsSimplesClient : System.ServiceModel.ClientBase<WpfApplication2.CalculsSimples.CalculsSimples>, WpfApplication2.CalculsSimples.CalculsSimples {
        
        public CalculsSimplesClient() {
        }
        
        public CalculsSimplesClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CalculsSimplesClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CalculsSimplesClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CalculsSimplesClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WpfApplication2.CalculsSimples.decomposerResponse WpfApplication2.CalculsSimples.CalculsSimples.decomposer(WpfApplication2.CalculsSimples.decomposerRequest request) {
            return base.Channel.decomposer(request);
        }
        
        public object[] decomposer(int entier) {
            WpfApplication2.CalculsSimples.decomposerRequest inValue = new WpfApplication2.CalculsSimples.decomposerRequest();
            inValue.entier = entier;
            WpfApplication2.CalculsSimples.decomposerResponse retVal = ((WpfApplication2.CalculsSimples.CalculsSimples)(this)).decomposer(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.decomposerResponse> WpfApplication2.CalculsSimples.CalculsSimples.decomposerAsync(WpfApplication2.CalculsSimples.decomposerRequest request) {
            return base.Channel.decomposerAsync(request);
        }
        
        public System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.decomposerResponse> decomposerAsync(int entier) {
            WpfApplication2.CalculsSimples.decomposerRequest inValue = new WpfApplication2.CalculsSimples.decomposerRequest();
            inValue.entier = entier;
            return ((WpfApplication2.CalculsSimples.CalculsSimples)(this)).decomposerAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WpfApplication2.CalculsSimples.additionnerResponse1 WpfApplication2.CalculsSimples.CalculsSimples.additionner(WpfApplication2.CalculsSimples.additionnerRequest request) {
            return base.Channel.additionner(request);
        }
        
        public double additionner(double nombreA, double nombreB) {
            WpfApplication2.CalculsSimples.additionnerRequest inValue = new WpfApplication2.CalculsSimples.additionnerRequest();
            inValue.nombreA = nombreA;
            inValue.nombreB = nombreB;
            WpfApplication2.CalculsSimples.additionnerResponse1 retVal = ((WpfApplication2.CalculsSimples.CalculsSimples)(this)).additionner(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.additionnerResponse1> WpfApplication2.CalculsSimples.CalculsSimples.additionnerAsync(WpfApplication2.CalculsSimples.additionnerRequest request) {
            return base.Channel.additionnerAsync(request);
        }
        
        public System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.additionnerResponse1> additionnerAsync(double nombreA, double nombreB) {
            WpfApplication2.CalculsSimples.additionnerRequest inValue = new WpfApplication2.CalculsSimples.additionnerRequest();
            inValue.nombreA = nombreA;
            inValue.nombreB = nombreB;
            return ((WpfApplication2.CalculsSimples.CalculsSimples)(this)).additionnerAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WpfApplication2.CalculsSimples.premierResponse1 WpfApplication2.CalculsSimples.CalculsSimples.premier(WpfApplication2.CalculsSimples.premierRequest request) {
            return base.Channel.premier(request);
        }
        
        public bool premier(int entier) {
            WpfApplication2.CalculsSimples.premierRequest inValue = new WpfApplication2.CalculsSimples.premierRequest();
            inValue.entier = entier;
            WpfApplication2.CalculsSimples.premierResponse1 retVal = ((WpfApplication2.CalculsSimples.CalculsSimples)(this)).premier(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.premierResponse1> WpfApplication2.CalculsSimples.CalculsSimples.premierAsync(WpfApplication2.CalculsSimples.premierRequest request) {
            return base.Channel.premierAsync(request);
        }
        
        public System.Threading.Tasks.Task<WpfApplication2.CalculsSimples.premierResponse1> premierAsync(int entier) {
            WpfApplication2.CalculsSimples.premierRequest inValue = new WpfApplication2.CalculsSimples.premierRequest();
            inValue.entier = entier;
            return ((WpfApplication2.CalculsSimples.CalculsSimples)(this)).premierAsync(inValue);
        }
    }
}
