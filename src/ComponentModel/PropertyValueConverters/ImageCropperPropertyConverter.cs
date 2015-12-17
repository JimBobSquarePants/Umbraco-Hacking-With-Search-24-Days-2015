namespace SearchDemo.ComponentModel
{
    using System;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Web.Models;

    /// <summary>
    /// The image cropper property converter. This allows Ditto to map the image cropper using the built in Umbraco
    /// methods.
    /// </summary>
    public class ImageCropperPropertyConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        /// <summary>
        /// Converts a property Source value to an Object value.
        /// </summary>
        /// <param name="propertyType">The property type.</param>
        /// <param name="source">The source value.</param>
        /// <param name="preview">A value indicating whether conversion should take place in preview mode.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        /// <remarks>
        /// The converter should know how to convert a <c>null</c> source value, or any source value
        /// indicating that no value has been assigned to the property. It is up to the converter to determine
        /// what to return in that case: either <c>null</c>, or the default value...
        /// </remarks>
        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            try
            {
                return JsonConvert.DeserializeObject<ImageCropDataSet>(source.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the converter supports a property type.
        /// </summary>
        /// <param name="propertyType">The property type.</param>
        /// <returns>
        /// A value indicating whether the converter supports a property type.
        /// </returns>
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return Constants.PropertyEditors.ImageCropperAlias.InvariantEquals(propertyType.PropertyEditorAlias);
        }

        /// <summary>
        /// Gets the property cache level of a specified value.
        /// </summary>
        /// <param name="propertyType">The property type.</param><param name="cacheValue">The property value.</param>
        /// <returns>
        /// The property cache level of the specified value.
        /// </returns>
        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return PropertyCacheLevel.Content;
        }

        /// <summary>
        /// Gets the type of values returned by the converter.
        /// </summary>
        /// <param name="propertyType">The property type.</param>
        /// <returns>
        /// The CLR type of values returned by the converter.
        /// </returns>
        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof(ImageCropDataSet);
        }
    }
}
