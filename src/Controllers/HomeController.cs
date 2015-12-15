namespace SearchDemo.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Our.Umbraco.Ditto;

    using SearchDemo.Helpers;
    using SearchDemo.Models;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : RenderMvcController
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
            Home home = model.As<Home>();

            RenderHome viewModel = new RenderHome(home)
            {
                SubPages = ContentHelper.Instance.GetChildren<SubPage>(home.Id).ToList()
            };

            return this.View("Home", viewModel);
        }
    }
}
