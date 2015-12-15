namespace SearchDemo.Controllers
{
    using System.Web.Mvc;

    using Our.Umbraco.Ditto;

    using SearchDemo.Models;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The sub page controller.
    /// </summary>
    public class SubPageController : RenderMvcController
    {
        /// <summary>
        /// Returns the default result of an action method for the controller used to perform a framework-level 
        /// operation on behalf of the action method.
        /// <remarks>The resultant view will always match the name of the document type.</remarks>
        /// </summary>
        /// <param name="model">The model to provide the result for.</param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Index(RenderModel model)
        {
            SubPage page = model.As<SubPage>();

            RenderSubPage viewModel = new RenderSubPage(page);

            return this.View("SubPage", viewModel);
        }
    }
}
