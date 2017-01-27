using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using bisSport.Domain.Core.Base;

namespace bisSport.WebClient.Helpers
{
  public static class GuidHelper
  {
    public static MvcHtmlString GuidFor<TModel, TValue>(this HtmlHelper<TModel> html,
      Expression<Func<TModel, TValue>> expression)
    {
      var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
      var entity = metadata.Model as Entity;

      return entity == null ? null : html.Hidden($"{metadata.PropertyName}.typeguid", entity.TypeGuid);
    }

    public static HtmlString GuidForModel(this HtmlHelper html)
    {
      var entity = html.ViewData.Model as Entity;
      return entity == null ? null : html.Hidden("typeguid", entity.TypeGuid);
    }
  }
}