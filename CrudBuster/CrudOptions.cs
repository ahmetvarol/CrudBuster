namespace CrudBuster;

    /// <summary>
    /// Configuration for CRUD operation
    /// </summary>
    public class CrudOptions
    {
        public CrudOptions WithDomainLayer(string name) { DomainLayerName = name; return this; }
        public CrudOptions WithViewModelLayer(string name) { ViewModelsLayerName = name; return this; }
        public CrudOptions WithRepositoryLayer(string name) { RepositoryLayerName = name; return this; }
        public CrudOptions WithRepositoryName(string name) { RepositoryName = name; return this; }
        public CrudOptions WithIsAuthenticateRequired(bool status) { IsAuthenticateRequired = status; return this; }
        public CrudOptions WithViewModelOutput(string name) { ViewModelOutputPath = name; return this; }
        public CrudOptions WithViewModelPattern(string name) { ViewModelPattern = name; return this; }
        public CrudOptions WithAuthorizationPolicy(string name) { AuthorizationPolicy = name; return this; }
        public CrudOptions WithBaseEntityName(string name) { BaseEntityName = name; return this; }
        public CrudOptions WithGetListService(string name) { GetListService = name; return this; }
        public CrudOptions WithGetByIdService(string name) { GetByIdService = name; return this; }
        public CrudOptions WithCreateService(string name) { CreateService = name; return this; }
        public CrudOptions WithUpdateService(string name) { UpdateService = name; return this; }
        public CrudOptions WithDeleteService(string name) { DeleteService = name; return this; }
        
        
        /// Your project in domain layer name.
        /// If you enter the name of the domain layer here, it will retrieve the classes marked with BaseEntity or anything within that layer and generate minimal APIs specifically for CRUD operations related to those domain entities.
        /// </summary>
        public string DomainLayerName { get; set; }

        /// <summary>
        /// The layer where the view models are located
        /// </summary>
        public string ViewModelsLayerName { get; set; }

        /// <summary>
        /// The layer where the repository are located
        /// </summary>
        public string RepositoryLayerName { get; set; }
        
        /// <summary>
        /// Your repository name
        /// Ex: IRepository, IRepositoryService, IGenericService, etc...
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Is authorization mandatory on a controller?
        /// </summary>
        public bool IsAuthenticateRequired { get; set; }
        
        /// <summary>
        /// Your view model of entity name pattern
        /// Ex: Product*ViewModel*, Product*VM*, Product*DTO* etc...
        /// </summary>
        public string ViewModelPattern { get; set; }

        /// <summary>
        /// This variable defines where the view model classes will be created. The path you provide here will be used to generate the Create, Update, Delete, Get, and List view model classes.
        /// </summary>
        public string ViewModelOutputPath { get; set; }

        /// <summary>
        /// Admin, User, SuperAdmin etc...
        /// </summary>
        public string? AuthorizationPolicy { get; set; }

        /// <summary>
        /// Your base entity name. It's used to retrieve the classes that inherit from this class.
        /// </summary>
        public string BaseEntityName { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string GetListService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string GetByIdService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string CreateService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string UpdateService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string DeleteService { get; set; }
    }
    
    