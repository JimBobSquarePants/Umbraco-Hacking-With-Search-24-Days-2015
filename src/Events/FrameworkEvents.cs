namespace SearchDemo.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Examine;
    using Examine.Providers;

    using SearchDemo.ComponentModel;
    using SearchDemo.Helpers;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// Runs initialization code for the framework.
    /// </summary>
    public class FrameworkEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Boot-up is completed, this allows you to perform any other boot-up logic required for the application.
        /// Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The current <see cref="UmbracoApplicationBase"/>
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco <see cref="ApplicationContext"/> for the current application.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Assign the fallback helper. This is hacky but there is simply no way to ensure that we have a context
            // When working with background threads.
            UmbracoHelper helper = new UmbracoHelper(UmbracoContext.Current);
            ContentHelper.FallbackUmbracoHelper = helper;

            // Assign indexer for full text searching.
            BaseIndexProvider baseIndexProvider = ExamineManager.Instance.IndexProviderCollection[SearchConstants.IndexerName];

            if (baseIndexProvider != null)
            {
                baseIndexProvider.GatheringNodeData += (sender, e) => this.GatheringNodeData(sender, e, helper);
            }
        }

        /// <summary>
        /// Gathers the information from each node to add to the Examine index.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments containing information about the nodes to be gathered.</param>
        /// <param name="helper">The <see cref="UmbracoHelper"/> to help gather node data.</param>
        // ReSharper disable once UnusedParameter.Local
        private void GatheringNodeData(object sender, IndexingNodeDataEventArgs e, UmbracoHelper helper)
        {
            StringBuilder mergedDataStringBuilder = new StringBuilder();
            StringBuilder categoryStringBuilder = new StringBuilder();

            // Convert the property and use reflection to grab the output property value adding it to the merged property collection.
            IPublishedContent content = null;

            switch (e.IndexType)
            {
                case "content":
                    content = helper.TypedContent(e.NodeId);
                    break;
                case "media":
                    content = helper.TypedMedia(e.NodeId);
                    break;
                case "member":
                    content = helper.TypedMember(e.NodeId);
                    break;
            }

            if (content == null)
            {
                return;
            }

            Type doctype = ContentHelper.Instance.GetRegisteredType(content.DocumentTypeAlias);

            List<string> mergedProperties = new List<string>();

            if (doctype != null)
            {
                // Match the Ditto properties filters.
                PropertyInfo[] properties =
                    doctype.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(x => x.CanWrite)
                            .ToArray();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (PropertyInfo property in properties)
                {
                    SearchMergedFieldAttribute attr = property.GetCustomAttribute<SearchMergedFieldAttribute>(true);

                    if (attr == null)
                    {
                        continue;
                    }

                    mergedProperties.Add(!string.IsNullOrWhiteSpace(attr.ExamineKey) ? attr.ExamineKey : property.Name);

                    // Look for any custom search resolvers to convert the information to a useful search result.
                    SearchResolverAttribute resolverAttribute = property.GetCustomAttribute<SearchResolverAttribute>(true);

                    // Combine property values.
                    foreach (KeyValuePair<string, string> field in e.Fields.Distinct())
                    {
                        if (mergedProperties.Distinct().InvariantContains(field.Key))
                        {
                            if (resolverAttribute != null)
                            {
                                SearchValueResolver resolver = (SearchValueResolver)Activator.CreateInstance(resolverAttribute.ResolverType);
                                mergedDataStringBuilder.AppendFormat(" {0}", helper.StripHtml(resolver.ResolveValue(resolverAttribute, content, property, field.Value, Thread.CurrentThread.CurrentUICulture)));
                            }
                            else
                            {
                                mergedDataStringBuilder.AppendFormat(" {0}", helper.StripHtml(field.Value));
                            }

                            mergedProperties.Remove(!string.IsNullOrWhiteSpace(attr.ExamineKey) ? attr.ExamineKey : property.Name);
                        }
                    }
                }

                // Combine categories.
                SearchCategoryAttribute categoryAttribute = doctype.GetCustomAttribute<SearchCategoryAttribute>();

                if (categoryAttribute != null)
                {
                    if (categoryAttribute.Categories.Any())
                    {
                        foreach (string category in categoryAttribute.Categories)
                        {
                            categoryStringBuilder.AppendFormat("{0} ", category);
                        }
                    }
                }
            }

            e.Fields[SearchConstants.CategoryField] = categoryStringBuilder.ToString().Trim();
            e.Fields[SearchConstants.MergedDataField] = mergedDataStringBuilder.ToString().Trim();
        }
    }
}
