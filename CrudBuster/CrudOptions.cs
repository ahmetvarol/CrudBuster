namespace CrudBuster;

  /// <summary>
    /// Configuration for CRUD operation
    /// </summary>
    public class CrudOptions
    {
        /// <summary>
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
        
        
        /// <summary>
        /// View model name of common API base response model
        /// Ex:
        /// public class Result(Class name doesn't matter) -> This
        /// {
        ///     public bool Status { get; set; }
        ///     public string Message { get; set; }
        ///     public T Data { get; set; }
        /// }
        /// </summary>
        public string ApiResulClassName { get; set; }
    }