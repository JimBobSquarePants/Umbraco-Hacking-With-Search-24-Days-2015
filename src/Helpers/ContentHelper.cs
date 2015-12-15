namespace SearchDemo.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;

    using SearchDemo.Models;

    using Our.Umbraco.Ditto;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Configuration.UmbracoSettings;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Routing;
    using global::Umbraco.Web.Security;

    using Newtonsoft.Json;

    /// <summary>
    /// Provides helper methods to return strong type models from the Umbraco back office.
    /// </summary>
    public class ContentHelper
    {
        /// <summary>
        /// A new instance Initializes a new instance of the <see cref="ContentHelper"/> class.
        /// with lazy initialization.
        /// </summary>
        private static readonly Lazy<ContentHelper> Lazy = new Lazy<ContentHelper>(() => new ContentHelper());

        /// <summary>
        /// The collection of registered types.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Type> RegisteredTypes
            = new ConcurrentDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Prevents a default instance of the <see cref="ContentHelper"/> class from being created.
        /// </summary>
        private ContentHelper()
        {
            IEnumerable<Type> registerTypes = PluginManager.Current.ResolveTypes<PublishedEntity>();

            foreach (Type type in registerTypes)
            {
                this.RegisterType(type);
            }
        }

        /// <summary>
        /// Gets the current instance of the <see cref="ContentHelper"/> class.
        /// </summary>
        public static ContentHelper Instance => Lazy.Value;

        /// <summary>
        /// Gets the <see cref="UmbracoHelper"/> for querying published content or media.
        /// </summary>
        public UmbracoHelper UmbracoHelper
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // Pull the item from the cache if possible to reduce the overhead for multiple operations
                    // taking place in a single request
                    return (UmbracoHelper)ApplicationContext.Current.ApplicationCache.RequestCache.GetCacheItem(
                            "ContentHelper.Instance.UmbracoHelper",
                        () =>
                            {
                                HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
                                ApplicationContext application = ApplicationContext.Current;
                                WebSecurity security = new WebSecurity(context, application);
                                IUmbracoSettingsSection umbracoSettings = UmbracoConfig.For.UmbracoSettings();
                                IEnumerable<IUrlProvider> providers = UrlProviderResolver.Current.Providers;
                                return new UmbracoHelper(UmbracoContext.EnsureContext(context, application, security, umbracoSettings, providers, false));
                            });
                }

                return FallbackUmbracoHelper;
            }
        }

        /// <summary>
        /// Gets or sets the fallback <see cref="UmbracoHelper"/>. 
        /// This is assign during application initialization.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static UmbracoHelper FallbackUmbracoHelper { get; set; }

        /// <summary>
        /// Gets the node matching the given id.
        /// </summary>
        /// <param name="id">
        /// The id of the node to return.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the node to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> matching the id.
        /// </returns>
        public T GetById<T>(int id) where T : class
        {
            return this.UmbracoHelper.TypedContent(id).As<T>();
        }

        /// <summary>
        /// Gets the node matching the given id.
        /// </summary>
        /// <param name="id">
        /// The id of the node to return.
        /// </param>
        /// <param name="type">
        /// The <see cref="Type"/> of object to return.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> matching the id.
        /// </returns>
        public object GetById(int id, Type type)
        {
            return this.UmbracoHelper.TypedContent(id).As(type);
        }

        /// <summary>
        /// Gets the nodes matching the given <see cref="Type"/>.
        /// </summary>
        /// <param name="rootId">
        /// The id of the root node to start from. If not given the method will return
        /// all nodes matching the type.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the node to return.
        /// </typeparam>
        /// <returns>
        /// The nodes matching the given <see cref="Type"/>.
        /// </returns>
        public IEnumerable<T> GetByNode<T>(int rootId = 0) where T : class
        {
            string name = typeof(T).Name;

            // Get only the root and descendants of the given root.
            if (rootId > 0)
            {
                IPublishedContent root = this.UmbracoHelper.TypedContent(rootId);
                foreach (IPublishedContent node in root.Descendants().Where(d => d.DocumentTypeAlias.InvariantEquals(name)))
                {
                    yield return node.As<T>();
                }
            }
            else
            {
                // Get all the nodes from all root nodes.
                IEnumerable<IPublishedContent> rootnodes = this.UmbracoHelper.TypedContentAtRoot();

                foreach (IPublishedContent rootnode in rootnodes)
                {
                    foreach (IPublishedContent node in rootnode.Descendants().Where(d => d.DocumentTypeAlias.InvariantEquals(name)))
                    {
                        yield return node.As<T>();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the root nodes matching the given type.
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the node to return.
        /// </typeparam>
        /// <returns>
        /// The root nodes matching the given <see cref="T"/> type.
        /// </returns>
        public IEnumerable<T> GetRootNodes<T>() where T : class
        {
            return this.UmbracoHelper.TypedContentAtRoot()
                                     .As<T>();
        }

        /// <summary>
        /// Gets the root nodes as an instance of <see cref="PageBase"/>.
        /// </summary>
        /// <returns>
        /// The root node as an instance of <see cref="IEnumerable{PageBase}"/>.
        /// </returns>
        public IEnumerable<PageBase> GetRootNodes()
        {
            IEnumerable<IPublishedContent> content = this.UmbracoHelper.TypedContentAtRoot();

            if (content != null)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (IPublishedContent publishedContent in content)
                {
                    string alias = publishedContent.DocumentTypeAlias;

                    Type type = this.GetRegisteredType(alias);

                    if (type != null)
                    {
                        yield return (PageBase)publishedContent.As(type);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first root node in the current site matching the given type.
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the node to return.
        /// </typeparam>
        /// <returns>
        /// The root node matching the given <see cref="T"/> type.
        /// </returns>
        public T GetRootNode<T>() where T : class
        {
            // We want to get the current site only.
            string name = typeof(T).Name;
            string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            return this.UmbracoHelper.TypedContentAtRoot()
                                     .FirstOrDefault(p => p.DocumentTypeAlias.InvariantEquals(name) && p.UrlAbsolute().StartsWith(root))
                                     .As<T>();
        }

        /// <summary>
        /// Gets the first root node in the current site as an instance of <see cref="PageBase"/>.
        /// </summary>
        /// <returns>
        /// The root node as an instance of <see cref="PageBase"/>.
        /// </returns>
        public PageBase GetRootNode()
        {
            // We want to get the current site only.
            string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            IPublishedContent content = this.UmbracoHelper.TypedContentAtRoot().FirstOrDefault(p => p.UrlAbsolute().StartsWith(root));

            if (content != null)
            {
                string alias = content.DocumentTypeAlias;

                Type type = this.GetRegisteredType(alias);

                if (type != null)
                {
                    return (PageBase)content.As(type);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the parent of the current instance as the given <see cref="Type"/>.
        /// </summary>
        /// <param name="nodeId">The id of the current node to search from.</param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the ancestor to return.
        /// </typeparam>
        /// <returns>
        /// The parent as an instance of <see cref="T"/>.
        /// </returns>
        public T GetParent<T>(int nodeId) where T : class
        {
            return this.UmbracoHelper.TypedContent(nodeId).Parent.As<T>();
        }

        /// <summary>
        /// Gets the parent of the current instance as an instance of <see cref="PageBase"/>.
        /// </summary>
        /// <param name="nodeId">The id of the current node to search from.</param>
        /// <returns>
        /// The parent as an instance of <see cref="PageBase"/>.
        /// </returns>
        public PageBase GetParent(int nodeId)
        {
            IPublishedContent content = this.UmbracoHelper.TypedContent(nodeId);
            IPublishedContent parent = content?.Parent;

            if (parent != null)
            {
                string alias = parent.DocumentTypeAlias;

                Type type = this.GetRegisteredType(alias);

                if (type != null)
                {
                    return (PageBase)parent.As(type);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the ancestors of the current instance as the given <see cref="Type"/>.
        /// </summary>
        /// <param name="nodeId">
        /// The id of the current node to search from.
        /// </param>
        /// <param name="maxLevel">
        /// The maximum level to search.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of the ancestor to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> GetAncestors<T>(int nodeId, int maxLevel = int.MaxValue) where T : class
        {
            string name = typeof(T).Name;
            return this.UmbracoHelper.TypedContent(nodeId)
                                     .Ancestors(maxLevel)
                                     .Where(a => a.DocumentTypeAlias.InvariantEquals(name))
                                     .As<T>();
        }

        /// <summary>
        /// Gets the ancestors of the current instance as an <see cref="IEnumerable{PageBase}"/>.
        /// </summary>
        /// <param name="nodeId">
        /// The id of the current node to search from.
        /// </param>
        /// <param name="maxLevel">
        /// The maximum level to search.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PageBase}"/>.
        /// </returns>
        public IEnumerable<PageBase> GetAncestors(int nodeId, int maxLevel = int.MaxValue)
        {
            return this.GetCollection((i, l) => this.UmbracoHelper.TypedContent(i).Ancestors(l), nodeId, maxLevel);
        }

        /// <summary>
        /// Gets the children of the current instance as the given <see cref="Type"/>.
        /// </summary>
        /// <param name="nodeId">
        /// The id of the current node to search from.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of child to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> GetChildren<T>(int nodeId) where T : class
        {
            string name = typeof(T).Name;
            return this.UmbracoHelper.TypedContent(nodeId)
                                     .Children(c => c.DocumentTypeAlias.InvariantEquals(name))
                                     .As<T>();
        }

        /// <summary>
        /// Gets the children of the current instance as an <see cref="IEnumerable{PageBase}"/>.
        /// </summary>
        /// <param name="nodeId">
        /// The id of the current node to search from.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PageBase}"/>.
        /// </returns>
        public IEnumerable<PageBase> GetChildren(int nodeId)
        {
            return this.GetCollection((i, l) => this.UmbracoHelper.TypedContent(i).Children, nodeId);
        }

        /// <summary>
        /// Gets the descendants of the current instance as the given <see cref="Type"/>.
        /// </summary>
        /// <param name="nodeId">
        /// The id of the current node to search from.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="Type"/> of descendant to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> GetDescendants<T>(int nodeId) where T : class
        {
            string name = typeof(T).Name;
            return this.UmbracoHelper.TypedContent(nodeId)
                                     .Descendants()
                                     .Where(d => d.DocumentTypeAlias.InvariantEquals(name))
                                     .As<T>();
        }

        /// <summary>
        /// Gets the descendants of the current instance as an <see cref="IEnumerable{PageBase}"/>.
        /// </summary>
        /// <param name="nodeId">
        /// The id of the current node to search from.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PageBase}"/>.
        /// </returns>
        public IEnumerable<PageBase> GetDescendants(int nodeId)
        {
            return this.GetCollection((i, l) => this.UmbracoHelper.TypedContent(i).Descendants(), nodeId);
        }

        /// <summary>
        /// Registers the given type to allow conversion.
        /// </summary>
        /// <param name="type">
        /// The type to register.
        /// </param>
        /// <param name="alias">
        /// Any alias for the given type.
        /// </param>
        public void RegisterType(Type type, string alias = null)
        {
            RegisteredTypes.GetOrAdd(type.Name, t => type);

            // Ensure that Json.NET can serialize/deserialize our types.
            TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(typeof(JsonConverter)));
        }

        /// <summary>
        /// Gets the list of stored types.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> containing the registered types.
        /// </returns>
        public IEnumerable<Type> GetRegisteredTypes()
        {
            return RegisteredTypes.Values.ToArray();
        }

        /// <summary>
        /// Gets the stored type matching the given name.
        /// </summary>
        /// <param name="name">
        /// The name of the type to retrieve.
        /// </param>
        /// <returns>
        /// The stored <see cref="Type"/>.
        /// </returns>
        public Type GetRegisteredType(string name)
        {
            Type type;
            RegisteredTypes.TryGetValue(name, out type);
            return type;
        }

        /// <summary>
        /// Gets a collection of <see cref="PageBase"/>.
        /// </summary>
        /// <param name="func">
        /// The delegate function returning a collection of <see cref="IPublishedContent"/>.
        /// </param>
        /// <param name="id">The current id to return related content for.</param>
        /// <param name="level">The maximum level to search.</param>
        /// <returns>
        /// The <see cref="IEnumerable{PageBase}"/>.
        /// </returns>
        private IEnumerable<PageBase> GetCollection(Func<int, int, IEnumerable<IPublishedContent>> func, int id, int level = int.MaxValue)
        {
            IEnumerable<IPublishedContent> contentList = func.Invoke(id, level);

            // Readablity.
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (IPublishedContent content in contentList)
            {
                Type type = this.GetRegisteredType(content.DocumentTypeAlias);

                if (type != null)
                {
                    object meta = content.As(type);
                    if (meta != null)
                    {
                        yield return (PageBase)meta;
                    }
                }
            }
        }
    }
}
